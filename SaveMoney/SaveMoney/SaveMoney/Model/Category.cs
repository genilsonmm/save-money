using SQLite;
using SQLiteNetExtensions.Attributes;
using System.Collections.Generic;

namespace SaveMoney.Model
{
    public class Category
    {
        [PrimaryKey, AutoIncrement]
        public int CategoryId { get; set; }
        public string Name { get; set; }
        public string Image { get; set; }

        public override string ToString()
        {
            return this.Name;
        }
    }
}
