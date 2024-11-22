using PROGPOE1.Models.DBEntities;
using Microsoft.EntityFrameworkCore;
using PROGPOE1.Models;


namespace PROGPOE1.DAL
{
    public class EmployeeDbContext : DbContext
    {
        public EmployeeDbContext(DbContextOptions options) : base(options) 
        { 
        }

        public DbSet<Employee> Employees { get; set; }
        public DbSet<User> Users { get; set; }

       
    }
}
