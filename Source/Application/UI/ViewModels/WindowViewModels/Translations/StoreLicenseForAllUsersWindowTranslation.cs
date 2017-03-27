using Translatable;

namespace pdfforge.PDFCreator.UI.ViewModels.WindowViewModels.Translations
{
     public class StoreLicenseForAllUsersWindowTranslation : ITranslatable
     {
          public string HeadlineText { get; private set; } = "Licensing was succesful.";
          public string NoButtonContent { get; private set; } = "No, thanks";
          public string StoreQueryText { get; private set; } = "Do you want to make the license available for all users on your system?";
          public string YesButtonText { get; private set; } = "Yes";
          public string StoreForAllUsersFailed { get; private set; } = "An error occured while storing license for all users.";
          public string StoreForAllUsersSuccessful { get; private set; } = "Store license for all users was successful.";
    }
}
