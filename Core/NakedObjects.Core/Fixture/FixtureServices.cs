// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System;
using System.Security.Principal;
using NakedObjects.Architecture.Security;
using NakedObjects.Core.Context;
using NakedObjects.Core.Security;

namespace NakedObjects.Core.Fixture {
    public class FixtureServices {
        public FixtureServices() {
            Clock = FixtureClock.Initialize();
        }

        protected FixtureClock Clock { get; set; }

        #region IFixtureServices Members

        public void EarlierDate(int years, int months, int days) {
            Clock.AddDate(-years, -months, -days);
        }

        public void EarlierTime(int hours, int minutes) {
            Clock.AddTime(-hours, -minutes);
        }

        public void LaterDate(int years, int months, int days) {
            Clock.AddDate(years, months, days);
        }

        public void LaterTime(int hours, int minutes) {
            Clock.AddTime(hours, minutes);
        }

        public void ResetClock() {
            Clock.Reset();
        }

        public void SetDate(int year, int month, int day) {
            Clock.SetDate(year, month, day);
        }

        public void SetTime(int hour, int minute) {
            Clock.SetTime(hour, minute);
        }

        //public void SetUser(string username, string[] roles) {
        //    var staticContext = (StaticContext) NakedObjectsContext.Instance;

        //    IIdentity identity = new GenericIdentity(username);
        //    IPrincipal principal = new GenericPrincipal(identity, roles);


        //    ISession session = new SimpleSession(principal);
        //    staticContext.SetSession(session);
        //}

        #endregion

        protected class FixtureClock {
            private DateTime time;

            /// <summary>
            ///     Access via <see cref="Clock.GetTicks" />
            /// </summary>
            protected long Ticks {
                get { return time.Ticks; }
            }

            /// <summary>
            ///     Create a return new FixtureClock
            /// </summary>
            public static FixtureClock Initialize() {
                return new FixtureClock();
            }

            /// <summary>
            ///     Set time - leaving date unchanged
            /// </summary>
            public void SetTime(int hour, int min) {
                time = new DateTime(time.Year,
                                    time.Month,
                                    time.Day,
                                    hour,
                                    min,
                                    0);
            }

            /// <summary>
            ///     Set date
            /// </summary>
            public void SetDate(int year, int month, int day) {
                time = new DateTime(year, month, day);
            }

            /// <summary>
            ///     Add time to current datetime
            /// </summary>
            public void AddTime(int hours, int minutes) {
                time = time.AddHours(hours);
                time = time.AddMinutes(minutes);
            }

            /// <summary>
            ///     Add years/month/days to current time
            /// </summary>
            public void AddDate(int years, int months, int days) {
                time = time.AddYears(years);
                time = time.AddMonths(months);
                time = time.AddDays(days);
            }

            /// <summary>
            ///     Reset time to now
            /// </summary>
            public void Reset() {
                time = DateTime.Now;
            }

            public override string ToString() {
                return time.ToString();
            }
        }
    }

    // Copyright (c) Naked Objects Group Ltd.
}