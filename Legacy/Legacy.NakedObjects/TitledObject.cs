﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NakedObjects;

// ReSharper disable InconsistentNaming

namespace Legacy.NakedObjects {
    public interface TitledObject {
        [NakedObjectsIgnore]
        Title title();
    }
}
