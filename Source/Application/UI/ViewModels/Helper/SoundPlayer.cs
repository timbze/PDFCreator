using System.Media;

namespace pdfforge.PDFCreator.UI.ViewModels.Helper
{
    public interface ISoundPlayer
    {
        void Play(SystemSound sound);
    }

    public class SoundPlayer : ISoundPlayer
    {
        public void Play(SystemSound sound)
        {
            sound.Play();
        }
    }
}