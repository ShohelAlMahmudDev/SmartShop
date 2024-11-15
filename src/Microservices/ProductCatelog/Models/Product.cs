using System.ComponentModel.DataAnnotations;

namespace ProductCatelog.Models
{
    public class Product
    {

        [Key]
        public int ProductId { get; set; }
        public required string Name { get; set; }
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public string? SKU { get; set; }
        
        //Foreign key to the ProductCategory
        public int CategoryId { get; set; }
        public ProductCategory? Category { get; set; }
    

            
    }
    
}
