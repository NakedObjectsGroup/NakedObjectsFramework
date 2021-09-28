// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.viewer.skylark.special.LookupSelection
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using org.nakedobjects.@object;
using org.nakedobjects.viewer.skylark.core;

namespace org.nakedobjects.viewer.skylark.special
{
  [JavaFlags(32)]
  public class LookupSelection : AbstractViewDecorator
  {
    [JavaFlags(4)]
    public LookupSelection(View wrappedView)
      : base(wrappedView)
    {
    }

    public override void draw(Canvas canvas)
    {
      if (this.getState().isViewIdentified())
      {
        Color secondarY3 = Style.SECONDARY3;
        canvas.clearBackground((View) this, secondarY3);
      }
      canvas.offset(View.HPADDING, 0);
      base.draw(canvas);
    }

    public override void entered()
    {
      this.getState().setObjectIdentified();
      this.getState().setViewIdentified();
      this.wrappedView.entered();
      this.markDamaged();
    }

    public override void exited()
    {
      this.getState().clearObjectIdentified();
      this.getState().clearViewIdentified();
      this.wrappedView.exited();
      this.markDamaged();
    }

    public override void firstClick(Click click)
    {
      LookupAxis viewAxis = (LookupAxis) this.getViewAxis();
      NakedObject @object = ((ObjectContent) this.getContent()).getObject();
      viewAxis.getContent().setObject(@object);
      View originalView = viewAxis.getOriginalView();
      originalView.getParent().updateView();
      originalView.getParent().invalidateContent();
      this.getParent().dispose();
    }

    public override Size getRequiredSize(Size maximumSize)
    {
      Size requiredSize = base.getRequiredSize(maximumSize);
      requiredSize.extendWidth(View.HPADDING * 2);
      return requiredSize;
    }

    public override void keyPressed(KeyboardAction key)
    {
      if (key.getKeyCode() == 27)
        this.dispose();
      base.keyPressed(key);
    }
  }
}
