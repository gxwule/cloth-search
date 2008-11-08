using System;
using System.Collections.Generic;
using NUnit.Framework;
using Zju.Util;
using Zju.Domain;
using NUnit.Mocks;
using Zju.Dao;

namespace Zju.Service
{
    [TestFixture]
    public class ClothLibServiceTest
    {
        private ClothLibService target;
        private DynamicMock clothDaoMock;


        [TestFixtureSetUp]
        public void SetUp()
        {
            clothDaoMock = new DynamicMock(typeof(IClothDao));
        }

        [TestFixtureTearDown]
        public void TearDown()
        {

        }
    }
}
