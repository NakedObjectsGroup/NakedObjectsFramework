using EFCoreBugDemoNet6;

using (var rc = new RecordContext()) { 
    rc.Database.EnsureCreated();
}