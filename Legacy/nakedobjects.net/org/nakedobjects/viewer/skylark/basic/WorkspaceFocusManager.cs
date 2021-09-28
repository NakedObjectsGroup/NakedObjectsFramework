// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.viewer.skylark.basic.WorkspaceFocusManager
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;

namespace org.nakedobjects.viewer.skylark.basic
{
  [JavaInterfaces("1;org/nakedobjects/viewer/skylark/FocusManager;")]
  public class WorkspaceFocusManager : FocusManager
  {
    public virtual void focusNextView()
    {
    }

    public virtual void focusPreviousView()
    {
    }

    public virtual void focusParentView()
    {
    }

    public virtual void focusFirstChildView()
    {
    }

    public virtual void focusLastChildView()
    {
    }

    public virtual void focusInitialChildView()
    {
    }

    public virtual View getFocus() => (View) null;

    public virtual void setFocus(View view)
    {
    }

    [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
    [JavaFlags(4227077)]
    public new virtual object MemberwiseClone()
    {
      WorkspaceFocusManager workspaceFocusManager = this;
      ObjectImpl.clone((object) workspaceFocusManager);
      return ((object) workspaceFocusManager).MemberwiseClone();
    }

    [JavaFlags(4227073)]
    public override string ToString() => ObjectImpl.jloToString((object) this);
  }
}
