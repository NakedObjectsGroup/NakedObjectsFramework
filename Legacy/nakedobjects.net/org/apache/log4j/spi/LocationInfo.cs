// Decompiled with JetBrains decompiler
// Type: org.apache.log4j.spi.LocationInfo
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using java.io;
using java.lang;
using System;
using System.ComponentModel;
using System.Threading;

namespace org.apache.log4j.spi
{
  [JavaInterfaces("1;java/io/Serializable;")]
  public class LocationInfo : Serializable
  {
    [JavaFlags(128)]
    [NonSerialized]
    public string lineNumber;
    [JavaFlags(128)]
    [NonSerialized]
    public string fileName;
    [JavaFlags(128)]
    [NonSerialized]
    public string className;
    [JavaFlags(128)]
    [NonSerialized]
    public string methodName;
    public string fullInfo;
    private static StringWriter sw;
    private static PrintWriter pw;
    public const string NA = "?";
    [JavaFlags(24)]
    public const long serialVersionUID = -1325822038990805636;
    [JavaFlags(8)]
    public static bool inVisualAge;

    public LocationInfo(Throwable t, string fqnOfCallingClass)
    {
      if (t == null)
        return;
      object sw = (object) LocationInfo.sw;
      \u003CCorArrayWrapper\u003E.Enter(sw);
      string str;
      try
      {
        t.printStackTrace(LocationInfo.pw);
        str = LocationInfo.sw.ToString();
        LocationInfo.sw.getBuffer().setLength(0);
      }
      finally
      {
        Monitor.Exit(sw);
      }
      int num1 = StringImpl.lastIndexOf(str, fqnOfCallingClass);
      if (num1 == -1)
        return;
      int num2 = StringImpl.indexOf(str, Layout.LINE_SEP, num1);
      if (num2 == -1)
        return;
      int num3 = num2 + Layout.LINE_SEP_LEN;
      int num4 = StringImpl.indexOf(str, Layout.LINE_SEP, num3);
      if (num4 == -1)
        return;
      if (!LocationInfo.inVisualAge)
      {
        int num5 = StringImpl.lastIndexOf(str, "at ", num4);
        if (num5 == -1)
          return;
        num3 = num5 + 3;
      }
      this.fullInfo = StringImpl.substring(str, num3, num4);
    }

    public virtual string getClassName()
    {
      if (this.fullInfo == null)
        return "?";
      if (this.className == null)
      {
        int num1 = StringImpl.lastIndexOf(this.fullInfo, 40);
        if (num1 == -1)
        {
          this.className = "?";
        }
        else
        {
          int num2 = StringImpl.lastIndexOf(this.fullInfo, 46, num1);
          int num3 = 0;
          if (LocationInfo.inVisualAge)
            num3 = StringImpl.lastIndexOf(this.fullInfo, 32, num2) + 1;
          this.className = num2 != -1 ? StringImpl.substring(this.fullInfo, num3, num2) : "?";
        }
      }
      return this.className;
    }

    public virtual string getFileName()
    {
      if (this.fullInfo == null)
        return "?";
      if (this.fileName == null)
      {
        int num = StringImpl.lastIndexOf(this.fullInfo, 58);
        this.fileName = num != -1 ? StringImpl.substring(this.fullInfo, StringImpl.lastIndexOf(this.fullInfo, 40, num - 1) + 1, num) : "?";
      }
      return this.fileName;
    }

    public virtual string getLineNumber()
    {
      if (this.fullInfo == null)
        return "?";
      if (this.lineNumber == null)
      {
        int num1 = StringImpl.lastIndexOf(this.fullInfo, 41);
        int num2 = StringImpl.lastIndexOf(this.fullInfo, 58, num1 - 1);
        this.lineNumber = num2 != -1 ? StringImpl.substring(this.fullInfo, num2 + 1, num1) : "?";
      }
      return this.lineNumber;
    }

    public virtual string getMethodName()
    {
      if (this.fullInfo == null)
        return "?";
      if (this.methodName == null)
      {
        int num1 = StringImpl.lastIndexOf(this.fullInfo, 40);
        int num2 = StringImpl.lastIndexOf(this.fullInfo, 46, num1);
        this.methodName = num2 != -1 ? StringImpl.substring(this.fullInfo, num2 + 1, num1) : "?";
      }
      return this.methodName;
    }

    [JavaFlags(32778)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    static LocationInfo()
    {
      // ISSUE: unable to decompile the method.
    }

    [JavaFlags(4227077)]
    [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
    public new virtual object MemberwiseClone()
    {
      LocationInfo locationInfo = this;
      ObjectImpl.clone((object) locationInfo);
      return ((object) locationInfo).MemberwiseClone();
    }

    [JavaFlags(4227073)]
    public override string ToString() => ObjectImpl.jloToString((object) this);
  }
}
