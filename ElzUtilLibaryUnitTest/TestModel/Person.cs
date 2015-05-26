﻿using System;
using ElzUtilLibary.Database.Attributes;
using ElzUtilLibary.Database.BaseClasses;

namespace ElzUtilLibaryUnitTest.TestModel
{
    public class Person : Entity
    {
        [PrimaryKey]
        [PrimaryKeyAutogeneratedByDatabase]
        public int PersId { get; set; }

        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public DateTime Birthday { get; set; }
    }
}