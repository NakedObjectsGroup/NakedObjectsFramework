// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 
using System.Collections.Generic;
using System.Threading;

namespace NakedObjects.Xat.Performance {
    public class Profiler {
        private const string DELIMITER = "\t";
        private static int nextId;
        private static int nextThread;
        protected internal static ProfilerSystem profilerSystem = new ProfilerSystem();
        private static readonly IDictionary<Thread, string> threads = new Dictionary<Thread, string>();

        private long elapsedTime;
        private readonly int id;
        private long memory;
        private readonly string name;
        private long start;
        private readonly string thread;
        private bool timing;

        public Profiler(string name) {
            this.name = name;
            lock (typeof (Profiler)) {
                id = nextId++;
            }
            Thread t = Thread.CurrentThread;

            lock (threads) {
                if (threads.ContainsKey(t)) {
                    thread = threads[t];
                }
                else {
                    thread = "t" + nextThread++;
                    threads.Add(t, thread);
                }
            }
            memory = Memory;
        }

        public static ProfilerSystem ProfilerSystem {
            set { profilerSystem = value; }
        }

        public virtual long ElapsedTime {
            get { return timing ? Time - start : elapsedTime; }
        }

        public virtual long MemoryUsage {
            get { return Memory - memory; }
        }

        public virtual string Name {
            get { return name; }
        }

        private static long Memory {
            get { return profilerSystem.Memory; }
        }

        private static long Time {
            get { return profilerSystem.Time; }
        }

        public static string MemoryLog() {
            return Memory + " bytes";
        }

        public virtual string Log() {
            return id + DELIMITER + thread + DELIMITER + Name + DELIMITER + MemoryUsage + DELIMITER + Log();
        }

        public virtual void Reset() {
            elapsedTime = 0;
            start = Time;
            memory = Memory;
        }

        public virtual void Start() {
            start = Time;
            timing = true;
        }

        public virtual void Stop() {
            timing = false;
            long end = Time;
            elapsedTime += end - start;
        }

        public virtual string MemoryUsageLog() {
            return MemoryUsage + " bytes";
        }

        public virtual string TimeLog() {
            return ElapsedTime.ToString("g") + " nS";
        }

        public override string ToString() {
            return ElapsedTime.ToString("g") + " nS - " + name;
        }
    }

    // Copyright (c) Naked Objects Group Ltd.
}