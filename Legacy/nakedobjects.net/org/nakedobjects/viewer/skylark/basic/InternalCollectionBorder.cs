// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.viewer.skylark.basic.InternalCollectionBorder
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using java.lang;
using org.nakedobjects.@object;
using org.nakedobjects.viewer.skylark.core;
using org.nakedobjects.viewer.skylark.metal;

namespace org.nakedobjects.viewer.skylark.basic
{
  public class InternalCollectionBorder : AbstractBorder
  {
    private IconGraphic icon;

    [JavaFlags(4)]
    public InternalCollectionBorder(View wrappedView)
      : base(wrappedView)
    {
      this.icon = (IconGraphic) new InternalCollectionIconGraphic((View) this, Style.NORMAL);
      this.left = this.icon.getSize().getWidth();
    }

    [JavaFlags(4)]
    public override void debugDetails(StringBuffer b) => b.append("InternalCollectionBorder ");

    public override Size getRequiredSize(Size maximumSize)
    {
      Size requiredSize = base.getRequiredSize(maximumSize);
      requiredSize.ensureWidth(this.left + 45 + this.right);
      requiredSize.ensureHeight(24);
      return requiredSize;
    }

    public override void draw(Canvas canvas)
    {
      this.icon.draw(canvas, 0, this.getBaseline());
      if (((CollectionContent) this.getContent()).getCollection().size() == 0)
      {
        canvas.drawText("empty", this.left, this.getBaseline(), Style.SECONDARY2, Style.NORMAL);
      }
      else
      {
        int num1 = this.icon.getSize().getWidth() / 2;
        int x2 = num1 + 4;
        int y = this.icon.getSize().getHeight() + 1;
        int num2 = this.getSize().getHeight() - 5;
        canvas.drawLine(num1, y, num1, num2, Style.SECONDARY2);
        canvas.drawLine(num1, num2, x2, num2, Style.SECONDARY2);
      }
      base.draw(canvas);
    }

    public override void contentMenuOptions(UserActionSet options)
    {
      base.contentMenuOptions(options);
      OptionFactory.addClassMenuOptions(((OneToManyField) this.getContent()).getSpecification(), options);
    }

    public override void objectActionResult(Naked result, Location at)
    {
      OneToManyAssociation toManyAssociation = ((OneToManyField) this.getContent()).getOneToManyAssociation();
      NakedObject nakedObject = ((ObjectContent) this.getParent().getContent()).getObject();
      if (nakedObject.canAdd(toManyAssociation, (NakedObject) result).isAllowed())
        nakedObject.setAssociation((NakedObjectField) toManyAssociation, (NakedObject) result);
      base.objectActionResult(result, at);
    }

    public override string ToString() => new StringBuffer().append("InternalCollectionBorder/").append((object) this.wrappedView).ToString();
  }
}
