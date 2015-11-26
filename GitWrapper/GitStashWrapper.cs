using LibGit2Sharp;
using Microsoft.VisualStudio.TeamFoundation.Git.Extensibility;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel;
using System.ComponentModel.Composition;
using EnvDTE80;
using EnvDTE;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;

namespace GitWrapper
{
    public class GitStashWrapper : INotifyPropertyChanged, IGitStashWrapper
    {
        private IGitExt gitService;
        private Repository repo;
        //private Lazy<DTE2> dte;
        //private Lazy<RunningDocumentTable> rdt;
        //private Lazy<EnvDTE.Events> events;
        //private Lazy<EnvDTE.DocumentEvents> documentEvents;
        private DocumentEvents documentEvents;
        private Signature stasher = null;
        private Lazy<SolutionEvents> solutionEvents;

        public delegate void StashesChangedEventHandler(object sender, StashesChangedEventArgs e);
        public event StashesChangedEventHandler StashesChangedEvent;

        private void OnStashesChanged(StashesChangedEventArgs e)
        {
            StashesChangedEvent?.Invoke(this, e);
        }
        private void DocumentEvents_DocumentSaved(Document Document)
        {
            OnStashesChanged(new StashesChangedEventArgs());
        }

        //sigh, for testing only
        public GitStashWrapper(string dir, IOutputLogger logger)
        {
            this.Logger = logger;
            try
            {
                repo = new Repository(dir);
            }
            catch (Exception ex)
            {
                throw new GitStashException(ex);
            }
        }

        //I need to figure out how to mock all this DTE stuff for testing
        public GitStashWrapper(IServiceProvider serviceProvider, IOutputLogger logger)
        {
            //dte = new Lazy<DTE2>(() => ServiceProvider.GlobalProvider.GetService(typeof(EnvDTE.DTE)) as DTE2);
            //dte = new Lazy<DTE2>(() => serviceProvider.GetService(typeof(EnvDTE.DTE)) as DTE2);
            //events = new Lazy<EnvDTE.Events>(() => dte.Value.Events);
            //documentEvents = new Lazy<EnvDTE.DocumentEvents>(() => events.Value.DocumentEvents);
            //documentEvents.Value.DocumentSaved += DocumentEvents_DocumentSaved;

            var dte = (DTE)serviceProvider.GetService(typeof(EnvDTE.DTE));
            documentEvents = dte.Events.DocumentEvents;
            documentEvents.DocumentSaved += DocumentEvents_DocumentSaved;

            this.Logger = logger;

            this.gitService = (IGitExt)serviceProvider.GetService(typeof(IGitExt));
            if (gitService == null || gitService.ActiveRepositories.Count() == 0) // ifthis is null we should ever be called
                throw new ArgumentException("Parameter not initialized", "gitService");
            this.gitService.PropertyChanged += OnGitServicePropertyChanged;
            //test repo exists
            try
            {
                repo = new Repository(gitService.ActiveRepositories.FirstOrDefault().RepositoryPath);
            }
            catch (Exception ex)
            {
                throw new GitStashException(ex);
            }
        }

        private void OnGitServicePropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (gitService == null || gitService.ActiveRepositories.Count() == 0) // ifthis is null we should ever be called
                return;
            repo = new Repository(gitService.ActiveRepositories.FirstOrDefault().RepositoryPath); // probably memory leak her
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs("Stashes"));
            }
        }

        public IList<IGitStash> Stashes
        {
            get
            {
                List<IGitStash> stashes = new List<IGitStash>();

                foreach (Stash stash in repo.Stashes)
                {
                    GitStashItem s = new GitStashItem(stash);
                    stashes.Add(s);
                }
                return stashes;
            }
        }

        public bool WorkingDirHasChanges()
        {

            return repo.RetrieveStatus().IsDirty;
        }

        private void CheckForValidStashIndex(int index, Repository repo)
        {
            int count = repo.Stashes.Count();
            if (count == 0 || index > (count - 1) || index < 0)
            {
                throw new GitStashInvalidIndexException(String.Format("{0} is an invalid stash index.", index));
            }
        }

        private void LogFilesInStash(Stash stash)
        {
            Tree commitTree = stash.WorkTree.Tree;
            IList<string> paths = commitTree.Select(t => t.Path).ToList();
            DiffTargets dt = DiffTargets.WorkingDirectory;
            var patch = repo.Diff.Compare<TreeChanges>(commitTree, dt,paths);

            foreach (var ptc in patch)
            {
                Logger.WriteLine(ptc.Status + " -> " + ptc.Path); // Status -> File Path
            }
        }

        public IGitStashResults PopStash(IGitStashPopOptions options, int index)
        {
            Logger.WriteLine("Apllying stash: " + index);
            int count = repo.Stashes.Count();
            CheckForValidStashIndex(index, repo);
            if (UntrackedFileChanges(index, repo))
            {
                Logger.WriteLine("There are changes in your working directory, aborting.");
                return new GitStashResultsFailure();
            }
            StashApplyOptions sao = new StashApplyOptions();
            sao.ApplyModifiers = (options.Index ? StashApplyModifiers.ReinstateIndex : 0);
            StashApplyStatus status = repo.Stashes.Pop(index, sao);
            GitStashResults results = new GitStashResults(status);
            if (repo.Stashes.Count() >= count && results.Success)
            {
                Logger.WriteLine("Failed.");
                throw new GitStashException("Command apply and delete was called, reported success, but stash count changed.");
            }
            Logger.WriteLine("Done." + Environment.NewLine);
            return results;

        }

        public IGitStashResults ApplyStash(IGitStashApplyOptions options, int index)
        {
            int count = repo.Stashes.Count();
            Logger.WriteLine("Apllying stash: " + index);
            CheckForValidStashIndex(index, repo);
            if (UntrackedFileChanges(index, repo))
            {
                Logger.WriteLine("There are changes in your working directory, aborting.");
                return new GitStashResultsFailure();
            }
            StashApplyOptions sao = new StashApplyOptions();
            sao.ApplyModifiers = (options.Index ? StashApplyModifiers.ReinstateIndex : 0);
            StashApplyStatus status = repo.Stashes.Apply(index, sao);
            GitStashResults results = new GitStashResults(status);
            if (repo.Stashes.Count() != count && results.Success)
            {
                Logger.WriteLine("Failed.");
                throw new GitStashException("Command apply was called and reported success, but stash count changed.");
            }
            Logger.WriteLine("Done." + Environment.NewLine);
            return results;

        }

        public IGitStashResults SaveStash(IGitStashSaveOptions options)
        {
            Logger.WriteLine("Saving stash: " + options.Message);
            int count = repo.Stashes.Count();
            if (!repo.RetrieveStatus().IsDirty)
            {
                Logger.WriteLine("Nothing to save.");
                return new GitStashResults(null);
            }
            StashModifiers sm = (options.KeepIndex ? StashModifiers.KeepIndex : 0);
            sm |= (options.Untracked ? StashModifiers.IncludeUntracked : 0);
            sm |= (options.Ignored ? StashModifiers.IncludeIgnored : 0);
            Stash stash = repo.Stashes.Add(Stasher, options.Message, sm);
            LogFilesInStash(stash);
            GitStashResults results = new GitStashResults(stash);
            if (repo.Stashes.Count() <= count && results.Success)
            {
                Logger.WriteLine("Failed.");
                throw new GitStashException("Command save was called and reported success, but stash didn't increase.");
        }
            Logger.WriteLine("Done." + Environment.NewLine);
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Stahses"));
            return results;

        }

        public IGitStashResults DropStash(IGitStashDropOptions options, int index)
        {
            Logger.WriteLine("Deleting stash: " + index);
            CheckForValidStashIndex(index, repo);
            int count = repo.Stashes.Count();
            repo.Stashes.Remove(index);
            if (repo.Stashes.Count() < count)
            {
                Logger.WriteLine("Done." + Environment.NewLine);
                return new GitStashResultsSuccess();
            }
            else
            {
                Logger.WriteLine("Failed.");
                return new GitStashResultsFailure();
            }

        }

        public event PropertyChangedEventHandler PropertyChanged;

        private Signature Stasher
        {
            get
            {
                if (stasher == null)
                {
                    stasher = new Signature(new Identity(GetUserNameVS14(), GetUserEmailAddressVS14()), DateTimeOffset.Now.AddDays(365));
                }
                return stasher;
            }
        }

        public string CurrentBranch
        {
            get
            {
                if (repo != null && repo.Head != null)
                    return repo.Head.FriendlyName;
                return "";
            }
        }

        public IOutputLogger Logger { get; private set; }

        private bool UntrackedFileChanges(int index, Repository repo)
        {
            Stash stash = repo.Stashes.ElementAt(index);
            if (stash.Untracked == null)
                return false;
            IList<string> p = stash.Untracked.Tree.Select(t => t.Path).ToList();
            if (p.Count() == 0)
                return false;
            DiffTargets dt = DiffTargets.WorkingDirectory;
            var r = repo.Diff.Compare<TreeChanges>(stash.Untracked.Tree, dt, p);
            if (r.Modified.Count() > 0)
                return true;
            return false;
        }

        public IList<string> GetUntrackedChangesList(int stashIndex)
        {
            Stash stash = repo.Stashes.ElementAt(stashIndex);
            IList<string> paths = new List<string>();
            IList<string> p = stash.Untracked.Tree.Select(t => t.Path).ToList();
            if (p.Count() == 0)
                return paths;
            DiffTargets dt = DiffTargets.WorkingDirectory;
            var r = repo.Diff.Compare<TreeChanges>(stash.Untracked.Tree, dt, p);
            if (!r.Any())
                return paths;
            return r.Modified.Select(c => c.Path).ToList();
        }

        private static string GetUserEmailAddressVS14()
        {
            // It's a good practice to request explicit permission from
            // the user that you want to use his email address and any
            // other information. This enables the user to be in control
            // of his/her privacy.

            // Assuming permission is granted, we obtain the email address.

            const string SubKey = "Software\\Microsoft\\VSCommon\\ConnectedUser\\IdeUser\\Cache";
            const string EmailAddressKeyName = "EmailAddress";
            const string UserNameKeyName = "DisplayName";

            RegistryKey root = Registry.CurrentUser;
            RegistryKey sk = root.OpenSubKey(SubKey);
            if (sk == null)
            {
                // The user is currently not signed in.
                return null;
            }
            else
            {
                // Get user email address.
                return (string)sk.GetValue(EmailAddressKeyName);

                // You can also get user name like this.
                // return (string)sk.GetValue(UserNameKeyName);
            }
        }

        private static string GetUserNameVS14()
        {
            // It's a good practice to request explicit permission from
            // the user that you want to use his email address and any
            // other information. This enables the user to be in control
            // of his/her privacy.

            // Assuming permission is granted, we obtain the email address.

            const string SubKey = "Software\\Microsoft\\VSCommon\\ConnectedUser\\IdeUser\\Cache";
            const string EmailAddressKeyName = "EmailAddress";
            const string UserNameKeyName = "DisplayName";

            RegistryKey root = Registry.CurrentUser;
            RegistryKey sk = root.OpenSubKey(SubKey);
            if (sk == null)
            {
                // The user is currently not signed in.
                return null;
            }
            else
            {
                // Get user email address.
                return (string)sk.GetValue(UserNameKeyName);

                // You can also get user name like this.
                // return (string)sk.GetValue(UserNameKeyName);
            }
        }
    }

}
