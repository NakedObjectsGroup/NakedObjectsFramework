// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 
using System;
using System.Threading;
using Common.Logging;
using NakedObjects.Core.Util;

namespace NakedObjects.Xat.Performance {
    /// <summary>
    /// A mechanism for adding delays to user actions
    /// </summary>
    public class Delay : Profiler {
        private static readonly ILog LOG = LogManager.GetLogger(typeof (Profiler));
        private static readonly Random random = new Random();

        public Delay(string name) : base(name) {}

        public static bool AddDelay { private get; set; }

        public static void UserDelay(int minInSeconds, int maxInSeconds) {
            if (AddDelay) {
                int seconds = random.Next(minInSeconds, maxInSeconds);
                try {
                    Thread.Sleep(new TimeSpan(0, 0, seconds));
                }
                catch (ThreadInterruptedException e) {
                    LOG.Error("Thread interupted:", e);
                }
            }
        }
    }

    // Copyright (c) INakedObject Objects Group Ltd.
}