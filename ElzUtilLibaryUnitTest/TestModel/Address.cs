﻿
namespace ElzUtilLibaryUnitTest.TestModel
{
    public class Address
    {
        public int AddressId { get; set; }

        public string Street { get; set; }
        public string HouseNumber { get; set; }
        public int Zipcode { get; set; }
        public string City { get; set; }

        public int PersId { get; set; }
    }
}