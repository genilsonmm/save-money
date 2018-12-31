using Xamarin.Forms;
using System;
using System.IO;

using SQLite;
using SQLiteNetExtensions.Extensions;
using SaveMoney.Model;

namespace SaveMoney.Data
{
    public class SQLiteContext<T> : IDisposable where T : class
    {
        protected SQLiteConnection _connection;

        public SQLiteContext()
        {
            var config = DependencyService.Get<IDBConfig>();
            _connection = new SQLiteConnection(Path.Combine(config.Diretory, "savemoney_database.db3"));

            //_connection.DropTable<ExpenseControl>();
            //_connection.DropTable<Transaction>();
            //_connection.DropTable<Category>();

            _connection.CreateTable<ExpenseControl>();
            _connection.CreateTable<Transaction>();
            _connection.CreateTable<Category>();
        }

        public int Insert(T obj)
        {
            return _connection.Insert(obj);
        }

        public int Delete(T obj)
        {
            int result = _connection.Delete(obj);
            if (result != 1)
                throw new Exception();
            return result;
        }

        public void UpdateWithChildren(T obj)
        {
            _connection.UpdateWithChildren(obj);
        }

        public void Dispose()
        {
            _connection.Dispose();
        }
    }
}
