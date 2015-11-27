using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using GitWrapper;
using Moq;
using System.Threading;

namespace GitStashTest
{
    /// <summary>
    /// Summary description for StashViewModelTest
    /// </summary>
    [TestClass]
    public class StashViewModelTest
    {
        private Mock<IGitStashWrapper> GetWrapper(int index, bool success)
        {
            var wrapper = new Mock<IGitStashWrapper>();
            var results = new Mock<IGitStashResults>();
            results.Setup(r => r.Success).Returns(success);
            wrapper.Setup(w => w.ApplyStash(It.IsAny<IGitStashApplyOptions>(), It.Is<int>(i => i == 0))).Returns(results.Object);
            wrapper.Setup(w => w.PopStash(It.IsAny<IGitStashPopOptions>(), It.Is<int>(i => i == 0))).Returns(results.Object);
            wrapper.Setup(w => w.DropStash(It.IsAny<IGitStashDropOptions>(), It.Is<int>(i => i == 0))).Returns(results.Object);

            return wrapper;
        }

        private Mock<IGitStash> GetStash(int index, string message)
        {
            var stash = new Mock<IGitStash>();
            stash.Setup(s => s.Index).Returns(index);
            stash.Setup(s => s.Message).Returns(message);
            return stash;
        }

        [TestMethod]
        public void TestPopCallsWrapper()
        {
            var stash = GetStash(0, "stash");
            var wrapper = new Mock<IGitStashWrapper>();
            var results = new Mock<IGitStashResults>();
            results.Setup(r => r.Success).Returns(true);

            AutoResetEvent waitHandle = new AutoResetEvent(false);

            wrapper.Setup(w => w.PopStash(
                It.IsAny<IGitStashPopOptions>(),
                It.Is<int>(i => i == 0)))
                .Callback(() => waitHandle.Set())
                .Returns(results.Object)
                .Verifiable();

            GitStash.ViewModels.StashViewModel vm = new GitStash.ViewModels.StashViewModel(wrapper.Object, stash.Object);
            Assert.IsTrue(vm.PopDropDownCommand != null);
            vm.PopDropDownCommand.Execute(null);
            waitHandle.WaitOne(5000);
            wrapper.Verify(w => w.PopStash(It.IsAny<IGitStashPopOptions>(), 0));
        }

        public void TestApplyCallsWrapper()
        {
            var stash = GetStash(0, "stash");
            var wrapper = new Mock<IGitStashWrapper>();
            var results = new Mock<IGitStashResults>();
            results.Setup(r => r.Success).Returns(true);

            AutoResetEvent waitHandle = new AutoResetEvent(false);
            wrapper.Setup(w => w.ApplyStash(
                It.IsAny<IGitStashApplyOptions>(),
                It.Is<int>(i => i == 0)))
                .Callback(() => waitHandle.Set())
                .Returns(results.Object)
                .Verifiable();

            GitStash.ViewModels.StashViewModel vm = new GitStash.ViewModels.StashViewModel(wrapper.Object, stash.Object);
            Assert.IsTrue(vm.ApplyDropDownCommand != null);
            vm.PopDropDownCommand.Execute(null);
            waitHandle.WaitOne(5000);
            wrapper.Verify(w => w.ApplyStash(It.IsAny<IGitStashApplyOptions>(), 0));
        }

        public void TestDropCallsWrapper()
        {
            var stash = GetStash(0, "stash");
            var wrapper = new Mock<IGitStashWrapper>();
            var results = new Mock<IGitStashResults>();
            results.Setup(r => r.Success).Returns(true);

            AutoResetEvent waitHandle = new AutoResetEvent(false);
            wrapper.Setup(w => w.DropStash(
                It.IsAny<IGitStashDropOptions>(),
                It.Is<int>(i => i == 0)))
                .Callback(() => waitHandle.Set())
                .Returns(results.Object)
                .Verifiable();

            GitStash.ViewModels.StashViewModel vm = new GitStash.ViewModels.StashViewModel(wrapper.Object, stash.Object);
            Assert.IsTrue(vm.ApplyDropDownCommand != null);
            vm.PopDropDownCommand.Execute(null);
            waitHandle.WaitOne(5000);
            wrapper.Verify(w => w.DropStash(It.IsAny<IGitStashDropOptions>(), 0));
        }
    }
}

