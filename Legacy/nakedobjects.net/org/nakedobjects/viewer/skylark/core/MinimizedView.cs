// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.viewer.skylark.core.MinimizedView
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using java.lang;
using org.nakedobjects.@object;
using org.nakedobjects.@object.control;
using org.nakedobjects.utility;
using org.nakedobjects.viewer.skylark.basic;
using org.nakedobjects.viewer.skylark.metal;
using System.ComponentModel;

namespace org.nakedobjects.viewer.skylark.core
{
  public class MinimizedView : AbstractView
  {
    private const int BORDER_WIDTH = 5;
    private WindowControl[] controls;
    private View iconView;
    private readonly View viewToMinimize;

    public MinimizedView(View viewToMinimize)
      : base(viewToMinimize.getContent(), (ViewSpecification) null, (ViewAxis) null)
    {
      this.viewToMinimize = viewToMinimize;
      this.iconView = new SubviewIconSpecification().createView(viewToMinimize.getContent(), (ViewAxis) null);
      this.iconView.setParent((View) this);
      int length = 2;
      WindowControl[] windowControlArray = length >= 0 ? new WindowControl[length] : throw new NegativeArraySizeException();
      windowControlArray[0] = (WindowControl) new MinimizedView.RestoreWindowControl(this, (View) this);
      windowControlArray[1] = (WindowControl) new MinimizedView.CloseWindowControl(this, (View) this);
      this.controls = windowControlArray;
    }

    public override string debugDetails()
    {
      DebugString debugString = new DebugString();
      debugString.append((object) base.debugDetails());
      debugString.appendln("minimized view", (object) this.viewToMinimize);
      debugString.appendln();
      debugString.appendln("icon size", (object) this.iconView.getSize());
      debugString.append((object) this.iconView);
      return debugString.ToString();
    }

    public override void dispose()
    {
      base.dispose();
      this.viewToMinimize.dispose();
    }

    public override Drag dragStart(DragStart drag)
    {
      if (!this.iconView.getBounds().contains(drag.getLocation()))
        return base.dragStart(drag);
      drag.subtract(5, 5);
      return this.iconView.dragStart(drag);
    }

    public override void draw(Canvas canvas)
    {
      base.draw(canvas);
      Size size = this.getSize();
      int width = size.getWidth();
      int height = size.getHeight();
      int num = 3;
      int y = 3;
      Color color = !this.containsFocus() ? Style.SECONDARY2 : Style.SECONDARY1;
      canvas.clearBackground((View) this, Style.WINDOW_BACKGROUND);
      canvas.drawRectangle(1, 0, width - 2, height, color);
      canvas.drawRectangle(0, 1, width, height - 2, color);
      for (int index = 2; index < num; ++index)
        canvas.drawRectangle(index, index, width - 2 * index, height - 2 * index, color);
      if (this.getState().isActive())
      {
        int x = num;
        canvas.drawRectangle(x, y, width - 2 * x, height - 2 * x - y, Style.ACTIVE);
      }
      int x1 = this.controls[0].getLocation().getX() - 3;
      canvas.drawSolidRectangle(x1, y, width - x1 - 3, height - y * 2, Style.SECONDARY3);
      canvas.drawLine(x1 - 1, y, x1 - 1, height - y * 2, color);
      for (int index = 0; this.controls != null && index < this.controls.Length; ++index)
      {
        Canvas subcanvas = canvas.createSubcanvas(this.controls[index].getBounds());
        this.controls[index].draw(subcanvas);
      }
      this.iconView.draw(canvas.createSubcanvas(this.iconView.getBounds()));
    }

    public override Size getMaximumSize()
    {
      Size size = new Size();
      size.extendWidth(5);
      Size maximumSize = this.iconView.getMaximumSize();
      size.extendWidth(maximumSize.getWidth());
      size.extendHeight(maximumSize.getHeight());
      size.ensureHeight(13);
      size.extendHeight(5);
      size.extendHeight(5);
      size.extendWidth(View.HPADDING);
      size.extendWidth(this.controls.Length * (15 + View.HPADDING));
      size.extendWidth(5);
      return size;
    }

    public override Padding getPadding() => new Padding(5, 5, 5, 5);

    public override void layout(Size maximumSize)
    {
      Size maximumSize1 = this.getMaximumSize();
      this.layoutControls(maximumSize1.getWidth());
      maximumSize1.contractWidth(10);
      maximumSize1.contractWidth(View.HPADDING);
      maximumSize1.contractWidth(this.controls.Length * (15 + View.HPADDING));
      maximumSize1.contractHeight(10);
      this.iconView.setLocation(new Location(5, 5));
      this.iconView.setSize(maximumSize1);
    }

    private void layoutControls(int width)
    {
      int num = 15 + View.HPADDING;
      int x = width - 5 + View.HPADDING - num * this.controls.Length;
      int y = 5;
      for (int index = 0; index < this.controls.Length; ++index)
      {
        this.controls[index].setSize(this.controls[index].getMaximumSize());
        this.controls[index].setLocation(new Location(x, y));
        x += num;
      }
    }

    public override void restore()
    {
      Workspace workspace = this.getWorkspace();
      foreach (View subview in workspace.getSubviews())
      {
        if (subview == this)
        {
          this.viewToMinimize.setParent((View) workspace);
          workspace.removeView((View) this);
          workspace.addView(this.viewToMinimize);
          workspace.invalidateLayout();
          break;
        }
      }
    }

    private void close()
    {
      Workspace workspace = this.getWorkspace();
      foreach (View subview in workspace.getSubviews())
      {
        if (subview == this)
        {
          this.viewToMinimize.setParent((View) workspace);
          workspace.removeView((View) this);
          workspace.invalidateLayout();
          break;
        }
      }
    }

    public override void secondClick(Click click) => this.restore();

    public override ViewAreaType viewAreaType(Location location)
    {
      location.subtract(5, 5);
      return this.iconView.viewAreaType(location);
    }

    public override void viewMenuOptions(UserActionSet options)
    {
      options.add((UserAction) new MinimizedView.\u0031(this, "Restore"));
      base.viewMenuOptions(options);
    }

    public override void firstClick(Click click) => this.overControl(click.getLocation())?.firstClick(click);

    private View overControl(Location location)
    {
      for (int index = 0; index < this.controls.Length; ++index)
      {
        WindowControl control = this.controls[index];
        if (control.getBounds().contains(location))
          return (View) control;
      }
      return (View) null;
    }

    public override void dragIn(ContentDrag drag)
    {
      if (!this.iconView.getBounds().contains(drag.getTargetLocation()))
        return;
      drag.subtract(5, 5);
      this.iconView.dragIn(drag);
    }

    public override void dragOut(ContentDrag drag)
    {
      if (!this.iconView.getBounds().contains(drag.getTargetLocation()))
        return;
      drag.subtract(5, 5);
      this.iconView.dragOut(drag);
    }

    public override View identify(Location location)
    {
      if (!this.iconView.getBounds().contains(location))
        return (View) this;
      location.subtract(5, 5);
      return this.iconView.identify(location);
    }

    public override void drop(ContentDrag drag)
    {
      if (!this.iconView.getBounds().contains(drag.getTargetLocation()))
        return;
      drag.subtract(5, 5);
      this.iconView.drop(drag);
    }

    [JavaFlags(34)]
    [Inner]
    private class CloseWindowControl : WindowControl
    {
      [JavaFlags(32770)]
      [EditorBrowsable(EditorBrowsableState.Never)]
      private MinimizedView this\u00240;

      public CloseWindowControl(MinimizedView _param1, View target)
        : base((UserAction) new MinimizedView.CloseWindowControl.\u0031(), target)
      {
        this.this\u00240 = _param1;
        if (_param1 != null)
          return;
        ObjectImpl.getClass((object) _param1);
      }

      public override void draw(Canvas canvas)
      {
        int num1 = 0;
        int num2 = 0;
        Color black = Style.BLACK;
        canvas.drawLine(num1 + 4, num2 + 3, num1 + 10, num2 + 9, black);
        canvas.drawLine(num1 + 5, num2 + 3, num1 + 11, num2 + 9, black);
        canvas.drawLine(num1 + 10, num2 + 3, num1 + 4, num2 + 9, black);
        canvas.drawLine(num1 + 11, num2 + 3, num1 + 5, num2 + 9, black);
      }

      [Inner]
      [JavaInterfaces("1;org/nakedobjects/viewer/skylark/UserAction;")]
      [JavaFlags(32)]
      public class \u0031 : UserAction
      {
        public virtual Consent disabled(View view) => (Consent) Allow.DEFAULT;

        public virtual void execute(Workspace workspace, View view, Location at) => ((MinimizedView) view).close();

        public virtual string getDescription(View view) => new StringBuffer().append("Close ").append(view.getSpecification().getName()).ToString();

        public virtual string getHelp(View view) => (string) null;

        public virtual string getName(View view) => "Close view";

        public virtual Action.Type getType() => UserAction.USER;

        [JavaFlags(4227077)]
        [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
        public new virtual object MemberwiseClone()
        {
          MinimizedView.CloseWindowControl.\u0031 obj = this;
          ObjectImpl.clone((object) obj);
          return ((object) obj).MemberwiseClone();
        }

        [JavaFlags(4227073)]
        public override string ToString() => ObjectImpl.jloToString((object) this);
      }
    }

    [JavaFlags(34)]
    [Inner]
    private class RestoreWindowControl : WindowControl
    {
      [JavaFlags(32770)]
      [EditorBrowsable(EditorBrowsableState.Never)]
      private MinimizedView this\u00240;

      public RestoreWindowControl(MinimizedView _param1, View target)
        : base((UserAction) new MinimizedView.RestoreWindowControl.\u0031(), target)
      {
        this.this\u00240 = _param1;
        if (_param1 != null)
          return;
        ObjectImpl.getClass((object) _param1);
      }

      public override void draw(Canvas canvas)
      {
        int num1 = 0;
        int num2 = 0;
        canvas.drawLine(num1 + 3, num2 + 8, num1 + 8, num2 + 8, Style.BLACK);
        canvas.drawLine(num1 + 3, num2 + 9, num1 + 8, num2 + 9, Style.BLACK);
      }

      [JavaFlags(32)]
      [Inner]
      [JavaInterfaces("1;org/nakedobjects/viewer/skylark/UserAction;")]
      public class \u0031 : UserAction
      {
        public virtual Consent disabled(View view) => (Consent) Allow.DEFAULT;

        public virtual void execute(Workspace workspace, View view, Location at) => ((MinimizedView) view).restore();

        public virtual string getDescription(View view) => new StringBuffer().append("Restore ").append(view.getSpecification().getName()).append(" to normal size").ToString();

        public virtual string getHelp(View view) => (string) null;

        public virtual string getName(View view) => "Restore view";

        public virtual Action.Type getType() => UserAction.USER;

        [JavaFlags(4227077)]
        [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
        public new virtual object MemberwiseClone()
        {
          MinimizedView.RestoreWindowControl.\u0031 obj = this;
          ObjectImpl.clone((object) obj);
          return ((object) obj).MemberwiseClone();
        }

        [JavaFlags(4227073)]
        public override string ToString() => ObjectImpl.jloToString((object) this);
      }
    }

    [JavaFlags(32)]
    [Inner]
    public new class \u0031 : AbstractUserAction
    {
      [EditorBrowsable(EditorBrowsableState.Never)]
      [JavaFlags(32770)]
      private MinimizedView this\u00240;

      public override void execute(Workspace workspace, View view, Location at) => this.this\u00240.restore();

      public \u0031(MinimizedView _param1, string dummy0)
        : base(dummy0)
      {
        this.this\u00240 = _param1;
        if (_param1 != null)
          return;
        ObjectImpl.getClass((object) _param1);
      }
    }
  }
}
