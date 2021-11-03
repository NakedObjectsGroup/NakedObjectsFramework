
using System;

namespace Legacy.Types {
    public class TimeStamp : TitledObject {
        private DateTime dateTime;

        // necessary for when used as a parameter
        public TimeStamp(DateTime dateTime) => DateTime = dateTime;

        public TimeStamp(DateTime dateTime, Action<DateTime> callback) : this(dateTime) => UpdateBackingField = callback;

        public DateTime DateTime {
            get => dateTime;
            set {
                dateTime = value;
                UpdateBackingField(dateTime);
            }
        }

        private Action<DateTime> UpdateBackingField { get; } = _ => { };

        public override string ToString() => DateTime.ToString("dd/MM/yyyy hh:mm:ss"); //TODO: match original format.

        public Title Title() => new Title(this);
    }
}