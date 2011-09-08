using System;

namespace Utilities.General
{
    public static class EnumUtil
    {
        public static string GetStringValue<T>(T enumValue) where T : struct, IConvertible
        {
            if (!typeof(T).IsEnum)
            {
                throw new ArgumentException("enumValue must be an enumerated type");
            }

            return Enum.GetName(typeof(T), enumValue);
        }

        public static T GetEnumFromString<T>(string value) where T : struct, IConvertible
        {
            return (T)Enum.Parse(typeof (T), value);
        }
    }
}
