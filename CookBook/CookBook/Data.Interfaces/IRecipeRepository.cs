using System.Collections.Generic;
using Data.Interfaces.Filters;
using Domain;
using Domain.Entities;

namespace Data.Interfaces
{
    public interface IRecipeRepository : IRepository<Recipe>
    {
        IEnumerable<Recipe> GetByCategory(int categoryId);
        IEnumerable<Recipe> GetFavorites();
        IEnumerable<Recipe> Search(string keyword);
        IEnumerable<Recipe> GetPremiumRecipes();
        IEnumerable<Recipe> GetRecipesWithIngredients();
        IEnumerable<Recipe> GetFiltered(RecipeFilter filter);
    }
}