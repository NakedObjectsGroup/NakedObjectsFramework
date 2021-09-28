// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.application.valueholder.MultilineTextString
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using java.lang;

namespace org.nakedobjects.application.valueholder
{
  public class MultilineTextString : TextString
  {
    private const long serialVersionUID = 1;

    public MultilineTextString()
    {
    }

    public MultilineTextString(string text)
      : base(text)
    {
    }

    public MultilineTextString(MultilineTextString textString)
      : base((TextString) textString)
    {
    }

    public override void setValue(string text) => base.setValue(this.convertLineEnding(text));

    public override void restoreFromEncodedString(string data) => base.restoreFromEncodedString(this.convertLineEnding(data));

    [JavaFlags(4)]
    public override bool isCharDisallowed(char c) => c == '\r';

    private string convertLineEnding(string original)
    {
      if (original == null)
        return (string) null;
      StringBuffer stringBuffer = new StringBuffer(StringImpl.length(original));
      for (int index = 0; index < StringImpl.length(original); ++index)
      {
        if (StringImpl.charAt(original, index) == '\r')
        {
          stringBuffer.append('\n');
          if (index + 1 < StringImpl.length(original) && StringImpl.charAt(original, index + 1) == '\n')
            ++index;
        }
        else
          stringBuffer.append(StringImpl.charAt(original, index));
      }
      return stringBuffer.ToString();
    }
  }
}
