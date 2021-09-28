// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.viewer.skylark.core.PopupMenu
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using java.lang;
using java.util;
using org.nakedobjects.@object;
using org.nakedobjects.@object.control;
using org.nakedobjects.utility;
using org.nakedobjects.viewer.skylark.basic;
using System.ComponentModel;

namespace org.nakedobjects.viewer.skylark.core
{
  public class PopupMenu : AbstractView
  {
    private static readonly org.apache.log4j.Logger LOG;
    private Color backgroundColor;
    private Bounds coreSize;
    private View forView;
    private PopupMenu.Item[] items;
    private int optionIdentified;
    private View submenu;
    private FocusManager simpleFocusManager;

    public PopupMenu()
      : base((Content) null, (ViewSpecification) new PopupMenu.PopupSpecification(), (ViewAxis) null)
    {
      int length = 0;
      this.items = length >= 0 ? new PopupMenu.Item[length] : throw new NegativeArraySizeException();
      this.setContent((Content) new PopupMenu.PopupContent(this));
      this.simpleFocusManager = (FocusManager) new SubviewFocusManager((View) this);
    }

    private void addItems(
      View target,
      UserAction[] options,
      int len,
      Vector list,
      Action.Type type)
    {
      int num = list.size();
      for (int index = 0; index < len; ++index)
      {
        if (options[index].getType() == type)
        {
          if (num > 0 && list.size() == num)
            list.addElement((object) PopupMenu.Item.createDivider());
          list.addElement((object) PopupMenu.Item.createOption(options[index], (object) null, target, this.getLocation()));
        }
      }
    }

    [JavaFlags(4)]
    public virtual Color backgroundColor() => this.backgroundColor;

    public override bool canChangeValue() => false;

    public override bool canFocus() => true;

    public override string debugDetails()
    {
      DebugString debugString = new DebugString();
      debugString.append((object) base.debugDetails());
      debugString.appendTitle("Submenu");
      debugString.append((object) this.submenu);
      debugString.append((object) "\n");
      return debugString.ToString();
    }

    [JavaFlags(4)]
    public virtual Color disabledColor() => Style.DISABLED_MENU;

    public override void dispose()
    {
      if (this.getParent() == null)
        base.dispose();
      else
        this.getParent().dispose();
    }

    public override void draw(Canvas canvas)
    {
      int width = this.coreSize.getWidth();
      int height1 = this.coreSize.getHeight();
      canvas.drawSolidRectangle(0, 0, width, height1, this.backgroundColor);
      canvas.draw3DRectangle(0, 0, width, height1, this.backgroundColor, true);
      int num1 = this.style().getLineHeight() + View.VPADDING;
      int y1 = num1 / 2 + this.style().getAscent() / 2 + this.getPadding().getTop();
      int left = this.getPadding().getLeft();
      for (int index = 0; index < this.items.Length; ++index)
      {
        if (this.items[index].isBlank)
        {
          int num2 = y1 - this.style().getAscent() / 2;
          canvas.drawLine(1, num2, width - 2, num2, this.backgroundColor.brighter());
          canvas.drawLine(1, num2 - 1, width - 2, num2 - 1, this.backgroundColor.darker());
        }
        else
        {
          Color color;
          if (this.items[index].isDisabled || this.items[index].action == null)
            color = this.disabledColor();
          else if (this.getOption() == index)
          {
            int y2 = this.getPadding().getTop() + index * num1;
            int height2 = this.style().getLineHeight() + 2;
            canvas.drawSolidRectangle(2, y2, width - 4, height2, this.backgroundColor.darker());
            color = this.reversedColor();
          }
          else
            color = this.normalColor();
          canvas.drawText(this.items[index].name, left, y1, color, this.style());
          if (this.items[index].action is UserActionSet)
          {
            Shape shape = new Shape(0, 0);
            shape.extendsLine(4, 4);
            shape.extendsLine(-4, 4);
            canvas.drawSolidShape(shape, width - 10, y1 - 8, color);
          }
        }
        y1 += num1;
      }
      if (this.submenu == null)
        return;
      this.submenu.draw(canvas.createSubcanvas(this.submenu.getBounds()));
    }

    public override void editComplete()
    {
    }

    public override void firstClick(Click click)
    {
      if (this.coreSize.contains(click.getLocation()))
      {
        if (!click.button1() && !click.button3())
          return;
        this.mouseMoved(click.getLocation());
        this.invoke();
      }
      else
      {
        if (this.submenu == null)
          return;
        click.subtract(this.submenu.getLocation());
        this.submenu.firstClick(click);
      }
    }

    public override void focusLost()
    {
    }

    public override void focusReceived()
    {
    }

    private void calculateCoreRequiredSize()
    {
      Size size = new Size();
      for (int index = 0; index < this.items.Length; ++index)
      {
        int width = !this.items[index].isBlank ? this.style().stringWidth(this.items[index].name) : 0;
        size.ensureWidth(width);
        size.extendHeight(this.style().getLineHeight() + View.VPADDING);
      }
      size.extend(this.getPadding());
      size.extendWidth(View.HPADDING * 2);
      this.coreSize = new Bounds(size);
    }

    public virtual int getOption() => this.optionIdentified;

    public virtual int getOptionCount() => this.items.Length;

    public override Padding getPadding()
    {
      Padding padding = base.getPadding();
      padding.extendTop(View.VPADDING);
      padding.extendBottom(View.VPADDING);
      padding.extendLeft(View.HPADDING + 5);
      padding.extendRight(View.HPADDING + 5);
      return padding;
    }

    public override Size getMaximumSize()
    {
      Size size = this.coreSize.getSize();
      if (this.submenu != null)
      {
        Size maximumSize = this.submenu.getMaximumSize();
        size.extendWidth(maximumSize.getWidth());
        size.ensureHeight(this.submenu.getLocation().getY() + maximumSize.getHeight());
      }
      return size;
    }

    public override Workspace getWorkspace() => this.forView.getWorkspace();

    public override FocusManager getFocusManager() => this.simpleFocusManager;

    public override bool hasFocus() => false;

    public virtual void init(View target, UserAction[] options, Color color)
    {
      this.forView = target;
      this.optionIdentified = 0;
      this.backgroundColor = color;
      int length1 = options.Length;
      if (length1 == 0)
      {
        int length2 = 1;
        PopupMenu.Item[] objArray = length2 >= 0 ? new PopupMenu.Item[length2] : throw new NegativeArraySizeException();
        objArray[0] = PopupMenu.Item.createNoOption();
        this.items = objArray;
      }
      else
      {
        Vector list = new Vector();
        this.addItems(target, options, length1, list, UserAction.USER);
        this.addItems(target, options, length1, list, UserAction.EXPLORATION);
        this.addItems(target, options, length1, list, UserAction.DEBUG);
        int length3 = list.size();
        this.items = length3 >= 0 ? new PopupMenu.Item[length3] : throw new NegativeArraySizeException();
        list.copyInto((object[]) this.items);
      }
      this.calculateCoreRequiredSize();
    }

    private void invoke()
    {
      int option = this.getOption();
      PopupMenu.Item obj = this.items[option];
      if (obj.isBlank || obj.action == null || obj.action.disabled(this.forView).isVetoed())
        return;
      if (obj.action is UserActionSet)
      {
        this.markDamaged();
        int num = this.style().getLineHeight() + View.VPADDING;
        Location point = new Location(this.coreSize.getWidth() - 4, num * option);
        this.submenu = (View) new PopupMenu();
        this.submenu.setParent((View) this);
        ((PopupMenu) this.submenu).init(this.forView, ((UserActionSet) obj.action).getMenuOptions(), this.backgroundColor);
        this.submenu.setLocation(point);
        this.invalidateLayout();
        Size maximumSize = this.getMaximumSize();
        this.setSize(maximumSize);
        this.layout(maximumSize);
        this.markDamaged();
      }
      else
      {
        Workspace workspace = this.getWorkspace();
        Location at = new Location(this.getAbsoluteLocation());
        at.subtract(workspace.getView().getAbsoluteLocation());
        Padding padding = workspace.getView().getPadding();
        at.move(-padding.getLeft(), -padding.getTop());
        at.move(30, 0);
        this.dispose();
        if (obj.isBlank || obj.action == null || !obj.action.disabled(this.forView).isAllowed())
          return;
        this.showStatus(new StringBuffer().append("Executing ").append((object) obj).ToString());
        if (PopupMenu.LOG.isDebugEnabled())
          PopupMenu.LOG.debug((object) new StringBuffer().append("execute ").append(obj.name).append(" on ").append((object) this.forView).append(" in ").append((object) workspace).ToString());
        obj.action.execute(workspace, this.forView, at);
        this.showStatus("");
      }
    }

    public override void keyPressed(KeyboardAction key)
    {
      if (this.submenu != null)
      {
        this.submenu.keyPressed(key);
      }
      else
      {
        int keyCode = key.getKeyCode();
        switch (keyCode)
        {
          case 10:
            key.consume();
            this.invoke();
            break;
          case 27:
            if (this.getParent() == null)
            {
              this.dispose();
            }
            else
            {
              this.markDamaged();
              ((PopupMenu) this.getParent()).submenu = (View) null;
            }
            key.consume();
            break;
          default:
            if (this.getParent() != null && keyCode == 37)
            {
              this.markDamaged();
              ((PopupMenu) this.getParent()).submenu = (View) null;
              key.consume();
              break;
            }
            if (keyCode == 39 && this.items[this.getOption()].action is UserActionSet)
            {
              key.consume();
              this.invoke();
              break;
            }
            switch (keyCode)
            {
              case 38:
                key.consume();
                if (this.optionIdentified == 0)
                  this.optionIdentified = this.items.Length;
                for (int option = this.optionIdentified - 1; option >= 0; option += -1)
                {
                  if (!this.items[option].isBlank && !this.items[option].isDisabled)
                  {
                    this.setOption(option);
                    break;
                  }
                }
                return;
              case 40:
                key.consume();
                if (this.optionIdentified == this.items.Length - 1)
                  this.optionIdentified = -1;
                for (int option = this.optionIdentified + 1; option < this.items.Length; ++option)
                {
                  if (!this.items[option].isBlank && !this.items[option].isDisabled)
                  {
                    this.setOption(option);
                    break;
                  }
                }
                return;
              default:
                return;
            }
        }
      }
    }

    public override void keyReleased(int keyCode, int modifiers)
    {
    }

    public override void keyTyped(char keyCode)
    {
    }

    public override void layout(Size maximumSize)
    {
      if (this.submenu != null)
        this.submenu.layout(maximumSize);
      this.setSize(this.getMaximumSize());
    }

    [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
    public virtual View makeView(Naked @object, NakedObjectField field) => throw new RuntimeException();

    public override void markDamaged()
    {
      if (this.getParent() == null)
        base.markDamaged();
      else
        this.getParent().markDamaged();
      this.markDamaged(new Bounds(this.getAbsoluteLocation(), this.getSize()));
    }

    public override void mouseMoved(Location at)
    {
      if (this.coreSize.contains(at))
      {
        int option = Math.min(Math.max((at.getY() - this.getPadding().getTop()) / (this.style().getLineHeight() + View.VPADDING), 0), this.items.Length - 1);
        if (option < 0 || this.optionIdentified == option)
          return;
        this.setOption(option);
        this.markDamaged();
      }
      else
      {
        if (this.submenu == null)
          return;
        at.subtract(this.submenu.getLocation());
        this.submenu.mouseMoved(at);
      }
    }

    [JavaFlags(4)]
    public virtual Color normalColor() => Style.NORMAL_MENU;

    [JavaFlags(4)]
    public virtual Color reversedColor() => Style.REVERSE_MENU;

    public virtual void setOption(int option)
    {
      if (option == this.optionIdentified)
        return;
      this.optionIdentified = option;
      PopupMenu.Item obj = this.items[this.optionIdentified];
      this.markDamaged();
      if (obj.isBlank)
        this.showStatus("");
      else if ((object) obj.reason == (object) "")
        this.showStatus(obj.description != null ? obj.description : "");
      else
        this.showStatus(obj.reason);
    }

    [JavaFlags(4)]
    public virtual void showStatus(string status) => this.getViewManager().setStatus(status);

    [JavaFlags(4)]
    public virtual Text style() => Style.MENU;

    public override string ToString() => new StringBuffer().append("PopupMenu [location=").append((object) this.getLocation()).append(",item=").append(this.optionIdentified).append(",itemCount=").append(this.items != null ? this.items.Length : 0).append("]").ToString();

    [JavaFlags(4)]
    public virtual bool transparentBackground() => false;

    [JavaFlags(32778)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    static PopupMenu()
    {
      // ISSUE: unable to decompile the method.
    }

    [JavaFlags(42)]
    private class Item
    {
      [JavaFlags(0)]
      public UserAction action;
      [JavaFlags(0)]
      public string description;
      [JavaFlags(0)]
      public View view;
      [JavaFlags(0)]
      public bool isBlank;
      [JavaFlags(0)]
      public bool isDisabled;
      [JavaFlags(0)]
      public string name;
      [JavaFlags(0)]
      public string reason;

      public static PopupMenu.Item createDivider() => new PopupMenu.Item()
      {
        isBlank = true
      };

      public static PopupMenu.Item createNoOption() => new PopupMenu.Item()
      {
        name = "no options"
      };

      public static PopupMenu.Item createOption(
        UserAction action,
        object @object,
        View view,
        Location location)
      {
        PopupMenu.Item obj = new PopupMenu.Item();
        if (action == null)
        {
          obj.isBlank = true;
        }
        else
        {
          obj.isBlank = false;
          obj.action = action;
          obj.view = view;
          obj.name = action.getName(view);
          obj.description = action.getDescription(view);
          Consent consent = action.disabled(view);
          obj.isDisabled = consent.isVetoed();
          obj.reason = consent.getReason();
        }
        return obj;
      }

      private Item()
      {
      }

      public override string ToString() => this.isBlank ? "NONE" : new StringBuffer().append(this.name).append(" ").append(!this.isDisabled ? new StringBuffer().append(" ").append((object) this.action).ToString() : "DISABLED ").ToString();

      public virtual string getHelp() => this.action.getHelp(this.view);

      [JavaFlags(4227077)]
      [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
      public new virtual object MemberwiseClone()
      {
        PopupMenu.Item obj = this;
        ObjectImpl.clone((object) obj);
        return ((object) obj).MemberwiseClone();
      }
    }

    [Inner]
    [JavaFlags(34)]
    private class PopupContent : AbstractContent
    {
      [EditorBrowsable(EditorBrowsableState.Never)]
      [JavaFlags(32770)]
      private PopupMenu this\u00240;

      public PopupContent(PopupMenu _param1)
      {
        this.this\u00240 = _param1;
        if (_param1 != null)
          return;
        ObjectImpl.getClass((object) _param1);
      }

      public override Consent canDrop(Content sourceContent) => (Consent) Veto.DEFAULT;

      public override void debugDetails(DebugString debug)
      {
      }

      public override Naked drop(Content sourceContent) => (Naked) null;

      public override string getDescription() => this.this\u00240.items[this.this\u00240.getOption()].description;

      public override string getHelp() => this.this\u00240.items[this.this\u00240.getOption()].getHelp();

      public override string getIconName() => (string) null;

      public override Image getIconPicture(int iconHeight) => (Image) null;

      public override string getId() => (string) null;

      public override Naked getNaked() => (Naked) null;

      public override NakedObjectSpecification getSpecification() => (NakedObjectSpecification) null;

      public override bool isTransient() => false;

      [JavaThrownExceptions("1;org/nakedobjects/object/InvalidEntryException;")]
      public override void parseTextEntry(string entryText)
      {
      }

      public override string title() => this.this\u00240.items[this.this\u00240.getOption()].name;
    }

    [JavaInterfaces("1;org/nakedobjects/viewer/skylark/ViewSpecification;")]
    [JavaFlags(42)]
    private class PopupSpecification : ViewSpecification
    {
      public virtual bool canDisplay(Content content) => false;

      public virtual View createView(Content content, ViewAxis axis) => (View) null;

      public virtual string getName() => "Popup Menu";

      public virtual bool isOpen() => true;

      public virtual bool isReplaceable() => false;

      public virtual bool isSubView() => false;

      [JavaFlags(2)]
      public PopupSpecification()
      {
      }

      [JavaFlags(4227077)]
      [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
      public new virtual object MemberwiseClone()
      {
        PopupMenu.PopupSpecification popupSpecification = this;
        ObjectImpl.clone((object) popupSpecification);
        return ((object) popupSpecification).MemberwiseClone();
      }

      [JavaFlags(4227073)]
      public override string ToString() => ObjectImpl.jloToString((object) this);
    }
  }
}
