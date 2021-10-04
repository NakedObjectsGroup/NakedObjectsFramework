// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.application.control.Role
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using System.ComponentModel;
using Legacy.NakedObjects.Application.ValueHolder;
// ReSharper disable InconsistentNaming

namespace Legacy.NakedObjects.Application.Control
{
  //[JavaInterfaces("1;org/nakedobjects/application/TitledObject;")]
  public sealed class Role : TitledObject
  {
    private const long serialVersionUID = 1;
    public static readonly Role SYSADMIN;
    private readonly TextString name;
    private readonly MultilineTextString description;

    public Role()
    {
      this.name = new TextString();
      this.description = new MultilineTextString();
    }

    public Role(string name)
      : this()
    {
      this.name.setValue(name);
    }

    public  void aboutFieldDefault(FieldAbout about) => about.visibleOnlyToRole(Role.SYSADMIN);

    //[JavaFlags(17)]
    public TextString getName() => this.name;

    //[JavaFlags(17)]
    public MultilineTextString getDescription() => this.description;

    public  Title title() => this.name.title();

    //[JavaFlags(32778)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    static Role()
    {
      // ISSUE: unable to decompile the method.
    }

    //[JavaFlags(4227077)]
    //[JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
    public new  object MemberwiseClone()
    {
      //Role role = this;
      //ObjectImpl.clone((object) role);
      //return ((object) role).MemberwiseClone();
      return null;
    }

    //[JavaFlags(4227073)]
    public override string ToString() {
        //return ObjectImpl.jloToString((object)this);
        return "";
    }
  }
}
