﻿using NakedFunctions;
using System;

namespace NakedFunctions.Test
{
    public class MockClock : IClock
    {
        public DateTime Time { get; set; }

        #region Implementation of IClock
        public DateTime Now() => Time;

        public DateTime Today() => Time.Date;
        #endregion

    }
}