using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Interfaces.Filters
{
    public record RecipeFilter
    {
        public static RecipeFilter Empty => new();

        public int? CategoryId { get; init; }
        public string? DifficultyLevel { get; init; }
        public int? MaxCookingTime { get; init; }
        public string? SearchKeyword { get; init; }
        public bool? IsFavorite { get; init; }
        public bool? IsPremium { get; init; }
        public DateTime? StartDate { get; init; }
        public DateTime? EndDate { get; init; }
    }
}
