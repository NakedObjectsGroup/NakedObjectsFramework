// Decompiled with JetBrains decompiler
// Type: org.apache.crimson.tree.Doctype
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using java.io;
using java.util;
using org.w3c.dom;

namespace org.apache.crimson.tree
{
  [JavaFlags(48)]
  [JavaInterfaces("1;org/w3c/dom/DocumentType;")]
  public sealed class Doctype : NodeBase, DocumentType
  {
    private string name;
    private Doctype.Nodemap entities;
    private Doctype.Nodemap notations;
    private string publicId;
    private string systemId;
    private string internalSubset;

    [JavaFlags(0)]
    public Doctype(string pub, string sys, string subset)
    {
      this.publicId = pub;
      this.systemId = sys;
      this.internalSubset = subset;
    }

    [JavaFlags(0)]
    public Doctype(string name, string publicId, string systemId, string internalSubset)
    {
      this.name = name;
      this.publicId = publicId;
      this.systemId = systemId;
      this.internalSubset = internalSubset;
      this.entities = new Doctype.Nodemap();
      this.notations = new Doctype.Nodemap();
    }

    [JavaFlags(0)]
    public virtual void setPrintInfo(string pub, string sys, string subset)
    {
      this.publicId = pub;
      this.systemId = sys;
      this.internalSubset = subset;
    }

    [JavaThrownExceptions("1;java/io/IOException;")]
    public override void writeXml(XmlWriteContext context)
    {
      Writer writer = context.getWriter();
      Element documentElement = this.getOwnerDocument().getDocumentElement();
      writer.write("<!DOCTYPE ");
      writer.write(documentElement != null ? documentElement.getNodeName() : "UNKNOWN-ROOT");
      if (this.systemId != null)
      {
        if (this.publicId != null)
        {
          writer.write(" PUBLIC '");
          writer.write(this.publicId);
          writer.write("' '");
        }
        else
          writer.write(" SYSTEM '");
        writer.write(this.systemId);
        writer.write("'");
      }
      if (this.internalSubset != null)
      {
        writer.write(XmlDocument.eol);
        writer.write("[");
        writer.write(this.internalSubset);
        writer.write("]");
      }
      writer.write(">");
      writer.write(XmlDocument.eol);
    }

    public override short getNodeType() => 10;

    public virtual string getName() => this.name;

    public override string getNodeName() => this.name;

    public override Node cloneNode(bool deep)
    {
      Doctype doctype = new Doctype(this.name, this.publicId, this.systemId, this.internalSubset);
      doctype.setOwnerDocument((XmlDocument) this.getOwnerDocument());
      return (Node) doctype;
    }

    public virtual NamedNodeMap getEntities() => (NamedNodeMap) this.entities;

    public virtual NamedNodeMap getNotations() => (NamedNodeMap) this.notations;

    public virtual string getPublicId() => this.publicId;

    public virtual string getSystemId() => this.systemId;

    public virtual string getInternalSubset() => this.internalSubset;

    [JavaFlags(4)]
    public override void setOwnerDocument(XmlDocument doc)
    {
      base.setOwnerDocument(doc);
      if (this.entities != null)
      {
        for (int index = 0; this.entities.item(index) != null; ++index)
          ((NodeBase) this.entities.item(index)).setOwnerDocument(doc);
      }
      if (this.notations == null)
        return;
      for (int index = 0; this.notations.item(index) != null; ++index)
        ((NodeBase) this.notations.item(index)).setOwnerDocument(doc);
    }

    [JavaFlags(0)]
    public virtual void addNotation(string name, string pub, string sys)
    {
      Doctype.NotationNode notationNode = new Doctype.NotationNode(name, pub, sys);
      notationNode.setOwnerDocument((XmlDocument) this.getOwnerDocument());
      this.notations.setNamedItem((Node) notationNode);
    }

    [JavaFlags(0)]
    public virtual void addEntityNode(string name, string pub, string sys, string not)
    {
      Doctype.EntityNode entityNode = new Doctype.EntityNode(name, pub, sys, not);
      entityNode.setOwnerDocument((XmlDocument) this.getOwnerDocument());
      this.entities.setNamedItem((Node) entityNode);
    }

    [JavaFlags(0)]
    public virtual void addEntityNode(string name, string value)
    {
      if (StringImpl.equals("lt", (object) name) || StringImpl.equals("gt", (object) name) || StringImpl.equals("apos", (object) name) || StringImpl.equals("quot", (object) name) || StringImpl.equals("amp", (object) name))
        return;
      Doctype.EntityNode entityNode = new Doctype.EntityNode(name, value);
      entityNode.setOwnerDocument((XmlDocument) this.getOwnerDocument());
      this.entities.setNamedItem((Node) entityNode);
    }

    [JavaFlags(0)]
    public virtual void setReadonly()
    {
      this.entities.@readonly = true;
      this.notations.@readonly = true;
    }

    [JavaInterfaces("1;org/w3c/dom/Notation;")]
    [JavaFlags(40)]
    public class NotationNode : NodeBase, Notation
    {
      private string notation;
      private string publicId;
      private string systemId;

      [JavaFlags(0)]
      public NotationNode(string name, string pub, string sys)
      {
        this.notation = name;
        this.publicId = pub;
        this.systemId = sys;
      }

      public virtual string getPublicId() => this.publicId;

      public virtual string getSystemId() => this.systemId;

      public override short getNodeType() => 12;

      public override string getNodeName() => this.notation;

      public override Node cloneNode(bool ignored)
      {
        Doctype.NotationNode notationNode = new Doctype.NotationNode(this.notation, this.publicId, this.systemId);
        notationNode.setOwnerDocument((XmlDocument) this.getOwnerDocument());
        return (Node) notationNode;
      }

      [JavaThrownExceptions("1;java/io/IOException;")]
      public override void writeXml(XmlWriteContext context)
      {
        Writer writer = context.getWriter();
        writer.write("<!NOTATION ");
        writer.write(this.notation);
        if (this.publicId != null)
        {
          writer.write(" PUBLIC '");
          writer.write(this.publicId);
          if (this.systemId != null)
          {
            writer.write("' '");
            writer.write(this.systemId);
          }
        }
        else
        {
          writer.write(" SYSTEM '");
          writer.write(this.systemId);
        }
        writer.write("'>");
      }
    }

    [JavaInterfaces("1;org/w3c/dom/Entity;")]
    [JavaFlags(40)]
    public class EntityNode : NodeBase, Entity
    {
      private string entityName;
      private string publicId;
      private string systemId;
      private string notation;
      private string value;

      [JavaFlags(0)]
      public EntityNode(string name, string pub, string sys, string not)
      {
        this.entityName = name;
        this.publicId = pub;
        this.systemId = sys;
        this.notation = not;
      }

      [JavaFlags(0)]
      public EntityNode(string name, string value)
      {
        this.entityName = name;
        this.value = value;
      }

      public override string getNodeName() => this.entityName;

      public override short getNodeType() => 6;

      public virtual string getPublicId() => this.publicId;

      public virtual string getSystemId() => this.systemId;

      public virtual string getNotationName() => this.notation;

      public override Node cloneNode(bool ignored)
      {
        Doctype.EntityNode entityNode = new Doctype.EntityNode(this.entityName, this.publicId, this.systemId, this.notation);
        entityNode.setOwnerDocument((XmlDocument) this.getOwnerDocument());
        return (Node) entityNode;
      }

      [JavaThrownExceptions("1;java/io/IOException;")]
      public override void writeXml(XmlWriteContext context)
      {
        Writer writer = context.getWriter();
        writer.write("<!ENTITY ");
        writer.write(this.entityName);
        if (this.value == null)
        {
          if (this.publicId != null)
          {
            writer.write(" PUBLIC '");
            writer.write(this.publicId);
            writer.write("' '");
          }
          else
            writer.write(" SYSTEM '");
          writer.write(this.systemId);
          writer.write("'");
          if (this.notation != null)
          {
            writer.write(" NDATA ");
            writer.write(this.notation);
          }
        }
        else
        {
          writer.write(" \"");
          int num = StringImpl.length(this.value);
          for (int index = 0; index < num; ++index)
          {
            char ch = StringImpl.charAt(this.value, index);
            if (ch == '"')
              writer.write("&quot;");
            else
              writer.write((int) ch);
          }
          writer.write(34);
        }
        writer.write(">");
      }
    }

    [JavaInterfaces("1;org/w3c/dom/NamedNodeMap;")]
    [JavaFlags(40)]
    public class Nodemap : NamedNodeMap
    {
      [JavaFlags(0)]
      public bool @readonly;
      [JavaFlags(0)]
      public Vector list;

      public virtual Node getNamedItem(string name)
      {
        int num = this.list.size();
        for (int index = 0; index < num; ++index)
        {
          Node node = this.item(index);
          if (StringImpl.equals(node.getNodeName(), (object) name))
            return node;
        }
        return (Node) null;
      }

      public virtual Node getNamedItemNS(string namespaceURI, string localName) => (Node) null;

      public virtual int getLength() => this.list.size();

      public virtual Node item(int index) => index < 0 || index >= this.list.size() ? (Node) null : (Node) this.list.elementAt(index);

      [JavaThrownExceptions("1;org/w3c/dom/DOMException;")]
      public virtual Node removeNamedItem(string name) => throw new DomEx((short) 7);

      [JavaThrownExceptions("1;org/w3c/dom/DOMException;")]
      public virtual Node removeNamedItemNS(string namespaceURI, string localName) => throw new DomEx((short) 7);

      [JavaThrownExceptions("1;org/w3c/dom/DOMException;")]
      public virtual Node setNamedItem(Node item)
      {
        if (this.@readonly)
          throw new DomEx((short) 7);
        this.list.addElement((object) item);
        return (Node) null;
      }

      [JavaThrownExceptions("1;org/w3c/dom/DOMException;")]
      public virtual Node setNamedItemNS(Node arg)
      {
        if (this.@readonly)
          throw new DomEx((short) 7);
        this.list.addElement((object) arg);
        return (Node) null;
      }

      [JavaFlags(0)]
      public Nodemap() => this.list = new Vector();

      [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
      [JavaFlags(4227077)]
      public new virtual object MemberwiseClone()
      {
        Doctype.Nodemap nodemap = this;
        ObjectImpl.clone((object) nodemap);
        return ((object) nodemap).MemberwiseClone();
      }

      [JavaFlags(4227073)]
      public override string ToString() => ObjectImpl.jloToString((object) this);
    }
  }
}
