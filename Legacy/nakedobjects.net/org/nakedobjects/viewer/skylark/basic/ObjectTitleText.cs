// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.viewer.skylark.basic.ObjectTitleText
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;

namespace org.nakedobjects.viewer.skylark.basic
{
  public class ObjectTitleText : TitleText
  {
    private Content content;

    public ObjectTitleText(View view, Text style)
      : base(view, style)
    {
      this.content = view.getContent();
    }

    [JavaFlags(4)]
    public override string title() => this.content.title();
  }
}
