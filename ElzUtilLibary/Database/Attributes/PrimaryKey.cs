using System;

namespace ElzUtilLibary.Database.Attributes
{
    [AttributeUsage(AttributeTargets.Property,
        AllowMultiple = false)
    ]
    public class PrimaryKey : Attribute
    {
    }
}