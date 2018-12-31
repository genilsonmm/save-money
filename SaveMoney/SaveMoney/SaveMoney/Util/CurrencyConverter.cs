namespace SaveMoney.Util
{
    public class CurrencyConverter
    {
        private static CurrencyConverter instance;

        public static CurrencyConverter Instance()
        {
            if (instance == null)
                instance = new CurrencyConverter();
            return instance;
        }

        public string Converter(string text)
        {
            decimal value = DecimalConverter.Instance().Converter(text.ToString());           
            string currencyValue = (value / 100).ToString("C");

            if (text != currencyValue)
                text = currencyValue;

            return text;
        }
    }
}
