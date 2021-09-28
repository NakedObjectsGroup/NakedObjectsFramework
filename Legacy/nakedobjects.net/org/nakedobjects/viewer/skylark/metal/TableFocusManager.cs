// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.viewer.skylark.metal.TableFocusManager
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;

namespace org.nakedobjects.viewer.skylark.metal
{
  [JavaInterfaces("1;org/nakedobjects/viewer/skylark/FocusManager;")]
  public class TableFocusManager : FocusManager
  {
    private int row;
    private int cell;
    private View table;

    public TableFocusManager(View table)
    {
      this.table = table;
      this.focusInitialChildView();
    }

    public virtual void focusNextView()
    {
      View[] subviews1 = this.table.getSubviews()[this.row].getSubviews();
      View source = subviews1[this.cell];
      for (int index = this.cell + 1; index < subviews1.Length; ++index)
      {
        if (subviews1[index].canFocus())
        {
          this.moveCells(source, subviews1[index]);
          this.cell = index;
          return;
        }
      }
      ++this.row;
      if (this.row == this.table.getSubviews().Length)
        this.row = 0;
      View[] subviews2 = this.table.getSubviews()[this.row].getSubviews();
      for (int index = 0; index < subviews2.Length; ++index)
      {
        if (subviews2[index].canFocus())
        {
          this.moveCells(source, subviews2[index]);
          this.cell = index;
          break;
        }
      }
    }

    public virtual void focusPreviousView()
    {
      View[] subviews1 = this.table.getSubviews()[this.row].getSubviews();
      View source = subviews1[this.cell];
      for (int index = this.cell - 1; index >= 0; index += -1)
      {
        if (subviews1[index].canFocus())
        {
          this.moveCells(source, subviews1[index]);
          this.cell = index;
          return;
        }
      }
      this.row += -1;
      if (this.row == -1)
        this.row = this.table.getSubviews().Length - 1;
      View[] subviews2 = this.table.getSubviews()[this.row].getSubviews();
      for (int index = subviews2.Length - 1; index >= 0; index += -1)
      {
        if (subviews2[index].canFocus())
        {
          this.moveCells(source, subviews2[index]);
          this.cell = index;
          break;
        }
      }
    }

    public virtual void focusParentView()
    {
    }

    public virtual void focusFirstChildView()
    {
    }

    public virtual void focusLastChildView()
    {
    }

    public virtual void focusInitialChildView()
    {
      this.row = this.cell = 0;
      View[] subviews1 = this.table.getSubviews();
      if (subviews1.Length <= 0)
        return;
      this.row = 0;
      View[] subviews2 = subviews1[0].getSubviews();
      for (int index = 0; index < subviews2.Length; ++index)
      {
        if (subviews2[index].canFocus())
        {
          subviews2[this.cell].markDamaged();
          this.cell = index;
          subviews2[index].markDamaged();
          break;
        }
      }
    }

    public virtual void focusUpOneRow() => this.moveFocusToRowByOffset(-1);

    public virtual void focusDownOneRow() => this.moveFocusToRowByOffset(1);

    private void moveFocusToRowByOffset(int offset)
    {
      int newRow = this.row + offset;
      View[] subviews1 = this.table.getSubviews();
      if (newRow < 0 || newRow >= subviews1.Length)
        return;
      View[] subviews2 = subviews1[newRow].getSubviews();
      if (this.cell >= subviews2.Length || !subviews2[this.cell].canFocus())
        return;
      this.setFocusToRow(newRow);
    }

    public virtual View getFocus()
    {
      View[] subviews1 = this.table.getSubviews();
      if (this.row < 0 || this.row >= subviews1.Length)
        return this.table;
      View view = subviews1[this.row];
      View[] subviews2 = view.getSubviews();
      return this.cell < 0 || this.cell >= subviews2.Length ? view : subviews2[this.cell];
    }

    public virtual void setFocus(View view)
    {
      if (view == this.table)
        return;
      View[] subviews1 = this.table.getSubviews();
      for (this.row = 0; this.row < subviews1.Length; ++this.row)
      {
        View[] subviews2 = subviews1[this.row].getSubviews();
        for (int index = 0; index < subviews2.Length; ++index)
        {
          if (view == subviews2[index] && subviews2[index].canFocus())
          {
            subviews2[this.cell].markDamaged();
            this.cell = index;
            subviews2[index].markDamaged();
            subviews2[index].focusReceived();
            return;
          }
        }
      }
    }

    private void setFocusToRow(int newRow)
    {
      if (newRow < 0)
        return;
      View[] subviews1 = this.table.getSubviews();
      if (newRow >= subviews1.Length)
        return;
      View[] subviews2 = subviews1[newRow].getSubviews();
      if (!subviews2[this.cell].canFocus())
        return;
      this.moveCells(subviews1[this.row].getSubviews()[this.cell], subviews2[this.cell]);
      this.row = newRow;
    }

    private void moveCells(View source, View destination)
    {
      source.focusLost();
      source.markDamaged();
      destination.markDamaged();
    }

    public override string ToString()
    {
      org.nakedobjects.utility.ToString toString = new org.nakedobjects.utility.ToString((object) this);
      toString.append("row", this.row);
      toString.append("cell", this.cell);
      return toString.ToString();
    }

    [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
    [JavaFlags(4227077)]
    public new virtual object MemberwiseClone()
    {
      TableFocusManager tableFocusManager = this;
      ObjectImpl.clone((object) tableFocusManager);
      return ((object) tableFocusManager).MemberwiseClone();
    }
  }
}
