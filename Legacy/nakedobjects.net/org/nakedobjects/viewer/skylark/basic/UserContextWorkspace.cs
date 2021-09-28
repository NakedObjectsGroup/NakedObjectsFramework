// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.viewer.skylark.basic.UserContextWorkspace
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using java.lang;
using org.nakedobjects.@object;

namespace org.nakedobjects.viewer.skylark.basic
{
  public class UserContextWorkspace : DefaultWorkspace
  {
    public UserContextWorkspace(
      Content content,
      CompositeViewSpecification specification,
      ViewAxis axis)
      : base(content, specification, axis)
    {
      if (!(content.getNaked() is UserContext))
        throw new IllegalArgumentException("Content must represent an AbstractUserContext");
    }

    public override void drop(ContentDrag drag)
    {
      base.drop(drag);
      NakedObject nakedObject = ((ObjectContent) drag.getSourceContent()).getObject();
      if (nakedObject.getObject() is NakedClass)
      {
        if (!drag.isShift())
          return;
        ((UserContext) this.getContent().getNaked()).getClasses().addElement((object) nakedObject);
      }
      else
      {
        if (drag.isShift())
          return;
        ((UserContext) this.getContent().getNaked()).getObjects().addElement((object) nakedObject);
      }
    }

    public override string ToString() => new StringBuffer().append(nameof (UserContextWorkspace)).append(this.getId()).ToString();
  }
}
