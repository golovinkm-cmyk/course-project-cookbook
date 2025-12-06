using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain;

namespace Interfaces
{
    public interface ICategoryRepository
    {
        int Add(Category category);
        Category? GetById(int id);
        List<Category> GetAll();
        bool Update(Category category);
        bool Delete(int id);

        // Дополнительные методы
        bool HasRecipes(int categoryId);
        Category? GetByName(string name);
    }
}

