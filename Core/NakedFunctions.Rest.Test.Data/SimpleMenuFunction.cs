using System;
using System.Linq;

namespace NakedFunctions.Rest.Test.Data
{
    public static class SimpleMenuFunction {
        public static SimpleRecord GetSimpleRecord([Injected] IQueryable<SimpleRecord> allSimpleRecords) => allSimpleRecords.First();
    }
}
