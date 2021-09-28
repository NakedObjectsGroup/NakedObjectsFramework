// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.viewer.skylark.metal.ClassIcon
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using java.lang;
using org.nakedobjects.@object;
using org.nakedobjects.viewer.skylark.basic;
using org.nakedobjects.viewer.skylark.core;
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace org.nakedobjects.viewer.skylark.metal
{
  public class ClassIcon : ObjectView
  {
    private const int CLASS_ICON_SIZE = 34;
    private static readonly string CLASS_ICON_SIZE_PROPERTY;
    private IconGraphic iconUnselected;
    private IconGraphic iconSelected;
    private IconGraphic icon;
    private readonly ClassTitleText title;

    public ClassIcon(Content content, ViewSpecification specification, ViewAxis axis)
      : base(content, specification, axis)
    {
      int integer = NakedObjects.getConfiguration().getInteger(ClassIcon.CLASS_ICON_SIZE_PROPERTY, 34);
      this.iconUnselected = (IconGraphic) new ClassIconGraphic((View) this, integer, 0);
      this.iconSelected = (IconGraphic) new ClassIconGraphic((View) this, integer, 0);
      this.icon = this.iconUnselected;
      this.title = new ClassTitleText((View) this, Style.CLASS);
    }

    public override void exited()
    {
      this.icon = this.iconUnselected;
      this.markDamaged();
      base.exited();
    }

    public override void entered()
    {
      this.icon = this.iconSelected;
      this.markDamaged();
      base.entered();
    }

    public override Drag dragStart(DragStart drag)
    {
      if (drag.isCtrl())
      {
        View dragView = (View) new DragContentIcon(this.getContent());
        return (Drag) new ContentDrag((View) this, drag.getLocation(), dragView);
      }
      View dragView1 = (View) new DragViewOutline(this.getView());
      return (Drag) new ViewDrag((View) this, new Offset(drag.getLocation()), dragView1);
    }

    public override void draw(Canvas canvas)
    {
      base.draw(canvas);
      int x1 = 0;
      int baseline1 = this.icon.getBaseline();
      this.icon.draw(canvas, x1, baseline1);
      int width = this.title.getSize().getWidth();
      int x2 = width <= this.icon.getSize().getWidth() ? this.getSize().getWidth() / 2 - width / 2 : x1;
      int baseline2 = this.icon.getSize().getHeight() + Style.CLASS.getAscent() + View.VPADDING;
      this.title.draw(canvas, x2, baseline2);
    }

    public override int getBaseline() => this.icon.getBaseline();

    public override Size getMaximumSize()
    {
      Size size1 = this.icon.getSize();
      Size size2 = this.title.getSize();
      size1.extendHeight(View.VPADDING + size2.getHeight() + View.VPADDING);
      size1.ensureWidth(size2.getWidth());
      return size1;
    }

    public virtual bool isOpen() => false;

    public override void secondClick(Click click) => BackgroundThread.run((View) this, (BackgroundTask) new ClassIcon.\u0031(this, click));

    private NakedClass getNakedClass() => (NakedClass) ((ObjectContent) this.getContent()).getObject().getObject();

    public override void contentMenuOptions(UserActionSet options)
    {
      OptionFactory.addClassMenuOptions(this.getNakedClass().forObjectType(), options);
      options.setColor(Style.CONTENT_MENU);
    }

    public override string ToString() => new StringBuffer().append("MetalClassIcon").append(this.getId()).ToString();

    [JavaFlags(32778)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    static ClassIcon()
    {
      // ISSUE: unable to decompile the method.
    }

    [JavaInterfaces("1;org/nakedobjects/viewer/skylark/ViewSpecification;")]
    [JavaFlags(41)]
    public class Specification : ViewSpecification
    {
      public virtual bool canDisplay(Content content) => content.isObject() && content.getNaked() is NakedClass;

      public virtual View createView(Content content, ViewAxis axis) => (View) new ObjectBorder((View) new ClassIcon(content, (ViewSpecification) this, axis));

      public virtual string getName() => "class icon";

      public virtual bool isOpen() => false;

      public virtual bool isReplaceable() => false;

      public virtual bool isSubView() => false;

      [JavaFlags(4227077)]
      [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
      public new virtual object MemberwiseClone()
      {
        ClassIcon.Specification specification = this;
        ObjectImpl.clone((object) specification);
        return ((object) specification).MemberwiseClone();
      }

      [JavaFlags(4227073)]
      public override string ToString() => ObjectImpl.jloToString((object) this);
    }

    [JavaFlags(32)]
    [JavaInterfaces("1;org/nakedobjects/viewer/skylark/core/BackgroundTask;")]
    [Inner]
    public new class \u0031 : BackgroundTask
    {
      [EditorBrowsable(EditorBrowsableState.Never)]
      [JavaFlags(32770)]
      private ClassIcon this\u00240;
      [JavaFlags(16)]
      public readonly Click click_\u003E;

      public virtual void execute()
      {
        NakedClass nakedClass = this.this\u00240.getNakedClass();
        Naked @object = (Naked) null;
        if (this.click_\u003E.isCtrl() && this.this\u00240.getViewManager().isRunningAsExploration() && nakedClass.useAllInstance().isAllowed())
          @object = (Naked) nakedClass.allInstances();
        if (@object == null)
          return;
        Location absoluteLocation = this.this\u00240.getView().getAbsoluteLocation();
        absoluteLocation.subtract(this.this\u00240.getWorkspace().getAbsoluteLocation());
        absoluteLocation.add(this.this\u00240.getSize().getWidth() + 10, 0);
        this.this\u00240.getWorkspace().addOpenViewFor(@object, absoluteLocation);
      }

      public virtual string getName() => "Open all instances";

      public virtual string getDescription() => "";

      public \u0031(ClassIcon _param1, [In] Click obj1)
      {
        this.this\u00240 = _param1;
        if (_param1 == null)
          ObjectImpl.getClass((object) _param1);
        this.click_\u003E = obj1;
      }

      [JavaFlags(4227077)]
      [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
      public new virtual object MemberwiseClone()
      {
        ClassIcon.\u0031 obj = this;
        ObjectImpl.clone((object) obj);
        return ((object) obj).MemberwiseClone();
      }

      [JavaFlags(4227073)]
      public override string ToString() => ObjectImpl.jloToString((object) this);
    }
  }
}
