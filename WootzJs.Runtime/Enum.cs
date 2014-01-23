﻿using System.Collections.Generic;
using System.Reflection;

namespace System
{
    public abstract class Enum : ValueType
    {
        private string name;
        private object value;

        private static Dictionary<string, Dictionary<string, Enum>> enumsByTypeAndName = new Dictionary<string, Dictionary<string, Enum>>();
        private static Dictionary<string, Dictionary<object, Enum>> enumsByTypeAndValue = new Dictionary<string, Dictionary<object, Enum>>();

        public Enum(string name, object value)
        {
            this.name = name;
            this.value = value;

            Dictionary<string, Enum> enumsByName;
            if (!enumsByTypeAndName.TryGetValue(___type.TypeName, out enumsByName))
            {
                enumsByName = new Dictionary<string, Enum>();
                enumsByTypeAndName[___type.TypeName] = enumsByName;
            }
            enumsByName[name] = this;

            Dictionary<object, Enum> enumsByValue;
            if (!enumsByTypeAndValue.TryGetValue(___type.TypeName, out enumsByValue))
            {
                enumsByValue = new Dictionary<object, Enum>();
                enumsByTypeAndValue[___type.TypeName] = enumsByValue;
            }
            enumsByValue[value] = this;
        }

        public object GetValue()
        {
            return value;
        }

        public override string ToString()
        {
            return name;
        }

        /// <summary>
        /// Converts the string representation of the name or numeric value of one or more enumerated constants to an equivalent enumerated object.
        /// </summary>
        /// 
        /// <returns>
        /// An object of type <paramref name="enumType"/> whose value is represented by <paramref name="value"/>.
        /// </returns>
        /// <param name="enumType">An enumeration type. </param><param name="value">A string containing the name or value to convert. </param><exception cref="T:System.ArgumentNullException"><paramref name="enumType"/> or <paramref name="value"/> is null. </exception><exception cref="T:System.ArgumentException"><paramref name="enumType"/> is not an <see cref="T:System.Enum"/>.-or- <paramref name="value"/> is either an empty string or only contains white space.-or- <paramref name="value"/> is a name, but not one of the named constants defined for the enumeration. </exception><exception cref="T:System.OverflowException"><paramref name="value"/> is outside the range of the underlying type of <paramref name="enumType"/>.</exception><filterpriority>1</filterpriority>
        public static object Parse(Type enumType, string value)
        {
            return enumsByTypeAndName[enumType.thisType.TypeName][value];
        }

        /// <summary>
        /// Converts the specified object with an integer value to an enumeration member.
        /// </summary>
        /// 
        /// <returns>
        /// An enumeration object whose value is <paramref name="value"/>.
        /// </returns>
        /// <param name="enumType">The enumeration type to return. </param><param name="value">The value convert to an enumeration member. </param><exception cref="T:System.ArgumentNullException"><paramref name="enumType"/> or <paramref name="value"/> is null. </exception><exception cref="T:System.ArgumentException"><paramref name="enumType"/> is not an <see cref="T:System.Enum"/>.-or- <paramref name="value"/> is not type <see cref="T:System.SByte"/>, <see cref="T:System.Int16"/>, <see cref="T:System.Int32"/>, <see cref="T:System.Int64"/>, <see cref="T:System.Byte"/>, <see cref="T:System.UInt16"/>, <see cref="T:System.UInt32"/>, or <see cref="T:System.UInt64"/>. </exception><filterpriority>1</filterpriority>
        public static object ToObject(Type enumType, object value)
        {
            return enumsByTypeAndValue[enumType.thisType.TypeName][value];
        }

        public static object InternalToObject(JsTypeFunction enumType, object value)
        {
            return enumsByTypeAndValue[enumType.TypeName][value];
        }
    }
}