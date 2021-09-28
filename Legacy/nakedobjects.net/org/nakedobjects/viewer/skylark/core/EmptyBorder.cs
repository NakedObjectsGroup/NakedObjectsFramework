// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.viewer.skylark.core.EmptyBorder
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

namespace org.nakedobjects.viewer.skylark.core
{
  public class EmptyBorder : AbstractBorder
  {
    public EmptyBorder(int width, View view)
      : base(view)
    {
      this.left = this.top = this.right = this.bottom = width;
    }
  }
}
