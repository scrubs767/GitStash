using LibGit2Sharp;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GitWrapper
{
    public class GitStashWrapper 
    {
        private readonly string repoDirectory;

        public GitStashWrapper(string repoDirectory)
        {
            Repository repo = null;
            this.repoDirectory = repoDirectory;
            //test repo exists
            try
            {
                repo = new Repository(repoDirectory);
            }
            catch(Exception ex)
            {
                throw new GitStashException(ex);
            }
            finally
            {
                if(repo != null)
                    repo.Dispose();
            }
        }

        public IList<IGitStash> Stashes
        {
            get
            {
                List<IGitStash> stashes = new List<IGitStash>();
                using (var repo = new Repository(repoDirectory))
                {
                    foreach (Stash stash in repo.Stashes)
                    {
                        GitStashItem s = new GitStashItem(stash);
                        stashes.Add(s);                       
                    }
                }
               return stashes;
            }
        }

        private void CheckForValidStashIndex(int index, Repository repo)
        {
            int count = repo.Stashes.Count();
            if (count == 0 || index > (count - 1) || index < 0)
            {
                throw new GitStashInvalidIndexException(String.Format("{0} is an invalid stash index.", index));
            }
        }

        public IGitStashResults PopStash(IGitStashPopOptions options, int index)
        {
            using (var repo = new Repository(repoDirectory))
            {
                int count = repo.Stashes.Count();
                CheckForValidStashIndex(index, repo);
                if (UntrackedFileChanges(index, repo))
                    return new GitStashResultsFailure();
                StashApplyOptions sao = new StashApplyOptions();
                sao.ApplyModifiers = (options.Index ? StashApplyModifiers.ReinstateIndex : 0);
                StashApplyStatus status = repo.Stashes.Pop(index,sao);
                GitStashResults results = new GitStashResults(status);
                if (repo.Stashes.Count() >= count && results.Success)
                    throw new GitStashException("Command pop was called and reported success, but stash count didnt decrement.");
                return results;
            }
        }

        public IGitStashResults ApplyStash(IGitStashApplyOptions options, int index)
        {
            using (var repo = new Repository(repoDirectory))
            {
                int count = repo.Stashes.Count();
                CheckForValidStashIndex(index, repo);
                if(UntrackedFileChanges(index,repo))
                    return new GitStashResultsFailure();
                StashApplyOptions sao = new StashApplyOptions();
                sao.ApplyModifiers = (options.Index ? StashApplyModifiers.ReinstateIndex : 0);
                StashApplyStatus status = repo.Stashes.Apply(index,sao);
                GitStashResults results = new GitStashResults(status);
                if (repo.Stashes.Count() != count && results.Success)
                    throw new GitStashException("Command apply was called and reported success, but stash count changed.");
                return results;
            }
        }

        public IGitStashResults SaveStash(IGitStashSaveOptions options)
        {
            using (var repo = new Repository(repoDirectory))
            {
                int count = repo.Stashes.Count();
                if (!repo.RetrieveStatus().IsDirty)
                {
                    return new GitStashResults(null);
                }
                StashModifiers sm = (options.KeepIndex ? StashModifiers.KeepIndex : 0);
                sm |= (options.Untracked ? StashModifiers.IncludeUntracked : 0);
                sm |= (options.Ignored ? StashModifiers.IncludeIgnored : 0);
                Stash stash = repo.Stashes.Add(Stasher, options.Message, sm);
                GitStashResults results = new GitStashResults(stash);
                if (repo.Stashes.Count() <= count && results.Success)
                    throw new GitStashException("Command save was called and reported success, but stash didn't increase.");
                return results;
            }
        }

        public IGitStashResults DropStash(IGitStashDropOptions options, int index)
        {
            using (var repo = new Repository(repoDirectory))
            {
                CheckForValidStashIndex(index, repo);
                int count = repo.Stashes.Count();
                repo.Stashes.Remove(index);
                if(repo.Stashes.Count() < count)
                    return new GitStashResultsSuccess();
                else
                    return new GitStashResultsFailure();
            }
        }
        

        private Signature stasher = null;
        private Signature Stasher
        {
            get
            {
                if(stasher == null)
                {
                    stasher = new Signature(new Identity(GetUserNameVS14(),GetUserEmailAddressVS14()), DateTimeOffset.Now.AddDays(365));
                }
                return stasher;
            }
        }

        private bool UntrackedFileChanges(int index, Repository repo)
        {
            Stash stash = repo.Stashes.ElementAt(index);
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
            Repository repo = new Repository(repoDirectory);

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
