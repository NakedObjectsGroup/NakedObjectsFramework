
using System;
using System.Globalization;

namespace NakedLegacy.Types {
    public class Date : TitledObject {
        private DateTime dateTime;

        // necessary for when used as a parameter
        public Date(DateTime dateTime) => DateTime = dateTime;

        public Date(DateTime dateTime, Action<DateTime> callback) : this(dateTime) => UpdateBackingField = callback;

        public DateTime DateTime {
            get => dateTime;
            set {
                dateTime = value;
                UpdateBackingField(dateTime);
            }
        }

        private Action<DateTime> UpdateBackingField { get; } = _ => { };

        public override string ToString() => DateTime.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture); //TODO: match original format.

        public Title Title() => new Title(ToString()); 
    }
}