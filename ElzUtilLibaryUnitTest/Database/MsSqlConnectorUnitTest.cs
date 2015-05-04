using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using ElzUtilLibary.Database;
using ElzUtilLibaryUnitTest.TestModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ElzUtilLibaryUnitTest.Database
{
    /// <summary>
    ///     Summary description for MsSqlConnectorUnitTest
    /// </summary>
    [TestClass]
    public class MsSqlConnectorUnitTest
    {
        private const string ConnectionString =
            @"Data Source=localhost\TESTDB;Initial Catalog=UnitTestDB;Integrated Security=True";

        private static MsSqlConnector _msSqlConnector;

        #region SQL-Statements

        private const string DropPersonTable =
            @"if exists (SELECT * FROM INFORMATION_SCHEMA.TABLES where TABLE_NAME = 'Person')
                                                    DROP TABLE Person;";

        private const string CreatePersonTable = @"CREATE TABLE [dbo].[Person](
	                                                    [PersId] [int] IDENTITY(1,1) NOT NULL,
	                                                    [Firstname] [varchar](50) NOT NULL,
	                                                    [Lastname] [varchar](50) NOT NULL,
	                                                    [Birthday] [date] NULL,
                                                        CONSTRAINT [PK_Person] PRIMARY KEY CLUSTERED 
                                                    (
	                                                    [PersId] ASC
                                                    )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
                                                    ) ON [PRIMARY]";

        private const string DropAddressTable =
            @"if exists (SELECT * FROM INFORMATION_SCHEMA.TABLES where TABLE_NAME = 'Address')
                                                    DROP TABLE Address;";

        private const string CreateAddressTable = @"CREATE TABLE [dbo].[Address](
	                                                    [AddressId] [int] IDENTITY(1,1) NOT NULL,
	                                                    [Street] [varchar](50) NOT NULL,
	                                                    [HouseNumber] [varchar](10) NOT NULL,
	                                                    [Zipcode] [int] NOT NULL,
	                                                    [City] [varchar](100) NOT NULL,
	                                                    [PersId] [int] NOT NULL,
                                                     CONSTRAINT [PK_Address] PRIMARY KEY CLUSTERED 
                                                    (
	                                                    [AddressId] ASC
                                                    )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
                                                    ) ON [PRIMARY]";

        private const string AddForeignKey =
            @"ALTER TABLE [dbo].[Address]  WITH CHECK ADD  CONSTRAINT [FK_PersonAddress] FOREIGN KEY([PersId])
                                                REFERENCES [dbo].[Person] ([PersId])";

        private const string InsertPersons = @"INSERT INTO Person
                                                (Firstname, Lastname, Birthday)
                                                OUTPUT inserted.PersId
                                                VALUES
                                                ('Marcel', 'Elz', NULL),
                                                ('Hans', 'Mustermann', '01.01.2015'),
                                                ('Optimus', 'Prime', '02.02.1942');";
        private const string InsertAddresses = @"INSERT INTO Address
                                                (Street, HouseNumber, Zipcode, City, PersId)
                                                VALUES
                                                ('Hauptstraße', '128', 12345, 'Wehlen', {0}),
                                                ('Musterstrasse', '123 a', 12346, 'Musterstadt', {1}),
                                                ('', '', 12347, '', {2});";
        private const string DeleteAddresses = @"DELETE FROM Address";
        private const string DeletePersons = @"DELETE FROM Person";

        #endregion SQL-Statements

        #region Additional test attributes

        //
        // You can use the following additional attributes as you write your tests:
        //
        // Use ClassInitialize to run code before running the first test in the class
        [ClassInitialize]
        public static void MyClassInitialize(TestContext testContext)
        {
            SetupDatabase();

            _msSqlConnector = new MsSqlConnector(ConnectionString);
        }

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

        #region Test Setup Helper Methods

        private static void SetupDatabase()
        {
            using (var connection = new SqlConnection(ConnectionString))
            {
                connection.Open();

                using (var command = new SqlCommand())
                {
                    command.Connection = connection;

                    command.CommandText = DropAddressTable;
                    command.ExecuteNonQuery();

                    command.CommandText = DropPersonTable;
                    command.ExecuteNonQuery();

                    command.CommandText = CreatePersonTable;
                    command.ExecuteNonQuery();

                    command.CommandText = CreateAddressTable;
                    command.ExecuteNonQuery();

                    command.CommandText = AddForeignKey;
                    command.ExecuteNonQuery();

                    command.Dispose();
                }

                connection.Close();
                connection.Dispose();
            }
        }

        private void SetupData()
        {
            using (var connection = new SqlConnection(ConnectionString))
            {
                connection.Open();

                using (var command = new SqlCommand())
                {
                    command.Connection = connection;

                    command.CommandText = InsertPersons;
                    SqlDataReader reader = command.ExecuteReader();

                    var ids = new object[3];
                    int i = 0;

                    while (reader.Read())
                    {
                        ids[i] = reader.GetInt32(0);
                        i++;
                    }
                    reader.Close();

                    command.CommandText = string.Format(InsertAddresses, ids);
                    command.ExecuteNonQuery();

                    command.Dispose();
                }

                connection.Close();
                connection.Dispose();
            }
        }

        private void CleanupData()
        {
            using (var connection = new SqlConnection(ConnectionString))
            {
                connection.Open();

                using (var command = new SqlCommand())
                {
                    command.Connection = connection;

                    command.CommandText = DeleteAddresses;
                    command.ExecuteNonQuery();

                    command.CommandText = DeletePersons;
                    command.ExecuteNonQuery();

                    command.Dispose();
                }

                connection.Close();
                connection.Dispose();
            }
        }

        private bool VerifyDataInsert()
        {
            bool isVerfied = false;

            using (var connection = new SqlConnection(ConnectionString))
            {
                connection.Open();

                using (var command = new SqlCommand())
                {
                    command.Connection = connection;

                    command.CommandText = "SELECT * FROM Person";
                    SqlDataReader reader = command.ExecuteReader();

                    var datatable = new DataTable();
                    datatable.Load(reader);

                    reader.Close();

                    if (datatable.Rows.Count == 3)
                        isVerfied = true;

                    command.Dispose();
                }

                connection.Close();
                connection.Dispose();
            }

            return isVerfied;
        }

        #endregion Test Setup Helper Methods

        [TestMethod]
        public void GetDataT()
        {
            SetupData();

            List<Person> personList =
                _msSqlConnector.GetData<Person>("SELECT PersId, Firstname, Lastname, Birthday FROM Person");
            Assert.IsTrue(personList != null);
            Assert.IsTrue(personList.Count == 3);

            List<Address> addressList =
                _msSqlConnector.GetData<Address>("SELECT AddressId, Street, HouseNumber, Zipcode, City FROM Address");
            Assert.IsTrue(addressList != null);
            Assert.IsTrue(addressList.Count == 3);

            List<PersonWithAddress> personWithAddressList =
                _msSqlConnector.GetData<PersonWithAddress>(
                    "SELECT p.PersId AS PersId, Firstname, Lastname, Birthday, Street, HouseNumber, Zipcode, City FROM Person p, Address a WHERE p.PersId = a.PersId");
            Assert.IsTrue(personWithAddressList != null);
            Assert.IsTrue(personWithAddressList.Count == 3);

            CleanupData();
        }

        [TestMethod]
        public void InsertData()
        {
            _msSqlConnector.InsertData(InsertPersons);

            Assert.IsTrue(VerifyDataInsert());

            CleanupData();
        }

        [TestMethod]
        public void InsertDataT()
        {
            var person1 = new Person
            {
                Firstname = "Marcel",
                Lastname = "Elz",
                Birthday = DateTime.MinValue,
            };

            var person2 = new Person
            {
                Firstname = "Hans",
                Lastname = "Mustermann",
                Birthday = new DateTime(2015, 1, 1),
            };

            var person3 = new Person
            {
                Firstname = "Optimus",
                Lastname = "Prime",
                Birthday = new DateTime(1942, 2, 2), //02.02.1942
            };

            _msSqlConnector.InsertData(person1);
            _msSqlConnector.InsertData(person2);
            _msSqlConnector.InsertData(person3);

            Assert.IsTrue(VerifyDataInsert());

            CleanupData();
        }
    }
}