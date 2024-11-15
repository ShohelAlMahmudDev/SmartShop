namespace ProductCatelog.Dtos
{
    public class ProductDto
    {
        public int ProductId { get; set; }
        public required string Name { get; set; }
        public decimal Price { get; set; }
        // Add other necessary fields
    }
}
