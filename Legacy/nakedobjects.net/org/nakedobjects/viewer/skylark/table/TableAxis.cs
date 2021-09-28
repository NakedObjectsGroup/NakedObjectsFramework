// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.viewer.skylark.table.TableAxis
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using java.lang;
using org.nakedobjects.@object;

namespace org.nakedobjects.viewer.skylark.table
{
  [JavaInterfaces("1;org/nakedobjects/viewer/skylark/ViewAxis;")]
  public class TableAxis : ViewAxis
  {
    private readonly NakedObjectField[] columns;
    private readonly string[] columnName;
    private int rowHeaderOffet;
    private View table;
    private readonly int[] widths;

    public TableAxis(NakedObjectField[] columns)
    {
      this.columns = columns;
      int length1 = columns.Length;
      this.widths = length1 >= 0 ? new int[length1] : throw new NegativeArraySizeException();
      int length2 = columns.Length;
      this.columnName = length2 >= 0 ? new string[length2] : throw new NegativeArraySizeException();
      for (int index = 0; index < this.widths.Length; ++index)
        this.columnName[index] = columns[index].getName();
    }

    public virtual void ensureOffset(int offset) => this.rowHeaderOffet = Math.max(this.rowHeaderOffet, offset + 5);

    public virtual int getColumnAt(int xPosition)
    {
      int headerOffset = this.getHeaderOffset();
      int column = 0;
      for (int index = this.getColumnCount() + 1; column < index && (xPosition < headerOffset - 1 || xPosition > headerOffset + 1); ++column)
      {
        if (xPosition < headerOffset - 1)
          return column;
        headerOffset += this.getColumnWidth(column);
      }
      return -1;
    }

    public virtual int getColumnBorderAt(int xPosition)
    {
      int headerOffset = this.getHeaderOffset();
      int column = 0;
      for (int columnCount = this.getColumnCount(); column < columnCount; ++column)
      {
        if (xPosition >= headerOffset - 1 && xPosition <= headerOffset + 1)
          return column;
        headerOffset += this.getColumnWidth(column);
      }
      return xPosition >= headerOffset - 1 && xPosition <= headerOffset + 1 ? this.getColumnCount() : -1;
    }

    public virtual int getColumnCount() => this.columnName.Length;

    public virtual string getColumnName(int column) => this.columnName[column];

    public virtual int getColumnWidth(int column) => this.widths[column];

    public virtual NakedObjectField getFieldForColumn(int column) => this.columns[column];

    public virtual int getHeaderOffset() => this.rowHeaderOffet;

    public virtual int getLeftEdge(int resizeColumn)
    {
      int headerOffset = this.getHeaderOffset();
      int column = 0;
      for (int columnCount = this.getColumnCount(); column < resizeColumn && column < columnCount; ++column)
        headerOffset += this.getColumnWidth(column);
      return headerOffset;
    }

    public virtual void invalidateLayout()
    {
      foreach (View subview in this.table.getSubviews())
        subview.invalidateLayout();
      this.table.invalidateLayout();
    }

    public virtual void setOffset(int offset) => this.rowHeaderOffet = offset;

    public virtual void setRoot(View view) => this.table = view;

    public virtual void setupColumnWidths(ColumnWidthStrategy strategy)
    {
      for (int i = 0; i < this.widths.Length; ++i)
        this.widths[i] = strategy.getPreferredWidth(i, this.columns[i]);
    }

    public virtual void setWidth(int index, int width) => this.widths[index] = width;

    [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
    [JavaFlags(4227077)]
    public new virtual object MemberwiseClone()
    {
      TableAxis tableAxis = this;
      ObjectImpl.clone((object) tableAxis);
      return ((object) tableAxis).MemberwiseClone();
    }

    [JavaFlags(4227073)]
    public override string ToString() => ObjectImpl.jloToString((object) this);
  }
}
