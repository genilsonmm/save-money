using SaveMoney.CustomException;
using SaveMoney.Model;
using System.Linq;

namespace SaveMoney.Data
{
    public class TransactionRepository : SQLiteContext<Transaction>
    {
        public Transaction GetById(int id)
        {
            var transaction = _connection.Table<Transaction>().SingleOrDefault(t => t.TransactionId == id);

            using (var category = new CategoryRepository())
            {
                transaction.Category = category.GetById(transaction.CategoryId);
            }
            using (var expenseControl = new ExpenseControlRepository())
            {
                transaction.ExpenseControl = expenseControl.GetById(transaction.ExpenseControlId, false);
            }
            return transaction;
        }

        public void Add(Transaction transaction)
        {
            int id = _connection.Insert(transaction);
            if (id == 0)
                throw new SQLiteInsertException(transaction.GetType().Name.ToLower());
        }

        public void Update(Transaction transaction)
        {
            Transaction currentTransaction = GetById(transaction.TransactionId);
            currentTransaction.Value = transaction.Value;
            currentTransaction.CategoryId = transaction.CategoryId;
            currentTransaction.Description = transaction.Description;
            _connection.Update(currentTransaction);
        }
    }
}
