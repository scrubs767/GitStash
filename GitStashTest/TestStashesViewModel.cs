using GitStash.ViewModels;
using GitWrapper;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using TeamExplorer.Common;

namespace GitStashTest
{
    [TestClass]
    public class TestStashesViewModel
    {
        [TestMethod]
        public void GitWrapperStashesChangedEventFiresPropertyChangedEvent()
        {
            var page = new Mock<ITeamExplorerBase>();
            var wrapper = new Mock<IGitStashWrapper>();
            IList<IGitStash> gitStashes = new List<IGitStash>();
            var gitStash = new Mock<IGitStash>();
            gitStash.Setup(s => s.Index).Returns(0);
            gitStash.Setup(s => s.Message).Returns("test");
            gitStashes.Add(gitStash.Object);
            wrapper.Setup(w => w.Stashes).Returns(gitStashes);

            StashesViewModel vm = new StashesViewModel(wrapper.Object, page.Object);
            AutoResetEvent waitHandle = new AutoResetEvent(false);
            bool eventWasDispatched = false;

            vm.PropertyChanged += (sender, args) => { waitHandle.Set(); eventWasDispatched = true; };
            wrapper.Raise(e => e.StashesChangedEvent += null, new StashesChangedEventArgs());
            waitHandle.WaitOne(5000);
            Assert.IsTrue(eventWasDispatched);
        }

        [TestMethod]
        public void PropertyStashesReturnsCorrectly()
        {
            var page = new Mock<ITeamExplorerBase>();
            var wrapper = new Mock<IGitStashWrapper>();
            IList<IGitStash> gitStashes = new List<IGitStash>();
            var gitStash = new Mock<IGitStash>();
            gitStash.Setup(s => s.Index).Returns(0);
            gitStash.Setup(s => s.Message).Returns("test");
            gitStashes.Add(gitStash.Object);
            wrapper.Setup(w => w.Stashes).Returns(gitStashes);

            StashesViewModel vm = new StashesViewModel(wrapper.Object, page.Object);

            Assert.IsTrue(vm.Stashes.Count() == 1);
            Assert.IsTrue(vm.Stashes.ElementAt(0).Stash.GetHashCode() == gitStash.Object.GetHashCode());
        }

        [TestMethod]
        public void AfterDeleteEventHandlerFiresPropertyChangedEvent()
        {
            var results = new Mock<IGitStashResults>();
            var page = new Mock<ITeamExplorerBase>();
            var wrapper = new Mock<IGitStashWrapper>();
            results.Setup(r => r.Success).Returns(true);
            results.Setup(r => r.Message).Returns("test");
            IList<IGitStash> gitStashes = new List<IGitStash>();
            var gitStash = new Mock<IGitStash>();
            gitStash.Setup(s => s.Index).Returns(0);
            gitStash.Setup(s => s.Message).Returns("test");
            gitStashes.Add(gitStash.Object);
            wrapper.Setup(w => w.Stashes).Returns(gitStashes);
            wrapper.Setup(w => w.DropStash(
                It.IsAny<IGitStashDropOptions>(),
                It.Is<int>(i => i == 0)))
                .Returns(results.Object);

            StashesViewModel vm = new StashesViewModel(wrapper.Object, page.Object);
            AutoResetEvent waitHandle = new AutoResetEvent(false);
            bool eventWasDispatched = false;
            vm.PropertyChanged += (o, e) => { waitHandle.Set(); eventWasDispatched = true; };

            Assert.IsTrue(vm.Stashes.Count() == 1);
            StashViewModel stashVM = vm.Stashes.ElementAt(0);
            stashVM.DeleteDropDownCommand.Execute(null);
            waitHandle.WaitOne(5000);

            Assert.IsTrue(eventWasDispatched);
        }
    }
}
