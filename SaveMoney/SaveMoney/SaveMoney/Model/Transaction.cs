using SaveMoney.Enums;
using SQLite;
using SQLiteNetExtensions.Attributes;
using System;

namespace SaveMoney.Model
{
    public class Transaction
    {
        [PrimaryKey, AutoIncrement]
        public int TransactionId { get; set; }
        public TransactionType TransactionType { get; set; }
        public string Value { get; set; }
        public int Year { get; set; }
        public int Month { get; set; }
        public int Day { get; set; }
        public int Hour { get; set; }
        public int Minutes { get; set; }
        public int Seconds { get; set; }
        public string Description { get; set; }
        
        public int ExpenseControlId { get; set; }
        public int CategoryId { get; set; }

        [ManyToOne]
        public ExpenseControl ExpenseControl { get; set; }

        [Ignore]
        public string FullDate => new DateTime(Year, Month, Day).ToShortDateString();

        [Ignore]
        public string FullTime => new DateTime(Year, Month, Day, Hour, Minutes, Seconds).ToShortTimeString();

        [Ignore]
        public Category Category { get; set; }
    }
}
