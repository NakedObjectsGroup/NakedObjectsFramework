// Decompiled with JetBrains decompiler
// Type: org.apache.crimson.tree.SimpleElementFactory
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using java.lang;
using java.util;

namespace org.apache.crimson.tree
{
  [JavaInterfaces("1;org/apache/crimson/tree/ElementFactory;")]
  public class SimpleElementFactory : ElementFactory
  {
    private Dictionary defaultMapping;
    private ClassLoader defaultLoader;
    private string defaultNs;
    private Dictionary nsMappings;
    private Dictionary nsLoaders;
    private Locale locale;

    public SimpleElementFactory() => this.locale = Locale.getDefault();

    public virtual void addMapping(Dictionary dict, ClassLoader loader)
    {
      this.defaultMapping = dict != null ? dict : throw new IllegalArgumentException();
      this.defaultLoader = loader;
    }

    public virtual void addMapping(string @namespace, Dictionary dict, ClassLoader loader)
    {
      if (@namespace == null || dict == null)
        throw new IllegalArgumentException();
      if (this.nsMappings == null)
      {
        this.nsMappings = (Dictionary) new Hashtable();
        this.nsLoaders = (Dictionary) new Hashtable();
      }
      this.nsMappings.put((object) @namespace, (object) dict);
      if (loader == null)
        return;
      this.nsLoaders.put((object) @namespace, (object) loader);
    }

    public virtual void setDefaultNamespace(string ns) => this.defaultNs = ns;

    private Class map2Class(string key, Dictionary node2class, ClassLoader loader)
    {
      object obj = node2class.get((object) key);
      if (obj is Class)
        return (Class) obj;
      if (obj == null)
        return (Class) null;
      if (\u003CVerifierFix\u003E.isInstanceOfString(obj))
      {
        string str = \u003CVerifierFix\u003E.genCastToString(obj);
        try
        {
          if (loader == null)
            loader = SimpleElementFactory.findClassLoader();
          Class @class = loader != null ? loader.loadClass(str) : Class.forName(str);
          if (!Class.FromType(typeof (ElementNode)).isAssignableFrom(@class))
          {
            int length = 2;
            object[] parameters = length >= 0 ? new object[length] : throw new NegativeArraySizeException();
            parameters[0] = (object) key;
            parameters[1] = (object) str;
            throw new IllegalArgumentException(this.getMessage("SEF-000", parameters));
          }
          node2class.put((object) key, (object) @class);
          return @class;
        }
        catch (ClassNotFoundException ex)
        {
          int length = 3;
          object[] parameters = length >= 0 ? new object[length] : throw new NegativeArraySizeException();
          parameters[0] = (object) key;
          parameters[1] = (object) str;
          parameters[2] = (object) ((Throwable) ex).getMessage();
          throw new IllegalArgumentException(this.getMessage("SEF-001", parameters));
        }
      }
      else
      {
        int length = 1;
        object[] parameters = length >= 0 ? new object[length] : throw new NegativeArraySizeException();
        parameters[0] = (object) key;
        throw new IllegalArgumentException(this.getMessage("SEF-002", parameters));
      }
    }

    private ElementNode doMap(string tagName, Dictionary node2class, ClassLoader loader)
    {
      // ISSUE: unable to decompile the method.
    }

    public virtual ElementEx createElementEx(string @namespace, string localName)
    {
      Dictionary node2class = (Dictionary) null;
      if (@namespace == null)
        @namespace = this.defaultNs;
      if (this.nsMappings != null)
        node2class = (Dictionary) this.nsMappings.get((object) @namespace);
      return node2class == null ? (ElementEx) this.doMap(localName, this.defaultMapping, this.defaultLoader) : (ElementEx) this.doMap(localName, node2class, (ClassLoader) this.nsLoaders.get((object) @namespace));
    }

    public virtual ElementEx createElementEx(string tag) => (ElementEx) this.doMap(tag, this.defaultMapping, this.defaultLoader);

    [JavaFlags(0)]
    public virtual string getMessage(string messageId) => this.getMessage(messageId, (object[]) null);

    [JavaFlags(0)]
    public virtual string getMessage(string messageId, object[] parameters) => XmlDocument.catalog.getMessage(this.locale, messageId, parameters);

    private static ClassLoader findClassLoader() => Class.FromType(typeof (SimpleElementFactory)).getClassLoader();

    [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
    [JavaFlags(4227077)]
    public new virtual object MemberwiseClone()
    {
      SimpleElementFactory simpleElementFactory = this;
      ObjectImpl.clone((object) simpleElementFactory);
      return ((object) simpleElementFactory).MemberwiseClone();
    }

    [JavaFlags(4227073)]
    public override string ToString() => ObjectImpl.jloToString((object) this);
  }
}
