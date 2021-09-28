// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.viewer.skylark.special.DropDownFocusBorder
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using java.lang;
using org.nakedobjects.@object;
using org.nakedobjects.viewer.skylark.core;

namespace org.nakedobjects.viewer.skylark.special
{
  [JavaFlags(32)]
  public class DropDownFocusBorder : AbstractBorder
  {
    private View activeSubviewView;

    [JavaFlags(4)]
    public DropDownFocusBorder(View view)
      : base(view)
    {
    }

    public override View getActiveSubview() => this.activeSubviewView;

    private void setActiveSubview(View subview) => this.activeSubviewView = subview;

    public override void keyPressed(KeyboardAction key)
    {
      if (key.getKeyCode() == 40)
      {
        View[] subviews = this.getSubviews();
        for (int index1 = 0; index1 < subviews.Length; ++index1)
        {
          if (subviews[index1].getState().isViewIdentified() || index1 == subviews.Length - 1)
          {
            subviews[index1].exited();
            int index2 = index1 + 1 < subviews.Length ? index1 + 1 : 0;
            subviews[index2].entered();
            this.setActiveSubview(subviews[index2]);
            break;
          }
        }
      }
      else if (key.getKeyCode() == 38)
      {
        View[] subviews = this.getSubviews();
        for (int index3 = 0; index3 < subviews.Length; ++index3)
        {
          if (subviews[index3].getState().isViewIdentified() || index3 == subviews.Length - 1)
          {
            subviews[index3].exited();
            int index4 = index3 != 0 ? index3 - 1 : subviews.Length - 1;
            subviews[index4].entered();
            this.setActiveSubview(subviews[index4]);
            break;
          }
        }
      }
      else if (key.getKeyCode() == 10)
      {
        this.selectOption();
      }
      else
      {
        if (key.getKeyCode() != 9)
          return;
        this.selectOption();
        View parent = ((LookupAxis) this.getViewAxis()).getOriginalView().getParent();
        if (key.getModifiers() == 1)
          parent.getFocusManager().focusPreviousView();
        else
          parent.getFocusManager().focusNextView();
      }
    }

    private void selectOption()
    {
      View[] subviews1 = this.getSubviews();
      for (int index1 = 0; index1 < subviews1.Length; ++index1)
      {
        if (subviews1[index1].getState().isViewIdentified())
        {
          this.setActiveSubview(subviews1[index1]);
          LookupAxis viewAxis = (LookupAxis) this.getViewAxis();
          NakedObject @object = ((ObjectContent) subviews1[index1].getContent()).getObject();
          viewAxis.getContent().setObject(@object);
          View originalView = viewAxis.getOriginalView();
          View parent = originalView.getParent();
          View[] subviews2 = parent.getSubviews();
          int index2 = 0;
          for (int index3 = 0; index3 < subviews2.Length; ++index3)
          {
            if (originalView == subviews2[index3])
            {
              index2 = index3;
              break;
            }
          }
          parent.updateView();
          parent.invalidateContent();
          this.dispose();
          parent.getFocusManager().setFocus(parent.getSubviews()[index2]);
          break;
        }
      }
    }

    public override void keyTyped(char keyCode)
    {
      View[] subviews = this.getSubviews();
      int index1 = 0;
      int index2;
      for (index2 = 0; index2 < subviews.Length; ++index2)
      {
        if (subviews[index2].getState().isViewIdentified())
        {
          index1 = index2;
          index2 = index2 + 1 < subviews.Length ? index2 + 1 : 0;
          break;
        }
      }
      if (index2 == subviews.Length)
        index2 = 0;
      string lowerCase = StringImpl.toLowerCase(new StringBuffer().append("").append(keyCode).ToString());
      for (int index3 = index2; index3 < subviews.Length; ++index3)
      {
        if (StringImpl.startsWith(StringImpl.toLowerCase(subviews[index3].getContent().title()), lowerCase))
        {
          subviews[index1].exited();
          subviews[index3].entered();
          this.setActiveSubview(subviews[index3]);
          return;
        }
      }
      for (int index4 = 0; index4 < index2; ++index4)
      {
        if (StringImpl.startsWith(StringImpl.toLowerCase(subviews[index4].getContent().title()), lowerCase))
        {
          subviews[index1].exited();
          subviews[index4].entered();
          this.setActiveSubview(subviews[index4]);
          break;
        }
      }
    }
  }
}
