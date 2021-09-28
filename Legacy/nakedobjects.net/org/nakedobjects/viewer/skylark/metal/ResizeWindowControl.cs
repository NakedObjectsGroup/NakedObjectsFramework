// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.viewer.skylark.metal.ResizeWindowControl
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using org.nakedobjects.@object;
using org.nakedobjects.@object.control;

namespace org.nakedobjects.viewer.skylark.metal
{
  public class ResizeWindowControl : WindowControl
  {
    public ResizeWindowControl(View target)
      : base((UserAction) new ResizeWindowControl.\u0031(), target)
    {
    }

    public override void draw(Canvas canvas)
    {
      int x = 0;
      int y = 0;
      canvas.drawRectangle(x + 1, y + 1, 14, 12, Style.WHITE);
      canvas.drawRectangle(x, y, 14, 12, Style.SECONDARY1);
      canvas.drawRectangle(x + 3, y + 2, 8, 8, Style.SECONDARY2);
      canvas.drawLine(x + 3, y + 3, x + 10, y + 3, Style.SECONDARY2);
    }

    [JavaFlags(32)]
    [Inner]
    [JavaInterfaces("1;org/nakedobjects/viewer/skylark/UserAction;")]
    public class \u0031 : UserAction
    {
      public virtual Consent disabled(View view) => (Consent) Allow.DEFAULT;

      public virtual void execute(Workspace workspace, View view, Location at)
      {
      }

      public virtual string getDescription(View view) => "";

      public virtual string getHelp(View view) => "";

      public virtual Action.Type getType() => UserAction.USER;

      public virtual string getName(View view) => "Resize";

      [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
      [JavaFlags(4227077)]
      public new virtual object MemberwiseClone()
      {
        ResizeWindowControl.\u0031 obj = this;
        ObjectImpl.clone((object) obj);
        return ((object) obj).MemberwiseClone();
      }

      [JavaFlags(4227073)]
      public override string ToString() => ObjectImpl.jloToString((object) this);
    }
  }
}
