// Decompiled with JetBrains decompiler
// Type: org.apache.log4j.config.PropertyPrinter
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using com.ms.vjsharp.util;
using java.io;
using java.lang;
using java.util;
using System.Runtime.CompilerServices;

namespace org.apache.log4j.config
{
  [JavaInterfaces("1;org/apache/log4j/config/PropertyGetter+PropertyCallback;")]
  public class PropertyPrinter : PropertyGetter.PropertyCallback
  {
    [JavaFlags(4)]
    public int numAppenders;
    [JavaFlags(4)]
    public Hashtable appenderNames;
    [JavaFlags(4)]
    public Hashtable layoutNames;
    [JavaFlags(4)]
    public PrintWriter @out;
    [JavaFlags(4)]
    public bool doCapitalize;

    public PropertyPrinter(PrintWriter @out)
      : this(@out, false)
    {
    }

    public PropertyPrinter(PrintWriter @out, bool doCapitalize)
    {
      this.numAppenders = 0;
      this.appenderNames = new Hashtable();
      this.layoutNames = new Hashtable();
      this.@out = @out;
      this.doCapitalize = doCapitalize;
      this.print(@out);
      @out.flush();
    }

    [JavaFlags(4)]
    public virtual string genAppName()
    {
      StringBuffer stringBuffer = new StringBuffer().append("A");
      int numAppenders;
      this.numAppenders = (numAppenders = this.numAppenders) + 1;
      int num = numAppenders;
      return stringBuffer.append(num).ToString();
    }

    [JavaFlags(4)]
    public virtual bool isGenAppName(string name)
    {
      if (StringImpl.length(name) < 2 || StringImpl.charAt(name, 0) != 'A')
        return false;
      for (int index = 0; index < StringImpl.length(name); ++index)
      {
        if (StringImpl.charAt(name, index) < '0' || StringImpl.charAt(name, index) > '9')
          return false;
      }
      return true;
    }

    public virtual void print(PrintWriter @out)
    {
      this.printOptions(@out, Category.getRoot());
      Enumeration currentCategories = Category.getCurrentCategories();
      while (currentCategories.hasMoreElements())
        this.printOptions(@out, (Category) currentCategories.nextElement());
    }

    [JavaFlags(4)]
    public virtual void printOptions(PrintWriter @out, Category cat)
    {
      Enumeration allAppenders = cat.getAllAppenders();
      Level level = cat.getLevel();
      string str1 = level != null ? level.ToString() : "";
      while (allAppenders.hasMoreElements())
      {
        Appender appender = (Appender) allAppenders.nextElement();
        string name;
        if ((name = \u003CVerifierFix\u003E.genCastToString(this.appenderNames.get((object) appender))) == null)
        {
          if ((name = appender.getName()) == null || this.isGenAppName(name))
            name = this.genAppName();
          this.appenderNames.put((object) appender, (object) name);
          this.printOptions(@out, (object) appender, new StringBuffer().append("log4j.appender.").append(name).ToString());
          if (appender.getLayout() != null)
            this.printOptions(@out, (object) appender.getLayout(), new StringBuffer().append("log4j.appender.").append(name).append(".layout").ToString());
        }
        str1 = new StringBuffer().append(str1).append(", ").append(name).ToString();
      }
      string str2 = cat != Category.getRoot() ? new StringBuffer().append("log4j.category.").append(cat.getName()).ToString() : "log4j.rootCategory";
      if ((object) str1 == (object) "")
        return;
      @out.println(new StringBuffer().append(str2).append("=").append(str1).ToString());
    }

    [JavaFlags(4)]
    public virtual void printOptions(PrintWriter @out, object obj, string fullname)
    {
      @out.println(new StringBuffer().append(fullname).append("=").append(ObjectImpl.getClass(obj).getName()).ToString());
      PropertyGetter.getProperties(obj, (PropertyGetter.PropertyCallback) this, new StringBuffer().append(fullname).append(".").ToString());
    }

    public virtual void foundProperty(object obj, string prefix, string name, object value)
    {
      if (obj is Appender && StringImpl.equals(nameof (name), (object) name))
        return;
      if (this.doCapitalize)
        name = PropertyPrinter.capitalize(name);
      this.@out.println(new StringBuffer().append(prefix).append(name).append("=").append(value.ToString()).ToString());
    }

    public static string capitalize(string name)
    {
      if (!Character.isLowerCase(StringImpl.charAt(name, 0)) || StringImpl.length(name) != 1 && !Character.isLowerCase(StringImpl.charAt(name, 1)))
        return name;
      StringBuffer stringBuffer = new StringBuffer(name);
      stringBuffer.setCharAt(0, Character.toUpperCase(StringImpl.charAt(name, 0)));
      return stringBuffer.ToString();
    }

    public static void main(string[] args)
    {
      // ISSUE: type reference
      RuntimeHelpers.RunClassConstructor(__typeref (ObjectImpl));
      PropertyPrinter propertyPrinter = new PropertyPrinter(new PrintWriter((OutputStream) java.lang.System.@out));
      Utilities.cleanupAfterMainReturns();
    }

    [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
    [JavaFlags(4227077)]
    public new virtual object MemberwiseClone()
    {
      PropertyPrinter propertyPrinter = this;
      ObjectImpl.clone((object) propertyPrinter);
      return ((object) propertyPrinter).MemberwiseClone();
    }

    [JavaFlags(4227073)]
    public override string ToString() => ObjectImpl.jloToString((object) this);
  }
}
