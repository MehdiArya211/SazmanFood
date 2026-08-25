using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Utilities.Extentions
{
    public static class EnumExtensions
    {



        /// <summary>
        /// گرفتن یک نام فارسی از Description یک enum
        /// get userfriendly name from Description attr of enum 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string GetEnumDescription(this Enum value)
        {
            if (value == null) return null;
            Type type = value.GetType();
            string name = Enum.GetName(type, value);
            if (name != null)
            {
                FieldInfo field = type.GetField(name);
                if (field != null)
                {
                    DescriptionAttribute attr =
                           Attribute.GetCustomAttribute(field,
                             typeof(DescriptionAttribute)) as DescriptionAttribute;
                    if (attr != null)
                    {
                        return attr.Description;
                    }
                }
            }
            return null;
        }



        /// <summary>
        /// نحوه استفاده 
        /// ExtentionMethods.ToEnumViewModel<EnumName>()
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static List<EnumViewModel> ToEnumViewModel<T>()
        {
            var model = Enum.GetValues(typeof(T)).Cast<int>()
                .Select(id => new EnumViewModel
                {
                    Id = id,
                    Key = Enum.GetName(typeof(T), id),
                    Title = ((Enum)Enum.Parse(typeof(T), Enum.GetName(typeof(T), id), true)).GetEnumDescription()
                }).ToList();
            return model;
        }




        /// <summary>
        /// گرفتن یک اتریبیوت کاستوم از اینام
        /// <para>
        /// enumName.GetEnumAttribute<DataTypeAttribute>()
        /// </para>
        /// </summary>
        /// <typeparam name="T">نوع کلاس اتریبیوت</typeparam>
        /// <param name="value">خود اینام</param>
        /// <returns></returns>
        public static T GetEnumAttribute<T>(this Enum value)
        {
            if (value == null) return default(T);
            Type type = value.GetType();
            string name = Enum.GetName(type, value);
            if (name != null)
            {
                FieldInfo field = type.GetField(name);
                if (field != null)
                {
                    var info = field.GetCustomAttributes(typeof(T), false).OfType<T>().FirstOrDefault();
                    return info;
                }
            }
            return default(T);
        }



        public static CustomDataTypeAttribute GetEnumCustomAttribute(this Enum value)
        {
            if (value == null) return null;
            Type type = value.GetType();
            string name = Enum.GetName(type, value);
            if (name != null)
            {
                FieldInfo field = type.GetField(name);
                if (field != null)
                {
                    CustomDataTypeAttribute attr =
                           Attribute.GetCustomAttribute(field,
                             typeof(CustomDataTypeAttribute)) as CustomDataTypeAttribute;
                    if (attr != null)
                    {
                        return attr;
                    }
                }
            }
            return null;
        }

    }










}
