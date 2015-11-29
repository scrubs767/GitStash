using GitStash.ViewModels;
using GitWrapper;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using TeamExplorer.Common;

namespace GitStashTest
{
    [TestClass]
    public class ReccomendedActionsViewModelsTests
    {
        private void TestVmFiresEvent(Action<IGitStashWrapper> eventExpression, EventArgs args, string PropertyName)
        {
            var wrapper = new Mock<IGitStashWrapper>();
            var page = new Mock<ITeamExplorerBase>();
            RecommendedActionsViewModel vm = new RecommendedActionsViewModel(wrapper.Object, page.Object);
            AutoResetEvent waitHandle = new AutoResetEvent(false);
            bool eventWasDispatched = false;
            vm.PropertyChanged += (s, a) =>
            {
                if (a.PropertyName == PropertyName) eventWasDispatched = true;
                if (eventWasDispatched) waitHandle.Set();
            };
            wrapper.Raise(eventExpression, args);
            waitHandle.WaitOne(5000);
            Assert.IsTrue(eventWasDispatched);
        }

        [TestMethod]
        public void VmFiresPropertyCanCreateStashEvent()
        {
            TestVmFiresEvent(e => e.StashesChangedEvent += null, new StashesChangedEventArgs(), "CanCreateStash");
        }

        [TestMethod]
        public void VmFiresPropertyCanClickCreateButtonEvent()
        {
            TestVmFiresEvent(e => e.StashesChangedEvent += null, new StashesChangedEventArgs(), "CanClickCreateButton");
        }

        [TestMethod]
        public void StashAllSetsProperties()
        {
            var page = new Mock<ITeamExplorerBase>();
            var wrapper = new Mock<IGitStashWrapper>();
            IList<IGitStash> gitStashes = new List<IGitStash>();
            var gitStash = new Mock<IGitStash>();
            gitStash.Setup(s => s.Index).Returns(0);
            gitStash.Setup(s => s.Message).Returns("test");
            gitStashes.Add(gitStash.Object);
            wrapper.Setup(w => w.Stashes).Returns(gitStashes);

            RecommendedActionsViewModel vm = new RecommendedActionsViewModel(wrapper.Object, page.Object);
            vm.StashAll = true;
            Assert.IsFalse(vm.StashKeepIndex);
            Assert.IsFalse(vm.StashUntracked);
        }

        [TestMethod]
        public void StashAllFiresStashUntrackedEvent()
        {
            var page = new Mock<ITeamExplorerBase>();
            var wrapper = new Mock<IGitStashWrapper>();
            RecommendedActionsViewModel vm = new RecommendedActionsViewModel(wrapper.Object, page.Object);

            AutoResetEvent waitHandle = new AutoResetEvent(false);
            bool clickeventWasDispatched = false;
            vm.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == "StashUntracked") clickeventWasDispatched = true;
                if (clickeventWasDispatched) waitHandle.Set();
            };
            vm.StashAll = true;            
            waitHandle.WaitOne(5000);
            Assert.IsTrue(clickeventWasDispatched);
        }
        [TestMethod]
        public void StashAllFiresStashIgnoredEvent()
        {
            var page = new Mock<ITeamExplorerBase>();
            var wrapper = new Mock<IGitStashWrapper>();
            RecommendedActionsViewModel vm = new RecommendedActionsViewModel(wrapper.Object, page.Object);

            AutoResetEvent waitHandle = new AutoResetEvent(false);
            bool clickeventWasDispatched = false;
            vm.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == "StashIgnored") clickeventWasDispatched = true;
                if (clickeventWasDispatched) waitHandle.Set();
            };
            vm.StashAll = true;
            waitHandle.WaitOne(5000);
            Assert.IsTrue(clickeventWasDispatched);
        }

        [TestMethod]
        public void StashUntrackedSetsProperties()
        {
            var page = new Mock<ITeamExplorerBase>();
            var wrapper = new Mock<IGitStashWrapper>();
            IList<IGitStash> gitStashes = new List<IGitStash>();
            var gitStash = new Mock<IGitStash>();
            gitStash.Setup(s => s.Index).Returns(0);
            gitStash.Setup(s => s.Message).Returns("test");
            gitStashes.Add(gitStash.Object);
            wrapper.Setup(w => w.Stashes).Returns(gitStashes);

            RecommendedActionsViewModel vm = new RecommendedActionsViewModel(wrapper.Object, page.Object);
            vm.StashKeepIndex = true;
            Assert.IsFalse(vm.StashAll);
            Assert.IsFalse(vm.StashUntracked);
        }
        [TestMethod]
        public void StashUntrackedFiresStashIgnoredEvents()
        {
            var page = new Mock<ITeamExplorerBase>();
            var wrapper = new Mock<IGitStashWrapper>();
            RecommendedActionsViewModel vm = new RecommendedActionsViewModel(wrapper.Object, page.Object);

            AutoResetEvent waitHandle = new AutoResetEvent(false);
            bool clickeventWasDispatched = false;
            vm.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == "StashIgnored") clickeventWasDispatched = true;
                if (clickeventWasDispatched) waitHandle.Set();
            };
            vm.StashUntracked = true;
            waitHandle.WaitOne(5000);
            Assert.IsTrue(clickeventWasDispatched);
        }
        [TestMethod]
        public void StashUntrackedFiresStashAllEvents()
        {
            var page = new Mock<ITeamExplorerBase>();
            var wrapper = new Mock<IGitStashWrapper>();
            RecommendedActionsViewModel vm = new RecommendedActionsViewModel(wrapper.Object, page.Object);

            AutoResetEvent waitHandle = new AutoResetEvent(false);
            bool clickeventWasDispatched = false;
            vm.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == "StashAll") clickeventWasDispatched = true;
                if (clickeventWasDispatched) waitHandle.Set();
            };
            vm.StashUntracked = true;
            waitHandle.WaitOne(5000);
            Assert.IsTrue(clickeventWasDispatched);
        }

        [TestMethod]
        public void StashIgnoredSetsProperties()
        {
            var page = new Mock<ITeamExplorerBase>();
            var wrapper = new Mock<IGitStashWrapper>();
            IList<IGitStash> gitStashes = new List<IGitStash>();
            var gitStash = new Mock<IGitStash>();
            gitStash.Setup(s => s.Index).Returns(0);
            gitStash.Setup(s => s.Message).Returns("test");
            gitStashes.Add(gitStash.Object);
            wrapper.Setup(w => w.Stashes).Returns(gitStashes);

            RecommendedActionsViewModel vm = new RecommendedActionsViewModel(wrapper.Object, page.Object);
            vm.StashUntracked = true;
            Assert.IsFalse(vm.StashKeepIndex);
            Assert.IsFalse(vm.StashAll);
        }

        [TestMethod]
        public void StashIgnoredFiresStashAllEvents()
        {
            var page = new Mock<ITeamExplorerBase>();
            var wrapper = new Mock<IGitStashWrapper>();
            RecommendedActionsViewModel vm = new RecommendedActionsViewModel(wrapper.Object, page.Object);

            AutoResetEvent waitHandle = new AutoResetEvent(false);
            bool clickeventWasDispatched = false;
            vm.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == "StashAll") clickeventWasDispatched = true;
                if (clickeventWasDispatched) waitHandle.Set();
            };
            vm.StashIgnored = true;
            waitHandle.WaitOne(5000);
            Assert.IsTrue(clickeventWasDispatched);
        }
        [TestMethod]
        public void StashIgnoredFiresStashUntrackedEvents()
        {
            var page = new Mock<ITeamExplorerBase>();
            var wrapper = new Mock<IGitStashWrapper>();
            RecommendedActionsViewModel vm = new RecommendedActionsViewModel(wrapper.Object, page.Object);

            AutoResetEvent waitHandle = new AutoResetEvent(false);
            bool clickeventWasDispatched = false;
            vm.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == "StashUntracked") clickeventWasDispatched = true;
                if (clickeventWasDispatched) waitHandle.Set();
            };
            vm.StashIgnored = true;
            waitHandle.WaitOne(5000);
            Assert.IsTrue(clickeventWasDispatched);
        }

        [TestMethod]
        public void OnClickCreateStashButtonCallsSave()
        {
            var page = new Mock<ITeamExplorerBase>();
            var wrapper = new Mock<IGitStashWrapper>();
            var results = new Mock<IGitStashResults>();
            results.Setup(r => r.Success).Returns(true);
            AutoResetEvent waitHandle = new AutoResetEvent(false);
            wrapper.Setup(w => w.SaveStash(It.IsAny<IGitStashSaveOptions>()))
                .Returns(results.Object)
                .Callback(() => waitHandle.Set())
                .Verifiable();
            RecommendedActionsViewModel vm = new RecommendedActionsViewModel(wrapper.Object, page.Object);
            vm.CreateStashButtonCommand.Execute(null);
            waitHandle.WaitOne(5000);

            wrapper.Verify(w => w.SaveStash(It.IsAny<IGitStashSaveOptions>()));
        }

        [TestMethod]
        public void OnClickCreateStashButtonFiresStashesEvents()
        {
            var page = new Mock<ITeamExplorerBase>();
            var wrapper = new Mock<IGitStashWrapper>();
            var results = new Mock<IGitStashResults>();
            results.Setup(r => r.Success).Returns(true);
            wrapper.Setup(w => w.SaveStash(It.IsAny<IGitStashSaveOptions>()))
                .Returns(results.Object)
                .Verifiable();
            RecommendedActionsViewModel vm = new RecommendedActionsViewModel(wrapper.Object, page.Object);
            ICommand command = vm.CreateStashButtonCommand;

            AutoResetEvent waitHandle = new AutoResetEvent(false);
            bool clickeventWasDispatched = false;
            vm.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == "Stashes") clickeventWasDispatched = true;
                if (clickeventWasDispatched) waitHandle.Set();
            };
            command.Execute(null);
            waitHandle.WaitOne(5000);
            Assert.IsTrue(clickeventWasDispatched);
        }
        [TestMethod]
        public void OnClickCreateStashButtonFiresNewStashMessageEvents()
        {
            var page = new Mock<ITeamExplorerBase>();
            var wrapper = new Mock<IGitStashWrapper>();
            var results = new Mock<IGitStashResults>();
            results.Setup(r => r.Success).Returns(true);
            wrapper.Setup(w => w.SaveStash(It.IsAny<IGitStashSaveOptions>()))
                .Returns(results.Object)
                .Verifiable();
            RecommendedActionsViewModel vm = new RecommendedActionsViewModel(wrapper.Object, page.Object);
            ICommand command = vm.CreateStashButtonCommand;

            AutoResetEvent waitHandle = new AutoResetEvent(false);
            bool clickeventWasDispatched = false;
            vm.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == "NewStashMessage") clickeventWasDispatched = true;
                if (clickeventWasDispatched) waitHandle.Set();
            };
            command.Execute(null);
            waitHandle.WaitOne(5000);
            Assert.IsTrue(clickeventWasDispatched);
        }
        [TestMethod]
        public void OnClickCreateStashButtonFiresCreateStashButtonCommandEvents()
        {
            var page = new Mock<ITeamExplorerBase>();
            var wrapper = new Mock<IGitStashWrapper>();
            var results = new Mock<IGitStashResults>();
            results.Setup(r => r.Success).Returns(true);
            wrapper.Setup(w => w.SaveStash(It.IsAny<IGitStashSaveOptions>()))
                .Returns(results.Object)
                .Verifiable();
            RecommendedActionsViewModel vm = new RecommendedActionsViewModel(wrapper.Object, page.Object);
            ICommand command = vm.CreateStashButtonCommand;

            AutoResetEvent waitHandle = new AutoResetEvent(false);
            bool clickeventWasDispatched = false;
            vm.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == "CreateStashButtonCommand") clickeventWasDispatched = true;
                if (clickeventWasDispatched) waitHandle.Set();
            };
            command.Execute(null);
            waitHandle.WaitOne(5000);
            Assert.IsTrue(clickeventWasDispatched);
        }
    }
}
