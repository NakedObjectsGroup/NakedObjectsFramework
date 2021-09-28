// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.application.valueholder.TextString
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using java.lang;
using org.apache.log4j;
using System;
using System.ComponentModel;

namespace org.nakedobjects.application.valueholder
{
  public class TextString : BusinessValueHolder
  {
    private static readonly Logger logger;
    private const long serialVersionUID = 1;
    private int maximumLength;
    private int minimumLength;
    private string text;

    public TextString()
    {
      this.maximumLength = 0;
      this.minimumLength = 0;
      this.setValue((string) null);
    }

    public TextString(string text)
    {
      this.maximumLength = 0;
      this.minimumLength = 0;
      this.setValue(text);
    }

    public TextString(TextString textString)
    {
      this.maximumLength = 0;
      this.minimumLength = 0;
      this.setValue(textString);
    }

    public override string asEncodedString() => this.isEmpty() ? "NULL" : this.text;

    private void checkForInvalidCharacters()
    {
      if (this.text == null)
        return;
      for (int index = 0; index < StringImpl.length(this.text); ++index)
      {
        if (this.isCharDisallowed(StringImpl.charAt(this.text, index)))
          throw new RuntimeException(new StringBuffer().append((object) ObjectImpl.getClass((object) this)).append(" cannot contain the character code 0x").append(Integer.toHexString((int) StringImpl.charAt(this.text, index))).ToString());
      }
    }

    public override void clear() => this.text = (string) null;

    public virtual bool contains(string text) => this.contains(text, Case.SENSITIVE);

    public virtual bool contains(string text, Case caseSensitive)
    {
      if (this.text == null)
        return false;
      return caseSensitive == Case.SENSITIVE ? StringImpl.indexOf(this.text, text) >= 0 : StringImpl.indexOf(StringImpl.toLowerCase(this.text), StringImpl.toLowerCase(text)) >= 0;
    }

    public override void copyObject(BusinessValueHolder @object)
    {
      if (!(@object is TextString))
        throw new IllegalArgumentException("Can only copy the value of  a TextString object");
      this.setValue((TextString) @object);
    }

    public virtual bool endsWith(string text) => this.endsWith(text, Case.SENSITIVE);

    public virtual bool endsWith(string text, Case caseSensitive)
    {
      if (this.text == null)
        return false;
      return caseSensitive == Case.SENSITIVE ? StringImpl.endsWith(this.text, text) : StringImpl.endsWith(StringImpl.toLowerCase(this.text), StringImpl.toLowerCase(text));
    }

    [Obsolete(null, false)]
    public override bool Equals(object @object)
    {
      if (!(@object is TextString))
        return base.Equals(@object);
      TextString textString = (TextString) @object;
      if (this.text != null)
        return StringImpl.equals(this.text, (object) textString.text);
      return textString.text == null;
    }

    [JavaFlags(4)]
    public virtual Logger getLogger() => TextString.logger;

    public virtual int getMaximumLength() => this.maximumLength;

    public virtual int getMinimumLength() => this.minimumLength;

    public virtual string getObjectHelpText() => "A TextString object.";

    [JavaFlags(4)]
    public virtual bool isCharDisallowed(char c) => c == '\n' || c == '\r' || c == '\t';

    public override bool isEmpty() => this.text == null || StringImpl.length(this.text) == 0;

    public override bool isSameAs(BusinessValueHolder @object) => @object is TextString && this.isSameAs((TextString) @object);

    public virtual bool isSameAs(string text) => this.isSameAs(text, Case.SENSITIVE);

    public virtual bool isSameAs(string text, Case caseSensitive)
    {
      if (this.text == null)
        return false;
      return caseSensitive == Case.SENSITIVE ? StringImpl.equals(this.text, (object) text) : StringImpl.equalsIgnoreCase(this.text, text);
    }

    public virtual bool isSameAs(TextString text) => this.isSameAs(text, Case.SENSITIVE);

    public virtual bool isSameAs(TextString text, Case caseSensitive) => this.text == null ? (object) this.text == (object) text.text : (caseSensitive == Case.SENSITIVE ? StringImpl.equals(this.text, (object) text.text) : StringImpl.equalsIgnoreCase(this.text, text.text));

    public virtual bool isValid() => false;

    [JavaThrownExceptions("1;org/nakedobjects/application/ValueParseException;")]
    public override void parseUserEntry(string text) => this.setValue(text);

    public virtual void reset() => this.setValue((string) null);

    public override void restoreFromEncodedString(string data)
    {
      if (data == null || StringImpl.equals(data, (object) "NULL"))
      {
        this.clear();
      }
      else
      {
        this.text = data;
        this.checkForInvalidCharacters();
      }
    }

    public virtual void setMaximumLength(int maximumLength) => this.maximumLength = maximumLength;

    public virtual void setMinimumLength(int minimumLength) => this.minimumLength = minimumLength;

    public virtual void setValue(string text)
    {
      this.text = text != null ? text : (string) null;
      this.checkForInvalidCharacters();
    }

    public virtual void setValue(TextString text)
    {
      if (text == null || text.isEmpty())
        this.clear();
      else
        this.setValue(text.text);
    }

    public virtual bool startsWith(string text) => this.startsWith(text, Case.SENSITIVE);

    public virtual bool startsWith(string text, Case caseSensitive)
    {
      if (this.text == null)
        return false;
      return caseSensitive == Case.SENSITIVE ? StringImpl.startsWith(this.text, text) : StringImpl.startsWith(StringImpl.toLowerCase(this.text), StringImpl.toLowerCase(text));
    }

    public virtual string stringValue() => this.isEmpty() ? "" : this.text;

    public override Title title() => new Title(this.stringValue());

    [JavaFlags(32778)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    static TextString()
    {
      // ISSUE: unable to decompile the method.
    }
  }
}
