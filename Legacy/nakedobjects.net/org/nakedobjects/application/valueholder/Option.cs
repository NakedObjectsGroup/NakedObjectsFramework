// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.application.valueholder.Option
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using java.lang;

namespace org.nakedobjects.application.valueholder
{
  public class Option : BusinessValueHolder
  {
    private string[] options;
    private int selection;

    public Option()
    {
      int length = 1;
      string[] options = length >= 0 ? new string[length] : throw new NegativeArraySizeException();
      options[0] = "";
      // ISSUE: explicit constructor call
      this.\u002Ector(options, 0);
    }

    public Option(string[] options)
      : this(options, 0)
    {
    }

    public Option(string[] options, int selected)
    {
      this.options = options != null && options.Length != 0 ? options : throw new IllegalArgumentException("Options array must exist and have at least one element");
      this.selection = selected;
    }

    public override void clear() => this.selection = -1;

    public override void copyObject(BusinessValueHolder @object) => this.selection = @object is Option ? ((Option) @object).selection : throw new IllegalArgumentException("Can only copy the value of  a SelectionObject object");

    public override bool Equals(object obj)
    {
      if (this == obj)
        return true;
      if (!(obj is Option))
        return false;
      Option option = (Option) obj;
      return option.isEmpty() && this.isEmpty() || option.selection == this.selection;
    }

    public virtual string getObjectHelpText() => "A Selection object.";

    public virtual string getOption(int index) => this.options[index];

    public virtual string getOptionAt(int index) => this.options[index];

    public virtual string[] getOptions() => this.options;

    public virtual string getSelection() => this.isEmpty() ? "" : this.options[this.selection];

    public virtual int getSelectionIndex() => this.selection;

    public override bool isEmpty() => this.selection == -1;

    public override bool isSameAs(BusinessValueHolder @object) => @object is Option && StringImpl.equals(((Option) @object).getSelection(), (object) this.getSelection());

    public virtual int noOptions() => this.options.Length;

    [JavaThrownExceptions("1;org/nakedobjects/application/ValueParseException;")]
    public override void parseUserEntry(string text) => this.setSelection(text);

    public virtual void reset() => this.selection = 0;

    public virtual void setSelection(string selection)
    {
      for (int index = 0; index < this.options.Length; ++index)
      {
        if (StringImpl.equalsIgnoreCase(this.options[index], selection))
        {
          this.selection = index;
          break;
        }
      }
    }

    public virtual void setSelectionIndex(int selection)
    {
      if (selection < 0 && selection >= this.options.Length)
        throw new IllegalArgumentException("Selection value must index one of the available options");
      this.selection = selection;
    }

    public virtual string stringValue() => this.getSelection();

    public override Title title() => new Title(this.options != null ? (this.selection < 0 ? "" : this.options[this.selection]) : "none");

    public virtual bool hasOption(string expectedTitle)
    {
      for (int index = 0; index < this.options.Length; ++index)
      {
        if (StringImpl.equalsIgnoreCase(this.options[index], expectedTitle))
          return true;
      }
      return false;
    }

    public override void restoreFromEncodedString(string data)
    {
      if (data == null)
        this.clear();
      else
        this.setSelection(data);
    }

    public override string asEncodedString() => this.isEmpty() ? (string) null : this.getSelection();
  }
}
