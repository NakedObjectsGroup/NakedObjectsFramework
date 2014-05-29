using System.Data.Entity;
using System.Data.Entity.ModelConfiguration;
using System.Data.Entity.Infrastructure;   

namespace $rootnamespace$
{
    public class $safeitemname$ : EntityTypeConfiguration<SpecifyType>
    {
        public $safeitemname$()
        {           
              //Example:         
              //this.HasKey(t => t.Employee_ID);        
              //this.ToTable("Employee", "HumanResources");
              //this.Property(t => t.Employee_ID).HasColumnName("EmployeeID");
              //this.Property(t => t.NationalIDNumber).HasColumnName("NationalIDNumber").IsRequired().HasMaxLength(15);
         }
    }
}