// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.viewer.skylark.basic.EmptyField
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using java.lang;
using org.nakedobjects.@object;
using org.nakedobjects.@object.control;
using org.nakedobjects.utility;
using org.nakedobjects.viewer.skylark.core;
using org.nakedobjects.viewer.skylark.special;

namespace org.nakedobjects.viewer.skylark.basic
{
  public class EmptyField : AbstractView
  {
    private IconGraphic icon;
    private TitleText text;

    public EmptyField(Content content, ViewSpecification specification, ViewAxis axis, Text style)
      : base(content, specification, axis)
    {
      if (((ObjectContent) content).getObject() != null)
        throw new IllegalArgumentException(new StringBuffer().append("Content for EmptyField must be null: ").append((object) content).ToString());
      NakedObject nakedObject = ((ObjectContent) this.getContent()).getObject();
      if (nakedObject != null)
        throw new IllegalArgumentException(new StringBuffer().append("Content for EmptyField must be null: ").append((object) nakedObject).ToString());
      this.icon = new IconGraphic((View) this, style);
      this.text = (TitleText) new EmptyFieldTitleText((View) this, style);
    }

    public override void draw(Canvas canvas)
    {
      base.draw(canvas);
      int x1 = 0;
      int baseline = this.icon.getBaseline();
      this.icon.draw(canvas, x1, baseline);
      int x2 = x1 + this.icon.getSize().getWidth() + View.HPADDING;
      this.text.draw(canvas, x2, baseline);
    }

    public override int getBaseline() => this.icon.getBaseline();

    public override Size getMaximumSize()
    {
      Size size = this.icon.getSize();
      size.extendWidth(View.HPADDING);
      size.extendWidth(this.text.getSize().getWidth());
      return size;
    }

    private Consent canDrop(NakedObject dragSource) => ((ObjectContent) this.getContent()).canSet(dragSource);

    public override void dragIn(ContentDrag drag)
    {
      Content sourceContent = drag.getSourceContent();
      if (sourceContent is ObjectContent)
      {
        Consent consent = this.canDrop(((ObjectContent) sourceContent).getObject());
        if (consent.getReason() != null)
          this.getViewManager().setStatus(consent.getReason());
        if (consent.isAllowed())
          this.getState().setCanDrop();
        else
          this.getState().setCantDrop();
      }
      else
        this.getState().setCantDrop();
      this.markDamaged();
    }

    public override void dragOut(ContentDrag drag)
    {
      this.getState().clearObjectIdentified();
      this.markDamaged();
    }

    public override void drop(ContentDrag drag) => this.setField(((ObjectContent) this.getParent().getContent()).getObject(), ((ObjectContent) drag.getSourceContent()).getObject());

    public override void objectActionResult(Naked result, Location at)
    {
      NakedObject parent = ((ObjectContent) this.getParent().getContent()).getObject();
      if (result is NakedObject)
        this.setField(parent, (NakedObject) result);
      base.objectActionResult(result, at);
    }

    private void setField(NakedObject parent, NakedObject @object)
    {
      if (@object is NakedClass)
        throw new UnexpectedCallException("Not ready for NakedClasses");
      if (!this.canDrop(@object).isAllowed())
        return;
      ((ObjectContent) this.getContent()).setObject(@object);
      this.getParent().invalidateContent();
    }

    public override string ToString() => new StringBuffer().append(nameof (EmptyField)).append(this.getId()).ToString();

    [JavaFlags(41)]
    [JavaInterfaces("1;org/nakedobjects/viewer/skylark/ViewSpecification;")]
    public class Specification : ViewSpecification
    {
      public virtual bool canDisplay(Content content) => content == null || content.getNaked() == null;

      public virtual View createView(Content content, ViewAxis axis)
      {
        EmptyField emptyField = new EmptyField(content, (ViewSpecification) this, axis, Style.NORMAL);
        NakedObjectSpecification specification = content.getSpecification();
        if (content is ObjectParameter && ((ObjectParameter) content).getOptions() != null && ((ObjectParameter) content).getOptions().Length > 0)
          return (View) new ObjectBorder((View) new OptionBorder((View) emptyField));
        return specification.isLookup() ? (View) new ObjectBorder((View) new LookupBorder((View) emptyField)) : (View) new ObjectBorder((View) emptyField);
      }

      public virtual string getName() => "empty field";

      public virtual bool isOpen() => false;

      public virtual bool isReplaceable() => true;

      public virtual bool isSubView() => true;

      [JavaFlags(4227077)]
      [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
      public new virtual object MemberwiseClone()
      {
        EmptyField.Specification specification = this;
        ObjectImpl.clone((object) specification);
        return ((object) specification).MemberwiseClone();
      }

      [JavaFlags(4227073)]
      public override string ToString() => ObjectImpl.jloToString((object) this);
    }
  }
}
