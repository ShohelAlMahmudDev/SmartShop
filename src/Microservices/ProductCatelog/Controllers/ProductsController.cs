using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ProductCatelog.Data;
using ProductCatelog.Dtos;
using ProductCatelog.Models;

namespace ProductCatelog.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly ProductCatelogContext _context;
        private readonly ILogger<ProductsController> _logger;

        // Constructor with dependency injection
        public ProductsController(ProductCatelogContext context, ILogger<ProductsController> logger)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        // GET: api/Products
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductDto>>> GetProducts([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            try
            {
                // Validate pagination parameters
                if (pageNumber <= 0 || pageSize <= 0)
                {
                    return BadRequest("Page number and page size must be positive values.");
                }

                // Fetch products with pagination
                var products = await _context.Products
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .Select(p => new ProductDto
                    {
                        ProductId = p.ProductId,
                        Name = p.Name,
                        Price = p.Price,
                        // Include other necessary fields from Product
                    })
                    .ToListAsync();

                return Ok(products);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching products.");
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while processing your request.");
            }
        }

        // GET: api/Products/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Product>> GetProduct(int id)
        {
            try
            {
                // Log the request to fetch product by ID
                _logger.LogInformation($"Fetching product with ID: {id}");

                // Fetch the product from the database
                var product = await _context.Products.FindAsync(id);

                if (product == null)
                {
                    // Log when the product is not found
                    _logger.LogWarning($"Product with ID {id} not found.");

                    // Return a NotFound response with a custom message
                    return NotFound(new { message = $"Product with ID {id} not found." });
                }

                // Log successful retrieval of the product
                _logger.LogInformation($"Product with ID {id} found successfully.");

                // Return the product with a 200 OK status
                return Ok(product);
            }
            catch (Exception ex)
            {
                // Log any exception that occurs during the process
                _logger.LogError(ex, $"Error occurred while retrieving product with ID {id}.");

                // Return a generic internal server error response
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "An error occurred while processing your request." });
            }
        }


        // PUT: api/Products/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutProduct(int id, ProductUpdateDto productDto)
        {
            if (productDto == null)
            {
                return BadRequest("Product data is null.");
            }

            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            // Map only the allowed fields from productDto to the product entity
            product.Name = productDto.Name;
            product.Price = productDto.Price;
            // Map other fields as necessary

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProductExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }


        // POST: api/Products
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Product>> PostProduct(Product product)
        {
            // Validate the input model
            if (product == null)
            {
                return BadRequest("Product data is null.");
            }
            try
            {
                _context.Products.Add(product);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                // Log the exception
                _logger.LogError(ex, "Error occurred while saving the product.");

                // Return a generic error message
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while processing your request.");
            }


            return CreatedAtAction(nameof(GetProduct), new { id = product.ProductId }, product);
        }

        // DELETE: api/Products/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            // Log the deletion request
            _logger.LogInformation($"Attempting to delete product with ID: {id}");

            // Fetch the product from the database
            var product = await _context.Products.FindAsync(id);

            if (product == null)
            {
                // Log when the product is not found
                _logger.LogWarning($"Product with ID {id} not found.");
                return NotFound(new { message = $"Product with ID {id} not found." });
            }

            try
            {
                // Remove the product from the database
                _context.Products.Remove(product);
                await _context.SaveChangesAsync();

                // Log the successful deletion
                _logger.LogInformation($"Product with ID {id} deleted successfully.");

                // Return NoContent status (indicates successful deletion with no content to return)
                return NoContent();
            }
            catch (Exception ex)
            {
                // Log any exception that occurs
                _logger.LogError(ex, $"Error occurred while deleting product with ID {id}.");

                // Return an internal server error
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "An error occurred while processing your request." });
            }
        }
        private bool ProductExists(int id)
        {
            try
            {
                // Log the check for product existence
                _logger.LogInformation($"Checking if product with ID {id} exists.");

                // Check if the product exists in the database
                var exists = _context.Products.Any(e => e.ProductId == id);

                // Log the result of the existence check
                if (exists)
                {
                    _logger.LogInformation($"Product with ID {id} exists.");
                }
                else
                {
                    _logger.LogInformation($"Product with ID {id} does not exist.");
                }

                return exists;
            }
            catch (Exception ex)
            {
                // Log any exceptions encountered during the existence check
                _logger.LogError(ex, $"An error occurred while checking for product with ID {id}.");
                return false;  // Return false in case of an error
            }
        }

    }
}
