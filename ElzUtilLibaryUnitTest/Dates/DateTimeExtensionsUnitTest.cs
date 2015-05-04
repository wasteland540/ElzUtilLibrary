using System;
using System.Collections.Generic;
using ElzUtilLibary.Dates;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ElzUtilLibaryUnitTest.Dates
{
    /// <summary>
    ///     Summary description for DateTimeExtensionsUnitTest
    /// </summary>
    [TestClass]
    public class DateTimeExtensionsUnitTest
    {
        #region Additional test attributes

        //
        // You can use the following additional attributes as you write your tests:
        //
        // Use ClassInitialize to run code before running the first test in the class
        // [ClassInitialize()]
        // public static void MyClassInitialize(TestContext testContext) { }
        //
        // Use ClassCleanup to run code after all tests in a class have run
        // [ClassCleanup()]
        // public static void MyClassCleanup() { }
        //
        // Use TestInitialize to run code before running each test 
        // [TestInitialize()]
        // public void MyTestInitialize() { }
        //
        // Use TestCleanup to run code after each test has run
        // [TestCleanup()]
        // public void MyTestCleanup() { }
        //

        #endregion

        [TestMethod]
        public void GetDayRange()
        {
            var startDate = new DateTime(2015, 5, 4);
            var endDate = new DateTime(2015, 5, 7);

            List<DateTime> dayRange = startDate.GetDayRange(endDate);

            Assert.IsTrue(dayRange.Count == 4);
            Assert.IsTrue(dayRange[0].Date == new DateTime(2015, 5, 4));
            Assert.IsTrue(dayRange[1].Date == new DateTime(2015, 5, 5));
            Assert.IsTrue(dayRange[2].Date == new DateTime(2015, 5, 6));
            Assert.IsTrue(dayRange[3].Date == new DateTime(2015, 5, 7));
        }

        [TestMethod]
        public void AreDayRangesConflicted()
        {
            var dayRange1 = new List<DateTime>
            {
                new DateTime(2015, 5, 4),
                new DateTime(2015, 5, 5),
                new DateTime(2015, 5, 6),
            };

            var dayRange2 = new List<DateTime>
            {
                new DateTime(2015, 5, 6),
                new DateTime(2015, 5, 7),
                new DateTime(2015, 5, 8),
            };

            var dayRange3 = new List<DateTime>
            {
                new DateTime(2015, 5, 7),
                new DateTime(2015, 5, 8),
                new DateTime(2015, 5, 9),
            };

            Assert.IsTrue(dayRange1.AreDayRangesConflicted(dayRange2));
            Assert.IsFalse(dayRange1.AreDayRangesConflicted(dayRange3));
        }
    }
}