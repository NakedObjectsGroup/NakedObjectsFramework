// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.object.defaults.ApplicationContext
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using java.lang;
using java.util;
using org.apache.log4j;
using org.nakedobjects.@object;
using org.nakedobjects.@object.defaults;
using System.ComponentModel;

namespace org.nakedobjects.@object.defaults
{
  [JavaInterfaces("1;org/nakedobjects/object/UserContext;")]
  public abstract class ApplicationContext : UserContext
  {
    private static readonly Logger LOG;
    private readonly Vector classes;
    private readonly Vector objects;
    private User user;

    public static string fieldOrder() => "user, classes, objects";

    [JavaFlags(4)]
    public virtual NakedClass addClass(Class cls) => this.addClass(cls.getName());

    [JavaFlags(4)]
    public virtual NakedClass addClass(string className)
    {
      if (ApplicationContext.LOG.isInfoEnabled())
        ApplicationContext.LOG.info((object) new StringBuffer().append("added class ").append(className).append(" to ").append((object) this).ToString());
      NakedObjectSpecification specification = NakedObjects.getSpecificationLoader().loadSpecification(className);
      NakedClass nakedClass = NakedObjects.getObjectPersistor().getNakedClass(specification);
      this.classes.addElement((object) nakedClass);
      return nakedClass;
    }

    public virtual void addToClasses(NakedClass cls)
    {
      this.classes.addElement((object) cls);
      this.objectChanged();
    }

    public virtual string getName() => "Naked Object Application";

    public virtual void addToObjects(NakedObject cls)
    {
      this.objects.addElement((object) cls);
      this.objectChanged();
    }

    public virtual void associateUser(SimpleUser user)
    {
      user.setRootObject((Naked) NakedObjects.getObjectLoader().getAdapterForElseCreateAdapterForTransient((object) this));
      this.setUser((User) user);
    }

    public virtual void dissociateUser(SimpleUser user)
    {
      user.setRootObject((Naked) null);
      this.setUser((User) null);
    }

    public virtual Vector getClasses() => this.classes;

    public virtual Vector getObjects() => this.objects;

    public virtual User getUser() => this.user;

    private void objectChanged()
    {
    }

    public virtual void removeFromClasses(NakedClass cls)
    {
      this.classes.addElement((object) cls);
      this.objectChanged();
    }

    public virtual void removeFromObjects(NakedObject cls)
    {
      this.objects.addElement((object) cls);
      this.objectChanged();
    }

    public virtual void setUser(User user) => this.user = user;

    public ApplicationContext()
    {
      this.classes = new Vector();
      this.objects = new Vector();
      this.user = (User) new SimpleUser("user name", "id");
    }

    [JavaFlags(32778)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    static ApplicationContext()
    {
      // ISSUE: unable to decompile the method.
    }

    [JavaFlags(4227077)]
    [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
    public new virtual object MemberwiseClone()
    {
      ApplicationContext applicationContext = this;
      ObjectImpl.clone((object) applicationContext);
      return ((object) applicationContext).MemberwiseClone();
    }

    [JavaFlags(4227073)]
    public override string ToString() => ObjectImpl.jloToString((object) this);
  }
}
