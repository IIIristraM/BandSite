namespace BandSite.Models.DataLayer
{
    public interface IDbContextFactory
    {
        IDbContext CreateContext();
    }
}
