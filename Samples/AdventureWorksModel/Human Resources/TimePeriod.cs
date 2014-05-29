// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 
using System;
using NakedObjects;
using System.ComponentModel.DataAnnotations.Schema;

namespace AdventureWorksModel
{
    //[Value(SemanticsProviderClass = typeof(TimePeriodValueSemanticsProvider))]
    [ComplexType]
    public class TimePeriod
    {
        public virtual DateTime StartTime { get; set; }

        public virtual DateTime EndTime { get; set; }
    }
}
