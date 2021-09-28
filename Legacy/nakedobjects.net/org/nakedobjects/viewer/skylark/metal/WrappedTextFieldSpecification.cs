// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.viewer.skylark.metal.WrappedTextFieldSpecification
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using org.nakedobjects.@object.value;
using org.nakedobjects.viewer.skylark.core;
using org.nakedobjects.viewer.skylark.value;

namespace org.nakedobjects.viewer.skylark.metal
{
  public class WrappedTextFieldSpecification : AbstractFieldSpecification
  {
    public override bool canDisplay(Content content) => content.isValue() && content.getNaked() is MultilineStringValue;

    public override View createView(Content content, ViewAxis axis)
    {
      WrappedTextField wrappedTextField = new WrappedTextField(content, (ViewSpecification) this, axis, true, 30);
      wrappedTextField.setNoLines(4);
      return (View) new TextFieldResizeBorder((View) wrappedTextField);
    }

    public override string getName() => "Wrapped Text Field";
  }
}
