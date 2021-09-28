// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.viewer.skylark.value.AbstractField
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using java.lang;
using org.nakedobjects.@object;
using org.nakedobjects.utility;
using org.nakedobjects.viewer.skylark.core;

namespace org.nakedobjects.viewer.skylark.value
{
  public abstract class AbstractField : AbstractView
  {
    private bool identified;

    [JavaFlags(4)]
    public AbstractField(Content content, ViewSpecification design, ViewAxis axis)
      : base(content, design, axis)
    {
    }

    public override bool canFocus() => false;

    public override bool canChangeValue()
    {
      ValueContent content = (ValueContent) this.getContent();
      return !content.isDerived() && content.isEditable().isAllowed();
    }

    public override void drag(InternalDrag drag)
    {
    }

    public override void dragCancel(InternalDrag drag)
    {
    }

    public override View dragFrom(Location location) => (View) null;

    public override void dragTo(InternalDrag drag)
    {
    }

    public override void draw(Canvas canvas)
    {
      if (this.getState().isActive())
        canvas.clearBackground((View) this, Style.IDENTIFIED);
      if (this.getState().isOutOfSynch())
        canvas.clearBackground((View) this, Style.OUT_OF_SYNCH);
      if (this.getState().isInvalid())
        canvas.clearBackground((View) this, Style.INVALID);
      base.draw(canvas);
    }

    public override void editComplete()
    {
    }

    public override void entered()
    {
      this.identified = true;
      this.markDamaged();
    }

    public override void exited()
    {
      this.identified = false;
      this.markDamaged();
    }

    public virtual bool getIdentified() => this.identified;

    public override Padding getPadding() => new Padding(0, 0, 0, 0);

    public virtual View getRoot() => throw new NotImplementedException();

    public override bool hasFocus() => this.getViewManager().hasFocus(this.getView());

    public virtual bool indicatesForView(Location mouseLocation) => false;

    public override void keyPressed(KeyboardAction key)
    {
    }

    public override void keyReleased(int keyCode, int modifiers)
    {
    }

    public override void keyTyped(char keyCode)
    {
    }

    public override void contentMenuOptions(UserActionSet options)
    {
      options.add((UserAction) new ClearValueOption());
      options.add((UserAction) new CopyValueOption());
      options.add((UserAction) new PasteValueOption());
      if (this.getView().getSpecification().isReplaceable())
        this.replaceOptions(Skylark.getViewFactory().valueViews(this.getContent(), (View) this), options);
      base.contentMenuOptions(options);
      options.setColor(Style.VALUE_MENU);
    }

    [JavaFlags(20)]
    public void initiateSave()
    {
      this.save();
      this.getParent().updateView();
      this.invalidateLayout();
    }

    [JavaFlags(1028)]
    public abstract void save();

    [JavaFlags(4)]
    [JavaThrownExceptions("1;org/nakedobjects/object/InvalidEntryException;")]
    public virtual void saveValue(NakedObject value) => this.parseEntry(value.titleString());

    [JavaThrownExceptions("1;org/nakedobjects/object/InvalidEntryException;")]
    [JavaFlags(4)]
    public virtual void parseEntry(string entryText)
    {
      ValueContent content = (ValueContent) this.getContent();
      content.parseTextEntry(entryText);
      content.entryComplete();
    }

    public override string ToString()
    {
      string name = ObjectImpl.getClass((object) this).getName();
      Naked naked = this.getContent().getNaked();
      return new StringBuffer().append(StringImpl.substring(name, StringImpl.lastIndexOf(name, 46) + 1)).append(this.getId()).append(" [location=").append((object) this.getLocation()).append(",object=").append(naked != null ? naked.getObject() : (object) "").append("]").ToString();
    }

    public override ViewAreaType viewAreaType(Location mouseLocation) => ViewAreaType.INTERNAL;

    public override int getBaseline() => Style.defaultBaseline();
  }
}
