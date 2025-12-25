using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Interfaces
{
    public interface IRecipeIngredientRepository
    {
        int Add(RecipeIngredient recipeIngredient);
        RecipeIngredient? GetById(int id);
        List<RecipeIngredient> GetAll();
        bool Update(RecipeIngredient recipeIngredient);
        bool Delete(int id);

        // Дополнительные методы
        List<RecipeIngredient> GetByRecipeId(int recipeId);
        bool DeleteByRecipeId(int recipeId);
    }
}
