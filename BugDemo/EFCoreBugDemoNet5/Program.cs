using EFCoreBugDemoNet5;

using (var rc = new RecordContext()) { 
    rc.Database.EnsureCreated();
}
