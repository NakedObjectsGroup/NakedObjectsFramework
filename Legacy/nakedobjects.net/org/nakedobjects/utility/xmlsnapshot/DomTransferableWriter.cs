// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.utility.xmlsnapshot.DomTransferableWriter
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using java.lang;
using org.nakedobjects.@object.io;
using org.w3c.dom;

namespace org.nakedobjects.utility.xmlsnapshot
{
  [JavaInterfaces("1;org/nakedobjects/object/io/TransferableWriter;")]
  [JavaFlags(48)]
  public sealed class DomTransferableWriter : TransferableWriter
  {
    private readonly Element parentElement;
    private readonly NofMetaModel nofMetaModel;
    private readonly Helper helper;
    private readonly Element oidElement;

    [JavaFlags(0)]
    public DomTransferableWriter(
      Element parentElement,
      NofMetaModel nofMetaModel,
      string fullyQualifiedClassName)
    {
      this.helper = new Helper();
      this.nofMetaModel = nofMetaModel;
      this.parentElement = parentElement;
      this.oidElement = nofMetaModel.appendElement(parentElement, "oid");
      if (fullyQualifiedClassName == null)
        return;
      this.oidElement.setAttributeNS("http://www.nakedobjects.org/ns/0.1/metamodel", "nof:fqn", fullyQualifiedClassName);
    }

    private DomTransferableWriter(Element parentElement, NofMetaModel nofMetaModel)
      : this(parentElement, nofMetaModel, (string) null)
    {
    }

    public virtual void writeInt(int i) => this.appendElement("int", new StringBuffer().append("").append(i).ToString());

    public virtual void writeString(string str) => this.appendElement("string", str);

    public virtual void writeLong(long l) => this.appendElement("long", new StringBuffer().append("").append(l).ToString());

    private void appendElement(string elementName, string value)
    {
      Element element = this.nofMetaModel.appendElement(this.oidElement, elementName);
      element.appendChild((Node) this.helper.docFor(element).createTextNode(value));
    }

    public virtual void writeObject(Transferable t)
    {
      DomTransferableWriter transferableWriter = new DomTransferableWriter(this.oidElement, this.nofMetaModel);
      t.writeData((TransferableWriter) transferableWriter);
      transferableWriter.close();
    }

    public virtual void close()
    {
    }

    [JavaFlags(4227077)]
    [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
    public new virtual object MemberwiseClone()
    {
      DomTransferableWriter transferableWriter = this;
      ObjectImpl.clone((object) transferableWriter);
      return ((object) transferableWriter).MemberwiseClone();
    }

    [JavaFlags(4227073)]
    public override string ToString() => ObjectImpl.jloToString((object) this);
  }
}
