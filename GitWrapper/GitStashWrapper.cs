using LibGit2Sharp;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel;
using EnvDTE;
using GitStash.Common;

namespace GitWrapper
{
    public class GitStashWrapper : IGitStashWrapper
    {
        private Repository repo;
        private Signature stasher = null;
        private IGitStashProjectEvents eventsService = null;
        private IGitStashProjects projects;
        private object lockObject = new Object();

        public delegate void StashesChangedEventHandler(object sender, StashesChangedEventArgs e);
        public event StashesChangedEventHandler StashesChangedEvent;

        public GitStashWrapper(string path, IGitStashProjectEvents eventsService, IGitStashOutputLogger logger, IGitStashProjects projects)
            : this(eventsService,logger,projects)
        {
            repo = new Repository(path);
        }

        public GitStashWrapper(IGitStashProjectEvents eventsService, IGitStashOutputLogger logger, IGitStashProjects projects)
        {
            this.Logger = logger;
            this.eventsService = eventsService;
            this.projects = projects;
            eventsService.ProjectDirectoryChanged += EventsService_ProjectDirectoryChanged;
            eventsService.StashesChangedEvent += EventsService_StashesChangedEvent;
        }

        private void EventsService_StashesChangedEvent(object sender, EventArgs e)
        {
            OnStashesChanged(new StashesChangedEventArgs());
        }

        private void EventsService_ProjectDirectoryChanged(object sender, ProjectDirectoryEventArgs e)
        {
            repo = new Repository(e.Path);
            OnStashesChanged(new StashesChangedEventArgs());
        }

        private void OnStashesChanged(StashesChangedEventArgs e)
        {
            StashesChangedEvent?.Invoke(this, e);
        }

        private IGitStashOutputLogger Logger { get; set; }

        public IList<IGitStash> Stashes
        {
            get
            {
                lock(lockObject)
                {
                    List<IGitStash> stashes = new List<IGitStash>();
                    if (repo == null) return stashes;
                    foreach (Stash stash in repo.Stashes)
                    {
                        GitStashItem s = new GitStashItem(stash);
                        stashes.Add(s);
                    }
                    return stashes;
                }
            }
        }

        public bool WorkingDirHasChanges()
        {
            lock (lockObject)
            {
                if (repo == null) return false;
                return repo.RetrieveStatus().IsDirty;
            }
        }

        private void CheckForValidStashIndex(int index)
        {
            if (repo == null) throw new GitStashInvalidIndexException(String.Format("{0} is an invalid stash index.", index));
            int count = repo.Stashes.Count();
            if (count == 0 || index > (count - 1) || index < 0)
            {
                throw new GitStashInvalidIndexException(String.Format("{0} is an invalid stash index.", index));
            }
        }

        private void LogFilesInStash(Stash stash)
        {
            if (stash == null)
                throw new ArgumentException("stash parameter can not be null");
            if (repo == null) throw new InvalidOperationException("Repository not initialized");
            Tree commitTree = stash.WorkTree.Tree;
            IList<string> paths = commitTree.Select(t => t.Path).ToList();
            DiffTargets dt = DiffTargets.WorkingDirectory;
            var patch = repo.Diff.Compare<TreeChanges>(commitTree, dt, paths);

            foreach (var ptc in patch)
            {
                Logger.WriteLine(ptc.Status + " -> " + ptc.Path); // Status -> File Path
            }
        }

        public IGitStashResults PopStash(IGitStashPopOptions options, int index)
        {
            lock (lockObject)
            {
                Logger.WriteLine("Apllying stash: " + index);
                if (repo == null) throw new InvalidOperationException("Repository not initialized");
                int count = repo.Stashes.Count();
                CheckForValidStashIndex(index);
                if (UntrackedFileChanges(index))
                {
                    Logger.WriteLine("There are changes in your working directory, aborting.");
                    return new GitStashResultsFailure();
                }
                StashApplyOptions sao = new StashApplyOptions();
                sao.ApplyModifiers = (options.Index ? StashApplyModifiers.ReinstateIndex : 0);
                StashApplyStatus status = repo.Stashes.Pop(index, sao);
                GitStashResults results = new GitStashResults(status);
                if (results.Success == false)
                    Logger.WriteLine("There are changes in your working directory, aborting.");
                if (repo.Stashes.Count() >= count && results.Success)
                {
                    throw new GitStashException("Command apply and delete was called, reported success, but stash count changed.");
                }
                Logger.WriteLine("Done." + Environment.NewLine);
                return results;
            }
        }

        public IGitStashResults ApplyStash(IGitStashApplyOptions options, int index)
        {
            lock (lockObject)
            {
                if (repo == null) throw new InvalidOperationException("Repository not initialized");
                int count = repo.Stashes.Count();
                Logger.WriteLine("Apllying stash: " + index);
                CheckForValidStashIndex(index);
                if (UntrackedFileChanges(index))
                {
                    Logger.WriteLine("There are changes in your working directory, aborting.");
                    return new GitStashResultsFailure();
                }
                StashApplyOptions sao = new StashApplyOptions();
                sao.ApplyModifiers = (options.Index ? StashApplyModifiers.ReinstateIndex : 0);
                StashApplyStatus status = repo.Stashes.Apply(index, sao);
                GitStashResults results = new GitStashResults(status);
                if (results.Success == false)
                    Logger.WriteLine("There are changes in your working directory, aborting.");
                if (repo.Stashes.Count() != count && results.Success)
                {
                    Logger.WriteLine("Failed.");
                    throw new GitStashException("Command apply was called and reported success, but stash count changed.");
                }
                Logger.WriteLine("Done." + Environment.NewLine);
                return results;
            }
        }

        public IGitStashResults SaveStash(IGitStashSaveOptions options)
        {
            lock (lockObject)
            {
                if (repo == null) throw new InvalidOperationException("Repository not initialized");
                if (projects.IsDirty)
                {
                    Logger.WriteLine("Your project has not been saved, aborting");
                    return new GitStashResultsFailure();
                }
                Logger.WriteLine("Saving stash: " + options.Message);
                bool hasChanges = WorkingDirHasChanges();
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

                GitStashResults results = new GitStashResults(stash);
                if (results.Success == false)
                    Logger.WriteLine("Failed");
                if (repo.Stashes.Count() <= count && results.Success)
                {
                    Logger.WriteLine("Failed.");
                    throw new GitStashException("Command save was called and reported success, but stash didn't increase.");
                }
                if (results.Success == false)
                {
                    if (stash != null)
                        LogFilesInStash(stash);
                    if (hasChanges)
                        Logger.WriteLine("perhaps you have untracked files, and didn't select Untracked.");
                    Logger.WriteLine("Failed.");
                    OnStashesChanged(new StashesChangedEventArgs());
                    return results;
                }
                LogFilesInStash(stash);
                Logger.WriteLine("Done." + Environment.NewLine);
                OnStashesChanged(new StashesChangedEventArgs());
                return results;
            }
        }

        public IGitStashResults DropStash(IGitStashDropOptions options, int index)
        {
            lock (lockObject)
            {
                if (repo == null) throw new InvalidOperationException("Repository not initialized");
                Logger.WriteLine("Deleting stash: " + index);
                CheckForValidStashIndex(index);
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
        }

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
                lock (lockObject)
                {
                    if (repo != null && repo.Head != null)
                        return repo.Head.FriendlyName;
                    return "";
                }
            }
        }        

        private bool UntrackedFileChanges(int index)
        {
            if (repo == null) throw new InvalidOperationException("Repository not initialized");
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
            lock (lockObject)
            {
                if (repo == null) throw new InvalidOperationException("Repository not initialized");
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
