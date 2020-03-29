using Microsoft.EntityFrameworkCore;

namespace CustomerApi.Models
{
    public class CustomerDatabaseContext : DbContext
    {
        public CustomerDatabaseContext(DbContextOptions<CustomerDatabaseContext> options) : base (options) {}
        public DbSet<Customer> Customer { get;set; }
        public DbSet<User> User { get;set; }
    }    
}