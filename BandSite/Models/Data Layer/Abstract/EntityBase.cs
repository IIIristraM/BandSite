using System;
using System.Linq;
using System.Reflection;

namespace BandSite.Models.DataLayer
{
    public class EntityBase
    {
        public virtual int Id { get; set; }

        public virtual bool TrySetPropertiesFrom(object source, bool ignoreNulls = true)
        {
            if (source != null)
            {
                return source.GetType().GetProperties().Aggregate(false, (current, property) => TrySetProperty(source, property.Name, ignoreNulls) || current);
            }
            return false;
        }

        public virtual bool TrySetProperty(object src, string propertyName, bool ignoreNulls = true)
        {
            if (!propertyName.Contains("Id"))
            {
                PropertyInfo property = GetType().GetProperty(propertyName);
                PropertyInfo srcProperty = src.GetType().GetProperty(propertyName);
                if ((property != null) && (srcProperty != null))
                {
                    Type propertyType = property.PropertyType;
                    if ((srcProperty.PropertyType == propertyType) && 
                        (property.CanWrite) && 
                        ((srcProperty.GetValue(src) != null) || (!ignoreNulls))) //дает false только когда и ignoreNulls = true и свойство равно null
                    {
                        property.SetValue(this, srcProperty.GetValue(src));
                        return true;
                    }
                }
            }
            return false;
        }
    }
}
