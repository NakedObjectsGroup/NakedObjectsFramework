// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.application.value.Image
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using java.lang;

namespace org.nakedobjects.application.value
{
  [JavaInterfaces("1;org/nakedobjects/application/value/SimpleBusinessValue;")]
  public class Image : SimpleBusinessValue
  {
    private int[][] image;

    public Image(int[][] image) => this.image = image;

    public Image()
    {
      int length1 = 0;
      int length2 = 0;
      this.image = length1 >= 0 && length2 >= 0 ? new int[length2, length1][] : throw new NegativeArraySizeException();
    }

    public virtual object getValue() => (object) this.image;

    public virtual void parseUserEntry(string text)
    {
    }

    public virtual void restoreFromEncodedString(string data) => throw new ApplicationException();

    public virtual string asEncodedString() => throw new ApplicationException();

    public override string ToString() => new StringBuffer().append("Image [size=").append(this.image.Length).append("x").append(this.image.Length == 0 || this.image[0] == null ? 0 : this.image[0].Length).append("]").ToString();

    public virtual bool userChangeable() => false;

    public virtual int[][] getImage() => this.image;

    public virtual void setImage(int[][] image) => this.image = image;

    [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
    [JavaFlags(4227077)]
    public new virtual object MemberwiseClone()
    {
      Image image = this;
      ObjectImpl.clone((object) image);
      return ((object) image).MemberwiseClone();
    }
  }
}
