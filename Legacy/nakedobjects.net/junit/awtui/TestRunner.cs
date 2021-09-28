// Decompiled with JetBrains decompiler
// Type: junit.awtui.TestRunner
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using com.ms.vjsharp.util;
using java.awt;
using java.awt.@event;
using java.lang;
using java.util;
using junit.framework;
using junit.runner;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;

namespace junit.awtui
{
  public class TestRunner : BaseTestRunner
  {
    [JavaFlags(4)]
    public Frame fFrame;
    [JavaFlags(4)]
    public Vector fExceptions;
    [JavaFlags(4)]
    public Vector fFailedTests;
    [JavaFlags(4)]
    public Thread fRunner;
    [JavaFlags(4)]
    public TestResult fTestResult;
    [JavaFlags(4)]
    public TextArea fTraceArea;
    [JavaFlags(4)]
    public TextField fSuiteField;
    [JavaFlags(4)]
    public Button fRun;
    [JavaFlags(4)]
    public ProgressBar fProgressIndicator;
    [JavaFlags(4)]
    public List fFailureList;
    [JavaFlags(4)]
    public Logo fLogo;
    [JavaFlags(4)]
    public Label fNumberOfErrors;
    [JavaFlags(4)]
    public Label fNumberOfFailures;
    [JavaFlags(4)]
    public Label fNumberOfRuns;
    [JavaFlags(4)]
    public Button fQuitButton;
    [JavaFlags(4)]
    public Button fRerunButton;
    [JavaFlags(4)]
    public TextField fStatusLine;
    [JavaFlags(4)]
    public Checkbox fUseLoadingRunner;
    [JavaFlags(28)]
    public static readonly Font PLAIN_FONT;
    private const int GAP = 4;

    private void about()
    {
      AboutDialog aboutDialog = new AboutDialog(this.fFrame);
      aboutDialog.setModal(true);
      ((Component) aboutDialog).setLocation(300, 300);
      ((Component) aboutDialog).setVisible(true);
    }

    public override void testStarted(string testName) => this.showInfo(new StringBuffer().append("Running: ").append(testName).ToString());

    public override void testEnded(string testName)
    {
      this.setLabelValue(this.fNumberOfRuns, this.fTestResult.runCount());
      object obj = (object) this;
      \u003CCorArrayWrapper\u003E.Enter(obj);
      try
      {
        this.fProgressIndicator.step(this.fTestResult.wasSuccessful());
      }
      finally
      {
        Monitor.Exit(obj);
      }
    }

    public override void testFailed(int status, Test test, Throwable t)
    {
      switch (status)
      {
        case 1:
          this.fNumberOfErrors.setText(Integer.toString(this.fTestResult.errorCount()));
          this.appendFailure("Error", test, t);
          break;
        case 2:
          this.fNumberOfFailures.setText(Integer.toString(this.fTestResult.failureCount()));
          this.appendFailure("Failure", test, t);
          break;
      }
    }

    [JavaFlags(4)]
    public virtual void addGrid(
      Panel p,
      Component co,
      int x,
      int y,
      int w,
      int fill,
      double wx,
      int anchor)
    {
      GridBagConstraints gridBagConstraints = new GridBagConstraints();
      gridBagConstraints.gridx = (__Null) x;
      gridBagConstraints.gridy = (__Null) y;
      gridBagConstraints.gridwidth = (__Null) w;
      gridBagConstraints.anchor = (__Null) anchor;
      gridBagConstraints.weightx = (__Null) wx;
      gridBagConstraints.fill = (__Null) fill;
      if (fill == 1 || fill == 3)
        gridBagConstraints.weighty = (__Null) 1.0;
      gridBagConstraints.insets = (__Null) new Insets(y != 0 ? 0 : 4, x != 0 ? 0 : 4, 4, 4);
      ((Container) p).add(co, (object) gridBagConstraints);
    }

    private void appendFailure(string kind, Test test, Throwable t)
    {
      kind = new StringBuffer().append(kind).append(": ").append((object) test).ToString();
      string message = t.getMessage();
      if (message != null)
        kind = new StringBuffer().append(kind).append(":").append(BaseTestRunner.truncate(message)).ToString();
      this.fFailureList.add(kind);
      this.fExceptions.addElement((object) t);
      this.fFailedTests.addElement((object) test);
      if (this.fFailureList.getItemCount() != 1)
        return;
      this.fFailureList.select(0);
      this.failureSelected();
    }

    [JavaFlags(4)]
    public virtual Menu createJUnitMenu()
    {
      Menu menu = new Menu("JUnit");
      MenuItem menuItem1 = new MenuItem("About...");
      menuItem1.addActionListener((ActionListener) new TestRunner.\u0031(this));
      menu.add(menuItem1);
      menu.addSeparator();
      MenuItem menuItem2 = new MenuItem("Exit");
      menuItem2.addActionListener((ActionListener) new TestRunner.\u0032(this));
      menu.add(menuItem2);
      return menu;
    }

    [JavaFlags(4)]
    public virtual void createMenus(MenuBar mb) => mb.add(this.createJUnitMenu());

    [JavaFlags(4)]
    public virtual TestResult createTestResult() => new TestResult();

    [JavaFlags(4)]
    public virtual Frame createUI(string suiteName)
    {
      Frame frame1 = new Frame("JUnit");
      Image image = this.loadFrameIcon();
      if (image != null)
        frame1.setIconImage(image);
      ((Container) frame1).setLayout((LayoutManager) new BorderLayout(0, 0));
      ((Container) frame1).setBackground((Color) SystemColor.control);
      Frame frame2 = frame1;
      ((Window) frame1).addWindowListener((WindowListener) new TestRunner.\u0033(this, frame2));
      MenuBar mb = new MenuBar();
      this.createMenus(mb);
      frame1.setMenuBar(mb);
      Label label1 = new Label("Test class name:");
      this.fSuiteField = new TextField(suiteName == null ? "" : suiteName);
      ((TextComponent) this.fSuiteField).selectAll();
      ((Component) this.fSuiteField).requestFocus();
      ((Component) this.fSuiteField).setFont(TestRunner.PLAIN_FONT);
      this.fSuiteField.setColumns(40);
      this.fSuiteField.addActionListener((ActionListener) new TestRunner.\u0034(this));
      ((TextComponent) this.fSuiteField).addTextListener((TextListener) new TestRunner.\u0035(this));
      this.fRun = new Button("Run");
      ((Component) this.fRun).setEnabled(false);
      this.fRun.addActionListener((ActionListener) new TestRunner.\u0036(this));
      this.fUseLoadingRunner = new Checkbox("Reload classes every run", this.useReloadingTestSuiteLoader());
      if (BaseTestRunner.inVAJava())
        ((Component) this.fUseLoadingRunner).setVisible(false);
      this.fProgressIndicator = new ProgressBar();
      this.fNumberOfErrors = new Label("0000", 2);
      this.fNumberOfErrors.setText("0");
      ((Component) this.fNumberOfErrors).setFont(TestRunner.PLAIN_FONT);
      this.fNumberOfFailures = new Label("0000", 2);
      this.fNumberOfFailures.setText("0");
      ((Component) this.fNumberOfFailures).setFont(TestRunner.PLAIN_FONT);
      this.fNumberOfRuns = new Label("0000", 2);
      this.fNumberOfRuns.setText("0");
      ((Component) this.fNumberOfRuns).setFont(TestRunner.PLAIN_FONT);
      Panel counterPanel = this.createCounterPanel();
      Label label2 = new Label("Errors and Failures:");
      this.fFailureList = new List(5);
      this.fFailureList.addItemListener((ItemListener) new TestRunner.\u0037(this));
      this.fRerunButton = new Button("Run");
      ((Component) this.fRerunButton).setEnabled(false);
      this.fRerunButton.addActionListener((ActionListener) new TestRunner.\u0038(this));
      Panel panel = new Panel((LayoutManager) new GridLayout(0, 1, 0, 2));
      ((Container) panel).add((Component) this.fRerunButton);
      this.fTraceArea = new TextArea();
      this.fTraceArea.setRows(5);
      this.fTraceArea.setColumns(60);
      this.fStatusLine = new TextField();
      ((Component) this.fStatusLine).setFont(TestRunner.PLAIN_FONT);
      ((TextComponent) this.fStatusLine).setEditable(false);
      ((Component) this.fStatusLine).setForeground((Color) Color.red);
      this.fQuitButton = new Button("Exit");
      this.fQuitButton.addActionListener((ActionListener) new TestRunner.\u0039(this));
      this.fLogo = new Logo();
      Panel p = new Panel((LayoutManager) new GridBagLayout());
      this.addGrid(p, (Component) label1, 0, 0, 2, 2, 1.0, 17);
      this.addGrid(p, (Component) this.fSuiteField, 0, 1, 2, 2, 1.0, 17);
      this.addGrid(p, (Component) this.fRun, 2, 1, 1, 2, 0.0, 10);
      this.addGrid(p, (Component) this.fUseLoadingRunner, 0, 2, 2, 0, 1.0, 17);
      this.addGrid(p, (Component) this.fProgressIndicator, 0, 3, 2, 2, 1.0, 17);
      this.addGrid(p, (Component) this.fLogo, 2, 3, 1, 0, 0.0, 11);
      this.addGrid(p, (Component) counterPanel, 0, 4, 2, 0, 0.0, 17);
      this.addGrid(p, (Component) label2, 0, 5, 2, 2, 1.0, 17);
      this.addGrid(p, (Component) this.fFailureList, 0, 6, 2, 1, 1.0, 17);
      this.addGrid(p, (Component) panel, 2, 6, 1, 2, 0.0, 10);
      this.addGrid(p, (Component) this.fTraceArea, 0, 7, 2, 1, 1.0, 17);
      this.addGrid(p, (Component) this.fStatusLine, 0, 8, 2, 2, 1.0, 10);
      this.addGrid(p, (Component) this.fQuitButton, 2, 8, 1, 2, 0.0, 10);
      ((Container) frame1).add((Component) p, (object) BorderLayout.CENTER);
      ((Window) frame1).pack();
      return frame1;
    }

    [JavaFlags(4)]
    public virtual Panel createCounterPanel()
    {
      Panel counter = new Panel((LayoutManager) new GridBagLayout());
      this.addToCounterPanel(counter, (Component) new Label("Runs:"), 0, 0, 1, 1, 0.0, 0.0, 10, 0, new Insets(0, 0, 0, 0));
      this.addToCounterPanel(counter, (Component) this.fNumberOfRuns, 1, 0, 1, 1, 0.33, 0.0, 10, 2, new Insets(0, 8, 0, 40));
      this.addToCounterPanel(counter, (Component) new Label("Errors:"), 2, 0, 1, 1, 0.0, 0.0, 10, 0, new Insets(0, 8, 0, 0));
      this.addToCounterPanel(counter, (Component) this.fNumberOfErrors, 3, 0, 1, 1, 0.33, 0.0, 10, 2, new Insets(0, 8, 0, 40));
      this.addToCounterPanel(counter, (Component) new Label("Failures:"), 4, 0, 1, 1, 0.0, 0.0, 10, 0, new Insets(0, 8, 0, 0));
      this.addToCounterPanel(counter, (Component) this.fNumberOfFailures, 5, 0, 1, 1, 0.33, 0.0, 10, 2, new Insets(0, 8, 0, 0));
      return counter;
    }

    private void addToCounterPanel(
      Panel counter,
      Component comp,
      int gridx,
      int gridy,
      int gridwidth,
      int gridheight,
      double weightx,
      double weighty,
      int anchor,
      int fill,
      Insets insets)
    {
      ((Container) counter).add(comp, (object) new GridBagConstraints()
      {
        gridx = (__Null) gridx,
        gridy = (__Null) gridy,
        gridwidth = (__Null) gridwidth,
        gridheight = (__Null) gridheight,
        weightx = (__Null) weightx,
        weighty = (__Null) weighty,
        anchor = (__Null) anchor,
        fill = (__Null) fill,
        insets = (__Null) insets
      });
    }

    public virtual void failureSelected()
    {
      ((Component) this.fRerunButton).setEnabled(this.isErrorSelected());
      this.showErrorTrace();
    }

    private bool isErrorSelected() => this.fFailureList.getSelectedIndex() != -1;

    private Image loadFrameIcon()
    {
      // ISSUE: unable to decompile the method.
    }

    public virtual Thread getRunner() => this.fRunner;

    public static void main(string[] args)
    {
      // ISSUE: type reference
      RuntimeHelpers.RunClassConstructor(__typeref (ObjectImpl));
      new TestRunner().start(args);
      Utilities.cleanupAfterMainReturns();
    }

    public static void run(Class test)
    {
      int length = 1;
      string[] args = length >= 0 ? new string[length] : throw new NegativeArraySizeException();
      args[0] = test.getName();
      TestRunner.main(args);
    }

    public static void run(string test)
    {
      int length = 1;
      string[] args = length >= 0 ? new string[length] : throw new NegativeArraySizeException();
      args[0] = test;
      TestRunner.main(args);
    }

    public virtual void rerun()
    {
      int selectedIndex = this.fFailureList.getSelectedIndex();
      if (selectedIndex == -1)
        return;
      this.rerunTest((Test) this.fFailedTests.elementAt(selectedIndex));
    }

    private void rerunTest(Test test)
    {
      // ISSUE: unable to decompile the method.
    }

    [JavaFlags(4)]
    public virtual void reset()
    {
      this.setLabelValue(this.fNumberOfErrors, 0);
      this.setLabelValue(this.fNumberOfFailures, 0);
      this.setLabelValue(this.fNumberOfRuns, 0);
      this.fProgressIndicator.reset();
      this.fFailureList.removeAll();
      this.fExceptions = new Vector(10);
      this.fFailedTests = new Vector(10);
      ((TextComponent) this.fTraceArea).setText("");
    }

    [JavaFlags(4)]
    public override void runFailed(string message)
    {
      this.showStatus(message);
      this.fRun.setLabel("Run");
      this.fRunner = (Thread) null;
    }

    [MethodImpl(MethodImplOptions.Synchronized)]
    public virtual void runSuite()
    {
      if (this.fRunner != null && this.fTestResult != null)
      {
        this.fTestResult.stop();
      }
      else
      {
        this.setLoading(this.shouldReload());
        this.fRun.setLabel("Stop");
        this.showInfo("Initializing...");
        this.reset();
        this.showInfo("Load Test Case...");
        Test test = this.getTest(((TextComponent) this.fSuiteField).getText());
        if (test == null)
          return;
        this.fRunner = (Thread) new TestRunner.\u00310(this, test);
        this.fRunner.start();
      }
    }

    private bool shouldReload() => !BaseTestRunner.inVAJava() && this.fUseLoadingRunner.getState();

    private void setLabelValue(Label label, int value)
    {
      label.setText(Integer.toString(value));
      ((Component) label).invalidate();
      ((Component) label).getParent().validate();
    }

    public virtual void setSuiteName(string suite) => ((TextComponent) this.fSuiteField).setText(suite);

    private void showErrorTrace()
    {
      int selectedIndex = this.fFailureList.getSelectedIndex();
      if (selectedIndex == -1)
        return;
      ((TextComponent) this.fTraceArea).setText(BaseTestRunner.getFilteredTrace((Throwable) this.fExceptions.elementAt(selectedIndex)));
    }

    private void showInfo(string message)
    {
      ((Component) this.fStatusLine).setFont(TestRunner.PLAIN_FONT);
      ((Component) this.fStatusLine).setForeground((Color) Color.black);
      ((TextComponent) this.fStatusLine).setText(message);
    }

    [JavaFlags(4)]
    public override void clearStatus() => this.showStatus("");

    private void showStatus(string status)
    {
      ((Component) this.fStatusLine).setFont(TestRunner.PLAIN_FONT);
      ((Component) this.fStatusLine).setForeground((Color) Color.red);
      ((TextComponent) this.fStatusLine).setText(status);
    }

    public virtual void start(string[] args)
    {
      string str = this.processArguments(args);
      this.fFrame = this.createUI(str);
      ((Component) this.fFrame).setLocation(20, 20);
      ((Component) this.fFrame).setSize(984, 728);
      ((Component) this.fFrame).setVisible(true);
      if (str == null)
        return;
      this.setSuiteName(str);
      this.runSuite();
    }

    [JavaFlags(32778)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    static TestRunner()
    {
      // ISSUE: unable to decompile the method.
    }

    [JavaInterfaces("1;java/awt/event/ActionListener;")]
    [JavaFlags(32)]
    [Inner]
    public class \u0031 : ActionListener
    {
      [EditorBrowsable(EditorBrowsableState.Never)]
      [JavaFlags(32770)]
      private TestRunner this\u00240;

      public virtual void actionPerformed(ActionEvent @event) => this.this\u00240.about();

      public \u0031(TestRunner _param1)
      {
        this.this\u00240 = _param1;
        if (_param1 != null)
          return;
        ObjectImpl.getClass((object) _param1);
      }

      [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
      [JavaFlags(4227077)]
      public new virtual object MemberwiseClone()
      {
        TestRunner.\u0031 obj = this;
        ObjectImpl.clone((object) obj);
        return ((object) obj).MemberwiseClone();
      }

      [JavaFlags(4227073)]
      public override string ToString() => ObjectImpl.jloToString((object) this);
    }

    [Inner]
    [JavaInterfaces("1;java/awt/event/ActionListener;")]
    [JavaFlags(32)]
    public class \u0032 : ActionListener
    {
      [EditorBrowsable(EditorBrowsableState.Never)]
      [JavaFlags(32770)]
      private TestRunner this\u00240;

      public virtual void actionPerformed(ActionEvent @event) => java.lang.System.exit(0);

      public \u0032(TestRunner _param1)
      {
        this.this\u00240 = _param1;
        if (_param1 != null)
          return;
        ObjectImpl.getClass((object) _param1);
      }

      [JavaFlags(4227077)]
      [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
      public new virtual object MemberwiseClone()
      {
        TestRunner.\u0032 obj = this;
        ObjectImpl.clone((object) obj);
        return ((object) obj).MemberwiseClone();
      }

      [JavaFlags(4227073)]
      public override string ToString() => ObjectImpl.jloToString((object) this);
    }

    [Inner]
    [JavaFlags(32)]
    public class \u0033 : WindowAdapter
    {
      [JavaFlags(32770)]
      [EditorBrowsable(EditorBrowsableState.Never)]
      private TestRunner this\u00240;
      [JavaFlags(16)]
      public readonly Frame finalFrame_\u003E;

      public virtual void windowClosing(WindowEvent e)
      {
        this.finalFrame_\u003E.dispose();
        java.lang.System.exit(0);
      }

      public \u0033(TestRunner _param1, [In] Frame obj1)
      {
        this.this\u00240 = _param1;
        if (_param1 == null)
          ObjectImpl.getClass((object) _param1);
        this.finalFrame_\u003E = obj1;
      }

      [JavaFlags(4227077)]
      [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
      public virtual object MemberwiseClone()
      {
        TestRunner.\u0033 obj = this;
        ObjectImpl.clone((object) obj);
        return ((object) obj).MemberwiseClone();
      }

      [JavaFlags(4227073)]
      public virtual string ToString() => ObjectImpl.jloToString((object) this);
    }

    [JavaInterfaces("1;java/awt/event/ActionListener;")]
    [JavaFlags(32)]
    [Inner]
    public class \u0034 : ActionListener
    {
      [EditorBrowsable(EditorBrowsableState.Never)]
      [JavaFlags(32770)]
      private TestRunner this\u00240;

      public virtual void actionPerformed(ActionEvent e) => this.this\u00240.runSuite();

      public \u0034(TestRunner _param1)
      {
        this.this\u00240 = _param1;
        if (_param1 != null)
          return;
        ObjectImpl.getClass((object) _param1);
      }

      [JavaFlags(4227077)]
      [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
      public new virtual object MemberwiseClone()
      {
        TestRunner.\u0034 obj = this;
        ObjectImpl.clone((object) obj);
        return ((object) obj).MemberwiseClone();
      }

      [JavaFlags(4227073)]
      public override string ToString() => ObjectImpl.jloToString((object) this);
    }

    [JavaFlags(32)]
    [Inner]
    [JavaInterfaces("1;java/awt/event/TextListener;")]
    public class \u0035 : TextListener
    {
      [EditorBrowsable(EditorBrowsableState.Never)]
      [JavaFlags(32770)]
      private TestRunner this\u00240;

      public virtual void textValueChanged(TextEvent e)
      {
        ((Component) this.this\u00240.fRun).setEnabled(StringImpl.length(((TextComponent) this.this\u00240.fSuiteField).getText()) > 0);
        ((TextComponent) this.this\u00240.fStatusLine).setText("");
      }

      public \u0035(TestRunner _param1)
      {
        this.this\u00240 = _param1;
        if (_param1 != null)
          return;
        ObjectImpl.getClass((object) _param1);
      }

      [JavaFlags(4227077)]
      [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
      public new virtual object MemberwiseClone()
      {
        TestRunner.\u0035 obj = this;
        ObjectImpl.clone((object) obj);
        return ((object) obj).MemberwiseClone();
      }

      [JavaFlags(4227073)]
      public override string ToString() => ObjectImpl.jloToString((object) this);
    }

    [JavaFlags(32)]
    [JavaInterfaces("1;java/awt/event/ActionListener;")]
    [Inner]
    public class \u0036 : ActionListener
    {
      [JavaFlags(32770)]
      [EditorBrowsable(EditorBrowsableState.Never)]
      private TestRunner this\u00240;

      public virtual void actionPerformed(ActionEvent e) => this.this\u00240.runSuite();

      public \u0036(TestRunner _param1)
      {
        this.this\u00240 = _param1;
        if (_param1 != null)
          return;
        ObjectImpl.getClass((object) _param1);
      }

      [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
      [JavaFlags(4227077)]
      public new virtual object MemberwiseClone()
      {
        TestRunner.\u0036 obj = this;
        ObjectImpl.clone((object) obj);
        return ((object) obj).MemberwiseClone();
      }

      [JavaFlags(4227073)]
      public override string ToString() => ObjectImpl.jloToString((object) this);
    }

    [JavaFlags(32)]
    [JavaInterfaces("1;java/awt/event/ItemListener;")]
    [Inner]
    public class \u0037 : ItemListener
    {
      [JavaFlags(32770)]
      [EditorBrowsable(EditorBrowsableState.Never)]
      private TestRunner this\u00240;

      public virtual void itemStateChanged(ItemEvent e) => this.this\u00240.failureSelected();

      public \u0037(TestRunner _param1)
      {
        this.this\u00240 = _param1;
        if (_param1 != null)
          return;
        ObjectImpl.getClass((object) _param1);
      }

      [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
      [JavaFlags(4227077)]
      public new virtual object MemberwiseClone()
      {
        TestRunner.\u0037 obj = this;
        ObjectImpl.clone((object) obj);
        return ((object) obj).MemberwiseClone();
      }

      [JavaFlags(4227073)]
      public override string ToString() => ObjectImpl.jloToString((object) this);
    }

    [JavaFlags(32)]
    [JavaInterfaces("1;java/awt/event/ActionListener;")]
    [Inner]
    public class \u0038 : ActionListener
    {
      [JavaFlags(32770)]
      [EditorBrowsable(EditorBrowsableState.Never)]
      private TestRunner this\u00240;

      public virtual void actionPerformed(ActionEvent e) => this.this\u00240.rerun();

      public \u0038(TestRunner _param1)
      {
        this.this\u00240 = _param1;
        if (_param1 != null)
          return;
        ObjectImpl.getClass((object) _param1);
      }

      [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
      [JavaFlags(4227077)]
      public new virtual object MemberwiseClone()
      {
        TestRunner.\u0038 obj = this;
        ObjectImpl.clone((object) obj);
        return ((object) obj).MemberwiseClone();
      }

      [JavaFlags(4227073)]
      public override string ToString() => ObjectImpl.jloToString((object) this);
    }

    [Inner]
    [JavaFlags(32)]
    [JavaInterfaces("1;java/awt/event/ActionListener;")]
    public class \u0039 : ActionListener
    {
      [EditorBrowsable(EditorBrowsableState.Never)]
      [JavaFlags(32770)]
      private TestRunner this\u00240;

      public virtual void actionPerformed(ActionEvent e) => java.lang.System.exit(0);

      public \u0039(TestRunner _param1)
      {
        this.this\u00240 = _param1;
        if (_param1 != null)
          return;
        ObjectImpl.getClass((object) _param1);
      }

      [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
      [JavaFlags(4227077)]
      public new virtual object MemberwiseClone()
      {
        TestRunner.\u0039 obj = this;
        ObjectImpl.clone((object) obj);
        return ((object) obj).MemberwiseClone();
      }

      [JavaFlags(4227073)]
      public override string ToString() => ObjectImpl.jloToString((object) this);
    }

    [JavaFlags(32)]
    [Inner]
    public class \u00310 : Thread
    {
      [EditorBrowsable(EditorBrowsableState.Never)]
      [JavaFlags(32770)]
      private TestRunner this\u00240;
      [JavaFlags(16)]
      public readonly Test testSuite_\u003E;

      public virtual void run()
      {
        this.this\u00240.fTestResult = this.this\u00240.createTestResult();
        this.this\u00240.fTestResult.addListener((TestListener) this.this\u00240);
        this.this\u00240.fProgressIndicator.start(this.testSuite_\u003E.countTestCases());
        this.this\u00240.showInfo("Running...");
        long num = java.lang.System.currentTimeMillis();
        this.testSuite_\u003E.run(this.this\u00240.fTestResult);
        if (this.this\u00240.fTestResult.shouldStop())
        {
          this.this\u00240.showStatus("Stopped");
        }
        else
        {
          long runTime = java.lang.System.currentTimeMillis() - num;
          this.this\u00240.showInfo(new StringBuffer().append("Finished: ").append(this.this\u00240.elapsedTimeAsString(runTime)).append(" seconds").ToString());
        }
        this.this\u00240.fTestResult = (TestResult) null;
        this.this\u00240.fRun.setLabel("Run");
        this.this\u00240.fRunner = (Thread) null;
        java.lang.System.gc();
      }

      public \u00310(TestRunner _param1, [In] Test obj1)
      {
        this.this\u00240 = _param1;
        if (_param1 == null)
          ObjectImpl.getClass((object) _param1);
        this.testSuite_\u003E = obj1;
      }

      [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
      [JavaFlags(4227077)]
      public virtual object MemberwiseClone()
      {
        TestRunner.\u00310 obj = this;
        ObjectImpl.clone((object) obj);
        return ((object) obj).MemberwiseClone();
      }
    }
  }
}
