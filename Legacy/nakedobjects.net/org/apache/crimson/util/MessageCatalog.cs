// Decompiled with JetBrains decompiler
// Type: org.apache.crimson.util.MessageCatalog
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using java.lang;
using java.text;
using java.util;

namespace org.apache.crimson.util
{
  public abstract class MessageCatalog
  {
    private string bundleName;
    private Hashtable cache;

    [JavaFlags(4)]
    public MessageCatalog(Class packageMember)
      : this(packageMember, "Messages")
    {
    }

    private MessageCatalog(Class packageMember, string bundle)
    {
      this.cache = new Hashtable(5);
      this.bundleName = packageMember.getName();
      int num = StringImpl.lastIndexOf(this.bundleName, 46);
      this.bundleName = num != -1 ? new StringBuffer().append(StringImpl.substring(this.bundleName, 0, num)).append(".").ToString() : "";
      this.bundleName = new StringBuffer().append(this.bundleName).append("resources.").append(bundle).ToString();
    }

    public virtual string getMessage(Locale locale, string messageId)
    {
      if (locale == null)
        locale = Locale.getDefault();
      try
      {
        return ResourceBundle.getBundle(this.bundleName, locale).getString(messageId);
      }
      catch (MissingResourceException ex)
      {
        return this.packagePrefix(messageId);
      }
    }

    private string packagePrefix(string messageId)
    {
      string name = ObjectImpl.getClass((object) this).getName();
      int num = StringImpl.lastIndexOf(name, 46);
      return new StringBuffer().append(num != -1 ? StringImpl.substring(name, 0, num) : "").append('/').append(messageId).ToString();
    }

    public virtual string getMessage(Locale locale, string messageId, object[] parameters)
    {
      if (parameters == null)
        return this.getMessage(locale, messageId);
      for (int index = 0; index < parameters.Length; ++index)
      {
        if (!\u003CVerifierFix\u003E.isInstanceOfString(parameters[index]) && !(parameters[index] is Number) && !(parameters[index] is Date))
          parameters[index] = parameters[index] != null ? (object) parameters[index].ToString() : (object) "(null)";
      }
      if (locale == null)
        locale = Locale.getDefault();
      MessageFormat messageFormat;
      try
      {
        messageFormat = new MessageFormat(ResourceBundle.getBundle(this.bundleName, locale).getString(messageId));
      }
      catch (MissingResourceException ex)
      {
        string str = this.packagePrefix(messageId);
        for (int index = 0; index < parameters.Length; ++index)
          str = new StringBuffer().append(new StringBuffer().append(str).append(' ').ToString()).append(parameters[index]).ToString();
        return str;
      }
      messageFormat.setLocale(locale);
      StringBuffer stringBuffer = new StringBuffer();
      return messageFormat.format(parameters, stringBuffer, new FieldPosition(0)).ToString();
    }

    public virtual Locale chooseLocale(string[] languages)
    {
      if ((languages = this.canonicalize(languages)) != null)
      {
        for (int index = 0; index < languages.Length; ++index)
        {
          if (this.isLocaleSupported(languages[index]))
            return this.getLocale(languages[index]);
        }
      }
      return (Locale) null;
    }

    private string[] canonicalize(string[] languages)
    {
      bool flag = false;
      int num1 = 0;
      if (languages == null)
        return languages;
      for (int index = 0; index < languages.Length; ++index)
      {
        string language = languages[index];
        int num2 = StringImpl.length(language);
        switch (num2)
        {
          case 2:
          case 5:
            if (num2 == 2)
            {
              string lowerCase = StringImpl.toLowerCase(language);
              if ((object) lowerCase != (object) languages[index])
              {
                if (!flag)
                {
                  languages = (string[]) \u003CCorArrayWrapper\u003E.clone((object) languages);
                  flag = true;
                }
                languages[index] = lowerCase;
                break;
              }
              break;
            }
            int length = 5;
            char[] chArray = length >= 0 ? new char[length] : throw new NegativeArraySizeException();
            chArray[0] = Character.toLowerCase(StringImpl.charAt(language, 0));
            chArray[1] = Character.toLowerCase(StringImpl.charAt(language, 1));
            chArray[2] = '_';
            chArray[3] = Character.toUpperCase(StringImpl.charAt(language, 3));
            chArray[4] = Character.toUpperCase(StringImpl.charAt(language, 4));
            if (!flag)
            {
              languages = (string[]) \u003CCorArrayWrapper\u003E.clone((object) languages);
              flag = true;
            }
            languages[index] = StringImpl.createString(chArray);
            break;
          default:
            if (!flag)
            {
              languages = (string[]) \u003CCorArrayWrapper\u003E.clone((object) languages);
              flag = true;
            }
            languages[index] = (string) null;
            ++num1;
            break;
        }
      }
      if (num1 != 0)
      {
        int length = languages.Length - num1;
        string[] strArray = length >= 0 ? new string[length] : throw new NegativeArraySizeException();
        int index = 0;
        int num3 = 0;
        for (; index < strArray.Length; ++index)
        {
          while (languages[index + num3] == null)
            ++num3;
          strArray[index] = languages[index + num3];
        }
        languages = strArray;
      }
      return languages;
    }

    private Locale getLocale(string localeName)
    {
      int num = StringImpl.indexOf(localeName, 95);
      string str1;
      string str2;
      if (num == -1)
      {
        if (StringImpl.equals(localeName, (object) "de"))
          return (Locale) Locale.GERMAN;
        if (StringImpl.equals(localeName, (object) "en"))
          return (Locale) Locale.ENGLISH;
        if (StringImpl.equals(localeName, (object) "fr"))
          return (Locale) Locale.FRENCH;
        if (StringImpl.equals(localeName, (object) "it"))
          return (Locale) Locale.ITALIAN;
        if (StringImpl.equals(localeName, (object) "ja"))
          return (Locale) Locale.JAPANESE;
        if (StringImpl.equals(localeName, (object) "ko"))
          return (Locale) Locale.KOREAN;
        if (StringImpl.equals(localeName, (object) "zh"))
          return (Locale) Locale.CHINESE;
        str1 = localeName;
        str2 = "";
      }
      else
      {
        if (StringImpl.equals(localeName, (object) "zh_CN"))
          return (Locale) Locale.SIMPLIFIED_CHINESE;
        if (StringImpl.equals(localeName, (object) "zh_TW"))
          return (Locale) Locale.TRADITIONAL_CHINESE;
        str1 = StringImpl.substring(localeName, 0, num);
        str2 = StringImpl.substring(localeName, num + 1);
      }
      return new Locale(str1, str2);
    }

    public virtual bool isLocaleSupported(string localeName)
    {
      // ISSUE: unable to decompile the method.
    }

    [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
    [JavaFlags(4227077)]
    public new virtual object MemberwiseClone()
    {
      MessageCatalog messageCatalog = this;
      ObjectImpl.clone((object) messageCatalog);
      return ((object) messageCatalog).MemberwiseClone();
    }

    [JavaFlags(4227073)]
    public override string ToString() => ObjectImpl.jloToString((object) this);
  }
}
