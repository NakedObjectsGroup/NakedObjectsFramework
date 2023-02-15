namespace ROSI.Test.Data;

public class EFCoreObjectDbContext : EFCoreTestDbContext {
    public EFCoreObjectDbContext() : base(Constants.CsROSI) { }
    public void Delete() => Database.EnsureDeleted();

    public void Create() => Database.EnsureCreated();
}