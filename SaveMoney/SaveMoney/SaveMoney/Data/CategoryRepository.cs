using SaveMoney.CustomException;
using SaveMoney.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SaveMoney.Data
{
    public class CategoryRepository : SQLiteContext<Category>
    {
        public Category GetById(int id)
        {
            return _connection.Table<Category>().SingleOrDefault(c => c.CategoryId == id);
        }

        public List<Category> GetAll()
        {
            return _connection.Table<Category>().OrderBy(c => c.Name).ToList();
        }

        public Category GetByName(string name)
        {
            return _connection.Table<Category>().Where(e => e.Name.ToUpper().Equals(name.ToUpper())).SingleOrDefault();
        }

        public int Add(Category newCategory)
        {
            Category category = GetByName(newCategory.Name);
            int id = 0;
            if (category == null)
            {
                id = _connection.Insert(newCategory);
                if (id == 0)
                    throw new SQLiteInsertException(newCategory.GetType().Name.ToLower());
            }
            else
            {
                category.Image = newCategory.Image;
                _connection.Update(category);
            }
            return id;
        }
    }
}
