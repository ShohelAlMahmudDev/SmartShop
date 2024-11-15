using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace ProductCatelog.Models
{
    public class ProductCategory
    {
        [Key]
        public int CategoryId { get; set; }
        public required string Name { get; set; }
        public string? Description { get; set; }

        public List<Product>? Products { get; set; }


    }
}
