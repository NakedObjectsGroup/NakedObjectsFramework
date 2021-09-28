// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.viewer.skylark.KeyboardAction
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;

namespace org.nakedobjects.viewer.skylark
{
  public class KeyboardAction
  {
    public const int NONE = 0;
    public const int ABORT = 1;
    public const int NEXT_VIEW = 2;
    public const int NEXT_WINDOW = 3;
    public const int PREVIOUS_VIEW = 4;
    public const int PREVIOUS_WINDOW = 5;
    [JavaFlags(16)]
    public readonly int keyCode;
    [JavaFlags(16)]
    public readonly int modifiers;
    private bool isConsumed;

    public KeyboardAction(int keyCode, int modifiers)
    {
      this.keyCode = keyCode;
      this.modifiers = modifiers;
      this.isConsumed = false;
    }

    public virtual int getKeyCode() => this.keyCode;

    public virtual int getModifiers() => this.modifiers;

    public virtual bool isConsumed() => this.isConsumed;

    public virtual void consume() => this.isConsumed = true;

    [JavaFlags(4227077)]
    [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
    public new virtual object MemberwiseClone()
    {
      KeyboardAction keyboardAction = this;
      ObjectImpl.clone((object) keyboardAction);
      return ((object) keyboardAction).MemberwiseClone();
    }

    [JavaFlags(4227073)]
    public override string ToString() => ObjectImpl.jloToString((object) this);
  }
}
