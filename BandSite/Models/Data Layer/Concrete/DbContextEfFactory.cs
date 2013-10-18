using System.Data.Entity;

namespace BandSite.Models.DataLayer
{
    public class DbContextEfFactory: IDbContextFactory
    {
        public string ConnectionName { get; set; }

        public DbContextEfFactory(string connectionName)
        {
            ConnectionName = connectionName;
            SetInitializer();
        }

        public DbContextEfFactory()
        { 
            ConnectionName = "DefaultConnection";
            SetInitializer();
        }

        protected void SetInitializer()
        {
            Database.SetInitializer(new DropCreateDatabaseIfModelChanges<DbContextEf>());
            using (CreateContext()) { }
        }

        public IDbContext CreateContext()
        {
            var context = new DbContextEf(ConnectionName);
            context.Database.Initialize(false);        
            return context;
        }
    }
}