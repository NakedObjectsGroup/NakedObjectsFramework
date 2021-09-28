// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.viewer.skylark.basic.ImmediateObjectOption
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using java.lang;
using org.nakedobjects.@object;
using org.nakedobjects.@object.control;
using org.nakedobjects.utility;
using org.nakedobjects.viewer.skylark.core;
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace org.nakedobjects.viewer.skylark.basic
{
  [JavaFlags(32)]
  public class ImmediateObjectOption : AbstractUserAction
  {
    private readonly Action action;
    private readonly NakedReference target;

    public static ImmediateObjectOption createOption(
      Action action,
      NakedReference @object)
    {
      Assert.assertTrue("Only suitable for 0 param methods", action.getParameterTypes().Length == 0);
      return !action.isAuthorised() || @object.isVisible(action).isVetoed() ? (ImmediateObjectOption) null : new ImmediateObjectOption(@object, action);
    }

    private ImmediateObjectOption(NakedReference @object, Action action)
      : base(action.getName())
    {
      this.target = @object;
      this.action = action;
    }

    public override Consent disabled(View view)
    {
      Consent consent = this.target.isValid(this.action, (Naked[]) null);
      if (!consent.isAllowed())
        return consent;
      string description = this.action.getDescription();
      return (Consent) new Allow(new StringBuffer().append(this.getName(view)).append(StringImpl.length(description) != 0 ? new StringBuffer().append(": ").append(description).ToString() : "").ToString());
    }

    public override string getHelp(View view) => this.action.getHelp();

    public override Action.Type getType() => this.action.getType();

    public override void execute(Workspace workspace, View view, Location at) => BackgroundThread.run(view, (BackgroundTask) new ImmediateObjectOption.\u0031(this, view, at));

    public override string ToString() => new StringBuffer().append("ObjectOption for ").append(this.action.getId()).ToString();

    [JavaFlags(32)]
    [JavaInterfaces("1;org/nakedobjects/viewer/skylark/core/BackgroundTask;")]
    [Inner]
    public class \u0031 : BackgroundTask
    {
      [EditorBrowsable(EditorBrowsableState.Never)]
      [JavaFlags(32770)]
      private ImmediateObjectOption this\u00240;
      [JavaFlags(16)]
      public readonly Location at_\u003E;
      [JavaFlags(16)]
      public readonly View view_\u003E;

      public virtual void execute()
      {
        Naked result = this.this\u00240.target.execute(this.this\u00240.action, (Naked[]) null);
        if (result != null)
          this.view_\u003E.objectActionResult(result, this.at_\u003E);
        this.view_\u003E.getViewManager().showMessages();
      }

      public virtual string getName() => new StringBuffer().append("Action ").append(this.this\u00240.action.getName()).ToString();

      public virtual string getDescription() => new StringBuffer().append("Running action ").append(this.getName()).append(" on  ").append((object) this.view_\u003E.getContent().getNaked()).ToString();

      public \u0031(ImmediateObjectOption _param1, [In] View obj1, [In] Location obj2)
      {
        this.this\u00240 = _param1;
        if (_param1 == null)
          ObjectImpl.getClass((object) _param1);
        this.view_\u003E = obj1;
        this.at_\u003E = obj2;
      }

      [JavaFlags(4227077)]
      [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
      public new virtual object MemberwiseClone()
      {
        ImmediateObjectOption.\u0031 obj = this;
        ObjectImpl.clone((object) obj);
        return ((object) obj).MemberwiseClone();
      }

      [JavaFlags(4227073)]
      public override string ToString() => ObjectImpl.jloToString((object) this);
    }
  }
}
