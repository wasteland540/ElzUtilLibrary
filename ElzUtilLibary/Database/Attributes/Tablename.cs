using System;

namespace ElzUtilLibary.Database.Attributes
{
    [AttributeUsage(AttributeTargets.Class,
        AllowMultiple = false)
    ]
    public class Tablename : Attribute
    {
        public Tablename()
        {
            Name = string.Empty;
        }

        public string Name { get; set; }
    }
}