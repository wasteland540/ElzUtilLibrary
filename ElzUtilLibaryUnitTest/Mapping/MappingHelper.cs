using System;
using System.Data;
using ElzUtilLibaryUnitTest.TestModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ElzUtilLibaryUnitTest.Mapping
{
    [TestClass]
    public class MappingHelper
    {
        [TestMethod]
        public void MapDataTableToObjectList()
        {
            var table = new DataTable();
            table.Columns.Add("PersId", typeof(int));
            table.Columns.Add("Firstname", typeof(string));
            table.Columns.Add("Lastname", typeof(string));
            table.Columns.Add("Birthday", typeof(DateTime));

            var row = table.NewRow();
            row["PersId"] = 1;
            row["Firstname"] = "Marcel";
            row["Lastname"] = "Elz";
            row["Birthday"] = new DateTime(2015, 5, 5);
            table.Rows.Add(row);

            var objectList = ElzUtilLibary.Mapping.MappingHelper.MapDataTableToObjectList<Person>(table);

            Assert.IsTrue(objectList != null);
            Assert.IsTrue(objectList.Count == 1);
            Assert.IsTrue(objectList[0].GetType() == typeof(Person));
            Assert.IsTrue(objectList[0].PersId == 1);
            Assert.IsTrue(objectList[0].Firstname == "Marcel");
            Assert.IsTrue(objectList[0].Lastname == "Elz");
            Assert.IsTrue(objectList[0].Birthday == new DateTime(2015, 5, 5));

        }

        [TestMethod]
        public void CreateTypedDataTable()
        {
            var dataTable = ElzUtilLibary.Mapping.MappingHelper.CreateTypedDataTable<Person>();

            Assert.IsTrue(dataTable != null);
            Assert.IsTrue(dataTable.Columns.Count == 4);
        }
    }
}
