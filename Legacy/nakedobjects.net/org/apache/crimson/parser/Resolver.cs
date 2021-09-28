// Decompiled with JetBrains decompiler
// Type: org.apache.crimson.parser.Resolver
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using java.io;
using java.lang;
using java.net;
using java.util;
using org.xml.sax;
using System.ComponentModel;

namespace org.apache.crimson.parser
{
  [JavaInterfaces("1;org/xml/sax/EntityResolver;")]
  public class Resolver : EntityResolver
  {
    private bool ignoringMIME;
    private Hashtable id2uri;
    private Hashtable id2resource;
    private Hashtable id2loader;
    private static readonly string[] types;

    [JavaThrownExceptions("1;java/io/IOException;")]
    public static InputSource createInputSource(
      string contentType,
      InputStream stream,
      bool checkType,
      string scheme)
    {
      string encoding = (string) null;
      if (contentType != null)
      {
        contentType = StringImpl.toLowerCase(contentType);
        int num1 = StringImpl.indexOf(contentType, 59);
        if (num1 != -1)
        {
          string str1 = StringImpl.substring(contentType, num1 + 1);
          contentType = StringImpl.substring(contentType, 0, num1);
          int num2 = StringImpl.indexOf(str1, "charset");
          if (num2 != -1)
          {
            string str2 = StringImpl.substring(str1, num2 + 7);
            int num3;
            if ((num3 = StringImpl.indexOf(str2, 59)) != -1)
              str2 = StringImpl.substring(str2, 0, num3);
            int num4;
            if ((num4 = StringImpl.indexOf(str2, 61)) != -1)
            {
              string str3 = StringImpl.substring(str2, num4 + 1);
              int num5;
              if ((num5 = StringImpl.indexOf(str3, 40)) != -1)
                str3 = StringImpl.substring(str3, 0, num5);
              int num6;
              if ((num6 = StringImpl.indexOf(str3, 34)) != -1)
              {
                string str4 = StringImpl.substring(str3, num6 + 1);
                str3 = StringImpl.substring(str4, 0, StringImpl.indexOf(str4, 34));
              }
              encoding = StringImpl.trim(str3);
            }
          }
        }
        if (checkType)
        {
          bool flag = false;
          for (int index = 0; index < Resolver.types.Length; ++index)
          {
            if (StringImpl.equals(Resolver.types[index], (object) contentType))
            {
              flag = true;
              break;
            }
          }
          if (!flag)
            throw new IOException(new StringBuffer().append("Not XML: ").append(contentType).ToString());
        }
        if (encoding == null)
        {
          contentType = StringImpl.trim(contentType);
          if (StringImpl.startsWith(contentType, "text/") && !StringImpl.equalsIgnoreCase("file", scheme))
            encoding = "US-ASCII";
        }
      }
      InputSource inputSource = new InputSource(XmlReader.createReader(stream, encoding));
      inputSource.setByteStream(stream);
      inputSource.setEncoding(encoding);
      return inputSource;
    }

    [JavaThrownExceptions("1;java/io/IOException;")]
    public static InputSource createInputSource(URL uri, bool checkType)
    {
      URLConnection urlConnection = uri.openConnection();
      if (urlConnection is HttpURLConnection)
      {
        int responseCode = ((HttpURLConnection) urlConnection).getResponseCode();
        if (responseCode >= 400 && responseCode <= 417 || responseCode >= 500 && responseCode <= 505)
          throw new IOException(new StringBuffer().append("Error in opening uri ").append((object) uri).append("status code=").append(responseCode).ToString());
      }
      InputSource inputSource = !checkType ? new InputSource(XmlReader.createReader(urlConnection.getInputStream())) : Resolver.createInputSource(urlConnection.getContentType(), urlConnection.getInputStream(), false, uri.getProtocol());
      inputSource.setSystemId(urlConnection.getURL().ToString());
      return inputSource;
    }

    [JavaThrownExceptions("1;java/io/IOException;")]
    public static InputSource createInputSource(File file)
    {
      InputSource inputSource = new InputSource(XmlReader.createReader((InputStream) new FileInputStream(file)));
      string str = file.getAbsolutePath();
      if ((int) File.separatorChar != 47)
        str = StringImpl.replace(str, (char) File.separatorChar, '/');
      if (!StringImpl.startsWith(str, "/"))
        str = new StringBuffer().append("/").append(str).ToString();
      if (!StringImpl.endsWith(str, "/") && file.isDirectory())
        str = new StringBuffer().append(str).append("/").ToString();
      inputSource.setSystemId(new StringBuffer().append("file:").append(str).ToString());
      return inputSource;
    }

    [JavaThrownExceptions("2;java/io/IOException;org/xml/sax/SAXException;")]
    public virtual InputSource resolveEntity(string name, string uri)
    {
      string str = this.name2uri(name);
      InputStream @in;
      InputSource inputSource;
      if (str == null && (@in = this.mapResource(name)) != null)
      {
        uri = new StringBuffer().append("java:resource:").append(\u003CVerifierFix\u003E.genCastToString(this.id2resource.get((object) name))).ToString();
        inputSource = new InputSource(XmlReader.createReader(@in));
      }
      else
      {
        if (str != null)
          uri = str;
        else if (uri == null)
          return (InputSource) null;
        URL url = new URL(uri);
        URLConnection urlConnection = url.openConnection();
        uri = urlConnection.getURL().ToString();
        inputSource = !this.ignoringMIME ? Resolver.createInputSource(urlConnection.getContentType(), urlConnection.getInputStream(), false, url.getProtocol()) : new InputSource(XmlReader.createReader(urlConnection.getInputStream()));
      }
      inputSource.setSystemId(uri);
      inputSource.setPublicId(name);
      return inputSource;
    }

    public virtual bool isIgnoringMIME() => this.ignoringMIME;

    public virtual void setIgnoringMIME(bool value) => this.ignoringMIME = value;

    private string name2uri(string publicId) => publicId == null || this.id2uri == null ? (string) null : \u003CVerifierFix\u003E.genCastToString(this.id2uri.get((object) publicId));

    public virtual void registerCatalogEntry(string publicId, string uri)
    {
      if (this.id2uri == null)
        this.id2uri = new Hashtable(17);
      this.id2uri.put((object) publicId, (object) uri);
    }

    private InputStream mapResource(string publicId)
    {
      if (publicId == null || this.id2resource == null)
        return (InputStream) null;
      string str = \u003CVerifierFix\u003E.genCastToString(this.id2resource.get((object) publicId));
      ClassLoader classLoader = (ClassLoader) null;
      if (str == null)
        return (InputStream) null;
      if (this.id2loader != null)
        classLoader = (ClassLoader) this.id2loader.get((object) publicId);
      return classLoader == null ? ClassLoader.getSystemResourceAsStream(str) : classLoader.getResourceAsStream(str);
    }

    public virtual void registerCatalogEntry(
      string publicId,
      string resourceName,
      ClassLoader loader)
    {
      if (this.id2resource == null)
        this.id2resource = new Hashtable(17);
      this.id2resource.put((object) publicId, (object) resourceName);
      if (loader == null)
        return;
      if (this.id2loader == null)
        this.id2loader = new Hashtable(17);
      this.id2loader.put((object) publicId, (object) loader);
    }

    [JavaFlags(32778)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    static Resolver()
    {
      // ISSUE: unable to decompile the method.
    }

    [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
    [JavaFlags(4227077)]
    public new virtual object MemberwiseClone()
    {
      Resolver resolver = this;
      ObjectImpl.clone((object) resolver);
      return ((object) resolver).MemberwiseClone();
    }

    [JavaFlags(4227073)]
    public override string ToString() => ObjectImpl.jloToString((object) this);
  }
}
