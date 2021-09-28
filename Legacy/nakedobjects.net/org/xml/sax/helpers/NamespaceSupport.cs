// Decompiled with JetBrains decompiler
// Type: org.xml.sax.helpers.NamespaceSupport
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using java.lang;
using java.util;
using System.ComponentModel;

namespace org.xml.sax.helpers
{
  public class NamespaceSupport
  {
    public const string XMLNS = "http://www.w3.org/XML/1998/namespace";
    private static readonly Enumeration EMPTY_ENUMERATION;
    private NamespaceSupport.Context[] contexts;
    private NamespaceSupport.Context currentContext;
    private int contextPos;

    public NamespaceSupport() => this.reset();

    public virtual void reset()
    {
      int length = 32;
      this.contexts = length >= 0 ? new NamespaceSupport.Context[length] : throw new NegativeArraySizeException();
      this.contextPos = 0;
      this.contexts[this.contextPos] = this.currentContext = new NamespaceSupport.Context(this);
      this.currentContext.declarePrefix("xml", "http://www.w3.org/XML/1998/namespace");
    }

    public virtual void pushContext()
    {
      int length1 = this.contexts.Length;
      ++this.contextPos;
      if (this.contextPos >= length1)
      {
        int length2 = length1 * 2;
        NamespaceSupport.Context[] contextArray = length2 >= 0 ? new NamespaceSupport.Context[length2] : throw new NegativeArraySizeException();
        java.lang.System.arraycopy((object) this.contexts, 0, (object) contextArray, 0, length1);
        int num = length1 * 2;
        this.contexts = contextArray;
      }
      this.currentContext = this.contexts[this.contextPos];
      if (this.currentContext == null)
        this.contexts[this.contextPos] = this.currentContext = new NamespaceSupport.Context(this);
      if (this.contextPos <= 0)
        return;
      this.currentContext.setParent(this.contexts[this.contextPos - 1]);
    }

    public virtual void popContext()
    {
      this.contextPos += -1;
      this.currentContext = this.contextPos >= 0 ? this.contexts[this.contextPos] : throw new EmptyStackException();
    }

    public virtual bool declarePrefix(string prefix, string uri)
    {
      if (StringImpl.equals(prefix, (object) "xml") || StringImpl.equals(prefix, (object) "xmlns"))
        return false;
      this.currentContext.declarePrefix(prefix, uri);
      return true;
    }

    public virtual string[] processName(string qName, string[] parts, bool isAttribute)
    {
      string[] strArray = this.currentContext.processName(qName, isAttribute);
      if (strArray == null)
        return (string[]) null;
      parts[0] = strArray[0];
      parts[1] = strArray[1];
      parts[2] = strArray[2];
      return parts;
    }

    public virtual string getURI(string prefix) => this.currentContext.getURI(prefix);

    public virtual Enumeration getPrefixes() => this.currentContext.getPrefixes();

    public virtual string getPrefix(string uri) => this.currentContext.getPrefix(uri);

    public virtual Enumeration getPrefixes(string uri)
    {
      Vector vector = new Vector();
      Enumeration prefixes = this.getPrefixes();
      while (prefixes.hasMoreElements())
      {
        string prefix = \u003CVerifierFix\u003E.genCastToString(prefixes.nextElement());
        if (StringImpl.equals(uri, (object) this.getURI(prefix)))
          vector.addElement((object) prefix);
      }
      return vector.elements();
    }

    public virtual Enumeration getDeclaredPrefixes() => this.currentContext.getDeclaredPrefixes();

    [JavaFlags(32778)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    static NamespaceSupport()
    {
      // ISSUE: unable to decompile the method.
    }

    [JavaFlags(4227077)]
    [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
    public new virtual object MemberwiseClone()
    {
      NamespaceSupport namespaceSupport = this;
      ObjectImpl.clone((object) namespaceSupport);
      return ((object) namespaceSupport).MemberwiseClone();
    }

    [JavaFlags(4227073)]
    public override string ToString() => ObjectImpl.jloToString((object) this);

    [Inner]
    [JavaFlags(48)]
    public sealed class Context
    {
      [JavaFlags(0)]
      public Hashtable prefixTable;
      [JavaFlags(0)]
      public Hashtable uriTable;
      [JavaFlags(0)]
      public Hashtable elementNameTable;
      [JavaFlags(0)]
      public Hashtable attributeNameTable;
      [JavaFlags(0)]
      public string defaultNS;
      private Vector declarations;
      private bool tablesDirty;
      private NamespaceSupport.Context parent;
      [EditorBrowsable(EditorBrowsableState.Never)]
      [JavaFlags(32770)]
      private NamespaceSupport this\u00240;

      [JavaFlags(0)]
      public Context(NamespaceSupport _param1)
      {
        this.this\u00240 = _param1;
        if (_param1 == null)
          ObjectImpl.getClass((object) _param1);
        this.defaultNS = (string) null;
        this.declarations = (Vector) null;
        this.tablesDirty = false;
        this.parent = (NamespaceSupport.Context) null;
        this.copyTables();
      }

      [JavaFlags(0)]
      public virtual void setParent(NamespaceSupport.Context parent)
      {
        this.parent = parent;
        this.declarations = (Vector) null;
        this.prefixTable = parent.prefixTable;
        this.uriTable = parent.uriTable;
        this.elementNameTable = parent.elementNameTable;
        this.attributeNameTable = parent.attributeNameTable;
        this.defaultNS = parent.defaultNS;
        this.tablesDirty = false;
      }

      [JavaFlags(0)]
      public virtual void declarePrefix(string prefix, string uri)
      {
        if (!this.tablesDirty)
          this.copyTables();
        if (this.declarations == null)
          this.declarations = new Vector();
        prefix = StringImpl.intern(prefix);
        uri = StringImpl.intern(uri);
        if (StringImpl.equals("", (object) prefix))
        {
          this.defaultNS = !StringImpl.equals("", (object) uri) ? uri : (string) null;
        }
        else
        {
          this.prefixTable.put((object) prefix, (object) uri);
          this.uriTable.put((object) uri, (object) prefix);
        }
        this.declarations.addElement((object) prefix);
      }

      [JavaFlags(0)]
      public virtual string[] processName(string qName, bool isAttribute)
      {
        Hashtable hashtable = !isAttribute ? this.attributeNameTable : this.elementNameTable;
        string[] strArray1 = (string[]) hashtable.get((object) qName);
        if (strArray1 != null)
          return strArray1;
        int length = 3;
        string[] strArray2 = length >= 0 ? new string[length] : throw new NegativeArraySizeException();
        int num = StringImpl.indexOf(qName, 58);
        if (num == -1)
        {
          strArray2[0] = isAttribute || this.defaultNS == null ? "" : this.defaultNS;
          strArray2[1] = StringImpl.intern(qName);
          strArray2[2] = strArray2[1];
        }
        else
        {
          string str1 = StringImpl.substring(qName, 0, num);
          string str2 = StringImpl.substring(qName, num + 1);
          string str3 = !StringImpl.equals("", (object) str1) ? \u003CVerifierFix\u003E.genCastToString(this.prefixTable.get((object) str1)) : this.defaultNS;
          if (str3 == null)
            return (string[]) null;
          strArray2[0] = str3;
          strArray2[1] = StringImpl.intern(str2);
          strArray2[2] = StringImpl.intern(qName);
        }
        hashtable.put((object) strArray2[2], (object) strArray2);
        this.tablesDirty = true;
        return strArray2;
      }

      [JavaFlags(0)]
      public virtual string getURI(string prefix)
      {
        if (StringImpl.equals("", (object) prefix))
          return this.defaultNS;
        return this.prefixTable == null ? (string) null : \u003CVerifierFix\u003E.genCastToString(this.prefixTable.get((object) prefix));
      }

      [JavaFlags(0)]
      public virtual string getPrefix(string uri) => this.uriTable == null ? (string) null : \u003CVerifierFix\u003E.genCastToString(this.uriTable.get((object) uri));

      [JavaFlags(0)]
      public virtual Enumeration getDeclaredPrefixes() => this.declarations == null ? NamespaceSupport.EMPTY_ENUMERATION : this.declarations.elements();

      [JavaFlags(0)]
      public virtual Enumeration getPrefixes() => this.prefixTable == null ? NamespaceSupport.EMPTY_ENUMERATION : this.prefixTable.keys();

      private void copyTables()
      {
        this.prefixTable = this.prefixTable == null ? new Hashtable() : (Hashtable) this.prefixTable.MemberwiseClone();
        this.uriTable = this.uriTable == null ? new Hashtable() : (Hashtable) this.uriTable.MemberwiseClone();
        this.elementNameTable = new Hashtable();
        this.attributeNameTable = new Hashtable();
        this.tablesDirty = true;
      }

      [JavaFlags(4227077)]
      [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
      public new virtual object MemberwiseClone()
      {
        NamespaceSupport.Context context = this;
        ObjectImpl.clone((object) context);
        return ((object) context).MemberwiseClone();
      }

      [JavaFlags(4227073)]
      public override string ToString() => ObjectImpl.jloToString((object) this);
    }
  }
}
