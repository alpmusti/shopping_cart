﻿using System;
namespace Trendyol.ShoppingCart.Models
{
    public class Product: BaseModel
    {
        public string Title { get; set; }

        public double Price { get; set; }

        public Category Category { get; set; }

        public Product(string title, double price, Category category)
        {
            Title = title;
            Price = price;
            Category = category;
        }
    }
}
