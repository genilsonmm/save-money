using SaveMoney.CustomException;
using SaveMoney.Model;
using SQLiteNetExtensions.Extensions;
using System.Collections.Generic;
using System.Linq;

namespace SaveMoney.Data
{
    public class ExpenseControlRepository : SQLiteContext<ExpenseControl>
    {
        public ExpenseControl GetById(int id, bool lazyLoad)
        {         
            if (lazyLoad)
            {
                ExpenseControl expenseControl = _connection.GetWithChildren<ExpenseControl>(id);
                foreach (var transaction in expenseControl.Transactions)
                {
                    using (var category = new CategoryRepository())
                    {
                        transaction.Category = category.GetById(transaction.CategoryId);
                    }
                }
                return expenseControl;
            }
            else
                return _connection.Table<ExpenseControl>().First(e => e.ExpenseControlId == id);
        }

        public int Remove(ExpenseControl expenseControl)
        {
            foreach (var transaction in expenseControl.Transactions)
            {
                using (var transactionRepository = new TransactionRepository())
                {
                    transactionRepository.Delete(transaction);
                }
            }

            return base.Delete(expenseControl);
        }

        public List<ExpenseControl> GetByDate(int year, int month)
        {
            return _connection.GetAllWithChildren<ExpenseControl>(e => e.BeginYear == year && e.BeginMonth == month).ToList();
        }

        public List<ExpenseControl> GetAll()
        {
            return _connection.Table<ExpenseControl>().OrderBy(y=>y.BeginYear).ThenBy(m=>m.BeginMonth).ToList();
        }

        public List<string> GetAllYearAndMonths()
        {
            var temp = _connection.Query<ExpenseControl>("select distinct BeginYear, BeginMonth from ExpenseControl");
            List<string> dates = new List<string>();
            foreach (var item in temp)
            {
                dates.Add($"{item.BeginYear}/{item.BeginMonth}");
            }
            return dates;
        }

        public void Add(ExpenseControl expenseControl)
        {
            int id = _connection.Insert(expenseControl);
            if (id == 0)
                throw new SQLiteInsertException(expenseControl.GetType().Name.ToLower());
        }

        public void Update(ExpenseControl expenseControl)
        {
            ExpenseControl currentExpenseControl = GetById(expenseControl.ExpenseControlId, false);
            currentExpenseControl.TotalValue = expenseControl.TotalValue;
            currentExpenseControl.EndYear = expenseControl.EndYear;
            currentExpenseControl.EndMonth = expenseControl.EndMonth;
            currentExpenseControl.EndDay = expenseControl.EndDay;
            currentExpenseControl.Title = expenseControl.Title;
            _connection.Update(currentExpenseControl);
        }
    }
}
