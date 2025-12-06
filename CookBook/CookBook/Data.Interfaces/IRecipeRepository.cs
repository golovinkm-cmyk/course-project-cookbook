using System.Collections.Generic;
using Domain;

namespace Data.Interfaces
{
    public interface IRecipeRepository
    {
        int Add(Recipe recipe);
        Recipe? GetById(int id);
        List<Recipe> GetAll();
        bool Update(Recipe recipe);
        bool Delete(int id);

        // Дополнительные методы
        List<Recipe> GetByCategory(int categoryId);
        List<Recipe> GetFavoriteRecipes();
        List<Recipe> SearchRecipes(string searchTerm);
    }
}