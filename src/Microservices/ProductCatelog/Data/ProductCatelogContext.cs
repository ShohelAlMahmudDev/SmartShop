using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ProductCatelog.Models;

namespace ProductCatelog.Data
{
    public class ProductCatelogContext : DbContext
    {
        public ProductCatelogContext (DbContextOptions<ProductCatelogContext> options)
            : base(options)
        {
        }

        public DbSet<Product> Products { get; set; } = default!;
        public DbSet<ProductCategory>  ProductCategories { get; set; }
    }
}
