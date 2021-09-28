// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.viewer.skylark.special.OptionBorder
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using System.ComponentModel;

namespace org.nakedobjects.viewer.skylark.special
{
  public class OptionBorder : OpenOptionFieldBorder
  {
    private static readonly OptionOverlaySpecification spec;

    public OptionBorder(View wrappedView)
      : base(wrappedView)
    {
    }

    [JavaFlags(4)]
    public override View createOverlay()
    {
      ObjectContent content = (ObjectContent) this.getContent();
      return OptionBorder.spec.createView(this.getContent(), (ViewAxis) new LookupAxis(content, this.getView()));
    }

    [JavaFlags(4)]
    public override bool isAvailable()
    {
      Content content = this.getContent();
      switch (content)
      {
        case OneToOneField _:
          return ((OneToOneField) content).isEditable().isAllowed();
        case ParameterContent _:
          return true;
        default:
          return false;
      }
    }

    [JavaFlags(32778)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    static OptionBorder()
    {
      // ISSUE: unable to decompile the method.
    }
  }
}
