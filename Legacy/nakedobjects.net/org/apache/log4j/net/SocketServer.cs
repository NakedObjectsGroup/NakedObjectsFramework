// Decompiled with JetBrains decompiler
// Type: org.apache.log4j.net.SocketServer
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using java.io;
using java.lang;
using java.net;
using java.util;
using org.apache.log4j.spi;
using System.ComponentModel;

namespace org.apache.log4j.net
{
  public class SocketServer
  {
    [JavaFlags(8)]
    public static string GENERIC;
    [JavaFlags(8)]
    public static string CONFIG_FILE_EXT;
    [JavaFlags(8)]
    public static Category cat;
    [JavaFlags(8)]
    public static SocketServer server;
    [JavaFlags(8)]
    public static int port;
    [JavaFlags(0)]
    public Hashtable hierarchyMap;
    [JavaFlags(0)]
    public LoggerRepository genericHierarchy;
    [JavaFlags(0)]
    public File dir;

    public static void main(string[] argv)
    {
      // ISSUE: unable to decompile the method.
    }

    [JavaFlags(8)]
    public static void usage(string msg)
    {
      ((PrintStream) java.lang.System.err).println(msg);
      ((PrintStream) java.lang.System.err).println(new StringBuffer().append("Usage: java ").append(Class.FromType(typeof (SocketServer)).getName()).append(" port configFile directory").ToString());
      java.lang.System.exit(1);
    }

    [JavaFlags(8)]
    public static void init(string portStr, string configFile, string dirStr)
    {
      // ISSUE: unable to decompile the method.
    }

    public SocketServer(File directory)
    {
      this.dir = directory;
      this.hierarchyMap = new Hashtable(11);
    }

    [JavaFlags(0)]
    public virtual LoggerRepository configureHierarchy(InetAddress inetAddress)
    {
      SocketServer.cat.info((object) new StringBuffer().append("Locating configuration file for ").append((object) inetAddress).ToString());
      string str = inetAddress.ToString();
      int num = StringImpl.indexOf(str, "/");
      if (num == -1)
      {
        SocketServer.cat.warn((object) new StringBuffer().append("Could not parse the inetAddress [").append((object) inetAddress).append("]. Using default hierarchy.").ToString());
        return this.genericHierarchy();
      }
      File file = new File(this.dir, new StringBuffer().append(StringImpl.substring(str, 0, num)).append(SocketServer.CONFIG_FILE_EXT).ToString());
      if (file.exists())
      {
        Hierarchy hierarchy = new Hierarchy((Logger) new RootCategory((Level) Priority.DEBUG));
        this.hierarchyMap.put((object) inetAddress, (object) hierarchy);
        new PropertyConfigurator().doConfigure(file.getAbsolutePath(), (LoggerRepository) hierarchy);
        return (LoggerRepository) hierarchy;
      }
      SocketServer.cat.warn((object) new StringBuffer().append("Could not find config file [").append((object) file).append("].").ToString());
      return this.genericHierarchy();
    }

    [JavaFlags(0)]
    public virtual LoggerRepository genericHierarchy()
    {
      if (this.genericHierarchy == null)
      {
        File file = new File(this.dir, new StringBuffer().append(SocketServer.GENERIC).append(SocketServer.CONFIG_FILE_EXT).ToString());
        if (file.exists())
        {
          this.genericHierarchy = (LoggerRepository) new Hierarchy((Logger) new RootCategory((Level) Priority.DEBUG));
          new PropertyConfigurator().doConfigure(file.getAbsolutePath(), this.genericHierarchy);
        }
        else
        {
          SocketServer.cat.warn((object) new StringBuffer().append("Could not find config file [").append((object) file).append("]. Will use the default hierarchy.").ToString());
          this.genericHierarchy = LogManager.getLoggerRepository();
        }
      }
      return this.genericHierarchy;
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [JavaFlags(32778)]
    static SocketServer()
    {
      // ISSUE: unable to decompile the method.
    }

    [JavaFlags(4227077)]
    [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
    public new virtual object MemberwiseClone()
    {
      SocketServer socketServer = this;
      ObjectImpl.clone((object) socketServer);
      return ((object) socketServer).MemberwiseClone();
    }

    [JavaFlags(4227073)]
    public override string ToString() => ObjectImpl.jloToString((object) this);
  }
}
