// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.viewer.skylark.metal.MessageDialogSpecification
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using java.lang;
using org.nakedobjects.viewer.skylark.basic;

namespace org.nakedobjects.viewer.skylark.metal
{
  [JavaInterfaces("1;org/nakedobjects/viewer/skylark/ViewSpecification;")]
  public class MessageDialogSpecification : ViewSpecification
  {
    public virtual bool canDisplay(Content content) => content is MessageContent;

    public virtual string getName() => "Message Dialog";

    public virtual View createView(Content content, ViewAxis axis)
    {
      int length = 1;
      ButtonAction[] actions = length >= 0 ? new ButtonAction[length] : throw new NegativeArraySizeException();
      actions[0] = (ButtonAction) new MessageDialogSpecification.CloseViewAction();
      ButtonBorder buttonBorder = new ButtonBorder(actions, (View) new MessageDialogView((MessageContent) content, (ViewSpecification) this, (ViewAxis) null));
      ExceptionDialogBorder exceptionDialogBorder = new ExceptionDialogBorder((View) buttonBorder, true);
      exceptionDialogBorder.setFocusManager((FocusManager) new ActionDialogFocusManager(buttonBorder));
      return (View) exceptionDialogBorder;
    }

    public virtual bool isOpen() => true;

    public virtual bool isReplaceable() => false;

    public virtual bool isSubView() => false;

    [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
    [JavaFlags(4227077)]
    public new virtual object MemberwiseClone()
    {
      MessageDialogSpecification dialogSpecification = this;
      ObjectImpl.clone((object) dialogSpecification);
      return ((object) dialogSpecification).MemberwiseClone();
    }

    [JavaFlags(4227073)]
    public override string ToString() => ObjectImpl.jloToString((object) this);

    [JavaFlags(41)]
    public class CloseViewAction : AbstractButtonAction
    {
      public CloseViewAction()
        : base("Close")
      {
      }

      public override void execute(Workspace workspace, View view, Location at) => view.dispose();
    }
  }
}
