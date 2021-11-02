// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.application.valueholder.Date
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using System;
using System.ComponentModel;
using System.Globalization;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using Legacy.NakedObjects.Application.System;
using Microsoft.VisualBasic;

namespace Legacy.NakedObjects.Application.ValueHolder
{
    public class TimeStamp : Magnitude
    {
        //private static Clock clock;
        //private static readonly DateFormat ISO_LONG;
        //private static readonly DateFormat ISO_SHORT;
        //private static readonly DateFormat LONG_FORMAT;
        //private static readonly DateFormat MEDIUM_FORMAT;
        //private const long serialVersionUID = 1;
        //private static readonly DateFormat SHORT_FORMAT;
        private DateTime? date;
        //private bool isNull;
        public Action<DateTime> BackingField { get; set; } = _ => { };

        public static void setClock(Clock clock)
        {
            //Date.clock = clock;
            //Date.ISO_LONG.setLenient(false);
            //Date.ISO_SHORT.setLenient(false);
            //Date.LONG_FORMAT.setLenient(false);
            //Date.MEDIUM_FORMAT.setLenient(false);
            //Date.SHORT_FORMAT.setLenient(false);
        }

        public TimeStamp() {
            //isNull = true;
            //today();
        }

        public TimeStamp(DateTime date) => this.date = date;

        public TimeStamp(DateTime date, Action<DateTime> callback) : this(date) => BackingField = callback;


        public TimeStamp(int year, int month, int day)
        {
            //this.isNull = true;
            //this.setValue(year, month, day);
        }


        public override Title title() =>
            new Title(date is null? "": date.Value.ToString("dd/MM/yyyy hh:mm:ss"));

        public override bool isEqualTo(Magnitude magnitude)
        {
            throw new NotImplementedException();
        }

        public override bool isLessThan(Magnitude magnitude)
        {
            throw new NotImplementedException();
        }

        public override bool isEmpty()
        {
            throw new NotImplementedException();
        }

        public override void parseUserEntry(string text)
        {
            date = (DateTime.Parse(text));
            BackingField(date.GetValueOrDefault());
        }

        public override void restoreFromEncodedString(string data)
        {
            throw new NotImplementedException();
        }

        public override string asEncodedString()
        {
            throw new NotImplementedException();
        }

        public override void copyObject(BusinessValueHolder @object)
        {
            throw new NotImplementedException();
        }

        public override void clear()
        {
            throw new NotImplementedException();
        }

        public virtual DateTime? dateValue() =>  date;
    }
}
