// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.application.Title
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using java.lang;

namespace org.nakedobjects.application
{
  public class Title
  {
    private StringBuffer @string;

    public Title() => this.@string = new StringBuffer();

    public Title(string text)
    {
      this.@string = new StringBuffer();
      this.concat(text);
    }

    public Title(TitledObject @object)
    {
      this.@string = new StringBuffer();
      this.concat(@object);
    }

    public Title(TitledObject @object, string defaultValue)
    {
      this.@string = new StringBuffer();
      this.concat(@object, defaultValue);
    }

    public virtual Title append(int number)
    {
      this.append(StringImpl.valueOf(number));
      return this;
    }

    public virtual Title append(string text)
    {
      if (!StringImpl.equals(text, (object) ""))
      {
        this.appendSpace();
        this.@string.append(text);
      }
      return this;
    }

    public virtual Title append(string joiner, string text)
    {
      if (!StringImpl.equals(text, (object) ""))
      {
        if (this.@string.length() > 0)
          this.concat(joiner);
        this.appendSpace();
        this.@string.append(text);
      }
      return this;
    }

    public virtual Title append(string joiner, TitledObject @object)
    {
      this.append(joiner, @object, "");
      return this;
    }

    public virtual Title append(string joiner, TitledObject @object, string defaultValue)
    {
      if (this.@string.length() > 0 && @object != null && StringImpl.length(Title.titleString(@object)) > 0 || defaultValue != null && StringImpl.length(defaultValue) > 0)
      {
        this.concat(joiner);
        this.appendSpace();
      }
      this.concat(@object, defaultValue);
      return this;
    }

    private static string titleString(TitledObject @object) => @object == null || @object.title() == null ? "" : @object.title().ToString();

    public virtual Title append(TitledObject @object)
    {
      if (@object != null && @object.title() != null && !StringImpl.equals(Title.titleString(@object), (object) ""))
      {
        this.appendSpace();
        this.@string.append(Title.titleString(@object));
      }
      return this;
    }

    public virtual Title append(TitledObject @object, string defaultValue)
    {
      this.appendSpace();
      this.concat(@object, defaultValue);
      return this;
    }

    public virtual Title appendSpace()
    {
      if (this.@string.length() > 0)
        this.@string.append(" ");
      return this;
    }

    [JavaFlags(17)]
    public Title concat(string text)
    {
      this.@string.append(text);
      return this;
    }

    [JavaFlags(17)]
    public Title concat(string joiner, TitledObject @object)
    {
      if (this.@string.length() > 0 && @object != null && StringImpl.length(Title.titleString(@object)) > 0)
        this.concat(joiner);
      this.concat(@object, "");
      return this;
    }

    [JavaFlags(17)]
    public Title concat(TitledObject @object)
    {
      this.concat(@object, "");
      return this;
    }

    [JavaFlags(17)]
    public Title concat(string joiner, TitledObject @object, string defaultValue)
    {
      if (this.@string.length() > 0 && @object != null && StringImpl.length(Title.titleString(@object)) > 0)
        this.concat(joiner);
      this.concat(@object, defaultValue);
      return this;
    }

    [JavaFlags(17)]
    public Title concat(TitledObject @object, string defaultValue)
    {
      if (@object == null || StringImpl.length(Title.titleString(@object)) == 0)
        this.@string.append(defaultValue);
      else
        this.@string.append((object) @object.title());
      return this;
    }

    public override string ToString() => this.@string.ToString();

    public virtual Title truncate(int noWords)
    {
      if (noWords < 1)
        throw new IllegalArgumentException("Truncation must be to one or more words");
      int num = 0;
      for (int index = 0; num < this.@string.length() && index < noWords; ++num)
      {
        if (this.@string.charAt(num) == ' ')
          ++index;
      }
      if (num < this.@string.length())
      {
        this.@string.setLength(num - 1);
        this.@string.append("...");
      }
      return this;
    }

    public static Title title(TitledObject @object) => @object == null ? new Title() : new Title(Title.titleString(@object));

    [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
    [JavaFlags(4227077)]
    public new virtual object MemberwiseClone()
    {
      Title title = this;
      ObjectImpl.clone((object) title);
      return ((object) title).MemberwiseClone();
    }
  }
}
