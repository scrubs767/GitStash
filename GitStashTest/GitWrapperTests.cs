using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using GitWrapper;
using System.Collections.Generic;
using Moq;
using LibGit2Sharp;
using GitStash.Common;

namespace GitStashTest
{
    [TestClass]
    public class GitWrapperTests
    {
        [TestInitialize]
        public void Setup()
        {
            Directory.CreateDirectory("testgit");
            string rootedPath = Repository.Init("testgit");
            using (Repository repo = new Repository(rootedPath))
            {
                var content = "Commit this!";
                File.WriteAllText(Path.Combine(repo.Info.WorkingDirectory, "file1"), content);

                repo.Stage("file1");

                Signature author = new Signature("James", "@jugglingnutcase", DateTime.Now);
                Signature committer = author;

                Commit commit = repo.Commit("Here's a commit i made!", author, committer);
            }
        }

        [TestCleanup]
        public void TearDown()
        {
            var rootInfo = new DirectoryInfo("testgit") { Attributes = FileAttributes.Normal };
            foreach (var fileInfo in rootInfo.GetFileSystemInfos()) fileInfo.Attributes = FileAttributes.Normal;
            foreach (var subDirectory in Directory.GetDirectories("testgit", "*", SearchOption.AllDirectories))
            {
                var subInfo = new DirectoryInfo(subDirectory) { Attributes = FileAttributes.Normal };
                foreach (var fileInfo in subInfo.GetFileSystemInfos()) fileInfo.Attributes = FileAttributes.Normal;
            }
            Directory.Delete("testgit", true);
        }

        private IGitStashProjectEvents GetEventService()
        {
            Mock<IGitStashProjectEvents> events = new Mock<IGitStashProjectEvents>();
            return events.Object;
        }

        private IGitStashOutputLogger GetLogger()
        {
            Mock<IGitStashOutputLogger> logger = new Mock<IGitStashOutputLogger>();
            logger.Setup(l => l.WriteLine(It.IsAny<string>()));
            return logger.Object;
        }

        private IGitStashProjects GetProjects()
        {
            Mock<IGitStashProjects> projects = new Mock<IGitStashProjects>();
            return projects.Object;
        }

        [TestMethod]
        public void TestStash()
        {
            FileStream fs = File.Create(@"testgit\file2");
            fs.Close();
            GitStashWrapper git = new GitStashWrapper("testgit", GetEventService(), GetLogger(), GetProjects());
            IEnumerable<IGitStash> stashes = git.Stashes;
            GitStashOptions options = new GitStashOptions { Untracked = true };
            options.Message = "Testing";
            IGitStashResults results = git.SaveStash(options);

            Assert.IsFalse(File.Exists("file2"));
            Assert.IsTrue(results.Success);
            Assert.IsTrue(git.Stashes.Count == 1);
        }

        [TestMethod]
        public void TestDropStash()
        {
            FileStream fs = File.Create(@"testgit\file2");
            fs.Close();
            GitStashWrapper git = new GitStashWrapper("testgit", GetEventService(), GetLogger(), GetProjects());
            IEnumerable<IGitStash> stashes = git.Stashes;
            GitStashOptions options = new GitStashOptions { Untracked = true };
            options.Message = "Testing";
            IGitStashResults results = git.SaveStash(options);
            Assert.IsTrue(results.Success);
            Assert.IsTrue(git.Stashes.Count == 1);

            results = git.DropStash(new GitStashOptions(), 0);
            Assert.IsTrue(results.Success);
            Assert.IsTrue(git.Stashes.Count == 0);
        }

        [TestMethod]
        public void TestPopStash()
        {
            FileStream fs = File.Create(@"testgit\file2");
            fs.Close();
            GitStashWrapper git = new GitStashWrapper("testgit", GetEventService(), GetLogger(), GetProjects());
            GitStashOptions options = new GitStashOptions { Untracked = true };
            options.Message = "Testing";
            IGitStashResults results = git.SaveStash(options);
            Assert.IsTrue(results.Success);
            Assert.IsTrue(git.Stashes.Count == 1);
            Assert.IsFalse(File.Exists("file2"));

            results = git.PopStash(new GitStashOptions(), 0);
            Assert.IsTrue(results.Success);
            Assert.IsTrue(git.Stashes.Count == 0);
            Assert.IsTrue(File.Exists(@"testgit\file2"));
        }

        [TestMethod]
        public void TestApplyStash()
        {
            FileStream fs = File.Create(@"testgit\file2");
            fs.Close();
            GitStashWrapper git = new GitStashWrapper("testgit", GetEventService(), GetLogger(), GetProjects());
            GitStashOptions options = new GitStashOptions { Untracked = true };
            options.Message = "Testing";
            IGitStashResults results = git.SaveStash(options);
            Assert.IsTrue(results.Success);
            Assert.IsTrue(git.Stashes.Count == 1);
            Assert.IsFalse(File.Exists("file2"));

            results = git.ApplyStash(new GitStashOptions(), 0);
            Assert.IsTrue(results.Success);
            Assert.IsTrue(git.Stashes.Count == 1);
            Assert.IsTrue(File.Exists(@"testgit\file2"));
        }

        [TestMethod]
        public void StashWontPopIfConflictedOnStagedFile()
        {
            File.WriteAllText(@"testgit\file1", "This is a test");

            GitStashWrapper git = new GitStashWrapper("testgit", GetEventService(), GetLogger(), GetProjects());
            GitStashOptions options = new GitStashOptions { Untracked = true };
            options.Message = "Testing";
            IGitStashResults results = git.SaveStash(options);
            Assert.IsTrue(results.Success);
            Assert.IsTrue(git.Stashes.Count == 1);
            Assert.IsFalse(File.Exists("file1"));

            using (StreamWriter sw = File.AppendText(@"testgit\file1"))
            {
                sw.WriteLine("This is another test");
            }

            results = git.PopStash(new GitStashOptions(), 0);
            Assert.IsFalse(results.Success);
            Assert.IsTrue(git.Stashes.Count == 1);
            string txt = File.ReadAllText(@"testgit\file1");
            Assert.IsTrue(txt == "Commit this!This is another test\r\n");
        }

        [TestMethod]
        public void StashWontApplyIfConflictedOnStagedFile()
        {
            File.WriteAllText(@"testgit\file1", "This is a test");

            GitStashWrapper git = new GitStashWrapper("testgit", GetEventService(), GetLogger(), GetProjects());
            GitStashOptions options = new GitStashOptions { Untracked = true };
            options.Message = "Testing";
            IGitStashResults results = git.SaveStash(options);
            Assert.IsTrue(results.Success);
            Assert.IsTrue(git.Stashes.Count == 1);
            Assert.IsFalse(File.Exists("file1"));

            using (StreamWriter sw = File.AppendText(@"testgit\file1"))
            {
                sw.WriteLine("This is another test");
            }

            results = git.ApplyStash(new GitStashOptions(), 0);
            Assert.IsFalse(results.Success);
            Assert.IsTrue(git.Stashes.Count == 1);
            string txt = File.ReadAllText(@"testgit\file1");
            Assert.IsTrue(txt == "Commit this!This is another test\r\n");

        }

        [TestMethod]
        public void StashWontPopIfConflictedOnUnStagedFile()
        {
            File.WriteAllText(@"testgit\file1", "This is a test");

            GitStashWrapper git = new GitStashWrapper("testgit", GetEventService(), GetLogger(), GetProjects());
            GitStashOptions options = new GitStashOptions { Untracked = true };
            options.Message = "Testing";
            IGitStashResults results = git.SaveStash(options);
            Assert.IsTrue(results.Success);
            Assert.IsTrue(git.Stashes.Count == 1);
            Assert.IsFalse(File.Exists("file1"));

            using (StreamWriter sw = File.AppendText(@"testgit\file1"))
            {
                sw.WriteLine("This is another test");
            }

            results = git.PopStash(new GitStashOptions(), 0);
            Assert.IsFalse(results.Success);
            Assert.IsTrue(git.Stashes.Count == 1);
            string txt = File.ReadAllText(@"testgit\file1");
            Assert.IsTrue(txt == "Commit this!This is another test\r\n");
        }

        [TestMethod]
        [ExpectedException(typeof(GitStashInvalidIndexException))]
        public void TestPopthrowsExceptionWithInvalidIndex()
        {
            FileStream fs = File.Create(@"testgit\file2");
            fs.Close();
            GitStashWrapper git = new GitStashWrapper("testgit", GetEventService(), GetLogger(), GetProjects());
            GitStashOptions options = new GitStashOptions { Untracked = true };
            options.Message = "Testing";
            IGitStashResults results = git.SaveStash(options);
            Assert.IsTrue(results.Success);
            Assert.IsTrue(git.Stashes.Count == 1);
            Assert.IsFalse(File.Exists("file2"));

            results = git.PopStash(new GitStashOptions(), -1);
            Assert.IsFalse(results.Success);
            Assert.IsTrue(git.Stashes.Count == 1);
            Assert.IsFalse(File.Exists(@"testgit\file2"));
        }

        [TestMethod]
        [ExpectedException(typeof(GitStashInvalidIndexException))]
        public void TestApplythrowsExceptionWithInvalidIndex()
        {
            FileStream fs = File.Create(@"testgit\file2");
            fs.Close();
            GitStashWrapper git = new GitStashWrapper("testgit", GetEventService(), GetLogger(), GetProjects());
            GitStashOptions options = new GitStashOptions { Untracked = true };
            options.Message = "Testing";
            IGitStashResults results = git.SaveStash(options);
            Assert.IsTrue(results.Success);
            Assert.IsTrue(git.Stashes.Count == 1);
            Assert.IsFalse(File.Exists("file2"));

            results = git.ApplyStash(new GitStashOptions(), 2);
            Assert.IsFalse(results.Success);
            Assert.IsTrue(git.Stashes.Count == 1);
            Assert.IsFalse(File.Exists(@"testgit\file2"));
        }

        [TestMethod]
        [ExpectedException(typeof(GitStashInvalidIndexException))]
        public void TestDeletethrowsExceptionWithInvalidIndex()
        {
            FileStream fs = File.Create(@"testgit\file2");
            fs.Close();
            GitStashWrapper git = new GitStashWrapper("testgit", GetEventService(), GetLogger(), GetProjects());
            GitStashOptions options = new GitStashOptions { Untracked = true };
            options.Message = "Testing";
            IGitStashResults results = git.SaveStash(options);
            Assert.IsTrue(results.Success);
            Assert.IsTrue(git.Stashes.Count == 1);
            Assert.IsFalse(File.Exists("file2"));

            results = git.DropStash(new GitStashOptions(), 2);
            Assert.IsFalse(results.Success);
            Assert.IsTrue(git.Stashes.Count == 1);
            Assert.IsFalse(File.Exists(@"testgit\file2"));
        }

        [TestMethod]
        public void TestStashesReturnsTheProperIndex()
        {
            FileStream fs = File.Create(@"testgit\file2");
            fs.Close();

            GitStashWrapper git = new GitStashWrapper("testgit", GetEventService(), GetLogger(), GetProjects());
            GitStashOptions options = new GitStashOptions { Untracked = true };
            options.Message = "one";
            IGitStashResults results = git.SaveStash(options);
            Assert.IsTrue(results.Success);
            Assert.IsTrue(git.Stashes.Count == 1);
            Assert.IsFalse(File.Exists("file2"));

            fs = File.Create(@"testgit\file2");
            fs.Close();

            options.Message = "two";
            results = git.SaveStash(options);

            Assert.IsTrue(results.Success);
            Assert.IsTrue(git.Stashes.Count == 2);
            Assert.IsFalse(File.Exists("file2"));

            IGitStash recent = git.Stashes[0];
            IGitStash older = git.Stashes[1];

            Assert.IsTrue(recent.Index == 0);
            Assert.IsTrue(recent.Message == "two");

            Assert.IsTrue(older.Index == 1);
            Assert.IsTrue(older.Message == "one");
        }

        [TestMethod]
        public void Bug_57_CreateStashWithNewUntrackedChangeDoesntThrowException()
        {
            FileStream fs = File.Create(@"testgit\file2");
            fs.Close();
            GitStashWrapper git = new GitStashWrapper("testgit", GetEventService(), GetLogger(), GetProjects());
            GitStashOptions options = new GitStashOptions { Untracked = false, All=false, Ignored = false, Index = false, KeepIndex = false, Message="bug_57" };

            IGitStashResults results = git.SaveStash(options);
            Assert.IsFalse(results.Success);
            Assert.IsTrue(git.Stashes.Count == 0);
            Assert.IsTrue(File.Exists(@"testgit\file2"));
        }
    }
}
