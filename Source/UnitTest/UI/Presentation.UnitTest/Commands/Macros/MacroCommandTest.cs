using NUnit.Framework;
using pdfforge.PDFCreator.Core.Services.Macros;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Presentation.UnitTest.Commands.Macros
{
    [TestFixture]
    public class MacroCommandTest
    {
        //private MacroCommand _macroCommand;
        //private TestWaitableCommand _waitableCommand;
        //private ICommandLocator _locator;

        //[SetUp]
        //public void Setup()
        //{
        //    _waitableCommand = new TestWaitableCommand();
        //    _locator = Substitute.For<ICommandLocator>();
        //    // todo: change to MacroCommandBuilder
        //    // todo: _macroCommand = new MacroCommand(_locator);
        //}

        //[Test]
        //public void AddCommand_CommandIsNull_DoesNotThrowError()
        //{
        //  // todo:  Assert.DoesNotThrow(() => _macroCommand.AddCommand(null));
        //}

        //[Test]
        //public void AddCommand_CommandIsNotNull_DoesNotThrowError()
        //{
        //    // todo:   Assert.DoesNotThrow(() => _macroCommand.AddCommand(_waitableCommand));
        //}

        //[Test]
        //public void AddCommandLocator_LocatorIsNull_DoesNotThrowError()
        //{
        //    // todo: Assert.DoesNotThrow(() => new MacroCommand(null));
        //}

        //[Test]
        //public void AddCommandByType_CommandNotNull_LocatorWasCalled()
        //{
        //    bool wasCalled = false;
        //    _locator.GetCommand<IWaitableCommand>().Returns(_waitableCommand);
        //    _locator.When(locator => locator.GetCommand<TestWaitableCommand>()).Do(info => wasCalled = true);
        //    // todo: _macroCommand.AddCommand<TestWaitableCommand>();
        //    Assert.IsTrue(wasCalled);
        //}

        //[Test]
        //public void AddCommandByType_CommandIsNull_DoesNotThrowError()
        //{
        //    bool wasCalled = false;

        //    _locator.When(locator => locator.GetCommand<TestWaitableCommand>()).Do(info => wasCalled = true);
        //    // todo:   Assert.DoesNotThrow(() => _macroCommand.AddCommand<TestWaitableCommand>());
        //    Assert.IsTrue(wasCalled);
        //}

        //[Test]
        //public void NoCommandsGiven_CanExecute_ReturnsTrue()
        //{
        //    Assert.IsTrue(_macroCommand.CanExecute(null));
        //}

        //[Test]
        //public void SingleCommandGivenAndCanExecute_CallCanExecute_ReturnsTrue()
        //{
        //    _macroCommand.AddCommand(_waitableCommand);
        //    Assert.IsTrue(_macroCommand.CanExecute(null));
        //}

        //[Test]
        //public void MultipleCommandGivenAndCanExecute_CallCanExecute_ReturnsTrue()
        //{
        //    var command2 = Substitute.For<IWaitableCommand>();

        //    command2.CanExecute(null).Returns(true);

        //    _macroCommand.AddCommand(_waitableCommand);
        //    _macroCommand.AddCommand(command2);

        //    Assert.IsTrue(_macroCommand.CanExecute(null));
        //}

        //[Test]
        //public void MultipleCommandGivenAndFirstCannotExecute_CallCanExecute_ReturnsFalse()
        //{
        //    var command2 = Substitute.For<IWaitableCommand>();

        //    command2.CanExecute(null).Returns(true);
        //    _waitableCommand.IsExecutable = false;

        //    _macroCommand.AddCommand(_waitableCommand);
        //    _macroCommand.AddCommand(command2);

        //    Assert.IsFalse(_macroCommand.CanExecute(null));
        //}

        //[Test]
        //public void MultipleCommandGivenAndAnyCannotExecute_CallCanExecute_ReturnsFalse()
        //{
        //    var command2 = Substitute.For<IWaitableCommand>();

        //    command2.CanExecute(null).Returns(false);
        //    _waitableCommand.IsExecutable = false;

        //    _macroCommand.AddCommand(_waitableCommand);
        //    _macroCommand.AddCommand(command2);

        //    Assert.IsFalse(_macroCommand.CanExecute(null));
        //}

        //[Test]
        //public void SingleCommand_Execute_NoError()
        //{
        //    _macroCommand.AddCommand(_waitableCommand);

        //    _macroCommand.Execute(new object());

        //    Assert.IsTrue(_waitableCommand.IsDoneWasRaised);
        //}

        //[Test]
        //public void SingleCommandResponseError_Execute_NoError()
        //{
        //    var waitableCommand = new TestWaitableCommand();
        //    _macroCommand.AddCommand(waitableCommand);

        //    _macroCommand.Execute(new object());

        //    Assert.IsTrue(waitableCommand.IsDoneWasRaised);
        //}

        //[Test]
        //public void MultipleCommand_Execute_NoError()
        //{
        //    var waitableCommand1 = new TestWaitableCommand();
        //    var waitableCommand2 = new TestWaitableCommand();

        //    _macroCommand.AddCommand(waitableCommand1);
        //    _macroCommand.AddCommand(waitableCommand2);

        //    _macroCommand.Execute(new object());

        //    Assert.IsTrue(waitableCommand1.IsDoneWasRaised);
        //    Assert.IsTrue(waitableCommand2.IsDoneWasRaised);

        //    // test that the listener for IsDone was deregistered
        //    Assert.IsFalse(waitableCommand1.HasEventListenerRegistered);
        //    Assert.IsFalse(waitableCommand2.HasEventListenerRegistered);
        //}

        //[Test]
        //public void MultipleCommandButFirstCommandCancel_Execute_SecondCommandNotCalled()
        //{
        //    var waitableCommand1 = new TestWaitableCommand(ResponseStatus.Cancel);
        //    var waitableCommand2 = new TestWaitableCommand();

        //    _macroCommand.AddCommand(waitableCommand1);
        //    _macroCommand.AddCommand(waitableCommand2);

        //    _macroCommand.Execute(new object());

        //    Assert.IsTrue(waitableCommand1.IsDoneWasRaised);
        //    Assert.IsFalse(waitableCommand2.IsDoneWasRaised);

        //    // test that the listener for IsDone was deregistered
        //    Assert.IsFalse(waitableCommand1.HasEventListenerRegistered);
        //    Assert.IsFalse(waitableCommand2.HasEventListenerRegistered);
        //}

        //[Test]
        //public void MultipleCommandButFirstCommandDelayed_Execute_BothSuccessful()
        //{
        //    var waitableCommand1 = new TestWaitableCommand(ResponseStatus.Success, true);
        //    var waitableCommand2 = new TestWaitableCommand();

        //    _macroCommand.AddCommand(waitableCommand1);
        //    _macroCommand.AddCommand(waitableCommand2);

        //    var isDoneResetEvent = new ManualResetEventSlim(false);

        //    _macroCommand.MacroIsDone += (sender, args) =>
        //    {
        //        isDoneResetEvent.Set();
        //    };

        //    _macroCommand.Execute(new object());

        //    Assert.IsFalse(waitableCommand1.IsDoneWasRaised);
        //    Assert.IsFalse(waitableCommand2.IsDoneWasRaised);

        //    waitableCommand1.DelayExecutionResetEvent.Set();

        //    var macroIsDone = isDoneResetEvent.Wait(1000);

        //    Assert.IsTrue(macroIsDone, "MacroCommand was not marked as done!");

        //    // To prevent timing issues, wait a bit if it was not updated yet
        //    if (!waitableCommand1.IsDoneWasRaised)
        //        Thread.Sleep(10);

        //    Assert.IsTrue(waitableCommand1.IsDoneWasRaised);
        //    Assert.IsTrue(waitableCommand2.IsDoneWasRaised);

        //    // test that the listener for IsDone was deregistered
        //    Assert.IsFalse(waitableCommand1.HasEventListenerRegistered);
        //    Assert.IsFalse(waitableCommand2.HasEventListenerRegistered);
        //}

        //[Test]
        //public void Test()
        //{
        //    var waitableCommand = new TestWaitableCommand(ResponseStatus.Success, true);
        //    _macroCommand.AddCommand(waitableCommand);
        //    var wasRaised = false;
        //    _macroCommand.CanExecuteChanged += (sender, args) => wasRaised = true;

        //    waitableCommand.RaiseCanExecuteChanged();

        //    Assert.IsTrue(wasRaised, "The macro command did not raise CanExecuteChanged when the subcommand did!");
        //}
    }

    public class TestWaitableCommand : IWaitableCommand
    {
        private readonly bool _waitForSetEvent;

        public TestWaitableCommand(ResponseStatus responseStatus = ResponseStatus.Success, bool waitForSetEvent = false)
        {
            _waitForSetEvent = waitForSetEvent;
            ExecutionResponse = responseStatus;
        }

        private void OnIsDone(object sender, MacroCommandIsDoneEventArgs args)
        {
            IsDoneWasRaised = true;
            IsDone -= OnIsDone;
        }

        public void RaiseCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }

        public bool IsDoneWasRaised { get; private set; }

        public bool HasEventListenerRegistered => IsDone != null;

        public bool IsExecutable { get; set; } = true;
        public ResponseStatus ExecutionResponse { get; set; } = ResponseStatus.Success;

        public ManualResetEventSlim DelayExecutionResetEvent { get; } = new ManualResetEventSlim(false);

        public event EventHandler<MacroCommandIsDoneEventArgs> IsDone;

#pragma warning disable 67

        public event EventHandler CanExecuteChanged;

#pragma warning restore 67

        public bool CanExecute(object parameter)
        {
            return IsExecutable;
        }

        public void Execute(object parameter)
        {
            IsDone += OnIsDone;

            if (_waitForSetEvent)
            {
                Task.Run(() =>
                {
                    var wasSet = DelayExecutionResetEvent.Wait(1000);
                    if (!wasSet)
                        throw new Exception("The event DelayExecutionResetEvent was not set within 1000ms");

                    IsDone?.Invoke(this, new MacroCommandIsDoneEventArgs(ExecutionResponse));
                });
            }
            else
            {
                IsDone?.Invoke(this, new MacroCommandIsDoneEventArgs(ExecutionResponse));
            }
        }
    }
}
