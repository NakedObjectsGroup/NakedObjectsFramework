using System;

namespace NakedObjects.Surface {
    public class LinkObjectId {
        static LinkObjectId() {
            // default 
            KeySeparator = "-";
        }

        public LinkObjectId(string domainType, string instanceId) {
            DomainType = domainType;
            InstanceId = instanceId;
        }

        public static string KeySeparator { get; set; }

        public string DomainType { get; set; }
        public string InstanceId { get; set; }

        public override string ToString() {
            return DomainType  + (String.IsNullOrEmpty(InstanceId) ? "" :  KeySeparator + InstanceId);
        }
    }
}