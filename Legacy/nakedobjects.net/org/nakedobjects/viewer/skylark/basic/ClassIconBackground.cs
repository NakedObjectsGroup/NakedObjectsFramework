// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.viewer.skylark.basic.ClassIconBackground
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using org.nakedobjects.@object;
using org.nakedobjects.viewer.skylark.core;

namespace org.nakedobjects.viewer.skylark.basic
{
  public class ClassIconBackground : AbstractViewDecorator
  {
    [JavaFlags(4)]
    public ClassIconBackground(View wrappedView)
      : base(wrappedView)
    {
    }

    public override void draw(Canvas canvas)
    {
      int height = this.getSize().getHeight();
      canvas.drawSolidOval(0, 0, height * 13 / 10, height, Style.PRIMARY3);
      base.draw(canvas);
    }

    public override void secondClick(Click click)
    {
      NakedObject nakedObject = ((ObjectContent) this.getContent()).getObject();
      Action objectAction = nakedObject.getSpecification().getObjectAction(Action.USER, "Instances");
      NakedCollection nakedCollection = (NakedCollection) nakedObject.execute(objectAction, (Naked[]) null);
      Workspace workspace = this.getWorkspace();
      View subviewFor = workspace.createSubviewFor((Naked) nakedCollection, false);
      subviewFor.setLocation(click.getLocation());
      workspace.addView(subviewFor);
    }

    public override void contentMenuOptions(UserActionSet options) => OptionFactory.addClassMenuOptions(((NakedClass) ((ObjectContent) this.getContent()).getObject().getObject()).forObjectType(), options);
  }
}
