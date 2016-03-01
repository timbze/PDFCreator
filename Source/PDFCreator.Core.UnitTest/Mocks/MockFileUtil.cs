using pdfforge.PDFCreator.Utilities;

namespace PDFCreator.Core.UnitTest.Mocks
{
    public class MockFileUtil : FileUtil
    {
        public bool DirIsEmpty { get; set; }
        public bool RootedPath { get; set; }

        public void SetInstanceToMock()
        {
            Instance = this;
        }

        public override bool DirectoryIsEmpty(string path)
        {
            return DirIsEmpty;
        }

        public override bool IsValidRootedPath(string path)
        {
            return RootedPath;
        }
    }
}
