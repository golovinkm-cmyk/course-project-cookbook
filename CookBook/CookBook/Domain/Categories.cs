using System.ComponentModel.DataAnnotations.Schema;

namespace Domain
{
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime CreatedDate { get; set; }

        // Вычисляемое свойство для UI
        [NotMapped] // Если будете использовать EF Core
        public int RecipesCount { get; set; }

        // Конструкторы
        public Category() { }

        public Category(string name, string description)
        {
            Name = name;
            Description = description;
            CreatedDate = DateTime.Now;
        }
    }
}
