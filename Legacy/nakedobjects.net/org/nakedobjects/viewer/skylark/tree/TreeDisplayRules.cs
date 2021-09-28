// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.viewer.skylark.tree.TreeDisplayRules
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using org.nakedobjects.@object;
using org.nakedobjects.@object.control;
using System.ComponentModel;

namespace org.nakedobjects.viewer.skylark.tree
{
  public class TreeDisplayRules
  {
    private static bool showCollectionsOnly;

    private TreeDisplayRules()
    {
    }

    public static void menuOptions(UserActionSet options)
    {
    }

    public static bool isCollectionsOnly() => TreeDisplayRules.showCollectionsOnly;

    public static bool canDisplay(Naked @object)
    {
      bool flag1 = @object != null && @object.getSpecification().isLookup();
      bool flag2 = ((TreeDisplayRules.isCollectionsOnly() ? 1 : 0) ^ 1) != 0;
      bool flag3 = @object is NakedObject && flag2;
      bool flag4 = @object is NakedCollection;
      return (flag3 || flag4) && !flag1;
    }

    [JavaFlags(32778)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    static TreeDisplayRules()
    {
      // ISSUE: unable to decompile the method.
    }

    [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
    [JavaFlags(4227077)]
    public new virtual object MemberwiseClone()
    {
      TreeDisplayRules treeDisplayRules = this;
      ObjectImpl.clone((object) treeDisplayRules);
      return ((object) treeDisplayRules).MemberwiseClone();
    }

    [JavaFlags(4227073)]
    public override string ToString() => ObjectImpl.jloToString((object) this);

    [Inner]
    [JavaInterfaces("1;org/nakedobjects/viewer/skylark/UserAction;")]
    [JavaFlags(32)]
    public class \u0031 : UserAction
    {
      public virtual void execute(Workspace workspace, View view, Location at) => TreeDisplayRules.showCollectionsOnly = ((TreeDisplayRules.showCollectionsOnly ? 1 : 0) ^ 1) != 0;

      public virtual string getName(View view) => TreeDisplayRules.showCollectionsOnly ? "Show collections only" : "Show all references";

      public virtual Consent disabled(View view) => (Consent) Allow.DEFAULT;

      public virtual string getDescription(View view) => "This option makes the system only show collections within the trees, and not single elements";

      public virtual Action.Type getType() => UserAction.USER;

      public virtual string getHelp(View view) => "";

      [JavaFlags(4227077)]
      [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
      public new virtual object MemberwiseClone()
      {
        TreeDisplayRules.\u0031 obj = this;
        ObjectImpl.clone((object) obj);
        return ((object) obj).MemberwiseClone();
      }

      [JavaFlags(4227073)]
      public override string ToString() => ObjectImpl.jloToString((object) this);
    }
  }
}
