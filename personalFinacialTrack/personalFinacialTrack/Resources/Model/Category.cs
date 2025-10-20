using System;

namespace personalFinacialTrack.Resources.Model
{
    public class Category
    {
        public int CategoryId { get; set; }
        public string Name { get; set; } = string.Empty;
        public CategoryType Type { get; set; } = CategoryType.Expense;
        public DateTime CreatedAt { get; set; }
    }

    public enum CategoryType
    {
        Expense,
        Income
    }
}
