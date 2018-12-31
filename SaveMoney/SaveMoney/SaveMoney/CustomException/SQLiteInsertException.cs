using System;

namespace SaveMoney.CustomException
{
    public class SQLiteInsertException : Exception
    {
        public SQLiteInsertException(string entity)
            :base(string.Format("Falha ao cadastrar {0}", entity))
        {

        }
    }
}
