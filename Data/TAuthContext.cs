using Microsoft.EntityFrameworkCore;
using MongoDB_Session_Login.Models.LoginForLongPv;

namespace MongoDB_Session_Login.Data
{
    public class TAuthContext : DbContext
    {
        public TAuthContext(DbContextOptions<TAuthContext> options) : base(options)
        {

        }
        //public DbSet<CheckLogin> ResponseLogin { get; set; }
        //public DbSet<UserTest> UserLogin { get; set; }
        //public DbSet<Permit> Permits { get; set; }

        public DbSet<TauthUserlogin> TAUTH_USERLOGIN { get; set; }
        public DbSet<TauthClientsession> TAUTH_CLIENTSESSION  { get; set; }
        public DbSet<TauthClientsessionlog> TAUTH_CLIENTSESSIONLOG { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }
}
