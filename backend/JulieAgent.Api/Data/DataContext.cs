using Microsoft.EntityFrameworkCore;
using JulieAgent.Api.Models;

namespace JulieAgent.Api.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
    }
}