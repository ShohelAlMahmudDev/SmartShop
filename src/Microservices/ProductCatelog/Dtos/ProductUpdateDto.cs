namespace ProductCatelog.Dtos
{
    public class ProductUpdateDto
    {
        public required string Name { get; set; }
        public decimal Price { get; set; }
        // Include only properties that should be updatable by clients
    }
}
