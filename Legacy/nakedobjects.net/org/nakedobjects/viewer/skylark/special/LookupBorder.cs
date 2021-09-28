// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.viewer.skylark.special.LookupBorder
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using org.nakedobjects.viewer.skylark.basic;
using System.ComponentModel;

namespace org.nakedobjects.viewer.skylark.special
{
  public class LookupBorder : OpenOptionFieldBorder
  {
    private static readonly LookupOverlaySpecification spec;

    public LookupBorder(View wrappedView)
      : base(wrappedView)
    {
    }

    [JavaFlags(4)]
    public override View createOverlay()
    {
      ObjectContent content = (ObjectContent) this.getContent();
      return LookupBorder.spec.createView(this.getContent(), (ViewAxis) new LookupAxis(content, this.getView()));
    }

    [JavaFlags(4)]
    public override bool isAvailable()
    {
      Content content = this.getContent();
      return content is OneToOneField && ((OneToOneField) content).isEditable().isAllowed() || content is ObjectParameter;
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [JavaFlags(32778)]
    static LookupBorder()
    {
      // ISSUE: unable to decompile the method.
    }
  }
}
