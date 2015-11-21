using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using LibGit2Sharp;

namespace GitWrapper
{
    public class GitFlowWrapper
    {
        public delegate void CommandOutputReceivedEventHandler(object sender, CommandOutputEventArgs args);
        public delegate void CommandErrorReceivedEventHandler(object sender, CommandOutputEventArgs args);

        private readonly string repoDirectory;
        public static StringBuilder Output = new StringBuilder("");
        public static StringBuilder Error = new StringBuilder("");
        private const string GitFlowDefaultValueRegExp = @"\[(.*?)\]";

        public event CommandOutputReceivedEventHandler CommandOutputDataReceived;
        public event CommandErrorReceivedEventHandler CommandErrorDataReceived;

        public GitFlowWrapper(string repoDirectory)
        {
            this.repoDirectory = repoDirectory;
        }

      //  public bool IsInitialized
      //  {
      //      get
      //      {
      //          using (var repo = new Repository(repoDirectory))
      //          {
      //              return repo.Config.Any(c => c.Key.StartsWith("gitflow.branch.master")) && 
						//repo.Config.Any(c => c.Key.StartsWith("gitflow.branch.develop"));
      //          }
      //      }
      //  }

        //public IEnumerable<BranchItem> AllFeatureBranches
        //{
        //    get
        //    {
        //        if (!IsInitialized)
        //            return new List<BranchItem>();

        //        using (var repo = new Repository(repoDirectory))
        //        {
        //            var prefix = repo.Config.Get<string>("gitflow.prefix.feature").Value;
        //            return
        //                repo.Branches.Where(b => (!b.IsRemote && b.Name.StartsWith(prefix)) /*|| (b.IsRemote && b.Name.Contains(prefix))*/)
        //                    .Select(c => new BranchItem
        //                    {
        //                        Author = c.Tip.Author.Name,
        //                        Name = c.IsRemote ? c.Name :  c.Name.Replace(prefix,""),
        //                        LastCommit = c.Tip.Author.When,
        //                        IsTracking = c.IsTracking,
        //                        IsCurrentBranch = c.IsCurrentRepositoryHead,
        //                        IsRemote = c.IsRemote,
        //                        CommitId = c.Tip.Id.ToString(),
        //                        Message = c.Tip.MessageShort
        //                    }).ToList();
        //        }   

        //    }
        //}
 
        public IEnumerable<StashItem>  StashList()
        {
            using (var repo = new Repository(repoDirectory))
            {
                IList<Stash> stashes = new List<Stash>();
                foreach(Stash s in repo.Stashes)
                {
                    StashItem stash = new StashItem { Message = s.Message};
                }
                return repo.Stashes.Select(s => new StashItem
                {
                    Stash = s,
                    Message = s.Message
                }).ToList();
                //var prefix = repo.Config.Get<string>("gitflow.prefix.feature").Value;
                //return
                //    repo.Branches.Where(b => (!b.IsRemote && b.Name.StartsWith(prefix)) /*|| (b.IsRemote && b.Name.Contains(prefix))*/)
                //        .Select(c => new BranchItem
                //        {
                //            Author = c.Tip.Author.Name,
                //            Name = c.IsRemote ? c.Name : c.Name.Replace(prefix, ""),
                //            LastCommit = c.Tip.Author.When,
                //            IsTracking = c.IsTracking,
                //            IsCurrentBranch = c.IsCurrentRepositoryHead,
                //            IsRemote = c.IsRemote,
                //            CommitId = c.Tip.Id.ToString(),
                //            Message = c.Tip.MessageShort
                //        }).ToList();
            }
        }

        public GitFlowCommandResult StashPop(int index)
        {
            string gitArguments = String.Format("stash pop {0}", index);
            return RunGitFlow(gitArguments);
        }

        protected virtual void OnCommandOutputDataReceived(CommandOutputEventArgs e)
        {
            CommandOutputReceivedEventHandler handler = CommandOutputDataReceived;
            if (handler != null) handler(this, e);
        }

        protected virtual void OnCommandErrorDataReceived(CommandOutputEventArgs e)
        {
            CommandErrorReceivedEventHandler handler = CommandErrorDataReceived;
            if (handler != null) handler(this, e);
        }

        
        
        //public GitFlowCommandResult Init(GitFlowRepoSettings settings)
        //{
        //    Error = new StringBuilder("");
        //    Output = new StringBuilder("");

        //    using (var p = CreateGitFlowProcess("init -f", repoDirectory))
        //    {
        //        OnCommandOutputDataReceived(new CommandOutputEventArgs("Running git " + p.StartInfo.Arguments + Environment.NewLine));
        //        p.Start();
        //        p.ErrorDataReceived += OnErrorReceived;
        //        p.BeginErrorReadLine();
        //        var input = new StringBuilder();

        //        var sr = p.StandardOutput;
        //        while (!sr.EndOfStream)
        //        {
        //            var inputChar = (char) sr.Read();
        //            input.Append(inputChar);
        //            if (StringBuilderEndsWith(input, Environment.NewLine))
        //            {
        //                Output.AppendLine(input.ToString());
        //                OnCommandOutputDataReceived(new CommandOutputEventArgs(input.ToString()));
        //                input = new StringBuilder();
        //            }
        //            //if (IsMasterBranchQuery(input.ToString()))
        //            //{
        //            //    p.StandardInput.Write(settings.MasterBranch + "\n");
        //            //    Output.Append(input);
        //            //    OnCommandOutputDataReceived(new CommandOutputEventArgs(input + Environment.NewLine));
        //            //    input = new StringBuilder();
        //            //}
                    
        //        }
        //    }
        //    if (Error != null && Error.Length > 0)
        //    {
        //        return new GitFlowCommandResult(false, Error.ToString());
        //    }
        //    return new GitFlowCommandResult(true, Output.ToString());
        //}

        private static Process CreateGitFlowProcess(string arguments, string repoDirectory)
        {
            var gitInstallationPath = GitHelper.GetGitInstallationPath();
            string pathToGit = Path.Combine(Path.Combine(gitInstallationPath,"bin\\git.exe"));
            return new Process
            {
                StartInfo =
                {
                    CreateNoWindow = true,
                    UseShellExecute = false,
                    RedirectStandardInput = true,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    FileName = pathToGit,
                    Arguments = "flow " + arguments,
                    WorkingDirectory = repoDirectory
                }
            };
        }

        private void OnOutputDataReceived(object sender, DataReceivedEventArgs dataReceivedEventArgs)
        {
            if (dataReceivedEventArgs.Data != null)
            {
                Output.Append(dataReceivedEventArgs.Data);
                Debug.WriteLine(dataReceivedEventArgs.Data);
                OnCommandOutputDataReceived(new CommandOutputEventArgs(dataReceivedEventArgs.Data + Environment.NewLine));
            }
        }
        private void OnErrorReceived(object sender, DataReceivedEventArgs dataReceivedEventArgs)
        {
            if (dataReceivedEventArgs.Data != null && dataReceivedEventArgs.Data.StartsWith("fatal:", StringComparison.OrdinalIgnoreCase))
            {
                Error = new StringBuilder();
                Error.Append(dataReceivedEventArgs.Data);
                Debug.WriteLine(dataReceivedEventArgs.Data);
                OnCommandErrorDataReceived(new CommandOutputEventArgs(dataReceivedEventArgs.Data + Environment.NewLine));
            }
        }

        private static bool MatchInput(string input, Regex regex)
        {
            var match = regex.Match(input);
            if (match.Success)
            {
                return true;
            }
            return false;
        }

        private static bool StringBuilderEndsWith(StringBuilder haystack, string needle)
        {
            if (haystack.Length == 0)
                return false;

            var needleLength = needle.Length - 1;
            var haystackLength = haystack.Length - 1;
            for (var i = 0; i < needleLength; i++)
            {
                if (haystack[haystackLength - i] != needle[needleLength - i])
                {
                    return false;
                }
            }
            return true;
        }

        private GitFlowCommandResult RunGitFlow(string gitArguments)
        {
            Error = new StringBuilder("");
            Output = new StringBuilder("");

            using (var p = CreateGitFlowProcess(gitArguments, repoDirectory))
            {
                OnCommandOutputDataReceived(new CommandOutputEventArgs("Running git " + p.StartInfo.Arguments + "\n"));
                p.Start();
                p.ErrorDataReceived += OnErrorReceived;
                p.OutputDataReceived += OnOutputDataReceived;
                p.BeginErrorReadLine();
                p.BeginOutputReadLine();
                p.WaitForExit(15000);
                if (!p.HasExited)
                {
                    OnCommandOutputDataReceived(new CommandOutputEventArgs("The command is taking longer than expected\n"));

                    p.WaitForExit(15000);
                    if (!p.HasExited)
                    {
                        return new GitFlowTimedOutCommandResult("git " + p.StartInfo.Arguments);
                    }
                }
                if (Error != null && Error.Length > 0)
                {
                    return new GitFlowCommandResult(false, Error.ToString());
                }
                return new GitFlowCommandResult(true, Output.ToString());
            }
        }
    }
}
