using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using pdfforge.PDFCreator.UI.ViewModels.Helper;

namespace pdfforge.PDFCreator.UnitTest.UI.ViewModels.Helper
{
    [TestFixture]
    internal class SynchronizedCollectionTest
    {
        [Test]
        public void CreatingSyncCollection_ContainsOriginalCollection()
        {
            IList<string> myList = new List<string>();
            var sync = new SynchronizedCollection<string>(myList);

            Assert.AreSame(myList, sync.Collection);
        }

        [Test]
        public void ObservableCollection_AddingSingleItemAtBeginning_IsReplicatedToCollection()
        {
            IList<string> myList = new[] {"a", "b", "c", "d"}.ToList();

            var sync = new SynchronizedCollection<string>(myList);

            sync.ObservableCollection.Insert(0, "e");
            Assert.AreEqual("e", myList[0]);
            Assert.AreEqual(myList, sync.ObservableCollection.ToList());
        }

        [Test]
        public void ObservableCollection_AddingSingleItemAtEnd_IsReplicatedToCollection()
        {
            IList<string> myList = new[] {"a", "b", "c", "d"}.ToList();

            var sync = new SynchronizedCollection<string>(myList);

            sync.ObservableCollection.Add("e");
            Assert.AreEqual("e", myList[4]);
            Assert.AreEqual(myList, sync.ObservableCollection.ToList());
        }

        [Test]
        public void ObservableCollection_AddingSingleItemAtGivenPosition_IsReplicatedToCollection()
        {
            IList<string> myList = new[] {"a", "b", "c", "d"}.ToList();

            var sync = new SynchronizedCollection<string>(myList);

            sync.ObservableCollection.Insert(2, "e");
            Assert.AreEqual("e", myList[2]);
            Assert.AreEqual(myList, sync.ObservableCollection.ToList());
        }

        [Test]
        public void ObservableCollection_Clear_IsReplicatedToCollection()
        {
            IList<string> myList = new[] {"a", "b", "c", "d"}.ToList();

            var sync = new SynchronizedCollection<string>(myList);

            sync.ObservableCollection.Clear();
            Assert.IsTrue(myList.Count == 0);
            Assert.AreEqual(myList, sync.ObservableCollection.ToList());
        }

        [Test]
        public void ObservableCollection_ContainsSameItems()
        {
            IList<string> myList = new[] {"a", "b", "c", "d"}.ToList();

            var sync = new SynchronizedCollection<string>(myList);

            Assert.AreEqual(myList, sync.ObservableCollection.ToList());
        }

        [Test]
        public void ObservableCollection_DeletingSingleItemAtGivenPosition_IsReplicatedToCollection()
        {
            IList<string> myList = new[] {"a", "b", "c", "d"}.ToList();

            var sync = new SynchronizedCollection<string>(myList);

            sync.ObservableCollection.RemoveAt(2);
            Assert.IsFalse(myList.Contains("c"));
            Assert.AreEqual(myList, sync.ObservableCollection.ToList());
        }

        [Test]
        public void ObservableCollection_DeletingSingleItemByReference_IsReplicatedToCollection()
        {
            IList<string> myList = new[] {"a", "b", "c", "d"}.ToList();

            var sync = new SynchronizedCollection<string>(myList);

            sync.ObservableCollection.Remove("c");
            Assert.IsFalse(myList.Contains("c"));
            Assert.AreEqual(myList, sync.ObservableCollection.ToList());
        }

        [Test]
        public void ObservableCollection_MovingSingleItemOnePositionBack_IsReplicatedToCollection()
        {
            IList<string> myList = new[] {"a", "b", "c", "d"}.ToList();

            var sync = new SynchronizedCollection<string>(myList);

            sync.ObservableCollection.Move(2, 1);
            Assert.AreEqual("b", myList[2]);
            Assert.AreEqual("c", myList[1]);
            Assert.AreEqual(myList, sync.ObservableCollection.ToList());
        }

        [Test]
        public void ObservableCollection_MovingSingleItemOnePositionToFront_IsReplicatedToCollection()
        {
            IList<string> myList = new[] {"a", "b", "c", "d"}.ToList();

            var sync = new SynchronizedCollection<string>(myList);

            sync.ObservableCollection.Move(1, 2);
            Assert.AreEqual("b", myList[2]);
            Assert.AreEqual("c", myList[1]);
            Assert.AreEqual(myList, sync.ObservableCollection.ToList());
        }

        [Test]
        public void ObservableCollection_MovingSingleItemTwoPositionsBack_IsReplicatedToCollection()
        {
            IList<string> myList = new[] {"a", "b", "c", "d"}.ToList();

            var sync = new SynchronizedCollection<string>(myList);

            sync.ObservableCollection.Move(1, 3);
            Assert.AreEqual("b", myList[3]);
            Assert.AreEqual("c", myList[1]);
            Assert.AreEqual(myList, sync.ObservableCollection.ToList());
        }

        [Test]
        public void ObservableCollection_MovingSingleItemTwoPositionsToFront_IsReplicatedToCollection()
        {
            IList<string> myList = new[] {"a", "b", "c", "d"}.ToList();

            var sync = new SynchronizedCollection<string>(myList);

            sync.ObservableCollection.Move(3, 1);
            Assert.AreEqual("d", myList[1]);
            Assert.AreEqual("b", myList[2]);
            Assert.AreEqual(myList, sync.ObservableCollection.ToList());
        }
    }
}