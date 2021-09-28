// Decompiled with JetBrains decompiler
// Type: org.xml.sax.helpers.ParserFactory
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using java.lang;
using System;

namespace org.xml.sax.helpers
{
  [Obsolete(null, false)]
  public class ParserFactory
  {
    private ParserFactory()
    {
    }

    [JavaThrownExceptions("5;java/lang/ClassNotFoundException;java/lang/IllegalAccessException;java/lang/InstantiationException;java/lang/NullPointerException;java/lang/ClassCastException;")]
    public static Parser makeParser() => ParserFactory.makeParser(java.lang.System.getProperty("org.xml.sax.parser") ?? throw new NullPointerException("No value for sax.parser property"));

    [JavaThrownExceptions("4;java/lang/ClassNotFoundException;java/lang/IllegalAccessException;java/lang/InstantiationException;java/lang/ClassCastException;")]
    public static Parser makeParser(string className) => (Parser) Class.forName(className).newInstance();

    [JavaFlags(4227077)]
    [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
    public new virtual object MemberwiseClone()
    {
      ParserFactory parserFactory = this;
      ObjectImpl.clone((object) parserFactory);
      return ((object) parserFactory).MemberwiseClone();
    }

    [JavaFlags(4227073)]
    public override string ToString() => ObjectImpl.jloToString((object) this);
  }
}
