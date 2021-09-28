// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.viewer.skylark.basic.FieldLabel
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

namespace org.nakedobjects.viewer.skylark.basic
{
  public class FieldLabel : LabelBorder
  {
    public static FieldLabel createInstance(View wrappedView)
    {
      FieldContent content = (FieldContent) wrappedView.getContent();
      return new FieldLabel(content.getFieldName(), !content.isMandatory() ? Style.LABEL : Style.NORMAL, wrappedView);
    }

    private FieldLabel(string name, Text style, View wrappedView)
      : base(name, style, wrappedView)
    {
    }
  }
}
