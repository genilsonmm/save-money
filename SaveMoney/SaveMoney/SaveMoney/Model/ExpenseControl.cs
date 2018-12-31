using SQLite;
using SQLiteNetExtensions.Attributes;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace SaveMoney.Model
{
    public class ExpenseControl
    {
        [PrimaryKey, AutoIncrement]
        public int ExpenseControlId { get; set; }
        public string Title { get; set; }
        public string TotalValue { get; set; }
        public int BeginYear { get; set; }
        public int BeginMonth { get; set; }
        public int BeginDay { get; set; }
        public int EndYear { get; set; }
        public int EndMonth { get; set; }
        public int EndDay { get; set; }

        [OneToMany]
        public List<Transaction> Transactions { get; set; }

        [Ignore]
        public string BeginDateFormatted => $"{BeginDay}/{BeginMonth}/{BeginYear}";
        [Ignore]
        public string EndDateFormatted => $"{EndDay}/{EndMonth}/{EndYear}";

        [Ignore]
        public string AvailableValue => GetAvailableValue();

        [Ignore]
        public string SpendValue => (TotalSpend() / 100).ToString("C");

        private string GetAvailableValue()
        {
            decimal availableValue = TotalSpend();

            string totalValueFromString = Regex.Replace(TotalValue, @"\D", "");
            Decimal.TryParse(totalValueFromString, out decimal decimalTotalValue);

            return ((decimalTotalValue - availableValue) / 100).ToString("C");
        }

        private decimal TotalSpend()
        {
            decimal availableValue = 0;

            if (Transactions != null)
            {
                foreach (var item in Transactions)
                {
                    string valueFromString = Regex.Replace(item.Value, @"\D", "");
                    Decimal.TryParse(valueFromString, out decimal decimalValue);
                    availableValue += decimalValue;
                }
            }

            return availableValue;
        }
    }
}
