// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.viewer.skylark.value.PasteValueOption
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using java.awt;
using java.awt.datatransfer;
using java.lang;
using org.apache.log4j;
using org.nakedobjects.@object;
using org.nakedobjects.@object.control;
using System;
using System.ComponentModel;

namespace org.nakedobjects.viewer.skylark.value
{
  public class PasteValueOption : AbstractValueOption
  {
    private static readonly Logger LOG;

    public PasteValueOption()
      : base("Replace with clipboard value")
    {
    }

    public override Consent disabled(View view) => !view.canChangeValue() ? (Consent) new Veto("Field cannot be edited") : (Consent) new Allow(new StringBuffer().append("Replace field content with '").append(this.getClipboard()).append("' from clipboard").ToString());

    public override void execute(Workspace workspace, View view, Location at)
    {
      string clipboard = this.getClipboard();
      ValueContent content = (ValueContent) view.getContent();
      try
      {
        content.parseTextEntry(clipboard);
        this.updateParent(view);
      }
      catch (TextEntryParseException ex)
      {
        PasteValueOption.LOG.error((object) new StringBuffer().append("invalid clipboard operation ").append((object) ex).ToString());
      }
      catch (InvalidEntryException ex)
      {
        PasteValueOption.LOG.error((object) new StringBuffer().append("invalid clipboard operation ").append((object) ex).ToString());
      }
    }

    private string getClipboard()
    {
      Transferable contents = Toolkit.getDefaultToolkit().getSystemClipboard().getContents((object) this);
      string str = "illegal value";
      try
      {
        str = \u003CVerifierFix\u003E.genCastToString(contents.getTransferData((DataFlavor) DataFlavor.stringFlavor));
      }
      catch (Exception ex)
      {
        Throwable throwable = ThrowableWrapper.wrapThrowable(ex);
        PasteValueOption.LOG.error((object) new StringBuffer().append("invalid clipboard operation ").append((object) throwable).ToString());
      }
      return str;
    }

    public override string ToString() => nameof (PasteValueOption);

    [EditorBrowsable(EditorBrowsableState.Never)]
    [JavaFlags(32778)]
    static PasteValueOption()
    {
      // ISSUE: unable to decompile the method.
    }
  }
}
