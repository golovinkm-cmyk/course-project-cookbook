using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class Recipe
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string Instructions { get; set; } = string.Empty;
        public int CategoryId { get; set; }

        public virtual Category? Category { get; set; }
        public virtual ICollection<RecipeIngredient> RecipeIngredients { get; set; } = new List<RecipeIngredient>();
        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }
        public bool IsFavorite { get; set; }
        public bool IsPremium { get; set; }
        public required string DifficultyLevel { get; set; }
        public int PreparationTime { get; set; }
        public int CookingTime { get; set; }
        public int TotalTime { get; set; }
        public int Servings { get; set; }
        public required Action<object, object> PropertyChanged { get; set; }
    }
}
