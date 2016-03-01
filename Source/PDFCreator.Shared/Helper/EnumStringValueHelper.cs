using System;
using System.Reflection;

namespace pdfforge.PDFCreator.Shared.Helper
{
    public static class EnumToStringValueHelper
    {
        public static string GetStringValue(Enum value)
        {
            FieldInfo fi = value.GetType().GetField(value.ToString());

            StringValueAttribute[] attributes =
                (StringValueAttribute[])fi.GetCustomAttributes(typeof(StringValueAttribute), false);

            if (attributes != null && attributes.Length > 0)
                return attributes[0].Value;
            else
                return value.ToString();
        }
    }

    public class StringValueAttribute : Attribute
    {
        private readonly string _value;

        public StringValueAttribute(string value)
        {
            _value = value;
        }

        public string Value
        {
            get { return _value; }
        }

    }
}
