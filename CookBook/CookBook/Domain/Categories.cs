namespace CookBook.Domain;

public class Category
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime CreatedDate { get; set; } = DateTime.Now;

    // Конструкторы
    public Category() { }

    public Category(string name, string? description = null)
    {
        Name = name;
        Description = description;
    }
}
