// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

namespace AdventureWorksModel
{
    //public class TimePeriodValueSemanticsProvider : IValueSemanticsProvider<TimePeriod>, IEncoderDecoder<TimePeriod>, IParser<TimePeriod>, IDefaultsProvider<TimePeriod>
    //{

    //    public bool IsImmutable
    //    {
    //        get
    //        {
    //            return false;
    //        }
    //    }

    //    public bool IsEqualByContent
    //    {
    //        get
    //        {
    //            return true;
    //        }
    //    }

    //    public IParser<TimePeriod> Parser
    //    {
    //        get
    //        {
    //            return this;
    //        }
    //    }

    //    public IEncoderDecoder<TimePeriod> EncoderDecoder
    //    {
    //        get
    //        {
    //            return this;
    //        }
    //    }

    //    public IDefaultsProvider<TimePeriod> DefaultsProvider
    //    {
    //        get
    //        {
    //            return this;
    //        }
    //    }

    //    public IFromStream FromStream {
    //        get { return null; }
    //    }

    //    public string ToEncodedString(TimePeriod toEncode)
    //    {
    //        return "";
    //    }

    //    public TimePeriod FromEncodedString(string encodedString)
    //    {
    //        return null;
    //    }

    //    //This is a simplistic implementation as it would require the user
    //    //to enter the two times as full DateTimes e.g.: 01/01/1900 09:30:00 ~ 01/01/1900 17:30:00 
    //    //for a finished result of 09:30 ~ 17:30.  A proper implementation should parse for
    //    //time representation only.
    //    public object ParseTextEntry(TimePeriod context, string entry)
    //    {

    //        char[] s = { '~', '-'};
    //        string[] ss = entry.Split(s);

    //        if (ss.Length != 2)
    //        {
    //            throw new InvalidEntryException();
    //        }
    //        DateTime startTime, endTime;

    //        if (!(System.DateTime.TryParse(ss[0].Trim(), out startTime)))
    //        {
    //            throw new InvalidEntryException();
    //        }

    //            if (!(System.DateTime.TryParse(ss[1].Trim(), out endTime)))
    //            {
    //                throw new InvalidEntryException();
    //            }
    //            if (endTime < startTime)
    //            {
    //                throw new InvalidEntryException();
    //            }
    //            context.StartTime = new DateTime(1900, 1, 1, startTime.Hour, startTime.Minute, startTime.Second);
    //            context.EndTime = new DateTime(1900, 1, 1, endTime.Hour, endTime.Minute, endTime.Second);

    //        return context;
    //    }

    //    public string DisplayTitleOf(TimePeriod obj)
    //    {
    //        return TitleWithMaskOf("t", obj);
    //    }



    //    public string TitleWithMaskOf(string mask, TimePeriod obj)
    //    {
    //        var t = Container.NewTitleBuilder();
    //        t.Append(obj.StartTime, mask, null).Append(" ~").Append(obj.EndTime, mask, null);
    //        return t.ToString();
    //    }

    //    public string EditableTitleOf(TimePeriod obj)
    //    {
    //        return DisplayTitleOf(obj);
    //    }

    //    public int TypicalLength
    //    {
    //        get
    //        {
    //            return 21;
    //        }
    //    }

    //    public TimePeriod DefaultValue
    //    {
    //        get
    //        {
    //            return (null);
    //        }
    //    }
    //}

}