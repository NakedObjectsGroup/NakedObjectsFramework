// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.utility.AboutNakedObjects
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using com.ms.vjsharp.util;
using java.io;
using java.lang;
using java.util;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace org.nakedobjects.utility
{
  public class AboutNakedObjects
  {
    private static string applicationCopyrightNotice;
    private static string applicationName;
    private static string applicationVersion;
    private static Vector aboutInformationExtension;

    public static void addAboutInformationExtension(string info) => AboutNakedObjects.aboutInformationExtension.addElement((object) info);

    public static string[] buildInformationExtension()
    {
      int length = AboutNakedObjects.aboutInformationExtension.size();
      string[] strArray = length >= 0 ? new string[length] : throw new NegativeArraySizeException();
      AboutNakedObjects.aboutInformationExtension.copyInto((object[]) strArray);
      return strArray;
    }

    public static string getApplicationCopyrightNotice() => AboutNakedObjects.applicationCopyrightNotice;

    public static string getApplicationName() => AboutNakedObjects.applicationName;

    public static string getApplicationVersion() => AboutNakedObjects.applicationVersion;

    public static string getFrameworkBuild() => AboutNakedObjects.select("20061003-1132", "000000");

    public static string getFrameworkCopyrightNotice() => AboutNakedObjects.select("Copyright  © 2000 - 2006 Naked Objects Group Ltd", "Copyright Naked Objects Group");

    public static string getFrameworkName() => AboutNakedObjects.select("Naked Objects Framework", "Naked Objects Framework");

    public static string getImageName() => AboutNakedObjects.select("logo.jpg", "logo.jpg");

    public static string getFrameworkVersion() => new StringBuffer().append("Version ").append(AboutNakedObjects.select("2.0fixes-", "")).ToString();

    public static void logVersion()
    {
      org.apache.log4j.Logger logger = org.apache.log4j.Logger.getLogger("Naked Objects");
      bool flag = logger.isInfoEnabled();
      if (flag)
        logger.info((object) AboutNakedObjects.getFrameworkName());
      if (flag)
        logger.info((object) new StringBuffer().append(AboutNakedObjects.getFrameworkVersion()).append(AboutNakedObjects.getFrameworkBuild()).ToString());
      if (AboutNakedObjects.getApplicationName() != null && flag)
        logger.info((object) AboutNakedObjects.getApplicationName());
      if (AboutNakedObjects.getApplicationVersion() == null || !flag)
        return;
      logger.info((object) AboutNakedObjects.getApplicationVersion());
    }

    public static void main(string[] args)
    {
      // ISSUE: type reference
      RuntimeHelpers.RunClassConstructor(__typeref (ObjectImpl));
      ((PrintStream) java.lang.System.@out).println(new StringBuffer().append(AboutNakedObjects.getFrameworkName()).append(" version ").append(AboutNakedObjects.getFrameworkVersion()).ToString());
      ((PrintStream) java.lang.System.@out).println(new StringBuffer().append("Build: ").append(AboutNakedObjects.getFrameworkBuild()).ToString());
      ((PrintStream) java.lang.System.@out).println(AboutNakedObjects.getFrameworkCopyrightNotice());
      Utilities.cleanupAfterMainReturns();
    }

    private static string select(string value, string defaultValue) => StringImpl.startsWith(value, "%") && StringImpl.endsWith(value, "%") ? defaultValue : value;

    public static void setApplicationCopyrightNotice(string applicationCopyrightNotice) => AboutNakedObjects.applicationCopyrightNotice = applicationCopyrightNotice;

    public static void setApplicationName(string applicationName) => AboutNakedObjects.applicationName = applicationName;

    public static void setApplicationVersion(string applicationVersion) => AboutNakedObjects.applicationVersion = applicationVersion;

    [EditorBrowsable(EditorBrowsableState.Never)]
    [JavaFlags(32778)]
    static AboutNakedObjects()
    {
      // ISSUE: unable to decompile the method.
    }

    [JavaFlags(4227077)]
    [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
    public new virtual object MemberwiseClone()
    {
      AboutNakedObjects aboutNakedObjects = this;
      ObjectImpl.clone((object) aboutNakedObjects);
      return ((object) aboutNakedObjects).MemberwiseClone();
    }

    [JavaFlags(4227073)]
    public override string ToString() => ObjectImpl.jloToString((object) this);
  }
}
