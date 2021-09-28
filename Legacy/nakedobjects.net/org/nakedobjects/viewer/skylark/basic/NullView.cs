// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.viewer.skylark.basic.NullView
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.lang;
using java.lang;
using org.nakedobjects.viewer.skylark.core;

namespace org.nakedobjects.viewer.skylark.basic
{
  public class NullView : AbstractView
  {
    public NullView()
      : base((Content) null, (ViewSpecification) null, (ViewAxis) null)
    {
    }

    public override string ToString()
    {
      string name = ObjectImpl.getClass((object) this).getName();
      return new StringBuffer().append(StringImpl.substring(name, StringImpl.lastIndexOf(name, 46) + 1)).append(this.getId()).ToString();
    }
  }
}
