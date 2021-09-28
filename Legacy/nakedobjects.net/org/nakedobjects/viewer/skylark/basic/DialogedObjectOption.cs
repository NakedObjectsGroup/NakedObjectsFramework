// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.viewer.skylark.basic.DialogedObjectOption
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
  public class DialogedObjectOption : AbstractUserAction
  {
    private ActionDialogSpecification dialogSpec;
    private readonly Action action;
    private readonly NakedReference target;

    public static DialogedObjectOption createOption(
      Action action,
      NakedReference @object)
    {
      Assert.assertTrue("Only for actions taking one or more params", action.getParameterCount() > 0);
      return !action.isAuthorised() || @object.isVisible(action).isVetoed() ? (DialogedObjectOption) null : new DialogedObjectOption(action, @object);
    }

    private DialogedObjectOption(Action action, NakedReference @object)
      : base(new StringBuffer().append(action.getName()).append("...").ToString())
    {
      this.dialogSpec = new ActionDialogSpecification();
      this.action = action;
      this.target = @object;
    }

    public override Action.Type getType() => this.action.getType();

    public override string getHelp(View view) => this.action.getHelp();

    public override Consent disabled(View view)
    {
      string description = this.action.getDescription();
      return (Consent) new Allow(new StringBuffer().append(this.getName(view)).append(StringImpl.length(description) != 0 ? new StringBuffer().append(": ").append(description).ToString() : "").ToString());
    }

    public override void execute(Workspace workspace, View view, Location at) => BackgroundThread.run(view, (BackgroundTask) new DialogedObjectOption.\u0031(this, at, workspace, view));

    public override string ToString() => new StringBuffer().append("DialogedObjectOption for ").append((object) this.action).ToString();

    [Inner]
    [JavaInterfaces("1;org/nakedobjects/viewer/skylark/core/BackgroundTask;")]
    [JavaFlags(32)]
    public class \u0031 : BackgroundTask
    {
      [EditorBrowsable(EditorBrowsableState.Never)]
      [JavaFlags(32770)]
      private DialogedObjectOption this\u00240;
      [JavaFlags(16)]
      public readonly Location at_\u003E;
      [JavaFlags(16)]
      public readonly View view_\u003E;
      [JavaFlags(16)]
      public readonly Workspace workspace_\u003E;

      public virtual void execute()
      {
        ActionHelper instance = ActionHelper.createInstance(this.this\u00240.target, this.this\u00240.action);
        View view = this.this\u00240.dialogSpec.createView(!(this.this\u00240.target is NakedObject) ? (Content) new CollectionActionContent(instance) : (Content) new ObjectActionContent(instance), (ViewAxis) null);
        view.setLocation(this.at_\u003E);
        this.workspace_\u003E.addView(view);
      }

      public virtual string getName() => new StringBuffer().append("Preparing action ").append(this.this\u00240.action.getName()).ToString();

      public virtual string getDescription() => new StringBuffer().append("Preparing action ").append(this.getName()).append(" on  ").append((object) this.view_\u003E.getContent().getNaked()).ToString();

      public \u0031(DialogedObjectOption _param1, [In] Location obj1, [In] Workspace obj2, [In] View obj3)
      {
        this.this\u00240 = _param1;
        if (_param1 == null)
          ObjectImpl.getClass((object) _param1);
        this.at_\u003E = obj1;
        this.workspace_\u003E = obj2;
        this.view_\u003E = obj3;
      }

      [JavaFlags(4227077)]
      [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
      public new virtual object MemberwiseClone()
      {
        DialogedObjectOption.\u0031 obj = this;
        ObjectImpl.clone((object) obj);
        return ((object) obj).MemberwiseClone();
      }

      [JavaFlags(4227073)]
      public override string ToString() => ObjectImpl.jloToString((object) this);
    }
  }
}
