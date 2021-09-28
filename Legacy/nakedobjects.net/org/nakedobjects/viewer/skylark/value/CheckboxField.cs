// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.viewer.skylark.value.CheckboxField
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using org.nakedobjects.@object.value;
using org.nakedobjects.viewer.skylark.core;
using System.ComponentModel;

namespace org.nakedobjects.viewer.skylark.value
{
  public class CheckboxField : AbstractField
  {
    private static readonly int size;

    public CheckboxField(Content content, ViewSpecification specification, ViewAxis axis)
      : base(content, specification, axis)
    {
    }

    public override bool canFocus() => this.canChangeValue();

    public override void draw(Canvas canvas)
    {
      Color color1 = !this.getIdentified() ? Style.SECONDARY2 : Style.IDENTIFIED;
      Color color2 = !this.hasFocus() ? color1 : Style.PRIMARY1;
      int vpadding = View.VPADDING;
      int hpadding = View.HPADDING;
      canvas.drawRectangle(hpadding, vpadding, CheckboxField.size - 1, CheckboxField.size - 1, color2);
      if (!this.isSet())
        return;
      int num1 = hpadding + 2;
      int y = vpadding + 2;
      int y2 = CheckboxField.size - 1;
      int num2 = CheckboxField.size - 2;
      Color black = Style.BLACK;
      canvas.drawLine(num1, y, num2, y2, black);
      canvas.drawLine(num1 + 1, y, num2 + 1, y2, black);
      canvas.drawLine(num2, y, num1, y2, black);
      canvas.drawLine(num2 + 1, y, num1 + 1, y2, black);
    }

    public override void firstClick(Click click) => this.toggle();

    public override void keyTyped(char keyCode)
    {
      if (keyCode == ' ')
        this.toggle();
      else
        base.keyTyped(keyCode);
    }

    private void toggle()
    {
      if (!this.canChangeValue())
        return;
      this.initiateSave();
    }

    public override int getBaseline() => View.VPADDING + Style.NORMAL.getAscent();

    public override Size getMaximumSize() => new Size(View.HPADDING + CheckboxField.size + View.HPADDING, View.VPADDING + CheckboxField.size + View.VPADDING);

    private bool isSet() => ((BooleanValue) this.getContent().getNaked()).isSet();

    [JavaFlags(4)]
    public override void save()
    {
      ((BooleanValue) this.getContent().getNaked()).toggle();
      this.markDamaged();
      ((ValueContent) this.getContent()).entryComplete();
      this.getParent().invalidateContent();
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [JavaFlags(32778)]
    static CheckboxField()
    {
      // ISSUE: unable to decompile the method.
    }

    [JavaFlags(41)]
    public class Specification : AbstractFieldSpecification
    {
      public override bool canDisplay(Content content) => content.isValue() && content.getNaked() is BooleanValue;

      public override View createView(Content content, ViewAxis axis) => (View) new CheckboxField(content, (ViewSpecification) this, axis);

      public override string getName() => "Checkbox";
    }
  }
}
