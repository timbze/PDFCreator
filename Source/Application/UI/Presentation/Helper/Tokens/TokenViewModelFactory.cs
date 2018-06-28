using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles;

namespace pdfforge.PDFCreator.UI.Presentation.Helper.Tokens
{
    public interface ITokenViewModelFactory
    {
        TokenViewModelBuilder<ConversionProfile> BuilderWithSelectedProfile();

        TokenViewModelBuilder<T> Builder<T>();
    }

    public class TokenViewModelFactory : ITokenViewModelFactory
    {
        private readonly ISelectedProfileProvider _selectedProfileProvider;
        private readonly TokenHelper _tokenHelper;

        public TokenViewModelFactory(ISelectedProfileProvider selectedProfileProvider, TokenHelper tokenHelper)
        {
            _selectedProfileProvider = selectedProfileProvider;
            _tokenHelper = tokenHelper;
        }

        public TokenViewModelBuilder<ConversionProfile> BuilderWithSelectedProfile()
        {
            return new SelectedProfileTokenViewModelBuilder(_selectedProfileProvider, _tokenHelper);
        }

        public TokenViewModelBuilder<T> Builder<T>()
        {
            return new TokenViewModelBuilder<T>(_tokenHelper);
        }
    }
}
