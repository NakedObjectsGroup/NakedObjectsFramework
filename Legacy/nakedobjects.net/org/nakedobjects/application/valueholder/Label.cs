// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.application.valueholder.Label
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

namespace org.nakedobjects.application.valueholder
{
  public class Label : TextString
  {
    private const long serialVersionUID = 1;

    public Label()
      : this((string) null)
    {
    }

    public Label(string text)
      : base(text)
    {
    }

    public override bool userChangeable() => false;

    public override string getObjectHelpText() => "A Label object.";
  }
}
