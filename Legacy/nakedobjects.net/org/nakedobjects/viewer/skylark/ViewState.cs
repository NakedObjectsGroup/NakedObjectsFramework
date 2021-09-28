// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.viewer.skylark.ViewState
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using java.lang;
using System;
using System.ComponentModel;

namespace org.nakedobjects.viewer.skylark
{
  [JavaInterfaces("1;System/ICloneable;")]
  public class ViewState : ICloneable
  {
    private const short CAN_DROP = 16;
    private const short CANT_DROP = 8;
    private const short OBJECT_IDENTIFIED = 4;
    private const short ROOT_VIEW_IDENTIFIED = 1;
    private const short VIEW_IDENTIFIED = 2;
    private const short INVALID = 64;
    private const short ACTIVE = 32;
    private const short OUT_OF_SYNCH = 128;
    private short state;

    public virtual void setCanDrop() => this.state |= (short) 16;

    public virtual void setCantDrop() => this.state |= (short) 8;

    public virtual void setObjectIdentified() => this.state |= (short) 4;

    public virtual bool isObjectIdentified() => ((int) this.state & 4) > 0;

    public virtual void setRootViewIdentified() => this.state |= (short) 1;

    public virtual bool isRootViewIdentified() => ((int) this.state & 1) > 0;

    public virtual void setViewIdentified() => this.state |= (short) 2;

    public virtual bool isViewIdentified() => ((int) this.state & 2) > 0;

    public virtual bool canDrop() => ((int) this.state & 16) == 16;

    public virtual bool cantDrop() => ((int) this.state & 8) == 8;

    public virtual void clearObjectIdentified() => this.state &= (short) -29;

    public virtual void clearRootViewIdentified() => this.state &= (short) -2;

    public virtual void clearViewIdentified() => this.state &= (short) -31;

    [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
    [JavaFlags(4)]
    public new virtual object MemberwiseClone()
    {
      ViewState viewState = this;
      ObjectImpl.clone((object) viewState);
      return ((object) viewState).MemberwiseClone();
    }

    public override string ToString()
    {
      string str = "";
      return this.state != (short) 0 ? new StringBuffer().append(new StringBuffer().append(new StringBuffer().append(new StringBuffer().append(new StringBuffer().append(new StringBuffer().append(new StringBuffer().append(new StringBuffer().append(new StringBuffer().append(str).append(!this.isObjectIdentified() ? "" : "Object-Identified ").ToString()).append(!this.isViewIdentified() ? "" : "View-identified ").ToString()).append(!this.isRootViewIdentified() ? "" : "Root-view-identified ").ToString()).append(!this.canDrop() ? "" : "Can-drop ").ToString()).append(!this.cantDrop() ? "" : "Cant-drop ").ToString()).append(!this.isActive() ? "" : "Active ").ToString()).append(!this.isInvalid() ? "" : "Invalid ").ToString()).append(!this.isOutOfSynch() ? "" : "Out-of-synch ").ToString()).append(" ").append(Integer.toBinaryString((int) this.state)).ToString() : "Normal";
    }

    public virtual void setActive() => this.setFlag((short) 32);

    public virtual void setInactive() => this.resetFlag((short) 32);

    public virtual bool isActive() => this.isFlagSet((short) 32);

    private bool isFlagSet(short flag) => ((int) this.state & (int) flag) > 0;

    public virtual void setValid() => this.resetFlag((short) 64);

    private void setFlag(short flag) => this.state |= flag;

    public virtual void setInvalid() => this.setFlag((short) 64);

    private void resetFlag(short flag) => this.state &= (short) ((int) flag ^ -1);

    public virtual bool isInvalid() => this.isFlagSet((short) 64);

    public virtual bool isOutOfSynch() => this.isFlagSet((short) 128);

    public virtual void setOutOfSynch() => this.setFlag((short) 128);

    public virtual void clearOutOfSynch() => this.resetFlag((short) 128);

    [JavaFlags(4227073)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual object Clone()
    {
      // ISSUE: unable to decompile the method.
    }
  }
}
