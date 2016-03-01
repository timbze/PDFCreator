using NUnit.Framework;
using pdfforge.DataStorage;
using pdfforge.PDFCreator.Helper;

namespace PDFCreator.UnitTest.Helper
{
    [TestFixture]
    public class DataUpgraderTest
    {
        [Test]
        public void MoveSection_WithTwoValues_MovesValuesToNewSection()
        {
            Data data = Data.CreateDataStorage();
            data.SetValue("MySection\\FirstValue", "first");
            data.SetValue("MySection\\SecondValue", "second");
            DataUpgrader upgrader = new DataUpgrader();
            upgrader.Data = data;

            upgrader.MoveSection("MySection\\", "MyNewSection\\");

            Assert.AreEqual("first", data.GetValue("MyNewSection\\FirstValue"));
            Assert.AreEqual("second", data.GetValue("MyNewSection\\SecondValue"));
        }

        [Test]
        public void MoveSection_WithValues_DeletesOldSection()
        {
            Data data = Data.CreateDataStorage();
            data.SetValue("MySection\\FirstValue", "first");
            data.SetValue("MySection\\SecondValue", "second");
            DataUpgrader upgrader = new DataUpgrader();
            upgrader.Data = data;

            upgrader.MoveSection("MySection\\", "MyNewSection\\");

            Assert.AreEqual(new[] { "MyNewSection\\" }, data.GetSections());
        }
    }
}
