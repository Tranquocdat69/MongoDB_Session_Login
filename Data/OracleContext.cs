using Microsoft.EntityFrameworkCore;
using MongoDB_Session_Login.Models.LoginForLongPv;

namespace MongoDB_Session_Login.Data
{
    public class OracleContext : DbContext
    {
        public OracleContext(DbContextOptions<OracleContext> options) : base(options)
        {

        }
        public DbSet<CheckLogin> ResponseLogin { get; set; }
        public DbSet<UserTest> UserLogin { get; set; }
        public DbSet<Permit> Permits { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }
}
