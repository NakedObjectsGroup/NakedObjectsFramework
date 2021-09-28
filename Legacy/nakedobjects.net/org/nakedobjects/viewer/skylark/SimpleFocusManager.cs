// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.viewer.skylark.SimpleFocusManager
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using java.lang;
using org.nakedobjects.utility;
using System;

namespace org.nakedobjects.viewer.skylark
{
  [Obsolete(null, false)]
  [JavaInterfaces("1;org/nakedobjects/viewer/skylark/FocusManager;")]
  public class SimpleFocusManager : FocusManager
  {
    private View focus;

    public virtual View firstView() => (View) null;

    public virtual void focusFirstChildView()
    {
      View[] subviews = this.focus.getSubviews();
      for (int index = 0; index < subviews.Length; ++index)
      {
        if (subviews[index].canFocus())
        {
          this.setFocus(subviews[index]);
          break;
        }
      }
    }

    public virtual void focusNextView()
    {
      View parent = this.focus.getParent();
      if (parent != null)
      {
        View[] subviews = parent.getSubviews();
        for (int index1 = 0; index1 < subviews.Length; ++index1)
        {
          if (subviews[index1] == this.focus)
          {
            for (int index2 = index1 + 1; index2 < subviews.Length; ++index2)
            {
              if (subviews[index2].canFocus())
              {
                this.setFocus(subviews[index2]);
                return;
              }
            }
            for (int index3 = 0; index3 < index1; ++index3)
            {
              if (subviews[index3].canFocus())
              {
                this.setFocus(subviews[index3]);
                break;
              }
            }
            return;
          }
        }
        throw new NakedObjectRuntimeException();
      }
    }

    public virtual void focusParentView()
    {
      View parent = this.focus.getParent();
      if (parent == null)
        return;
      this.setFocus(parent);
    }

    public virtual void focusPreviousView()
    {
      View parent = this.focus.getParent();
      if (parent != null)
      {
        View[] subviews = parent.getSubviews();
        for (int index1 = 0; index1 < subviews.Length; ++index1)
        {
          if (subviews[index1] == this.focus)
          {
            for (int index2 = index1 - 1; index2 >= 0; index2 += -1)
            {
              if (subviews[index2].canFocus())
              {
                this.setFocus(subviews[index2]);
                return;
              }
            }
            for (int index3 = subviews.Length - 1; index3 > index1; index3 += -1)
            {
              if (subviews[index3].canFocus())
              {
                this.setFocus(subviews[index3]);
                break;
              }
            }
            return;
          }
        }
        throw new NakedObjectRuntimeException(new StringBuffer().append("Can't move to previous peer from ").append((object) this.focus).ToString());
      }
    }

    public virtual View getFocus() => this.focus;

    public virtual View initialView() => (View) null;

    public virtual View lastView() => (View) null;

    public virtual void setFocus(View view)
    {
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

    public virtual void focusLastChildView()
    {
    }

    public virtual void focusInitialChildView()
    {
    }

    [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
    [JavaFlags(4227077)]
    public new virtual object MemberwiseClone()
    {
      SimpleFocusManager simpleFocusManager = this;
      ObjectImpl.clone((object) simpleFocusManager);
      return ((object) simpleFocusManager).MemberwiseClone();
    }

    [JavaFlags(4227073)]
    public override string ToString() => ObjectImpl.jloToString((object) this);
  }
}
