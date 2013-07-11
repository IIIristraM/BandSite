using BandSite.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace BandSite.Models.Implementations
{
    public class DbContextEfFactory: IDbContextFactory
    {
        public string ConnectionName { get; set; }

        public DbContextEfFactory(string connectionName)
        {
            ConnectionName = connectionName;
            Database.SetInitializer<DbContextEf>(new DropCreateDatabaseIfModelChanges<DbContextEf>());
        }

        public IDbContext CreateContext()
        {
            return new DbContextEf(ConnectionName);
        }
    }
}