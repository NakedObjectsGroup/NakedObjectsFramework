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
    public class Date : Magnitude
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

        public static void setClock(Clock clock)
        {
            //Date.clock = clock;
            //Date.ISO_LONG.setLenient(false);
            //Date.ISO_SHORT.setLenient(false);
            //Date.LONG_FORMAT.setLenient(false);
            //Date.MEDIUM_FORMAT.setLenient(false);
            //Date.SHORT_FORMAT.setLenient(false);
        }

        public Date()
        {
            //this.isNull = true;
            //this.today();
        }

        public Date(Date date)
        {
            //this.isNull = true;
            //this.setValue(date);
        }

        public Date(DateTime date)
        {
            //this.isNull = true;
            //this.setValue(date);
        }

        public Date(int year, int month, int day)
        {
            //this.isNull = true;
            //this.setValue(year, month, day);
        }

        public virtual void add(int years, int months, int days)
        {
            //Calendar instance =    Calendar.getInstance();
            //instance.setTime(this.date);
            //instance.AddDays(5, days);
            //instance.add(2, months);
            //instance.add(1, years);
            //this.set(instance);
        }

        public virtual Calendar calendarValue()
        {
            //if (this.isNull)
            //  return (Calendar) null;
            //Calendar instance = Calendar.getInstance();
            //instance.setTime(this.date);
            //return instance;
            throw new NotImplementedException();
        }

        private void checkDate(int year, int month, int day)
        {
            //if (month < 1 || month > 12)
            //  throw new IllegalArgumentException("Month must be in the range 1 - 12 inclusive");
            //Calendar instance = Calendar.getInstance();
            //instance.set(year, month - 1, 0);
            //int maximum = instance.getMaximum(5);
            //if (day < 1 || day > maximum)
            //  throw new IllegalArgumentException(new StringBuilder().append("Day must be in the range 1 - ").append(maximum).append(" inclusive: ").append(day).ToString());
        }

        public override void clear()
        {
            //this.isNull = true;
        }

        private void clearDate(Calendar cal)
        {
            //cal.set(10, 0);
            //cal.set(11, 0);
            //cal.set(12, 0);
            //cal.set(13, 0);
            //cal.set(9, 0);
            //cal.set(14, 0);
        }

        public override void copyObject(BusinessValueHolder @object)
        {
            //if (!(@object is Date))
            //  throw new IllegalArgumentException("Can only copy the value of  a Date object");
            //this.setValue((Date) @object);
        }

        public virtual Date dateValue()
        {
            //return this.isNull ? (Date)null : this.date;
            throw new NotImplementedException();
        }

        //public override bool Equals(object obj) {
        //    //return obj is Date ? ((Date)obj).date.Equals((object)this.date) : base.Equals(obj);
        //}

        public virtual int getDay()
        {
            //Calendar instance = Calendar.getInstance();
            //instance.setTime(this.date);
            //return instance.get(5);
            throw new NotImplementedException();
        }

        public virtual int getMonth()
        {
            //Calendar instance = Calendar.getInstance();
            //instance.setTime(this.date);
            //return instance.get(2) + 1;
            throw new NotImplementedException();
        }

        public virtual int getYear()
        {
            //Calendar instance = Calendar.getInstance();
            //instance.setTime(this.date);
            //return instance.get(1);
            throw new NotImplementedException();
        }

        public override bool isEmpty()
        {
            //return this.isNull;
            throw new NotImplementedException();
        }

        public override bool isEqualTo(Magnitude date)
        {
            //if (!(date is Date))
            //  throw new IllegalArgumentException("Parameter must be of type Time");
            //return this.isNull ? date.isEmpty() : this.date.Equals((object) ((Date) date).date);
            throw new NotImplementedException();
        }

        public override bool isLessThan(Magnitude date)
        {
            //if (!(date is Date))
            //  throw new IllegalArgumentException("Parameter must be of type Time");
            //return !this.isNull && !date.isEmpty() && this.date.before(((Date) date).date);
            throw new NotImplementedException();
        }

        public virtual long longValue()
        {
            //return this.date.getTime();
            throw new NotImplementedException();
        }

        //[JavaThrownExceptions("1;org/nakedobjects/application/ValueParseException;")]
        public override void parseUserEntry(string dateString)
        {
            //dateString = StringImpl.trim(dateString);
            //if (StringImpl.equals(dateString, (object) ""))
            //{
            //  this.clear();
            //}
            //else
            //{
            //  string str = StringImpl.toLowerCase(dateString);
            //  Calendar instance = Calendar.getInstance();
            //  instance.setTime(this.date);
            //  this.clearDate(instance);
            //  if (StringImpl.equals(str, (object) "today") || StringImpl.equals(str, (object) "now"))
            //  {
            //    this.today();
            //  }
            //  else
            //  {
            //    if (StringImpl.startsWith(str, "+"))
            //    {
            //      int num1 = 5;
            //      int num2 = 1;
            //      if (StringImpl.endsWith(str, "w"))
            //      {
            //        num2 = 7;
            //        str = StringImpl.substring(str, 0, StringImpl.length(str) - 1);
            //      }
            //      else if (StringImpl.endsWith(str, "m"))
            //      {
            //        num1 = 2;
            //        str = StringImpl.substring(str, 0, StringImpl.length(str) - 1);
            //      }
            //      int num3 = Integer.valueOf(StringImpl.substring(str, 1)).intValue();
            //      instance.setTime(this.date);
            //      instance.add(num1, num3 * num2);
            //    }
            //    else if (StringImpl.startsWith(str, "-"))
            //    {
            //      int num4 = 5;
            //      int num5 = 1;
            //      if (StringImpl.endsWith(str, "w"))
            //      {
            //        num5 = 7;
            //        str = StringImpl.substring(str, 0, StringImpl.length(str) - 1);
            //      }
            //      else if (StringImpl.endsWith(str, "m"))
            //      {
            //        num4 = 2;
            //        str = StringImpl.substring(str, 0, StringImpl.length(str) - 1);
            //      }
            //      int num6 = Integer.valueOf(StringImpl.substring(str, 1)).intValue();
            //      instance.setTime(this.date);
            //      instance.add(num4, -num6 * num5);
            //    }
            //    else
            //    {
            //      int length = 5;
            //      DateFormat[] dateFormatArray1 = length >= 0 ? new DateFormat[length] : throw new NegativeArraySizeException();
            //      dateFormatArray1[0] = Date.LONG_FORMAT;
            //      dateFormatArray1[1] = Date.MEDIUM_FORMAT;
            //      dateFormatArray1[2] = Date.SHORT_FORMAT;
            //      dateFormatArray1[3] = Date.ISO_LONG;
            //      dateFormatArray1[4] = Date.ISO_SHORT;
            //      DateFormat[] dateFormatArray2 = dateFormatArray1;
            //      for (int index = 0; index < dateFormatArray2.Length; ++index)
            //      {
            //        try
            //        {
            //          instance.setTime(dateFormatArray2[index].parse(dateString));
            //          break;
            //        }
            //        catch (ParseException ex)
            //        {
            //          if (index + 1 == dateFormatArray2.Length)
            //            throw new ValueParseException(new StringBuilder().append("Invalid date ").append(dateString).ToString(), (Throwable) ex);
            //        }
            //      }
            //    }
            //    this.set(instance);
            //  }
            //}
        }

        //[JavaThrownExceptions("2;java/io/IOException;java/lang/ClassNotFoundException;")]
        //public virtual void readExternal(ObjectInput @in)
        //{
        //  //this.isNull = ((DataInput) @in).readBoolean();
        //  //this.date.setTime(((DataInput) @in).readLong());
        //}

        public virtual void reset() => this.today();

        public override void restoreFromEncodedString(string data)
        {
            //if (StringImpl.equals(data, (object) "NULL"))
            //  this.clear();
            //else
            //  this.setValue(Integer.valueOf(StringImpl.substring(data, 0, 4)).intValue(), Integer.valueOf(StringImpl.substring(data, 4, 6)).intValue(), Integer.valueOf(StringImpl.substring(data, 6)).intValue());
        }

        private bool sameAs(Date @as, int field)
        {
            //Calendar instance1 = Calendar.getInstance();
            //instance1.setTime(this.date);
            //Calendar instance2 = Calendar.getInstance();
            //instance2.setTime(@as.date);
            //return instance1.get(field) == instance2.get(field);
            throw new NotImplementedException();
        }

        public virtual bool sameDayAs(Date @as) => this.sameAs(@as, 6);

        public virtual bool sameMonthAs(Date @as) => this.sameAs(@as, 2);

        public virtual bool sameWeekAs(Date @as) => this.sameAs(@as, 3);

        public virtual bool sameYearAs(Date @as) => this.sameAs(@as, 1);

        public override string asEncodedString()
        {
            //if (this.isEmpty())
            //  return "NULL";
            //Calendar calendar = this.calendarValue();
            //StringBuilder stringBuffer = new StringBuilder(8);
            //string str = StringImpl.valueOf(calendar.get(1));
            //stringBuffer.append(StringImpl.substring("0000", 0, 4 - StringImpl.length(str)));
            //stringBuffer.append(str);
            //int num1 = calendar.get(2) + 1;
            //stringBuffer.append(num1 > 9 ? "" : "0");
            //stringBuffer.append(num1);
            //int num2 = calendar.get(5);
            //stringBuffer.append(num2 > 9 ? "" : "0");
            //stringBuffer.append(num2);
            //return stringBuffer.ToString();
            throw new NotImplementedException();
        }

        private void set(Calendar cal)
        {
            //this.date = cal.getTime();
            //this.isNull = false;
        }

        public virtual void setValue(Date date)
        {
            //if (date == null || date.isEmpty())
            //  this.clear();
            //else
            //  this.setValue(new Date(date.longValue()));
            this.date = date.date;
        }

        public virtual void setValue(DateTime date)
        {
            //if (date == null || date.isEmpty())
            //  this.clear();
            //else
            //  this.setValue(new Date(date.longValue()));
            this.date = date.Date;
        }


        public virtual void setValue(int year, int month, int day)
        {
            //this.checkDate(year, month, day);
            //Calendar instance = Calendar.getInstance();
            //this.clearDate(instance);
            //instance.set(year, month - 1, day);
            //this.set(instance);
        }

        public virtual void setValue1(Date date)
        {
            //if (date == null)
            //{
            //  this.isNull = true;
            //}
            //else
            //{
            //  Calendar instance = Calendar.getInstance();
            //  instance.setTime(date);
            //  instance.set(10, 0);
            //  instance.set(12, 0);
            //  instance.set(13, 0);
            //  instance.set(14, 0);
            //  this.set(instance.
            //}
        }

        public override Title title() =>
            new Title(date is null? "": date.Value.ToString("d"));
        

        public virtual void today()
        {
            //Calendar instance = Calendar.getInstance();
            //Date date = Date.clock != null ? new Date(Date.clock.getTime()) : throw new ApplicationException("Clock not set up");
            //instance.setTime(date);
            //this.clearDate(instance);
            //this.set(instance);
        }

        public virtual void toStartOfMonth()
        {
            //Calendar instance = Calendar.getInstance();
            //instance.setTime(this.date);
            //instance.set(2, instance.getMinimum(2));
            //this.date = instance.getTime();
        }

        public virtual void toStartOfWeek()
        {
            //Calendar instance = Calendar.getInstance();
            //instance.setTime(this.date);
            //instance.set(8, instance.getMinimum(8));
            //this.date = instance.getTime();
        }

        public virtual void toStartOfYear()
        {
            //Calendar instance = Calendar.getInstance();
            //instance.setTime(this.date);
            //instance.set(6, instance.getMinimum(6));
            //this.date = instance.getTime();
        }

        //[JavaFlags(32778)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        static Date()
        {
            // ISSUE: unable to decompile the method.
        }
    }
}
