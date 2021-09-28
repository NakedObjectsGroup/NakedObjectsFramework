// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.viewer.skylark.Shape
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using java.lang;

namespace org.nakedobjects.viewer.skylark
{
  public class Shape
  {
    [JavaFlags(0)]
    public int count;
    [JavaFlags(0)]
    public int[] x;
    [JavaFlags(0)]
    public int[] y;

    public Shape()
    {
      this.count = 0;
      int length1 = 6;
      this.x = length1 >= 0 ? new int[length1] : throw new NegativeArraySizeException();
      int length2 = 6;
      this.y = length2 >= 0 ? new int[length2] : throw new NegativeArraySizeException();
    }

    public Shape(int xOrigin, int yOrigin)
    {
      this.count = 0;
      int length1 = 6;
      this.x = length1 >= 0 ? new int[length1] : throw new NegativeArraySizeException();
      int length2 = 6;
      this.y = length2 >= 0 ? new int[length2] : throw new NegativeArraySizeException();
      this.x[0] = xOrigin;
      this.y[0] = yOrigin;
      this.count = 1;
    }

    public Shape(Shape shape)
    {
      this.count = 0;
      int length1 = 6;
      this.x = length1 >= 0 ? new int[length1] : throw new NegativeArraySizeException();
      int length2 = 6;
      this.y = length2 >= 0 ? new int[length2] : throw new NegativeArraySizeException();
      this.count = shape.count;
      int count1 = this.count;
      this.x = count1 >= 0 ? new int[count1] : throw new NegativeArraySizeException();
      int count2 = this.count;
      this.y = count2 >= 0 ? new int[count2] : throw new NegativeArraySizeException();
      for (int index = 0; index < this.count; ++index)
      {
        this.x[index] = shape.x[index];
        this.y[index] = shape.y[index];
      }
    }

    public virtual void extendsLine(int width, int height) => this.addVertex(this.x[this.count - 1] + width, this.y[this.count - 1] + height);

    public virtual void addVertex(int x, int y)
    {
      this.x[this.count] = x;
      this.y[this.count] = y;
      ++this.count;
    }

    public virtual int count() => this.count;

    public virtual int[] getX()
    {
      int count = this.count;
      int[] numArray = count >= 0 ? new int[count] : throw new NegativeArraySizeException();
      System.arraycopy((object) this.x, 0, (object) numArray, 0, this.count);
      return numArray;
    }

    public virtual int[] getY()
    {
      int count = this.count;
      int[] numArray = count >= 0 ? new int[count] : throw new NegativeArraySizeException();
      System.arraycopy((object) this.y, 0, (object) numArray, 0, this.count);
      return numArray;
    }

    public override string ToString()
    {
      StringBuffer stringBuffer = new StringBuffer();
      for (int index = 0; index < this.count; ++index)
      {
        if (index > 0)
          stringBuffer.append("; ");
        stringBuffer.append(this.x[index]);
        stringBuffer.append(",");
        stringBuffer.append(this.y[index]);
      }
      return new StringBuffer().append("Shape {").append((object) stringBuffer).append("}").ToString();
    }

    public virtual void translate(int x, int y)
    {
      for (int index = 0; index < this.count; ++index)
      {
        this.x[index] = this.x[index] + x;
        this.y[index] = this.y[index] + y;
      }
    }

    [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
    [JavaFlags(4227077)]
    public new virtual object MemberwiseClone()
    {
      Shape shape = this;
      ObjectImpl.clone((object) shape);
      return ((object) shape).MemberwiseClone();
    }
  }
}
