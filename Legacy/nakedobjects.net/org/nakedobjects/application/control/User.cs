// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.application.control.User
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using java.util;
using org.nakedobjects.application.valueholder;

namespace org.nakedobjects.application.control
{
  [JavaInterfaces("1;org/nakedobjects/application/TitledObject;")]
  public sealed class User : TitledObject
  {
    private const long serialVersionUID = 1;
    private readonly TextString name;
    private readonly Vector roles;
    private object rootObject;

    public User()
    {
      this.name = new TextString();
      this.roles = new Vector();
    }

    public User(string name)
      : this()
    {
      this.name.setValue(name);
    }

    public static string fieldOrder() => "name";

    public virtual void aboutFieldDefault(FieldAbout about) => about.visibleOnlyToRole(Role.SYSADMIN);

    [JavaFlags(17)]
    public TextString getName() => this.name;

    [JavaFlags(17)]
    public Vector getRoles() => this.roles;

    [JavaFlags(17)]
    public void addToRoles(Role role)
    {
      this.roles.addElement((object) role);
      this.objectChanged();
    }

    [JavaFlags(17)]
    public void removeFromRoles(Role role)
    {
      this.roles.addElement((object) role);
      this.objectChanged();
    }

    private void objectChanged()
    {
    }

    public virtual object getRootObject() => this.rootObject;

    public virtual void setRootObject(object rootObject)
    {
      this.rootObject = rootObject;
      this.objectChanged();
    }

    public virtual Title title() => this.name.title();

    [JavaFlags(4227077)]
    [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
    public new virtual object MemberwiseClone()
    {
      User user = this;
      ObjectImpl.clone((object) user);
      return ((object) user).MemberwiseClone();
    }

    [JavaFlags(4227073)]
    public override string ToString() => ObjectImpl.jloToString((object) this);
  }
}
