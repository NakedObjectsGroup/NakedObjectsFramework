// Decompiled with JetBrains decompiler
// Type: org.apache.log4j.or.RendererMap
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using java.lang;
using java.util;
using org.apache.log4j.helpers;
using org.apache.log4j.spi;
using System.ComponentModel;

namespace org.apache.log4j.or
{
  public class RendererMap
  {
    [JavaFlags(0)]
    public Hashtable map;
    [JavaFlags(8)]
    public static ObjectRenderer defaultRenderer;

    public RendererMap() => this.map = new Hashtable();

    public static void addRenderer(
      RendererSupport repository,
      string renderedClassName,
      string renderingClassName)
    {
      LogLog.debug(new StringBuffer().append("Rendering class: [").append(renderingClassName).append("], Rendered class: [").append(renderedClassName).append("].").ToString());
      ObjectRenderer renderer = (ObjectRenderer) OptionConverter.instantiateByClassName(renderingClassName, Class.FromType(typeof (ObjectRenderer)), (object) null);
      if (renderer == null)
      {
        LogLog.error(new StringBuffer().append("Could not instantiate renderer [").append(renderingClassName).append("].").ToString());
      }
      else
      {
        try
        {
          Class renderedClass = Loader.loadClass(renderedClassName);
          repository.setRenderer(renderedClass, renderer);
        }
        catch (ClassNotFoundException ex)
        {
          LogLog.error(new StringBuffer().append("Could not find class [").append(renderedClassName).append("].").ToString(), (Throwable) ex);
        }
      }
    }

    public virtual string findAndRender(object o) => o == null ? (string) null : this.get(ObjectImpl.getClass(o)).doRender(o);

    public virtual ObjectRenderer get(object o) => o == null ? (ObjectRenderer) null : this.get(ObjectImpl.getClass(o));

    public virtual ObjectRenderer get(Class clazz)
    {
      for (Class c = clazz; c != null; c = c.getSuperclass())
      {
        ObjectRenderer objectRenderer1 = (ObjectRenderer) this.map.get((object) c);
        if (objectRenderer1 != null)
          return objectRenderer1;
        ObjectRenderer objectRenderer2 = this.searchInterfaces(c);
        if (objectRenderer2 != null)
          return objectRenderer2;
      }
      return RendererMap.defaultRenderer;
    }

    [JavaFlags(0)]
    public virtual ObjectRenderer searchInterfaces(Class c)
    {
      ObjectRenderer objectRenderer1 = (ObjectRenderer) this.map.get((object) c);
      if (objectRenderer1 != null)
        return objectRenderer1;
      foreach (Class c1 in c.getInterfaces())
      {
        ObjectRenderer objectRenderer2 = this.searchInterfaces(c1);
        if (objectRenderer2 != null)
          return objectRenderer2;
      }
      return (ObjectRenderer) null;
    }

    public virtual ObjectRenderer getDefaultRenderer() => RendererMap.defaultRenderer;

    public virtual void clear() => this.map.clear();

    public virtual void put(Class clazz, ObjectRenderer or) => this.map.put((object) clazz, (object) or);

    [JavaFlags(32778)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    static RendererMap()
    {
      // ISSUE: unable to decompile the method.
    }

    [JavaFlags(4227077)]
    [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
    public new virtual object MemberwiseClone()
    {
      RendererMap rendererMap = this;
      ObjectImpl.clone((object) rendererMap);
      return ((object) rendererMap).MemberwiseClone();
    }

    [JavaFlags(4227073)]
    public override string ToString() => ObjectImpl.jloToString((object) this);
  }
}
