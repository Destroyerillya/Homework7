using Microsoft.EntityFrameworkCore;

namespace Blog
{
    public class Context: DbContext
    {
        public DbSet<UserData> Users { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(@"Server=.\SQLEXPRESS;Database=blog;Trusted_Connection=True;");
        }
    }
}