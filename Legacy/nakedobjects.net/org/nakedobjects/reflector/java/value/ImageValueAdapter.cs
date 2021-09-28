// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.reflector.java.value.ImageValueAdapter
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using java.lang;
using org.nakedobjects.@object.value;
using org.nakedobjects.@object.value.adapter;
using org.nakedobjects.application.value;
using org.nakedobjects.utility;

namespace org.nakedobjects.reflector.java.value
{
  [JavaInterfaces("1;org/nakedobjects/object/value/ImageValue;")]
  public class ImageValueAdapter : AbstractNakedValue, ImageValue
  {
    private readonly Image image;

    public ImageValueAdapter(Image image) => this.image = image;

    public override string getValueClass() => Class.FromType(typeof (Image)).getName();

    public override sbyte[] asEncodedString() => throw new NotImplementedException();

    [JavaThrownExceptions("1;org/nakedobjects/object/InvalidEntryException;")]
    public override void parseTextEntry(string text) => throw new UnexpectedCallException();

    public override void restoreFromEncodedString(sbyte[] data) => throw new NotImplementedException();

    public override string getIconName() => (string) null;

    public override object getObject() => (object) this.image;

    public override string titleString() => "image";

    public virtual int[][] getImage() => this.image.getImage();

    public virtual void setImage(int[][] pixels) => this.image.setImage(pixels);
  }
}
