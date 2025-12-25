using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class RecipeIngredient
    {
        public int Id { get; set; }
        public int RecipeId { get; set; }
        public int IngredientId { get; set; }

        public virtual Recipe? Recipe { get; set; }
        public virtual Ingredient? Ingredient { get; set; }
    }
}
