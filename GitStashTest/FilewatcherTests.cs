using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using GitStash;
using System.Threading;

namespace GitStashTest
{
    /// <summary>
    /// Summary description for FilewatcherTests
    /// </summary>
    [TestClass]
    public class FilewatcherTests
    {
        public FilewatcherTests()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region Additional test attributes
        //
        // You can use the following additional attributes as you write your tests:
        //
         //Use ClassInitialize to run code before running the first test in the class
         [ClassInitialize()]
         public static void MyClassInitialize(TestContext testContext)
        {
            Directory.CreateDirectory("testwatcher");
        }
        
         //Use ClassCleanup to run code after all tests in a class have run
         [ClassCleanup()]
         public static void MyClassCleanup()
        {
            foreach(var file in Directory.EnumerateFiles("testwatcher"))
            {
                File.Delete(file);
            }
            Directory.Delete("testwatcher");
        }
        //
        // Use TestInitialize to run code before running each test 
        // [TestInitialize()]
        // public void MyTestInitialize() { }
        //
        // Use TestCleanup to run code after each test has run
        // [TestCleanup()]
        // public void MyTestCleanup() { }
        //
        #endregion

        [TestMethod]
        public void FiresEventWhenDirectoryChanged()
        {
            GitStashFileWatcher watcher = new GitStashFileWatcher();
            bool eventFired = false;
            AutoResetEvent waitHandle = new AutoResetEvent(false);
            watcher.ProjectDirectoryChanged += (o, e) => { eventFired = true; waitHandle.Set(); };

            watcher.ChangeDirectory("testwatcher");
            waitHandle.WaitOne(5000);
            Assert.IsTrue(eventFired);
        }

        [TestMethod]
        public void FiresEventWhenFileCreated()
        {
            GitStashFileWatcher watcher = new GitStashFileWatcher();
            bool eventFired = false;
            AutoResetEvent waitHandle = new AutoResetEvent(false);
            watcher.ProjectDirectoryChanged += (o, e) => { eventFired = true; waitHandle.Set(); };
            
            watcher.ChangeDirectory("testwatcher");
            File.Create(@"testwatcher\file1").Close();
            waitHandle.WaitOne(5000);
            Assert.IsTrue(eventFired);
        }

        [TestMethod]
        public void FiresEventWhenFileChanged()
        {
            GitStashFileWatcher watcher = new GitStashFileWatcher();
            bool eventFired = false;
            AutoResetEvent waitHandle = new AutoResetEvent(false);
            watcher.ProjectDirectoryChanged += (o, e) => { eventFired = true; waitHandle.Set(); };
            File.Create(@"testwatcher\file1").Close();
                watcher.ChangeDirectory("testwatcher");
            File.WriteAllText(@"testwatcher\file1", "foo");
            waitHandle.WaitOne(5000);
            Assert.IsTrue(eventFired);
        }

        [TestMethod]
        public void FiresEventWhenFileDeleted()
        {
            GitStashFileWatcher watcher = new GitStashFileWatcher();
            bool eventFired = false;
            AutoResetEvent waitHandle = new AutoResetEvent(false);
            watcher.ProjectDirectoryChanged += (o, e) => { eventFired = true; waitHandle.Set(); };
            File.Create(@"testwatcher\file1").Close();
            watcher.ChangeDirectory("testwatcher");
            File.Delete(@"testwatcher\file1");
            waitHandle.WaitOne(5000);
            Assert.IsTrue(eventFired);
        }

        [TestMethod]
        public void FiresEventWhenFileRenamed()
        {
            GitStashFileWatcher watcher = new GitStashFileWatcher();
            bool eventFired = false;
            AutoResetEvent waitHandle = new AutoResetEvent(false);
            watcher.ProjectDirectoryChanged += (o, e) => { eventFired = true; waitHandle.Set(); };
            File.Create(@"testwatcher\file1").Close();
            watcher.ChangeDirectory("testwatcher");
            File.Move(@"testwatcher\file1", @"testwatcher\file2");
            waitHandle.WaitOne(5000);
            Assert.IsTrue(eventFired);
        }
    }
}
