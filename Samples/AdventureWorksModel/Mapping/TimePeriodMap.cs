using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventureWorksModel {
    public class TimePeriodMap : ComplexTypeConfiguration<TimePeriod> {

        public TimePeriodMap() {
            Property(p => p.StartTime).HasColumnName("StartTime");
            Property(p => p.EndTime).HasColumnName("EndTime");
        }
    }
}
