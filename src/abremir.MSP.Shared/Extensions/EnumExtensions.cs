﻿using System.ComponentModel;
using System.Reflection;

namespace abremir.MSP.Shared.Extensions
{
    public static class EnumExtensions
    {
        public static string GetDescription(this Enum value)
        {
            FieldInfo? fi = value.GetType().GetField(value.ToString());

            DescriptionAttribute[] attributes = (DescriptionAttribute[])fi
                !.GetCustomAttributes(typeof(DescriptionAttribute), false);

            return attributes is not null && attributes.Length > 0
                ? attributes[0].Description
                : value.ToString();
        }
    }
}
