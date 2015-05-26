using System;
using ElzUtilLibary.Database.BaseClasses;

namespace ElzUtilLibaryUnitTest.TestModel
{
    public class PersonWithAddress : Entity
    {
        public int PersId { get; set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public DateTime Birthday { get; set; }

        public string Street { get; set; }
        public string HouseNumber { get; set; }
        public int Zipcode { get; set; }
        public string City { get; set; }
    }
}