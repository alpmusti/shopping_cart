using System;
namespace Trendyol.ShoppingCart.Models
{
    public class Category: BaseModel
    {
        private Category Parent { get; set; }

        public string Title { get; set; }

        public Category(string title)
        {
            Title = title;
        }
    }
}
