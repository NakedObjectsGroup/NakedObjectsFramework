// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.viewer.skylark.AbstractFocusManager
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using java.lang;
using org.nakedobjects.utility;

namespace org.nakedobjects.viewer.skylark
{
  [JavaInterfaces("1;org/nakedobjects/viewer/skylark/FocusManager;")]
  public abstract class AbstractFocusManager : FocusManager
  {
    [JavaFlags(4)]
    public View container;
    [JavaFlags(4)]
    public View focus;
    private View initialFocus;

    public AbstractFocusManager(View container)
      : this(container, (View) null)
    {
    }

    public AbstractFocusManager(View container, View initalFocus)
    {
      Assert.assertNotNull((object) container);
      this.container = container;
      this.initialFocus = initalFocus;
      this.focus = initalFocus;
    }

    private void checkCanFocusOn(View view)
    {
      View[] childViews = this.getChildViews();
      bool flag = view == this.container.getView();
      for (int index = 0; !flag && index < childViews.Length; ++index)
      {
        if (childViews[index] == view)
          flag = true;
      }
      if (flag)
        ;
    }

    public virtual void focusFirstChildView()
    {
      View[] childViews = this.getChildViews();
      for (int index = 0; index < childViews.Length; ++index)
      {
        if (childViews[index].canFocus())
        {
          this.setFocus(childViews[index]);
          break;
        }
      }
    }

    public virtual void focusInitialChildView()
    {
      if (this.initialFocus == null)
        this.focusFirstChildView();
      else
        this.setFocus(this.initialFocus);
    }

    public virtual void focusLastChildView()
    {
      View[] childViews = this.getChildViews();
      for (int index = childViews.Length - 1; index > 0; index += -1)
      {
        if (childViews[index].canFocus())
        {
          this.setFocus(childViews[index]);
          break;
        }
      }
    }

    public virtual void focusNextView()
    {
      View[] childViews = this.getChildViews();
      for (int index1 = 0; index1 < childViews.Length; ++index1)
      {
        if (childViews[index1] == this.focus)
        {
          for (int index2 = index1 + 1; index2 < childViews.Length; ++index2)
          {
            if (childViews[index2].canFocus())
            {
              this.setFocus(childViews[index2]);
              return;
            }
          }
          for (int index3 = 0; index3 < index1; ++index3)
          {
            if (childViews[index3].canFocus())
            {
              this.setFocus(childViews[index3]);
              break;
            }
          }
          break;
        }
      }
    }

    public virtual void focusParentView() => this.container.getFocusManager().setFocus(this.container.getFocusManager().getFocus());

    public virtual void focusPreviousView()
    {
      View[] childViews = this.getChildViews();
      for (int index1 = 0; index1 < childViews.Length; ++index1)
      {
        if (childViews[index1] == this.focus)
        {
          for (int index2 = index1 - 1; index2 >= 0; index2 += -1)
          {
            if (childViews[index2].canFocus())
            {
              this.setFocus(childViews[index2]);
              return;
            }
          }
          for (int index3 = childViews.Length - 1; index3 > index1; index3 += -1)
          {
            if (childViews[index3].canFocus())
            {
              this.setFocus(childViews[index3]);
              break;
            }
          }
          return;
        }
      }
      throw new NakedObjectRuntimeException(new StringBuffer().append("Can't move to previous peer from ").append((object) this.focus).ToString());
    }

    [JavaFlags(1028)]
    public abstract View[] getChildViews();

    public virtual View getFocus() => this.focus;

    public virtual void setFocus(View view)
    {
      this.checkCanFocusOn(view);
      if (view == null || !view.canFocus())
        return;
      if (this.focus != null && this.focus != view)
      {
        this.focus.focusLost();
        this.focus.markDamaged();
      }
      this.focus = view;
      this.focus.focusReceived();
      view.markDamaged();
    }

    public override string ToString()
    {
      org.nakedobjects.utility.ToString toString = new org.nakedobjects.utility.ToString((object) this);
      toString.append("container", (object) this.container);
      toString.append("initialFocus", (object) this.initialFocus);
      toString.append("focus", (object) this.focus);
      return toString.ToString();
    }

    [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
    [JavaFlags(4227077)]
    public new virtual object MemberwiseClone()
    {
      AbstractFocusManager abstractFocusManager = this;
      ObjectImpl.clone((object) abstractFocusManager);
      return ((object) abstractFocusManager).MemberwiseClone();
    }
  }
}
