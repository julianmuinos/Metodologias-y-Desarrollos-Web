using MyApp.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MyApp.Core.Models
{
    public class Product : IEntity
    {
        [Key]
        public int Id { get; set; }

        [Display(Name = "Name")]
        [Required(ErrorMessage = "Insert a name")]
        public string Name { get; set; }

        [Display(Name = "Description")]
        [Required(ErrorMessage = "Insert a description")]
        public string Description { get; set; }

        [Display(Name = "Category")]
        [Required(ErrorMessage = "Insert a category")]
        public string Category { get; set; }

        [Display(Name = "Price")]
        [Required(ErrorMessage = "Insert a price")]
        public decimal Price { get; set; }
        public string ImageURL { get; set; }
        public Product() { }
        public Product(object[] ItemArray)
        {
            Id = int.Parse(ItemArray[0].ToString());
            Name = ItemArray[1].ToString();
            Description = ItemArray[2].ToString();
            Category = ItemArray[3].ToString();
            Price = decimal.Parse(ItemArray[4].ToString());
            ImageURL = ItemArray[5].ToString();
        }
    }
}