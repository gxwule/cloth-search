using System;
using System.Collections.Generic;
using Perst;
using NUnit.Framework;
using Zju.Domain;

namespace Zju.Dao
{
    [TestFixture]
    public class DaoHelperTest
    {
        [TestFixtureSetUp]
        public void SetUp()
        {

        }

        [Test]
        public void TestGetDbStorage()
        {
            Storage storage = DaoHelper.Instance.DbStorage;
            Assert.IsNotNull(storage);
            ClothRoot root = (ClothRoot)storage.Root;
            Assert.IsNotNull(root);
            Assert.IsNotNull(root.ClothOidIndex);
            Assert.IsNotNull(root.ColorIndex);
            Assert.IsNotNull(root.ShapeIndex);
            DaoHelper.CloseDb();
        }

        [TestFixtureTearDown]
        public void TearDown()
        {

        }
    }
}