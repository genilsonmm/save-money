using System;
using System.Text.RegularExpressions;

namespace SaveMoney.Util
{
    public class DecimalConverter
    {
        private static DecimalConverter instance;

        public static DecimalConverter Instance()
        {
            if (instance == null)
                instance = new DecimalConverter();
            return instance;
        }

        public Decimal Converter(string text)
        {
            string valueFromString = Regex.Replace(text, @"\D", "");
            Decimal.TryParse(valueFromString, out decimal decimalValue);
            return decimalValue;
        }
    }
}
