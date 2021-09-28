// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.viewer.skylark.KeyboardManager
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using java.awt.@event;
using java.lang;
using org.nakedobjects.utility;
using System.ComponentModel;

namespace org.nakedobjects.viewer.skylark
{
  public class KeyboardManager
  {
    private static readonly org.apache.log4j.Logger LOG;
    private readonly Viewer viewer;
    private FocusManager focusManager;

    public KeyboardManager(Viewer viewer) => this.viewer = viewer;

    private View getFocus() => this.focusManager == null ? (View) null : this.focusManager.getFocus();

    public virtual void pressed(int keyCode, int modifiers)
    {
      if (this.ignoreKey(keyCode))
        return;
      if (KeyboardManager.LOG.isDebugEnabled())
        KeyboardManager.LOG.debug((object) new StringBuffer().append("key ").append(KeyEvent.getKeyModifiersText(modifiers)).append(" '").append(KeyEvent.getKeyText(keyCode)).append("' pressed").ToString());
      KeyboardAction keyboardAction = new KeyboardAction(keyCode, modifiers);
      if (this.viewer.isOverlayAvailable())
      {
        this.viewer.getOverlayView().keyPressed(keyboardAction);
        if (keyboardAction.isConsumed() || keyCode != 112)
          return;
        this.viewer.openHelp(this.viewer.getOverlayView());
      }
      else
      {
        View focus = this.getFocus();
        if (focus == null)
        {
          if (KeyboardManager.LOG.isDebugEnabled())
            KeyboardManager.LOG.debug((object) "No focus set");
          this.viewer.filterKeyShortcuts(keyboardAction);
        }
        else
        {
          focus.keyPressed(keyboardAction);
          if (keyboardAction.isConsumed())
            return;
          if ((modifiers & 1) == 1 && keyCode == 121)
          {
            Location absoluteLocation = focus.getAbsoluteLocation();
            absoluteLocation.add(20, 14);
            this.viewer.popupMenu(focus, absoluteLocation, true, false, false);
          }
          else if (keyCode == 121)
          {
            Location absoluteLocation = focus.getAbsoluteLocation();
            absoluteLocation.add(20, 14);
            this.viewer.popupMenu(focus, absoluteLocation, false, false, false);
          }
          else
          {
            if (keyCode == 115 && (modifiers & 2) == 2)
              return;
            switch (keyCode)
            {
              case 36:
                this.viewer.makeRootFocus();
                break;
              case 37:
                this.focusManager.focusPreviousView();
                break;
              case 38:
                this.focusManager.focusParentView();
                break;
              case 39:
                this.focusManager.focusNextView();
                break;
              case 40:
                this.focusManager.focusFirstChildView();
                break;
              default:
                int num = 0;
                if (keyCode == 112)
                  this.viewer.openHelp(focus);
                else if (keyCode == 9)
                  num = this.tab(modifiers);
                switch (num)
                {
                  case 2:
                    this.focusManager.focusNextView();
                    break;
                  case 3:
                    this.focusManager.focusParentView();
                    break;
                  case 4:
                    this.focusManager.focusPreviousView();
                    break;
                  case 5:
                    this.focusManager.focusFirstChildView();
                    break;
                }
                if (keyboardAction.isConsumed())
                  break;
                this.viewer.filterKeyShortcuts(keyboardAction);
                break;
            }
          }
        }
      }
    }

    private bool ignoreKey(int keyCode) => keyCode == 16 || keyCode == 17 || keyCode == 18;

    private int tab(int modifiers) => (modifiers & 2) != 2 ? ((modifiers & 1) != 1 ? 2 : 4) : ((modifiers & 1) != 1 ? 3 : 5);

    public virtual void released(int keyCode, int modifiers)
    {
      if (this.ignoreKey(keyCode))
        return;
      if (KeyboardManager.LOG.isDebugEnabled())
        KeyboardManager.LOG.debug((object) new StringBuffer().append("key ").append(KeyEvent.getKeyText(keyCode)).append(" released\n").ToString());
      this.getFocus()?.keyReleased(keyCode, modifiers);
    }

    public virtual void typed(char keyChar)
    {
      if (KeyboardManager.LOG.isDebugEnabled())
        KeyboardManager.LOG.debug((object) new StringBuffer().append("typed '").append(keyChar).append("'").ToString());
      if (this.viewer.isOverlayAvailable())
      {
        this.viewer.getOverlayView().keyTyped(keyChar);
      }
      else
      {
        View focus = this.getFocus();
        if (focus == null || Character.isISOControl(keyChar))
          return;
        focus.keyTyped(keyChar);
      }
    }

    public virtual FocusManager getFocusManager() => this.focusManager;

    public virtual void setFocusManager(FocusManager focusManager) => this.focusManager = focusManager != null ? focusManager : throw new NakedObjectRuntimeException("No focus manager set up");

    [JavaFlags(32778)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    static KeyboardManager()
    {
      // ISSUE: unable to decompile the method.
    }

    [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
    [JavaFlags(4227077)]
    public new virtual object MemberwiseClone()
    {
      KeyboardManager keyboardManager = this;
      ObjectImpl.clone((object) keyboardManager);
      return ((object) keyboardManager).MemberwiseClone();
    }

    [JavaFlags(4227073)]
    public override string ToString() => ObjectImpl.jloToString((object) this);
  }
}
