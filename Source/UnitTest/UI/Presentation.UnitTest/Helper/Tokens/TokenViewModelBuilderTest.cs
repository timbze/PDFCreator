using NUnit.Framework;
using pdfforge.PDFCreator.UI.Presentation.DesignTime.Helper;
using pdfforge.PDFCreator.UI.Presentation.Helper.Tokens;
using pdfforge.PDFCreator.Utilities.Tokens;
using System;
using System.Linq;

namespace Presentation.UnitTest.Helper.Tokens
{
    [TestFixture]
    public class TokenViewModelBuilderTest
    {
        private TokenViewModelBuilder<SampleClass> _builder;
        private SampleClass _currentValue;
        private TokenHelper _tokenHelper;

        [SetUp]
        public void Setup()
        {
            _tokenHelper = new TokenHelper(new DesignTimeTranslationUpdater());
            _currentValue = new SampleClass();

            _builder = new TokenViewModelBuilder<SampleClass>(_tokenHelper, _currentValue);
        }

        [Test]
        public void Build_WithOnlySelector_ThrowsException()
        {
            Assert.Throws<InvalidOperationException>(() => _builder.WithSelector(p => p.PropertyOne).Build());
        }

        [Test]
        public void Build_WithOnlyPreviewFunction_ThrowsException()
        {
            Assert.Throws<InvalidOperationException>(() => _builder.WithTokenCustomPreview(s => s).Build());
        }

        [Test]
        public void Build_WithPreviewAndSelector_CreatesViewModel()
        {
            var vm = _builder
                .WithTokenCustomPreview(s => s)
                .WithSelector(p => p.PropertyOne)
                .Build();

            Assert.NotNull(vm);
        }

        [Test]
        public void CreatedViewModel_UsesSelectedProperty()
        {
            var vm = _builder
                .WithTokenCustomPreview(s => s)
                .WithSelector(p => p.PropertyOne)
                .Build();

            Assert.AreEqual(_currentValue.PropertyOne, vm.Text);
        }

        [Test]
        public void CreatedViewModel_UsesPreviewFunction()
        {
            var vm = _builder
                .WithTokenCustomPreview(s => "PREVIEW:" + s)
                .WithSelector(p => p.PropertyOne)
                .Build();

            Assert.AreEqual("PREVIEW:" + vm.Text, vm.Preview);
        }

        [Test]
        public void CreatedViewModel_UsesInitialProfile()
        {
            var vm = _builder
                .WithTokenCustomPreview(s => "PREVIEW:" + s)
                .WithSelector(p => p.PropertyOne)
                .Build();

            Assert.AreSame(_currentValue, vm.CurrentValue);
        }

        [Test]
        public void Build_WithTokenList_UsesTokenList()
        {
            var tokenList = new[] { "token1", "token2" };

            var vm = _builder
                .WithTokenCustomPreview(s => s)
                .WithSelector(p => p.PropertyOne)
                .WithTokenList(tokenList)
                .Build();

            Assert.AreEqual(tokenList, vm.Tokens.Select(t => t.Name));
        }

        [Test]
        public void Build_WithTokenReplacerAndTokenList_UsesTokenList()
        {
            var tokenList = new[] { "token1", "token2" };

            var vm = _builder
                .WithSelector(p => p.PropertyOne)
                .WithTokenReplacerPreview(new TokenReplacer())
                .WithTokenList(tokenList)
                .Build();

            Assert.AreEqual(tokenList, vm.Tokens.Select(t => t.Name));
        }

        [Test]
        public void Build_WithTokenReplacer_UsesTokenReplacerPreview()
        {
            var token = "token1";
            var tokenValue = "token-value-1";
            var tokenList = new[] { "<token1>", "<token2>" };
            var tokenReplacer = new TokenReplacer();
            tokenReplacer.AddStringToken(token, tokenValue);

            _currentValue.PropertyOne = "<" + token + ">";

            var vm = _builder
                .WithSelector(p => p.PropertyOne)
                .WithTokenReplacerPreview(tokenReplacer)
                .WithTokenList(tokenList)
                .Build();

            Assert.AreEqual(tokenValue, vm.Preview);
        }

        [Test]
        public void Build_WithDefaultTokenReplacerPreview_UsesSelectedTokenList()
        {
            var tokenHelper = new TokenHelper(new DesignTimeTranslationUpdater());

            var tokenList = tokenHelper.GetTokenListForEmail();

            var vm = _builder
                .WithSelector(p => p.PropertyOne)
                .WithDefaultTokenReplacerPreview(th => th.GetTokenListForEmail())
                .Build();

            Assert.AreEqual(tokenList, vm.Tokens.Select(t => t.Name));
        }

        [Test]
        public void Build_WithDefaultTokenReplacerPreview_UsesPreviewFunction()
        {
            _currentValue.PropertyOne = "<username>";
            var tokenHelper = new TokenHelper(new DesignTimeTranslationUpdater());

            var tokenReplacer = tokenHelper.TokenReplacerWithPlaceHolders;
            var expectedText = tokenReplacer.ReplaceTokens(_currentValue.PropertyOne);

            var vm = _builder
                .WithSelector(p => p.PropertyOne)
                .WithDefaultTokenReplacerPreview(th => th.GetTokenListForEmail())
                .Build();

            Assert.AreEqual(expectedText, vm.Preview);
        }

        private class SampleClass
        {
            public string PropertyOne { get; set; } = "One";
            public string PropertyTwo { get; set; } = "Two";
        }
    }
}
