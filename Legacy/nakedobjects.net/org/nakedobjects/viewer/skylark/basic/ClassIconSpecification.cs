// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.viewer.skylark.basic.ClassIconSpecification
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;

namespace org.nakedobjects.viewer.skylark.basic
{
  public class ClassIconSpecification : IconSpecification
  {
    public ClassIconSpecification()
      : this(false, false)
    {
    }

    [JavaFlags(0)]
    public ClassIconSpecification(bool isSubView, bool isReplaceable)
      : base(isSubView, isReplaceable)
    {
    }

    public override View createView(Content content, ViewAxis axis) => (View) new SimpleBorder(1, (View) new ClassIconBackground((View) new IconView(content, (ViewSpecification) this, (ViewAxis) null, Style.TITLE)));
  }
}
