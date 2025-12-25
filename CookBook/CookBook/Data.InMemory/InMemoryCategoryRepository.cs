using Domain.Entities;
using Interfaces;


namespace Data.InMemory;

public class InMemoryCategoryRepository : ICategoryRepository
{
    private readonly List<Category> _categories = new();
    private int _nextId = 1;

    public Category? GetById(int id)
    {
        return _categories.FirstOrDefault(c => c.Id == id);
    }

    public IEnumerable<Category> GetAll()
    {
        return _categories;
    }

    public int Add(Category category)
    {
        category.Id = _nextId++;
        category.CreatedDate = DateTime.Now;
        _categories.Add(category);
        return category.Id;
    }

    public bool Update(Category category)
    {
        var existing = GetById(category.Id);
        if (existing == null) return false;

        var index = _categories.IndexOf(existing);
        _categories[index] = category;
        return true;
    }

    public bool Delete(int id)
    {
        var category = GetById(id);
        if (category == null) return false;

        return _categories.Remove(category);
    }

    public IEnumerable<Category> GetCategoriesWithRecipes()
    {
        // В реальной реализации здесь была бы связь с рецептами
        return _categories;
    }

    public Category? GetByName(string name)
    {
        return _categories.FirstOrDefault(c =>
            c.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
    }
}