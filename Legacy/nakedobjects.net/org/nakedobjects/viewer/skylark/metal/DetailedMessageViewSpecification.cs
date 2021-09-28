// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.viewer.skylark.metal.DetailedMessageViewSpecification
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using java.lang;
using org.nakedobjects.utility;
using org.nakedobjects.viewer.skylark.basic;
using System.ComponentModel;

namespace org.nakedobjects.viewer.skylark.metal
{
  [JavaInterfaces("1;org/nakedobjects/viewer/skylark/ViewSpecification;")]
  public class DetailedMessageViewSpecification : ViewSpecification
  {
    public virtual bool canDisplay(Content content) => content is MessageContent && ((MessageContent) content).getDetail() != null;

    public virtual string getName() => "Detailed Message";

    public virtual View createView(Content content, ViewAxis axis)
    {
      int length = 4;
      ButtonAction[] actions = length >= 0 ? new ButtonAction[length] : throw new NegativeArraySizeException();
      actions[0] = (ButtonAction) new DetailedMessageViewSpecification.\u0031(this, "Print...");
      actions[1] = (ButtonAction) new DetailedMessageViewSpecification.\u0032(this, "Save...");
      actions[2] = (ButtonAction) new DetailedMessageViewSpecification.\u0033(this, "Copy");
      actions[3] = (ButtonAction) new ActionDialogSpecification.CancelAction();
      ButtonBorder buttonBorder = new ButtonBorder(actions, (View) new DetailedMessageView(content, (ViewSpecification) this, (ViewAxis) null));
      ExceptionDialogBorder exceptionDialogBorder = new ExceptionDialogBorder((View) buttonBorder, true);
      exceptionDialogBorder.setFocusManager((FocusManager) new ActionDialogFocusManager(buttonBorder));
      return (View) exceptionDialogBorder;
    }

    private string extract(View view)
    {
      Content content = view.getContent();
      string str1 = StringImpl.trim(((MessageContent) content).getMessage());
      string str2 = StringImpl.trim(content.title());
      string str3 = StringImpl.trim(((MessageContent) content).getDetail());
      StringBuffer stringBuffer = new StringBuffer();
      stringBuffer.append(str1);
      stringBuffer.append("\n\n");
      stringBuffer.append(str2);
      stringBuffer.append("\n\n");
      stringBuffer.append(str3);
      stringBuffer.append("\n\n");
      return stringBuffer.ToString();
    }

    public virtual bool isOpen() => true;

    public virtual bool isReplaceable() => false;

    public virtual bool isSubView() => false;

    [JavaFlags(4227077)]
    [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
    public new virtual object MemberwiseClone()
    {
      DetailedMessageViewSpecification viewSpecification = this;
      ObjectImpl.clone((object) viewSpecification);
      return ((object) viewSpecification).MemberwiseClone();
    }

    [JavaFlags(4227073)]
    public override string ToString() => ObjectImpl.jloToString((object) this);

    [Inner]
    [JavaFlags(32)]
    public class \u0031 : AbstractButtonAction
    {
      [EditorBrowsable(EditorBrowsableState.Never)]
      [JavaFlags(32770)]
      private DetailedMessageViewSpecification this\u00240;

      public override void execute(Workspace workspace, View view, Location at) => DebugOutput.print("Print exception", this.this\u00240.extract(view));

      public \u0031(DetailedMessageViewSpecification _param1, string dummy0)
        : base(dummy0)
      {
        this.this\u00240 = _param1;
        if (_param1 != null)
          return;
        ObjectImpl.getClass((object) _param1);
      }
    }

    [JavaFlags(32)]
    [Inner]
    public class \u0032 : AbstractButtonAction
    {
      [JavaFlags(32770)]
      [EditorBrowsable(EditorBrowsableState.Never)]
      private DetailedMessageViewSpecification this\u00240;

      public override void execute(Workspace workspace, View view, Location at) => DebugOutput.saveToFile("Save exception", "Exception", this.this\u00240.extract(view));

      public \u0032(DetailedMessageViewSpecification _param1, string dummy0)
        : base(dummy0)
      {
        this.this\u00240 = _param1;
        if (_param1 != null)
          return;
        ObjectImpl.getClass((object) _param1);
      }
    }

    [Inner]
    [JavaFlags(32)]
    public class \u0033 : AbstractButtonAction
    {
      [JavaFlags(32770)]
      [EditorBrowsable(EditorBrowsableState.Never)]
      private DetailedMessageViewSpecification this\u00240;

      public override void execute(Workspace workspace, View view, Location at) => DebugOutput.saveToClipboard(this.this\u00240.extract(view));

      public \u0033(DetailedMessageViewSpecification _param1, string dummy0)
        : base(dummy0)
      {
        this.this\u00240 = _param1;
        if (_param1 != null)
          return;
        ObjectImpl.getClass((object) _param1);
      }
    }
  }
}
