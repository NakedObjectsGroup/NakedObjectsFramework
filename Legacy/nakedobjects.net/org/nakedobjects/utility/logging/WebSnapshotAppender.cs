// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.utility.logging.WebSnapshotAppender
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using java.io;
using java.lang;
using java.net;
using org.apache.log4j.spi;
using System.ComponentModel;

namespace org.nakedobjects.utility.logging
{
  public class WebSnapshotAppender : SnapshotAppender
  {
    private static readonly org.apache.log4j.Logger LOG;
    private string proxyAddress;
    private int proxyPort;
    private string url_spec;

    public WebSnapshotAppender()
      : this((TriggeringEventEvaluator) new DefaultEvaluator())
    {
    }

    public WebSnapshotAppender(TriggeringEventEvaluator evaluator)
      : base(evaluator)
    {
      this.proxyPort = -1;
      this.url_spec = "http://development.nakedobjects.net/errors/log.php";
    }

    public virtual string getProxyAddress() => this.proxyAddress;

    public virtual int getProxyPort() => this.proxyPort;

    public virtual void setProxyAddress(string proxyAddess) => this.proxyAddress = proxyAddess;

    public virtual void setProxyPort(int proxyPort) => this.proxyPort = proxyPort;

    public virtual void setUrl(string url) => this.url_spec = url;

    [JavaFlags(4)]
    public override void writeSnapshot(string message, string details)
    {
      try
      {
        URL url = this.proxyAddress != null ? new URL("http", this.proxyAddress, this.proxyPort, this.url_spec) : new URL(this.url_spec);
        if (WebSnapshotAppender.LOG.isInfoEnabled())
          WebSnapshotAppender.LOG.info((object) new StringBuffer().append("connect to ").append((object) url).ToString());
        URLConnection urlConnection = url.openConnection();
        urlConnection.setDoOutput(true);
        WebSnapshotAppender.HttpQueryWriter httpQueryWriter = new WebSnapshotAppender.HttpQueryWriter(urlConnection.getOutputStream());
        httpQueryWriter.addParameter("error", message);
        httpQueryWriter.addParameter("trace", details);
        httpQueryWriter.close();
        InputStream inputStream = urlConnection.getInputStream();
        StringBuffer stringBuffer = new StringBuffer();
        int num;
        while ((num = inputStream.read()) != -1)
          stringBuffer.append((char) num);
        if (WebSnapshotAppender.LOG.isInfoEnabled())
          WebSnapshotAppender.LOG.info((object) stringBuffer);
        inputStream.close();
      }
      catch (UnknownHostException ex)
      {
        if (!WebSnapshotAppender.LOG.isInfoEnabled())
          return;
        WebSnapshotAppender.LOG.info((object) "could not find host (unknown host) to submit log to");
      }
      catch (IOException ex)
      {
        if (!WebSnapshotAppender.LOG.isDebugEnabled())
          return;
        WebSnapshotAppender.LOG.debug((object) "i/o problem submitting log", (Throwable) ex);
      }
    }

    [JavaFlags(32778)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    static WebSnapshotAppender()
    {
      // ISSUE: unable to decompile the method.
    }

    [JavaFlags(42)]
    private class HttpQueryWriter : OutputStreamWriter
    {
      private int parameter;

      [JavaThrownExceptions("1;java/io/UnsupportedEncodingException;")]
      public HttpQueryWriter(OutputStream outputStream)
        : base(outputStream, "ASCII")
      {
        this.parameter = 1;
      }

      [JavaThrownExceptions("1;java/io/IOException;")]
      public virtual void addParameter(string name, string value)
      {
        if (name == null || value == null)
          return;
        if (this.parameter > 1)
          ((Writer) this).write("&");
        ++this.parameter;
        ((Writer) this).write(URLEncoder.encode(name));
        ((Writer) this).write("=");
        ((Writer) this).write(URLEncoder.encode(value));
      }

      [JavaThrownExceptions("1;java/io/IOException;")]
      public virtual void close()
      {
        ((Writer) this).write("\r\n");
        this.flush();
        base.close();
      }

      [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
      [JavaFlags(4227077)]
      public virtual object MemberwiseClone()
      {
        WebSnapshotAppender.HttpQueryWriter httpQueryWriter = this;
        ObjectImpl.clone((object) httpQueryWriter);
        return ((object) httpQueryWriter).MemberwiseClone();
      }

      [JavaFlags(4227073)]
      public virtual string ToString() => ObjectImpl.jloToString((object) this);
    }
  }
}
