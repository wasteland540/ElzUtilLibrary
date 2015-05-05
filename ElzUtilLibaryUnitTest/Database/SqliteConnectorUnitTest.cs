using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using ElzUtilLibary.Database;
using ElzUtilLibaryUnitTest.TestModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ElzUtilLibaryUnitTest.Database
{
    /// <summary>
    ///     Summary description for SQLiteConnectorUnitTest
    /// </summary>
    [TestClass]
    public class SqliteConnectorUnitTest
    {
        private const string ConnectionString = @"Data Source=UnitTestDB.db";

        private static SQLiteConnector _sqliteConnector;

        #region SQL-Statements

        private const string DropPersonTable = @"DROP TABLE IF EXISTS Person;";

        private const string CreatePersonTable = @"CREATE TABLE IF NOT EXISTS `Person` (
                                                      `PersId` INTEGER PRIMARY KEY NOT NULL,
                                                      `Firstname` varchar(50) NOT NULL,
                                                      `Lastname` varchar(50) NOT NULL,
                                                      `Birthday` date DEFAULT NULL
                                                    )";

        private const string DropAddressTable = @"DROP TABLE IF EXISTS Address;";

        private const string CreateAddressTable = @"CREATE TABLE IF NOT EXISTS `Address` (
                                                      `AddressId` INTEGER PRIMARY KEY NOT NULL,
                                                      `Street` varchar(50) NOT NULL,
                                                      `HouseNumber` varchar(10) NOT NULL,
                                                      `Zipcode` INTEGER NOT NULL,
                                                      `City` varchar(100) NOT NULL,
                                                      `PersId` INTEGER NOT NULL
                                                    )";

        private const string InsertPersons = @"INSERT INTO Person
                                                (Firstname, Lastname, Birthday)
                                                VALUES
                                                ('Marcel', 'Elz', NULL),
                                                ('Hans', 'Mustermann', '2015-01-01'),
                                                ('Optimus', 'Prime', '1942-02-02');";
        private const string InsertPerson1 = @"INSERT INTO Person
                                                (Firstname, Lastname, Birthday)
                                                VALUES
                                                ('Marcel', 'Elz', NULL);
                                               SELECT last_insert_rowid();";
        private const string InsertPerson2 = @"INSERT INTO Person
                                                (Firstname, Lastname, Birthday)
                                                VALUES
                                                ('Hans', 'Mustermann', '2015-01-01');
                                               SELECT last_insert_rowid();";
        private const string InsertPerson3 = @"INSERT INTO Person
                                                (Firstname, Lastname, Birthday)
                                                VALUES
                                                 ('Optimus', 'Prime', '1942-02-02');
                                               SELECT last_insert_rowid();";
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

            _sqliteConnector = new SQLiteConnector(ConnectionString);
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
            using (var connection = new SQLiteConnection(ConnectionString))
            {
                connection.Open();

                using (var command = new SQLiteCommand())
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

                    command.Dispose();
                }

                connection.Close();
                connection.Dispose();
            }
        }

        private void SetupData()
        {
            using (var connection = new SQLiteConnection(ConnectionString))
            {
                connection.Open();

                using (var command = new SQLiteCommand())
                {
                    command.Connection = connection;

                    var ids = new object[3];
                    int i = 0;

                    command.CommandText = InsertPerson1;
                    SQLiteDataReader reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        ids[i] = reader.GetInt32(0);
                        i++;
                    }
                    reader.Close();

                    command.CommandText = InsertPerson2;
                    reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        ids[i] = reader.GetInt32(0);
                        i++;
                    }
                    reader.Close();

                    command.CommandText = InsertPerson3;
                    reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        ids[i] = reader.GetInt32(0);
                        i++;
                    }
                    reader.Close();


                    //insert addresses
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
            using (var connection = new SQLiteConnection(ConnectionString))
            {
                connection.Open();

                using (var command = new SQLiteCommand())
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

            using (var connection = new SQLiteConnection(ConnectionString))
            {
                connection.Open();

                using (var command = new SQLiteCommand())
                {
                    command.Connection = connection;

                    command.CommandText = "SELECT * FROM Person";
                    SQLiteDataReader reader = command.ExecuteReader();

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
                _sqliteConnector.GetData<Person>("SELECT PersId, Firstname, Lastname, Birthday FROM Person");
            Assert.IsTrue(personList != null);
            Assert.IsTrue(personList.Count == 3);

            List<Address> addressList =
                _sqliteConnector.GetData<Address>("SELECT AddressId, Street, HouseNumber, Zipcode, City FROM Address");
            Assert.IsTrue(addressList != null);
            Assert.IsTrue(addressList.Count == 3);

            List<PersonWithAddress> personWithAddressList =
                _sqliteConnector.GetData<PersonWithAddress>(
                    "SELECT p.PersId AS PersId, Firstname, Lastname, Birthday, Street, HouseNumber, Zipcode, City FROM Person p, Address a WHERE p.PersId = a.PersId");
            Assert.IsTrue(personWithAddressList != null);
            Assert.IsTrue(personWithAddressList.Count == 3);

            CleanupData();
        }

        [TestMethod]
        public void InsertData()
        {
            _sqliteConnector.InsertData(InsertPersons);

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
                Birthday = new DateTime(1942, 2, 2),
            };

            _sqliteConnector.InsertData(person1);
            _sqliteConnector.InsertData(person2);
            _sqliteConnector.InsertData(person3);

            Assert.IsTrue(VerifyDataInsert());

            CleanupData();
        }
    }
}