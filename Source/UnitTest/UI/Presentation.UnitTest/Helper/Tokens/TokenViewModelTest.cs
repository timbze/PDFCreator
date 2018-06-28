using NUnit.Framework;
using Optional;
using pdfforge.PDFCreator.UI.Presentation.Helper.Tokens;
using pdfforge.PDFCreator.UnitTest.UnitTestHelper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Presentation.UnitTest.Helper.Tokens
{
    [TestFixture]
    public class TokenViewModelTest
    {
        private List<string> _tokenList;
        private SampleClass _testObject;

        [SetUp]
        public void Setup()
        {
            _testObject = new SampleClass();
            _tokenList = new List<string>();
        }

        private TokenViewModel<SampleClass> BuildViewModel(Expression<Func<SampleClass, string>> selector = null, Func<string, Option<string>> buttonCommandFunc = null)
        {
            if (selector == null)
                selector = c => c.PropertyOne;

            return new TokenViewModel<SampleClass>(selector, _testObject, _tokenList, s => "PREVIEW:" + s, buttonCommandFunc);
        }

        [Test]
        public void Selector_Get_ReturnsPropertyValue()
        {
            var vm = BuildViewModel(c => c.PropertyOne);

            Assert.AreEqual("One", vm.Text);
        }

        [Test]
        public void Selector_Set_SetsPropertyValue()
        {
            var expectedValue = "Abc";
            var vm = BuildViewModel(c => c.PropertyOne);
            vm.Text = expectedValue;

            Assert.AreEqual(expectedValue, _testObject.PropertyOne);
        }

        [Test]
        public void Selector_ReadOnly_ThrowsException()
        {
            Assert.Throws<ArgumentException>(() => BuildViewModel(c => "constant string"));
        }

        [Test]
        public void Preview_UsesReplaceFunction()
        {
            var vm = BuildViewModel(c => c.PropertyTwo);

            Assert.AreEqual("PREVIEW:" + _testObject.PropertyTwo, vm.Preview);
        }

        [Test]
        public void Preview_CurrentValueIsNull_ReturnsEmptyString()
        {
            var vm = BuildViewModel(c => c.PropertyTwo);
            vm.CurrentValue = null;

            Assert.AreEqual("", vm.Preview);
        }

        [Test]
        public void Preview_TextIsNull_ReturnsEmptyString()
        {
            var vm = BuildViewModel(c => c.PropertyTwo);
            vm.Text = null;

            Assert.AreEqual("", vm.Preview);
        }

        [Test]
        public void CurrentValue_UsesInitialObject()
        {
            var vm = BuildViewModel();

            Assert.AreSame(_testObject, vm.CurrentValue);
        }

        [Test]
        public void TextChanged_RaisesEvents()
        {
            var vm = BuildViewModel();
            var previewChanged = new PropertyChangedListenerMock(vm, nameof(vm.Preview));

            vm.Text = "Some Text";

            Assert.IsTrue(previewChanged.WasCalled);
        }

        [Test]
        public void TokenCommand_WithEmptyProperty_Execute_AddsTextAndSetsCursor()
        {
            _testObject.PropertyOne = "";
            _tokenList.Add("test");
            var vm = BuildViewModel();
            var tokenCommand = vm.Tokens.First();

            tokenCommand.MyCommand.Execute(null);

            Assert.AreEqual("test", vm.Text);
            Assert.AreEqual(tokenCommand.Name.Length, vm.CurrentCursorPos);
        }

        [Test]
        public void TokenCommand_WithStringInProperty_Execute_AddsTextAndSetsCursor()
        {
            _testObject.PropertyOne = "a__b";
            _tokenList.Add("test");
            var vm = BuildViewModel();
            var initialCursorPos = 2;
            vm.CurrentCursorPos = initialCursorPos;
            var tokenCommand = vm.Tokens.First();

            tokenCommand.MyCommand.Execute(null);

            Assert.AreEqual("a_test_b", vm.Text);
            Assert.AreEqual(initialCursorPos + tokenCommand.Name.Length, vm.CurrentCursorPos);
        }

        [Test]
        public void TokenCommand_Execute_RaisesPreviewChangedEvent()
        {
            _tokenList.Add("test");
            var vm = BuildViewModel();
            var tokenCommand = vm.Tokens.First();
            var previewEvent = new PropertyChangedListenerMock(vm, nameof(vm.Preview));

            tokenCommand.MyCommand.Execute(null);

            Assert.IsTrue(previewEvent.WasCalled);
        }

        [Test]
        public void ButtonCommand_Execute_FuncIsEvaluated()
        {
            var wasCalled = false;
            var vm = BuildViewModel(buttonCommandFunc: delegate
            {
                wasCalled = true;
                return Option.None<string>();
            });

            vm.ButtonCommand.Execute(null);

            Assert.IsTrue(wasCalled);
        }

        [Test]
        public void ButtonCommand_ExecuteReturnsString_StringIsApplied()
        {
            var expectedString = "abc";
            var vm = BuildViewModel(buttonCommandFunc: s => expectedString.Some());

            vm.ButtonCommand.Execute(null);

            Assert.AreEqual(expectedString, vm.Text);
        }

        [Test]
        public void ButtonCommand_ExecuteReturnsNone_StringIsUnchanged()
        {
            var expectedString = _testObject.PropertyOne;
            var vm = BuildViewModel(buttonCommandFunc: s => Option.None<string>());

            vm.ButtonCommand.Execute(null);

            Assert.AreEqual(expectedString, vm.Text);
        }

        [Test]
        public void ButtonCommandWithNoFunction_Execute_DoesNothing()
        {
            var expectedString = _testObject.PropertyOne;
            var vm = BuildViewModel(buttonCommandFunc: null);

            vm.ButtonCommand.Execute(null);

            Assert.AreEqual(expectedString, vm.Text);
        }

        [Test]
        public void ButtonCommandWithNoFunction_ShowButton_IsFalse()
        {
            var vm = BuildViewModel(buttonCommandFunc: null);

            Assert.IsFalse(vm.ShowButton);
        }

        [Test]
        public void ButtonCommandWithFunction_ShowButton_IsTrue()
        {
            var vm = BuildViewModel(buttonCommandFunc: s => s.Some());

            Assert.IsTrue(vm.ShowButton);
        }

        private class SampleClass
        {
            public string PropertyOne { get; set; } = "One";
            public string PropertyTwo { get; set; } = "Two";
        }
    }
}
