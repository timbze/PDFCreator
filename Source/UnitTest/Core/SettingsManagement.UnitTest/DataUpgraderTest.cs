using NUnit.Framework;
using pdfforge.DataStorage;
using pdfforge.PDFCreator.Core.SettingsManagement;

namespace pdfforge.PDFCreator.UnitTest.Core.SettingsManagement
{
    [TestFixture]
    public class DataUpgraderTest
    {
        [Test]
        public void MoveSection_WithTwoValues_MovesValuesToNewSection()
        {
            var data = Data.CreateDataStorage();
            data.SetValue("MySection\\FirstValue", "first");
            data.SetValue("MySection\\SecondValue", "second");
            var upgrader = new DataUpgrader();
            upgrader.Data = data;

            upgrader.MoveSection("MySection\\", "MyNewSection\\");

            Assert.AreEqual("first", data.GetValue("MyNewSection\\FirstValue"));
            Assert.AreEqual("second", data.GetValue("MyNewSection\\SecondValue"));
        }

        [Test]
        public void MoveSection_WithValues_DeletesOldSection()
        {
            var data = Data.CreateDataStorage();
            data.SetValue("MySection\\FirstValue", "first");
            data.SetValue("MySection\\SecondValue", "second");
            var upgrader = new DataUpgrader();
            upgrader.Data = data;

            upgrader.MoveSection("MySection\\", "MyNewSection\\");

            Assert.AreEqual(new[] {"MyNewSection\\"}, data.GetSections());
        }
    }
}