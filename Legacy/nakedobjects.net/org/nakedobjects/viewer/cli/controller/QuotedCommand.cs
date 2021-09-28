// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.viewer.cli.controller.QuotedCommand
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using java.lang;
using org.nakedobjects.@object;

namespace org.nakedobjects.viewer.cli.controller
{
  [JavaInterfaces("1;org/nakedobjects/viewer/cli/Command;")]
  public class QuotedCommand : Command
  {
    private const char QUOTE = '"';
    private const char SPACE = ' ';
    private string name;
    private string[] parameters;

    public QuotedCommand(string entry)
    {
      int length = 0;
      this.parameters = length >= 0 ? new string[length] : throw new NegativeArraySizeException();
      string str = StringImpl.trim(entry);
      StringBuffer @string = new StringBuffer();
      bool flag = false;
      for (int index = 0; index < StringImpl.length(str); ++index)
      {
        char ch = StringImpl.charAt(str, index);
        if (ch == ' ' && !flag)
        {
          if (@string.length() > 0)
            this.setPortion(@string);
        }
        else if (ch == '"')
        {
          if (ch == '"' && flag)
            this.setPortion(@string);
          flag = ((flag ? 1 : 0) ^ 1) != 0;
        }
        else
          @string.append(ch);
      }
      if (flag)
        throw new InvalidEntryException("Incorrectly quoted entry");
      this.setPortion(@string);
    }

    public virtual string getName() => this.name;

    public virtual int getNumberOfParameters() => this.parameters.Length;

    public virtual string getParameter(int index) => this.parameters[index];

    public virtual string getParameterAsLowerCase(int index) => StringImpl.toLowerCase(this.getParameter(index));

    public virtual bool hasParameters() => this.getNumberOfParameters() > 0;

    private void setPortion(StringBuffer @string)
    {
      if (this.name == null)
        this.name = StringImpl.toLowerCase(@string.ToString());
      else if (@string.length() > 0)
      {
        int length1 = this.parameters.Length;
        int length2 = length1 + 1;
        string[] strArray = length2 >= 0 ? new string[length2] : throw new NegativeArraySizeException();
        System.arraycopy((object) this.parameters, 0, (object) strArray, 0, length1);
        strArray[length1] = @string.ToString();
        this.parameters = strArray;
      }
      @string.setLength(0);
    }

    [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
    [JavaFlags(4227077)]
    public new virtual object MemberwiseClone()
    {
      QuotedCommand quotedCommand = this;
      ObjectImpl.clone((object) quotedCommand);
      return ((object) quotedCommand).MemberwiseClone();
    }

    [JavaFlags(4227073)]
    public override string ToString() => ObjectImpl.jloToString((object) this);
  }
}
