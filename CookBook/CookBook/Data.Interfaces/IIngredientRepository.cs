using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain;

namespace Data.Interfaces
{
    public interface IIngredientRepository
    {
        int Add(Ingredient ingredient);
        Ingredient? GetById(int id);
        List<Ingredient> GetAll();
        bool Update(Ingredient ingredient);
        bool Delete(int id);

        // Дополнительные методы
        List<Ingredient> SearchIngredients(string searchTerm);
        Ingredient? GetByName(string name);
    }
}
