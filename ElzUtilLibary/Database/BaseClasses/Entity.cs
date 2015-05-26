using System.Collections.Generic;
using System.Reflection;

namespace ElzUtilLibary.Database.BaseClasses
{
    public abstract class Entity
    {
        internal readonly Dictionary<string, object> OriginalProperties;

        protected Entity()
        {
            OriginalProperties = new Dictionary<string, object>();
        }

        internal void SaveOriginalProperties()
        {
            PropertyInfo[] properties = GetType().GetProperties();

            foreach (PropertyInfo propertyInfo in properties)
            {
                if (OriginalProperties.ContainsKey(propertyInfo.Name))
                {
                    OriginalProperties[propertyInfo.Name] = propertyInfo.GetValue(this);
                }
                else
                {
                    OriginalProperties.Add(propertyInfo.Name, propertyInfo.GetValue(this));
                }
            }
        }
    }
}