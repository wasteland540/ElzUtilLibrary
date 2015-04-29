using System;
using System.Data;
using ExifWriter.Model;
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
            table.Columns.Add("Filename", typeof(string));
            table.Columns.Add("ExposureTime", typeof(string));
            table.Columns.Add("Aperture", typeof(float));
            table.Columns.Add("FocalLength", typeof(float));
            table.Columns.Add("Iso", typeof(int));
            table.Columns.Add("Copyright", typeof(string));

            var row = table.NewRow();
            row["Filename"] = "Test.cr2";
            row["ExposureTime"] = "1/200";
            row["Aperture"] = 8.0f;
            row["FocalLength"] = 50.0f;
            row["Iso"] = 800;
            row["Copyright"] = "m.elz";
            table.Rows.Add(row);

            var objectList = ElzUtilLibary.Mapping.MappingHelper.MapDataTableToObjectList<ImageExifData>(table);

            Assert.IsTrue(objectList != null);
            Assert.IsTrue(objectList.Count == 1);
            Assert.IsTrue(objectList[0].GetType() == typeof(ImageExifData));
            Assert.IsTrue(objectList[0].Filename == "Test.cr2");
            Assert.IsTrue(objectList[0].ExposureTime == "1/200");
            Assert.IsTrue(Math.Abs(objectList[0].Aperture - 8.0f) < 0.00000001);
            Assert.IsTrue(Math.Abs(objectList[0].FocalLength - 50.0f) < 0.00000001);
            Assert.IsTrue(objectList[0].Iso == 800);
            Assert.IsTrue(objectList[0].Copyright == "m.elz");

        }

        [TestMethod]
        public void CreateTypedDataTable()
        {
            var dataTable = ElzUtilLibary.Mapping.MappingHelper.CreateTypedDataTable<ImageExifData>();

            Assert.IsTrue(dataTable != null);
            Assert.IsTrue(dataTable.Columns.Count == 6);
        }
    }
}
