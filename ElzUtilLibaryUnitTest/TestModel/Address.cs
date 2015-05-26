
using ElzUtilLibary.Database.BaseClasses;

namespace ElzUtilLibaryUnitTest.TestModel
{
    public class Address : Entity
    {
        public int AddressId { get; set; }

        public string Street { get; set; }
        public string HouseNumber { get; set; }
        public int Zipcode { get; set; }
        public string City { get; set; }

        public int PersId { get; set; }
    }
}