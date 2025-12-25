using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain;
using Domain.Entities;

namespace Data.Interfaces
{
    public interface IIngredientRepository : IRepository<Ingredient>
    {
        Ingredient? GetByName(string name);
        IEnumerable<Ingredient> GetByCategory(string category);
    }
}
