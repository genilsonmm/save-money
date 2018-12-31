using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Xamarin.Forms;

namespace SaveMoney.Behaviors
{
    public class CurrencyBehavior : Behavior<Entry>
    {
        protected override void OnAttachedTo(Entry entry)
        {
            entry.TextChanged += OnEntryTextChanged;
            base.OnAttachedTo(entry);
        }

        protected override void OnDetachingFrom(Entry entry)
        {
            entry.TextChanged -= OnEntryTextChanged;
            base.OnDetachingFrom(entry);
        }

        private void OnEntryTextChanged(object sender, TextChangedEventArgs args)
        {
            var entry = sender as Entry;

            string text = entry.Text;
            string valueFromString = Regex.Replace(text.ToString(), @"\D", "");

            Decimal.TryParse(valueFromString, out decimal valueLong);
            string currencyValue = (valueLong / 100).ToString("C");

            if (entry.Text != currencyValue)
                entry.Text = currencyValue;

        }
    }
}
