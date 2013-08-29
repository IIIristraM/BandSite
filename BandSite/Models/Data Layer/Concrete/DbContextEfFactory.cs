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
            #if DEBUG
                 ConnectionName = "BandSiteDB-Debug";
            #else
                 ConnectionName = "BandSiteDB";
            #endif
            SetInitializer();
        }

        protected void SetInitializer()
        {
            Database.SetInitializer<DbContextEf>(new DropCreateDatabaseIfModelChanges<DbContextEf>());
            using (CreateContext()) { }
        }

        public IDbContext CreateContext()
        {
            var context = new DbContextEf(ConnectionName);
            context.Database.CreateIfNotExists();
            context.Database.Initialize(false);
            return context;
        }
    }
}