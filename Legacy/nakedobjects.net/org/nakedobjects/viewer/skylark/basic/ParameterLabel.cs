// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.viewer.skylark.basic.ParameterLabel
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

namespace org.nakedobjects.viewer.skylark.basic
{
  public class ParameterLabel : LabelBorder
  {
    public static ParameterLabel createInstance(View wrappedView)
    {
      ParameterContent content = (ParameterContent) wrappedView.getContent();
      return new ParameterLabel(content.getParameterName(), !content.isRequired() ? Style.LABEL : Style.NORMAL, wrappedView);
    }

    private ParameterLabel(string name, Text style, View wrappedView)
      : base(name, style, wrappedView)
    {
    }
  }
}
