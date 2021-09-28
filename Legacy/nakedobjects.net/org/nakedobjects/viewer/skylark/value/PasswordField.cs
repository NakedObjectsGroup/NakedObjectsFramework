// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.viewer.skylark.value.PasswordField
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using java.lang;
using org.nakedobjects.@object;
using org.nakedobjects.viewer.skylark.basic;
using System.ComponentModel;

namespace org.nakedobjects.viewer.skylark.value
{
  public class PasswordField : AbstractField
  {
    [JavaFlags(28)]
    public static readonly Text style;
    private bool isSaved;
    private int maxTextWidth;
    private string password;
    private int width;
    private bool identified;
    private string invalidReason;

    public PasswordField(Content content, ViewSpecification design, ViewAxis axis, int width)
      : base(content, design, axis)
    {
      this.setMaxTextWidth(width);
      this.width = PasswordField.style.charWidth('O') + 2;
      this.password = this.text();
    }

    public override bool canFocus() => this.canChangeValue();

    public override void contentMenuOptions(UserActionSet options)
    {
      options.add((UserAction) new ClearValueOption());
      base.contentMenuOptions(options);
      options.setColor(Style.VALUE_MENU);
    }

    private void delete()
    {
      this.isSaved = false;
      this.password = StringImpl.substring(this.password, 0, Math.max(0, StringImpl.length(this.password) - 1));
      this.markDamaged();
    }

    public override void draw(Canvas canvas)
    {
      base.draw(canvas);
      Color color1 = !this.identified ? Style.SECONDARY2 : Style.IDENTIFIED;
      Color color2 = !this.hasFocus() ? color1 : Style.PRIMARY1;
      int baseline = this.getBaseline();
      canvas.drawLine(View.HPADDING, baseline, View.HPADDING + this.getSize().getWidth(), baseline, color2);
      int num1 = StringImpl.length(this.password);
      int x = 3;
      for (int index = 0; index < num1; ++index)
      {
        canvas.drawSolidOval(x, 1, this.width, this.width, !this.hasFocus() ? Color.LIGHT_GRAY : Color.YELLOW);
        x += this.width + 2;
      }
      int num2 = 3;
      for (int index = 0; index < num1; ++index)
      {
        canvas.drawOval(num2, 1, this.width, this.width, !this.hasFocus() ? Color.GRAY : Color.BLACK);
        num2 += this.width + 2;
      }
      if (!this.hasFocus() || !this.canChangeValue())
        return;
      canvas.drawLine(num2, baseline + PasswordField.style.getDescent(), num2, 0, Style.PRIMARY1);
    }

    public override void editComplete()
    {
      if (!this.canChangeValue() || this.isSaved)
        return;
      this.isSaved = true;
      this.initiateSave();
    }

    public virtual void escape()
    {
      this.password = "";
      this.isSaved = false;
      this.markDamaged();
    }

    public override void entered()
    {
      if (!this.canChangeValue())
        return;
      this.getViewManager().showTextCursor();
      this.identified = true;
    }

    public override void focusReceived() => this.updateState();

    private void updateState()
    {
      this.getViewManager().setStatus(this.invalidReason != null ? this.invalidReason : "");
      if (this.invalidReason == null)
        this.getState().setValid();
      else
        this.getState().setInvalid();
      this.markDamaged();
    }

    public override void exited()
    {
      if (!this.canChangeValue())
        return;
      this.getViewManager().showArrowCursor();
      this.identified = false;
      this.markDamaged();
    }

    public override void focusLost() => this.editComplete();

    public override Size getMaximumSize() => new Size(View.HPADDING + this.maxTextWidth + View.HPADDING, Math.max(PasswordField.style.getTextHeight() + View.VPADDING, Style.defaultFieldHeight()));

    public override void keyPressed(KeyboardAction key)
    {
      if (!this.canChangeValue())
        return;
      switch (key.getKeyCode())
      {
        case 8:
          key.consume();
          this.delete();
          break;
        case 9:
          this.editComplete();
          break;
        case 10:
          key.consume();
          this.editComplete();
          this.getParent().keyPressed(key);
          break;
        case 27:
          key.consume();
          this.escape();
          break;
        case 37:
          key.consume();
          this.delete();
          break;
        case (int) sbyte.MaxValue:
          key.consume();
          this.delete();
          break;
      }
    }

    public override void keyTyped(char keyCode)
    {
      this.password = new StringBuffer().append(this.password).append(keyCode).ToString();
      this.isSaved = false;
      this.markDamaged();
    }

    [JavaFlags(4)]
    public override void save()
    {
      ValueContent content = (ValueContent) this.getContent();
      try
      {
        this.invalidReason = (string) null;
        content.parseTextEntry(this.password);
        this.getParent().invalidateContent();
      }
      catch (InvalidEntryException ex)
      {
        if (StringImpl.length(this.password) > 0)
          this.invalidReason = new StringBuffer().append("INVALID ENTRY: ").append(((Throwable) ex).getMessage()).ToString();
      }
      this.updateState();
    }

    public virtual void setMaxTextWidth(int noCharacters) => this.maxTextWidth = PasswordField.style.charWidth('o') * noCharacters;

    public virtual void setMaxWidth(int width) => this.maxTextWidth = width;

    private string text() => ((AbstractContent) this.getContent()).getNaked().titleString();

    [JavaFlags(32778)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    static PasswordField()
    {
      // ISSUE: unable to decompile the method.
    }
  }
}
