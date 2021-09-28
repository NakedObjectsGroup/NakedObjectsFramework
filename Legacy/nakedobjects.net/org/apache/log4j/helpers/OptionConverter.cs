// Decompiled with JetBrains decompiler
// Type: org.apache.log4j.helpers.OptionConverter
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using java.lang;
using java.net;
using java.util;
using org.apache.log4j.spi;
using System;
using System.ComponentModel;

namespace org.apache.log4j.helpers
{
  public class OptionConverter
  {
    [JavaFlags(8)]
    public static string DELIM_START;
    [JavaFlags(8)]
    public static char DELIM_STOP;
    [JavaFlags(8)]
    public static int DELIM_START_LEN;
    [JavaFlags(8)]
    public static int DELIM_STOP_LEN;

    private OptionConverter()
    {
    }

    public static string[] concatanateArrays(string[] l, string[] r)
    {
      int length = l.Length + r.Length;
      string[] strArray = length >= 0 ? new string[length] : throw new NegativeArraySizeException();
      java.lang.System.arraycopy((object) l, 0, (object) strArray, 0, l.Length);
      java.lang.System.arraycopy((object) r, 0, (object) strArray, l.Length, r.Length);
      return strArray;
    }

    public static string convertSpecialChars(string s)
    {
      int num1 = StringImpl.length(s);
      StringBuffer stringBuffer = new StringBuffer(num1);
      int num2 = 0;
      while (num2 < num1)
      {
        string str1 = s;
        int num3;
        num2 = (num3 = num2) + 1;
        int num4 = num3;
        char ch = StringImpl.charAt(str1, num4);
        if (ch == '\\')
        {
          string str2 = s;
          int num5;
          num2 = (num5 = num2) + 1;
          int num6 = num5;
          ch = StringImpl.charAt(str2, num6);
          if (ch == 'n')
            ch = '\n';
          else if (ch == 'r')
            ch = '\r';
          else if (ch == 't')
            ch = '\t';
          else if (ch == 'f')
            ch = '\f';
          else if (ch == '\b')
            ch = '\b';
          else if (ch == '"')
            ch = '"';
          else if (ch == '\'')
            ch = '\'';
          else if (ch == '\\')
            ch = '\\';
        }
        stringBuffer.append(ch);
      }
      return stringBuffer.ToString();
    }

    public static string getSystemProperty(string key, string def)
    {
      try
      {
        return java.lang.System.getProperty(key, def);
      }
      catch (Exception ex)
      {
        ThrowableWrapper.wrapThrowable(ex);
        LogLog.debug(new StringBuffer().append("Was not allowed to read system property \"").append(key).append("\".").ToString());
        return def;
      }
    }

    public static object instantiateByKey(
      Properties props,
      string key,
      Class superClass,
      object defaultValue)
    {
      string andSubst = OptionConverter.findAndSubst(key, props);
      if (andSubst != null)
        return OptionConverter.instantiateByClassName(StringImpl.trim(andSubst), superClass, defaultValue);
      LogLog.error(new StringBuffer().append("Could not find value for key ").append(key).ToString());
      return defaultValue;
    }

    public static bool toBoolean(string value, bool dEfault)
    {
      if (value == null)
        return dEfault;
      string str = StringImpl.trim(value);
      if (StringImpl.equalsIgnoreCase("true", str))
        return true;
      return !StringImpl.equalsIgnoreCase("false", str) && dEfault;
    }

    public static int toInt(string value, int dEfault)
    {
      // ISSUE: unable to decompile the method.
    }

    public static Level toLevel(string value, Level defaultValue)
    {
      // ISSUE: unable to decompile the method.
    }

    public static long toFileSize(string value, long dEfault)
    {
      // ISSUE: unable to decompile the method.
    }

    public static string findAndSubst(string key, Properties props)
    {
      // ISSUE: unable to decompile the method.
    }

    public static object instantiateByClassName(
      string className,
      Class superClass,
      object defaultValue)
    {
      // ISSUE: unable to decompile the method.
    }

    [JavaThrownExceptions("1;java/lang/IllegalArgumentException;")]
    public static string substVars(string val, Properties props)
    {
      StringBuffer stringBuffer = new StringBuffer();
      int num1 = 0;
      int num2;
      while (true)
      {
        num2 = StringImpl.indexOf(val, OptionConverter.DELIM_START, num1);
        if (num2 != -1)
        {
          stringBuffer.append(StringImpl.substring(val, num1, num2));
          int num3 = StringImpl.indexOf(val, (int) OptionConverter.DELIM_STOP, num2);
          if (num3 != -1)
          {
            int num4 = num2 + OptionConverter.DELIM_START_LEN;
            string key = StringImpl.substring(val, num4, num3);
            string val1 = OptionConverter.getSystemProperty(key, (string) null);
            if (val1 == null && props != null)
              val1 = props.getProperty(key);
            if (val1 != null)
            {
              string str = OptionConverter.substVars(val1, props);
              stringBuffer.append(str);
            }
            num1 = num3 + OptionConverter.DELIM_STOP_LEN;
          }
          else
            goto label_6;
        }
        else
          break;
      }
      if (num1 == 0)
        return val;
      stringBuffer.append(StringImpl.substring(val, num1, StringImpl.length(val)));
      return stringBuffer.ToString();
label_6:
      throw new IllegalArgumentException(new StringBuffer().append('"').append(val).append("\" has no closing brace. Opening brace at position ").append(num2).append('.').ToString());
    }

    public static void selectAndConfigure(URL url, string clazz, LoggerRepository hierarchy)
    {
      string file = url.getFile();
      if (clazz == null && file != null && StringImpl.endsWith(file, ".xml"))
        clazz = "org.apache.log4j.xml.DOMConfigurator";
      Configurator configurator;
      if (clazz != null)
      {
        LogLog.debug(new StringBuffer().append("Preferred configurator class: ").append(clazz).ToString());
        configurator = (Configurator) OptionConverter.instantiateByClassName(clazz, Class.FromType(typeof (Configurator)), (object) null);
        if (configurator == null)
        {
          LogLog.error(new StringBuffer().append("Could not instantiate configurator [").append(clazz).append("].").ToString());
          return;
        }
      }
      else
        configurator = (Configurator) new PropertyConfigurator();
      configurator.doConfigure(url, hierarchy);
    }

    [JavaFlags(32778)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    static OptionConverter()
    {
      // ISSUE: unable to decompile the method.
    }

    [JavaFlags(4227077)]
    [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
    public new virtual object MemberwiseClone()
    {
      OptionConverter optionConverter = this;
      ObjectImpl.clone((object) optionConverter);
      return ((object) optionConverter).MemberwiseClone();
    }

    [JavaFlags(4227073)]
    public override string ToString() => ObjectImpl.jloToString((object) this);
  }
}
