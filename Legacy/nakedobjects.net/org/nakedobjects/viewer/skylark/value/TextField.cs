// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.viewer.skylark.value.TextField
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using java.awt;
using java.awt.datatransfer;
using java.lang;
using org.nakedobjects.@object;
using org.nakedobjects.@object.control;
using org.nakedobjects.utility;
using org.nakedobjects.viewer.skylark.basic;
using org.nakedobjects.viewer.skylark.core;
using org.nakedobjects.viewer.skylark.text;
using System;
using System.ComponentModel;

namespace org.nakedobjects.viewer.skylark.value
{
  [JavaInterfaces("1;org/nakedobjects/viewer/skylark/text/TextBlockTarget;")]
  public abstract class TextField : AbstractField, TextBlockTarget
  {
    private static readonly org.apache.log4j.Logger LOG;
    [JavaFlags(28)]
    public static readonly Text style;
    [JavaFlags(4)]
    public CursorPosition cursor;
    private bool identified;
    private string invalidReason;
    private bool isSaved;
    private int maxTextWidth;
    [JavaFlags(4)]
    public TextSelection selection;
    [JavaFlags(4)]
    public TextContent textContent;
    private bool showLines;
    private int maximumLength;
    private int minumumLength;

    public TextField(
      Content content,
      ViewSpecification specification,
      ViewAxis axis,
      bool showLines,
      int width,
      int wrap)
      : base(content, specification, axis)
    {
      this.invalidReason = (string) null;
      this.isSaved = true;
      this.maximumLength = 0;
      this.minumumLength = 0;
      this.showLines = showLines;
      this.setMaxTextWidth(width);
      NakedValue nakedValue = this.getValue();
      if (nakedValue != null)
      {
        this.maximumLength = nakedValue.getMaximumLength();
        this.minumumLength = nakedValue.getMinumumLength();
      }
      this.textContent = new TextContent((TextBlockTarget) this, 1, wrap);
      this.cursor = new CursorPosition(this.textContent, 0, 0);
      this.selection = new TextSelection(this.textContent, this.cursor);
      this.textContent.setText(nakedValue != null ? nakedValue.titleString() : "");
      this.cursor.home();
      this.isSaved = true;
    }

    private NakedValue getValue() => (NakedValue) this.getContent().getNaked();

    public override void setSize(Size size)
    {
      base.setSize(size);
      this.setMaxWidth(size.getWidth() - 2 * View.HPADDING);
    }

    public override bool canFocus() => this.canChangeValue();

    private void copy()
    {
      Clipboard systemClipboard = Toolkit.getDefaultToolkit().getSystemClipboard();
      string text = this.textContent.getText(this.selection);
      StringSelection stringSelection = new StringSelection(text);
      systemClipboard.setContents((Transferable) stringSelection, (ClipboardOwner) stringSelection);
      if (!TextField.LOG.isDebugEnabled())
        return;
      TextField.LOG.debug((object) new StringBuffer().append("copied ").append(text).ToString());
    }

    public override string debugDetails() => new StringBuffer().append(base.debugDetails()).append("\n").append((object) this.textContent).ToString();

    public virtual void delete()
    {
      if (this.selection.hasSelection())
      {
        this.textContent.delete(this.selection);
        this.selection.resetTo(this.selection.from());
      }
      else
      {
        this.textContent.deleteLeft(this.cursor);
        this.cursor.left();
        this.selection.resetTo(this.cursor);
      }
      this.isSaved = false;
      this.markDamaged();
    }

    public virtual void deleteForward()
    {
      if (this.selection.hasSelection())
      {
        this.textContent.delete(this.selection);
        this.selection.resetTo(this.selection.from());
      }
      else
        this.textContent.deleteRight(this.cursor);
      this.isSaved = false;
      this.markDamaged();
    }

    public override void drag(InternalDrag drag)
    {
      if (!this.canChangeValue())
        return;
      this.selection.extendTo(drag.getLocation());
      this.markDamaged();
    }

    public override Drag dragStart(DragStart drag)
    {
      Location location = drag.getLocation();
      Size size = this.getView().getSize();
      Location absoluteLocation = this.getView().getAbsoluteLocation();
      ViewAxis viewAxis = this.getViewAxis();
      if (viewAxis is LabelAxis)
      {
        int width = ((LabelAxis) viewAxis).getWidth();
        size.contractWidth(width);
        absoluteLocation.add(width, 0);
      }
      if (this.canChangeValue())
      {
        this.cursor.cursorAt(location);
        this.resetSelection();
        return (Drag) new SimpleInternalDrag((View) this, absoluteLocation);
      }
      this.markDamaged();
      return (Drag) null;
    }

    public override void dragTo(InternalDrag drag)
    {
      Location location = drag.getLocation();
      if (!this.canChangeValue())
        return;
      this.selection.extendTo(location);
      this.markDamaged();
    }

    public override void draw(Canvas canvas)
    {
      base.draw(canvas);
      int maxWidth = this.getMaxWidth();
      this.align();
      if (this.hasFocus() && this.selection.hasSelection())
        this.drawHighlight(canvas, maxWidth);
      if (this.showLines && this.canChangeValue())
      {
        Color color1 = !this.identified ? Style.SECONDARY2 : Style.IDENTIFIED;
        Color color2 = !this.hasFocus() ? color1 : Style.PRIMARY1;
        this.drawLines(canvas, color2, maxWidth);
      }
      Color textColor = !this.getState().isInvalid() ? (!this.hasFocus() ? Style.BLACK : (!this.isSaved ? Style.TEXT_EDIT : Style.PRIMARY1)) : Style.BLACK;
      this.drawText(canvas, textColor, maxWidth);
    }

    [JavaFlags(1028)]
    public abstract void align();

    [JavaFlags(1028)]
    public abstract void drawHighlight(Canvas canvas, int maxWidth);

    [JavaFlags(1028)]
    public abstract void drawLines(Canvas canvas, Color color, int width);

    [JavaFlags(1028)]
    public abstract void drawText(Canvas canvas, Color textColor, int width);

    public override void editComplete()
    {
      if (!this.canChangeValue() || this.isSaved)
        return;
      this.isSaved = true;
      this.initiateSave();
    }

    [JavaFlags(4)]
    public override void save()
    {
      string text = this.textContent.getText();
      NakedValue nakedValue = this.getValue();
      if (!StringImpl.equals(text, nakedValue != null ? (object) nakedValue.titleString() : (object) ""))
      {
        if (TextField.LOG.isDebugEnabled())
          TextField.LOG.debug((object) new StringBuffer().append("field edited: '").append(text).append("' to replace '").append(nakedValue != null ? nakedValue.titleString() : "").append("'").ToString());
        if (StringImpl.length(text) < this.minumumLength)
        {
          this.invalidReason = new StringBuffer().append("Entry not long enough, must be at least ").append(this.minumumLength).append(" characters").ToString();
          TextField.LOG.error((object) this.invalidReason);
          this.getViewManager().setStatus(this.invalidReason);
          this.getState().setInvalid();
          this.markDamaged();
        }
        else
        {
          try
          {
            this.parseEntry(StringImpl.toString(text));
            this.invalidReason = (string) null;
            this.getViewManager().getSpy().addAction(new StringBuffer().append("VALID ENTRY: ").append(text).ToString());
            this.getState().setValid();
            this.markDamaged();
            this.getParent().invalidateContent();
          }
          catch (TextEntryParseException ex)
          {
            this.invalidReason = new StringBuffer().append("INVALID ENTRY: ").append(((Throwable) ex).getMessage()).ToString();
            this.getViewManager().setStatus(this.invalidReason);
            this.getState().setInvalid();
            this.markDamaged();
          }
          catch (InvalidEntryException ex)
          {
            this.invalidReason = new StringBuffer().append("INVALID ENTRY: ").append(((Throwable) ex).getMessage()).ToString();
            this.getViewManager().setStatus(this.invalidReason);
            this.getState().setInvalid();
            this.markDamaged();
          }
          catch (ConcurrencyException ex)
          {
            ConcurrencyException concurrencyException1 = ex;
            this.invalidReason = new StringBuffer().append("UPDATE FAILURE: ").append(((Throwable) concurrencyException1).getMessage()).ToString();
            if (TextField.LOG.isWarnEnabled())
              TextField.LOG.warn((object) this.invalidReason, (Throwable) concurrencyException1);
            this.getState().setOutOfSynch();
            this.markDamaged();
            ConcurrencyException concurrencyException2 = concurrencyException1;
            if (concurrencyException2 != ex)
              throw concurrencyException2;
            throw;
          }
          catch (NakedObjectRuntimeException ex)
          {
            this.invalidReason = new StringBuffer().append("UPDATE FAILURE: ").append(((Throwable) ex).getMessage()).ToString();
            if (TextField.LOG.isWarnEnabled())
              TextField.LOG.warn((object) this.invalidReason, (Throwable) ex);
            this.getViewManager().setStatus(this.invalidReason);
            this.getState().setOutOfSynch();
            this.markDamaged();
          }
        }
      }
      else
        this.getState().setValid();
    }

    public override void entered()
    {
      if (!this.canChangeValue())
        return;
      this.getViewManager().showTextCursor();
      this.identified = true;
      this.markDamaged();
    }

    public override void exited()
    {
      if (!this.canChangeValue())
        return;
      this.getViewManager().showArrowCursor();
      this.identified = false;
      this.markDamaged();
    }

    public override void firstClick(Click click)
    {
      if (this.canChangeValue())
      {
        Location location = click.getLocation();
        location.subtract(View.HPADDING, View.VPADDING);
        this.cursor.cursorAt(location);
        this.resetSelection();
        if (this.cursor.getLine() > this.textContent.getNoLinesOfContent())
          throw new NakedObjectRuntimeException(new StringBuffer().append("not inside content for line ").append(this.cursor.getLine()).append(" : ").append(this.textContent.getNoLinesOfContent()).ToString());
        this.markDamaged();
      }
      if (this.canChangeValue() && !click.isShift() && !click.button2())
        return;
      View view = (View) new PanelBorder(1, Style.PRIMARY1, Style.PRIMARY3, (View) new TextView(this.getContent().getNaked().titleString()));
      this.getViewManager().setOverlayView(view);
      view.setLocation(this.getAbsoluteLocation());
      view.markDamaged();
    }

    public override void focusLost()
    {
      base.focusLost();
      this.editComplete();
    }

    public override void focusReceived()
    {
      this.getViewManager().setStatus(this.invalidReason != null ? this.invalidReason : "");
      this.resetSelection();
    }

    public override int getBaseline() => this.getText().getAscent();

    public virtual int getMaxWidth() => this.maxTextWidth;

    public override Size getMaximumSize() => new Size(View.HPADDING + this.maxTextWidth + View.HPADDING, Math.max(this.textContent.getNoDisplayLines() != 1 ? this.textContent.getNoDisplayLines() * this.getText().getLineHeight() : this.getText().getTextHeight(), Style.defaultFieldHeight()));

    public virtual Text getText() => TextField.style;

    private void highlight(bool select)
    {
      if (!this.canChangeValue())
        return;
      if (!select)
        this.selection.resetTo(this.cursor);
      else
        this.selection.extendTo(this.cursor);
    }

    private void insert(char character)
    {
      if (this.withinMaximum(1))
      {
        this.insert(new StringBuffer().append("").append(character).ToString());
        this.selection.resetTo(this.cursor);
      }
      else
        this.getViewManager().setStatus(new StringBuffer().append("Entry can be no longer than ").append(this.maximumLength).append(" characters").ToString());
    }

    private bool withinMaximum(int characters) => this.maximumLength == 0 || StringImpl.length(this.textContent.getText()) + characters <= this.maximumLength;

    private void insert(string characters)
    {
      if (this.withinMaximum(StringImpl.length(characters)))
      {
        int noDisplayLines = this.textContent.getNoDisplayLines();
        this.textContent.insert(this.cursor, characters);
        this.cursor.right(StringImpl.length(characters));
        if (this.textContent.getNoDisplayLines() != noDisplayLines)
          this.invalidateLayout();
        this.isSaved = false;
        this.markDamaged();
      }
      else
        this.getViewManager().setStatus(new StringBuffer().append("Entry can be no longer than ").append(this.maximumLength).append(" characters").ToString());
    }

    public virtual bool isIdentified() => this.identified;

    public override void keyPressed(KeyboardAction key)
    {
      if (!this.canChangeValue())
        return;
      int keyCode = key.getKeyCode();
      switch (keyCode)
      {
        case 16:
          break;
        case 17:
          break;
        case 18:
          break;
        default:
          int modifiers = key.getModifiers();
          bool alt = (modifiers & 8) > 0;
          bool shift = (modifiers & 1) > 0;
          bool ctrl = (modifiers & 2) > 0;
          switch (keyCode)
          {
            case 8:
              key.consume();
              this.delete();
              break;
            case 9:
              this.tab();
              break;
            case 10:
              if (!this.enter())
              {
                this.getParent().keyPressed(key);
                break;
              }
              break;
            case 27:
              key.consume();
              this.escape();
              break;
            case 33:
              key.consume();
              this.pageUp(shift, ctrl);
              break;
            case 34:
              key.consume();
              this.pageDown(shift, ctrl);
              break;
            case 35:
              key.consume();
              this.end(alt, shift);
              break;
            case 36:
              key.consume();
              this.home(alt, shift);
              break;
            case 37:
              key.consume();
              this.left(alt, shift);
              break;
            case 38:
              key.consume();
              this.up(shift);
              break;
            case 39:
              key.consume();
              this.right(alt, shift);
              break;
            case 40:
              key.consume();
              this.down(shift);
              break;
            case 67:
              if (ctrl)
              {
                key.consume();
                this.copy();
                break;
              }
              break;
            case 86:
              if (ctrl)
              {
                key.consume();
                this.paste();
                this.highlight(false);
                break;
              }
              break;
            case (int) sbyte.MaxValue:
              key.consume();
              this.deleteForward();
              break;
          }
          if (TextField.LOG.isDebugEnabled())
            TextField.LOG.debug((object) new StringBuffer().append("character at ").append(this.cursor.getCharacter()).append(" line ").append(this.cursor.getLine()).ToString());
          if (!TextField.LOG.isDebugEnabled())
            break;
          TextField.LOG.debug((object) this.selection);
          break;
      }
    }

    public virtual CursorPosition getCursor() => this.cursor;

    [JavaFlags(4)]
    public virtual void pageDown(bool shift, bool ctrl)
    {
      if (ctrl)
      {
        if (this.textContent.decreaseDepth())
        {
          this.textContent.alignDisplay(this.cursor.getLine());
          this.invalidateLayout();
        }
      }
      else
      {
        this.cursor.pageDown();
        this.highlight(shift);
      }
      this.markDamaged();
    }

    [JavaFlags(4)]
    public virtual void pageUp(bool shift, bool ctrl)
    {
      if (ctrl)
      {
        this.textContent.increaseDepth();
        this.textContent.alignDisplay(this.cursor.getLine());
        this.invalidateLayout();
      }
      else
      {
        this.cursor.pageUp();
        this.highlight(shift);
      }
      this.markDamaged();
    }

    [JavaFlags(4)]
    public virtual void down(bool shift)
    {
      this.cursor.lineDown();
      this.highlight(shift);
      this.markDamaged();
    }

    [JavaFlags(4)]
    public virtual void up(bool shift)
    {
      this.cursor.lineUp();
      this.highlight(shift);
      this.markDamaged();
    }

    [JavaFlags(4)]
    public virtual void home(bool alt, bool shift)
    {
      if (alt)
        this.cursor.top();
      else
        this.cursor.home();
      this.highlight(shift);
      this.markDamaged();
    }

    [JavaFlags(4)]
    public virtual void end(bool alt, bool shift)
    {
      if (alt)
        this.cursor.bottom();
      else
        this.cursor.end();
      this.highlight(shift);
      this.markDamaged();
    }

    [JavaFlags(4)]
    public virtual void left(bool alt, bool shift)
    {
      if (alt)
        this.cursor.wordLeft();
      else
        this.cursor.left();
      this.highlight(shift);
      this.markDamaged();
    }

    [JavaFlags(4)]
    public virtual void right(bool alt, bool shift)
    {
      if (alt)
        this.cursor.wordRight();
      else
        this.cursor.right();
      this.highlight(shift);
      this.markDamaged();
    }

    [JavaFlags(4)]
    public virtual void escape()
    {
      if (this.isSaved)
      {
        this.textContent.setText("");
        this.cursor.home();
        this.selection.resetTo(this.cursor);
        this.isSaved = false;
        this.markDamaged();
      }
      else
      {
        this.invalidReason = (string) null;
        this.refresh();
        this.markDamaged();
      }
    }

    [JavaFlags(4)]
    public virtual void tab() => this.editComplete();

    [JavaFlags(4)]
    public virtual bool enter()
    {
      this.editComplete();
      return false;
    }

    public override void keyReleased(int keyCode, int modifiers)
    {
    }

    public override void keyTyped(char keyCode)
    {
      if (!this.canChangeValue())
        return;
      this.insert(keyCode);
    }

    public override void contentMenuOptions(UserActionSet options)
    {
      options.add((UserAction) new TextField.\u0031(this, "Refresh"));
      base.contentMenuOptions(options);
    }

    public virtual void paste()
    {
      Transferable contents = Toolkit.getDefaultToolkit().getSystemClipboard().getContents((object) this);
      try
      {
        string characters = \u003CVerifierFix\u003E.genCastToString(contents.getTransferData((DataFlavor) DataFlavor.stringFlavor));
        this.insert(characters);
        if (!TextField.LOG.isDebugEnabled())
          return;
        TextField.LOG.debug((object) new StringBuffer().append("pasted ").append(characters).ToString());
      }
      catch (Exception ex)
      {
        Throwable throwable = ThrowableWrapper.wrapThrowable(ex);
        TextField.LOG.error((object) new StringBuffer().append("invalid paste operation ").append((object) throwable).ToString());
      }
    }

    public override void refresh()
    {
      base.refresh();
      NakedValue nakedValue = this.getValue();
      if (nakedValue == null)
      {
        this.textContent.setText("");
      }
      else
      {
        this.textContent.setText(nakedValue.titleString());
        this.maximumLength = nakedValue.getMaximumLength();
        this.minumumLength = nakedValue.getMinumumLength();
      }
      this.isSaved = true;
    }

    private void resetSelection() => this.selection.resetTo(this.cursor);

    public override void secondClick(Click click)
    {
      if (!this.canChangeValue())
        return;
      this.selection.selectWord();
    }

    public virtual void setMaxTextWidth(int noCharacters) => this.maxTextWidth = this.getText().charWidth('o') * noCharacters;

    public virtual void setMaxWidth(int width) => this.maxTextWidth = width;

    public override void thirdClick(Click click)
    {
      if (!this.canChangeValue())
        return;
      this.selection.selectSentence();
      this.markDamaged();
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [JavaFlags(32778)]
    static TextField()
    {
      // ISSUE: unable to decompile the method.
    }

    [Inner]
    [JavaFlags(32)]
    public new class \u0031 : AbstractUserAction
    {
      [EditorBrowsable(EditorBrowsableState.Never)]
      [JavaFlags(32770)]
      private TextField this\u00240;

      public override void execute(Workspace workspace, View view, Location at)
      {
        this.this\u00240.invalidReason = (string) null;
        this.this\u00240.refresh();
      }

      public override Consent disabled(View component) => AbstractConsent.allow(this.this\u00240.invalidReason != null);

      public \u0031(TextField _param1, string dummy0)
        : base(dummy0)
      {
        this.this\u00240 = _param1;
        if (_param1 != null)
          return;
        ObjectImpl.getClass((object) _param1);
      }
    }
  }
}
