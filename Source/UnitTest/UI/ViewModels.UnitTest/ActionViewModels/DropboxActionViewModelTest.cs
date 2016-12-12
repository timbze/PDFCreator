using System.Collections.Generic;
using NSubstitute;
using NUnit.Framework;
using pdfforge.DynamicTranslator;
using pdfforge.Obsidian;
using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.UI.Interactions;
using pdfforge.PDFCreator.UI.ViewModels.ActionViewModels;
using pdfforge.PDFCreator.UI.ViewModels.Helper;

namespace pdfforge.PDFCreator.UnitTest.UI.ViewModels.ActionViewModels
{
    [TestFixture]
    internal class DropboxActionViewModelTest
    {
        [SetUp]
        public void Setup()
        {
            _invoker = Substitute.For<IInteractionInvoker>();
            _translator = Substitute.For<ITranslator>();
            _dropboxActionViewModel = new DropboxActionViewModel(_translator, _invoker);
            _dropboxActionViewModel.Accounts = new Accounts();
            _dropboxActionViewModel.CurrentProfile = new ConversionProfile();
            _invoker.When(x => x.Invoke(Arg.Any<DropboxInteraction>())).Do(info =>
            {
                info.Arg<DropboxInteraction>().AccessToken = AccessToken;
                info.Arg<DropboxInteraction>().AccountId = AccountID;
                info.Arg<DropboxInteraction>().AccountInfo = AccountInfo;
                info.Arg<DropboxInteraction>().Success = true;
            });
        }

        private IInteractionInvoker _invoker;
        private DropboxActionViewModel _dropboxActionViewModel;
        private ITranslator _translator;

        private const string AccessToken = "AccessToken";
        private const string AccountID = "AccountID";
        private const string AccountInfo = "AccountInfo";

        [Test]
        public void AuthoriseDropboxUser_CheckIsCurrentAccountSet()
        {
            _dropboxActionViewModel.AddDropboxAccountCommand.Execute(null);
            Assert.AreEqual(_dropboxActionViewModel.CurrentProfile.DropboxSettings.AccountId, AccountID);
        }
        [Test]
        public void RemoveDropboxUser_CurrentAccountIsRemovedFromSystem()
        {
            var dropboxAccountList = new List<DropboxAccount>();
            dropboxAccountList.Add(new DropboxAccount() { AccountId = _dropboxActionViewModel.CurrentProfile.DropboxSettings.AccountId });
            var accounts = new Accounts();
            accounts.DropboxAccounts = dropboxAccountList;
            _dropboxActionViewModel.Accounts = accounts;
            _dropboxActionViewModel.RemoveDropboxAccountCommand.Execute(null);
            Assert.AreEqual(_dropboxActionViewModel.Accounts.DropboxAccounts.Count, 0);
            Assert.AreEqual(_dropboxActionViewModel.CurrentProfile.DropboxSettings.AccountId, string.Empty);
        }

        [Test]
        public void RemoveDropboxUser_CanExecuteChangedGetsCalled()
        {
            var account = new DropboxAccount { AccountId = "1" };
            _dropboxActionViewModel.DropboxAccounts.Add(account);
            bool wasCalled = false;
            _dropboxActionViewModel.RemoveDropboxAccountCommand.CanExecuteChanged += (sender, args) => wasCalled = true;
            _dropboxActionViewModel.CurrentProfile.DropboxSettings.AccountId = account.AccountId;
            _dropboxActionViewModel.RemoveDropboxAccountCommand.Execute(null);
            Assert.IsTrue(wasCalled);
        }

        [Test]
        public void RemoveDropboxCanExecute_DropboxAccountsListIsNotEmpty_ReturnsTrue()
        {
            var dropboxAccountList = new List<DropboxAccount>();
            dropboxAccountList.Add(new DropboxAccount() { AccountId = _dropboxActionViewModel.CurrentProfile.DropboxSettings.AccountId });
            var accounts = new Accounts();
            accounts.DropboxAccounts = dropboxAccountList;
            _dropboxActionViewModel.Accounts = accounts;
            Assert.IsTrue(_dropboxActionViewModel.RemoveDropboxAccountCommand.IsExecutable);
        }

        [Test]
        public void RemoveDropboxCanExecute_DropboxAccountsListIsEmpty_ReturnsFalse()
        {
            var dropboxAccountList = new List<DropboxAccount>();
            var accounts = new Accounts();
            accounts.DropboxAccounts = dropboxAccountList;
            _dropboxActionViewModel.Accounts = accounts;
            Assert.IsFalse(_dropboxActionViewModel.RemoveDropboxAccountCommand.IsExecutable);
        }

        [Test]
        public void AuthoriseDropboxUserAndAddItToAccounts_ReturnCountBiggerThan0()
        {
            _dropboxActionViewModel.AddDropboxAccountCommand.Execute(null);
            Assert.IsTrue(_dropboxActionViewModel.DropboxAccounts.Count == 1);
        }

        [Test]
        public void GetIsDropboxOptionIsEnabled_ReturnsTrue()
        {
            _dropboxActionViewModel.CurrentProfile.DropboxSettings.Enabled = true;
            Assert.IsTrue(_dropboxActionViewModel.IsEnabled);
        }

        [Test]
        public void SetIsDropboxOptionIsEnabled_ReturnsTrue()
        {
            _dropboxActionViewModel.IsEnabled = true;
            Assert.IsTrue(_dropboxActionViewModel.IsEnabled);
        }

        [Test]
        public void GetContextBasedHelpTopic_ReturnsHelpTopicDropbox()
        {
            var helpTopic = _dropboxActionViewModel.GetContextBasedHelpTopic();
            Assert.AreEqual(HelpTopic.Dropbox, helpTopic);
        }
    }
}
