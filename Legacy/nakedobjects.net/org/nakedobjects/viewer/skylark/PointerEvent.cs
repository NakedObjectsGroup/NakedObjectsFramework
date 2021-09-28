// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.viewer.skylark.PointerEvent
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using java.lang;

namespace org.nakedobjects.viewer.skylark
{
  public abstract class PointerEvent
  {
    [JavaFlags(4)]
    public int mods;

    [JavaFlags(0)]
    public PointerEvent(int mods) => this.mods = mods;

    public virtual bool isAlt() => (this.mods & 8) > 0;

    private bool isButton1() => (this.mods & 16) > 0;

    private bool isButton2() => (this.mods & 8) > 0;

    private bool isButton3() => (this.mods & 4) > 0;

    public virtual bool isCtrl() => (this.mods & 2) > 0;

    public virtual bool isMeta() => (this.mods & 4) > 0;

    public virtual bool isShift() => (this.mods & 1) > 0;

    public override string ToString()
    {
      string str1 = new StringBuffer().append(!this.isButton1() ? "-" : "^").append(!this.isButton2() ? "-" : "^").append(!this.isButton3() ? "-" : "^").ToString();
      string str2 = new StringBuffer().append(!this.isShift() ? "-" : "S").append(!this.isAlt() ? "-" : "A").append(!this.isCtrl() ? "-" : "C").ToString();
      return new StringBuffer().append("buttons=").append(str1).append(",modifiers=").append(str2).ToString();
    }

    public virtual bool button1() => this.isButton1() && !this.isShift() || this.isButton2() && this.isShift();

    public virtual bool button2() => this.isButton2() && !this.isShift() || this.isButton1() && this.isShift();

    public virtual bool button3() => this.isButton3();

    [JavaFlags(4227077)]
    [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
    public new virtual object MemberwiseClone()
    {
      PointerEvent pointerEvent = this;
      ObjectImpl.clone((object) pointerEvent);
      return ((object) pointerEvent).MemberwiseClone();
    }
  }
}
