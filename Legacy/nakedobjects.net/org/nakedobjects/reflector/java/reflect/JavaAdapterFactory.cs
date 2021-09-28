// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.reflector.java.reflect.JavaAdapterFactory
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using java.lang;
using java.util;
using org.apache.log4j;
using org.nakedobjects.@object;
using org.nakedobjects.application.collection;
using org.nakedobjects.application.value;
using org.nakedobjects.application.valueholder;
using org.nakedobjects.reflector.java.collection;
using org.nakedobjects.reflector.java.value;
using org.nakedobjects.reflector.java.valueholder;
using System;
using System.ComponentModel;

namespace org.nakedobjects.reflector.java.reflect
{
  [JavaInterfaces("1;org/nakedobjects/object/AdapterFactory;")]
  public class JavaAdapterFactory : AdapterFactory
  {
    private static readonly Logger LOG;

    public virtual NakedValue createValueAdapter(object @object)
    {
      switch (@object)
      {
        case MultilineTextString _:
          return (NakedValue) new MultilineTextStringAdapter((MultilineTextString) @object);
        case TextString _:
          return (NakedValue) new TextStringAdapter((TextString) @object);
        case Password _:
          return (NakedValue) new PasswordAdapter((Password) @object);
        case Logical _:
          return (NakedValue) new LogicalValueObjectAdapter((Logical) @object);
        case org.nakedobjects.application.valueholder.Color _:
          return (NakedValue) new ColorValueObjectAdapter((org.nakedobjects.application.valueholder.Color) @object);
        case org.nakedobjects.application.value.Date _:
          return (NakedValue) new DateValueAdapter((org.nakedobjects.application.value.Date) @object);
        case Image _:
          return (NakedValue) new ImageValueAdapter((Image) @object);
        case SimpleBusinessValue _:
          return (NakedValue) new SimpleBusinessValueAdapter((SimpleBusinessValue) @object);
        case BusinessValueHolder _:
          return (NakedValue) new BusinessValueAdapter((BusinessValueHolder) @object);
        default:
          return (NakedValue) null;
      }
    }

    public virtual NakedCollection createCollectionAdapter(
      object collection,
      NakedObjectSpecification specification)
    {
      switch (collection)
      {
        case Vector _:
          return (NakedCollection) new VectorCollectionAdapter((Vector) collection, specification);
        case InternalCollection _:
          return (NakedCollection) new InternalCollectionAdapter((InternalCollection) collection, specification);
        case object[] _:
          return (NakedCollection) new ArrayAdapter((object[]) collection, specification);
        default:
          return (NakedCollection) null;
      }
    }

    [JavaThrownExceptions("1;java/lang/Throwable;")]
    [JavaFlags(4)]
    public override void Finalize()
    {
      try
      {
        // ISSUE: explicit finalizer call
        base.Finalize();
        if (!JavaAdapterFactory.LOG.isInfoEnabled())
          return;
        JavaAdapterFactory.LOG.info((object) new StringBuffer().append("finalizing reflector factory ").append((object) this).ToString());
      }
      catch (Exception ex)
      {
      }
    }

    public virtual void init()
    {
    }

    public virtual void shutdown()
    {
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [JavaFlags(32778)]
    static JavaAdapterFactory()
    {
      // ISSUE: unable to decompile the method.
    }

    [JavaFlags(4227077)]
    [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
    public new virtual object MemberwiseClone()
    {
      JavaAdapterFactory javaAdapterFactory = this;
      ObjectImpl.clone((object) javaAdapterFactory);
      return ((object) javaAdapterFactory).MemberwiseClone();
    }

    [JavaFlags(4227073)]
    public override string ToString() => ObjectImpl.jloToString((object) this);
  }
}
