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
        void Add(RecipeIngredient recipeIngredient);
        void AddRange(IEnumerable<RecipeIngredient> recipeIngredients);
        void Update(RecipeIngredient recipeIngredient);
        void Delete(int id);
        void DeleteByRecipeId(int recipeId);
        RecipeIngredient GetById(int id);
        IEnumerable<RecipeIngredient> GetByRecipeId(int recipeId);
        IEnumerable<RecipeIngredient> GetAll();
    }
}
