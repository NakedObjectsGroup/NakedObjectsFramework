// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.viewer.skylark.event.MouseWheelEvent
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using java.awt;
using java.awt.@event;
using org.nakedobjects.viewer.skylark.@event;

namespace org.nakedobjects.viewer.skylark.@event
{
  public class MouseWheelEvent : MouseEvent
  {
    private readonly int wheelRotation;

    public MouseWheelEvent(
      Component component,
      int id,
      long when,
      int modifiers,
      int x,
      int y,
      int clickCount,
      bool popupTrigger,
      int wheelRotation)
      : base(component, id, when, modifiers, x, y, clickCount, popupTrigger)
    {
      this.wheelRotation = wheelRotation;
    }

    public virtual int getWheelRotation() => this.wheelRotation;

    public virtual bool isUpWheelRotation() => this.wheelRotation < 0;

    public virtual bool isDownWheelRotation() => this.wheelRotation > 0;

    public virtual string paramString() => "MOUSE_WHEEL";

    [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
    [JavaFlags(4227077)]
    public virtual object MemberwiseClone()
    {
      MouseWheelEvent mouseWheelEvent = this;
      ObjectImpl.clone((object) mouseWheelEvent);
      return ((object) mouseWheelEvent).MemberwiseClone();
    }
  }
}
