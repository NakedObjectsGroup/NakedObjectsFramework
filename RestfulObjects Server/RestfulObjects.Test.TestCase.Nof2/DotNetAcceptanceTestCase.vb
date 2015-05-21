Option Strict On

Imports org.nakedobjects.object.repository
Imports sdm.systems.reflector.container
Imports sdm.systems.application.container
Imports sdm.systems.application
Imports org.nakedobjects.object.fixture
Imports Spring.Context
Imports NUnit.Framework
Imports org.nakedobjects.object
Imports org.nakedobjects.xat
Imports sdm.utilities
Imports Spring.Context.Support
Imports sdm.systems.application.value

'Imports org.apache.log4j


Imports Nfx = NUnit.Framework

'*
' Moved from nakedobjects.dotnet.
'
' <p>
' TODO: merge with SdmAcceptanceTestCase.
<TestFixture()>
Public MustInherit Class DotNetAcceptanceTestCase
    ' Inherits TestCase

    Protected Shared NO_PARAMTERS(0) As TestNaked

#Region "constructors"

    Public Sub New()
        Me.new("(no name)")
    End Sub


    '*
    ' Looks up container using "SdmBusinessObjectContainer", and test object
    ' factory using "TestObjectFactory".
    Public Sub New(ByVal name As String)
        Me.New(name, "SdmBusinessObjectContainer", "TestObjectFactory", "objects.xml")
    End Sub

    '*
    ' Looks up container using "SdmBusinessObjectContainer", and test object
    ' factory using "TestObjectFactory".
    Public Sub New(ByVal name As String, ByVal springConfigFile As String)
        Me.New(name, "SdmBusinessObjectContainer", "TestObjectFactory", springConfigFile)
    End Sub

    Public Sub New(ByVal name As String,
                   ByVal containerId As String, ByVal testObjectFactoryId As String,
                   ByVal springConfigFile As String)
        'MyBase.new(name)
        SetUpWorkingFolder()
        'startLogging()
        Me.myContainerId = containerId
        Me.myTestObjectFactoryId = testObjectFactoryId
        Me.mySpringConfigFile = springConfigFile
    End Sub


    Private myContainerId As String
    Private myTestObjectFactoryId As String
    Private mySpringConfigFile As String

    Private nakedObjects As NakedObjects

    Private time As Double

#Region "Set Up Working FOlder"

    Private Sub SetUpWorkingFolder()
        'If Not String.IsNullOrEmpty(GetRunningFolderToSet()) Then
        '    TestRunner.WorkingFolder = Path.GetFullPath(GetRunningFolderToSet())
        'Else
        '    TestRunner.UseDefaultWorkingFolder()
        'End If
    End Sub

    'Private Function GetRunningFolderToSet() As String
    '    Return ConfigurationSettings.AppSettings("WorkingFolder")
    'End Function

#End Region

#End Region

#Region "Get Test Name"

    'Public Function getName() As String
    '    Return TestRunner.GetRunningTestName(MyBase.getName())
    'End Function

    'Private Shared myTestRunnerPlatform As ITestRunnerPlatform
    'Private Shared ReadOnly Property TestRunner() As ITestRunnerPlatform
    '    Get
    '        If myTestRunnerPlatform Is Nothing Then _
    '            myTestRunnerPlatform = TestRunnerPlatform.GetCurrentPlatform()

    '        Return myTestRunnerPlatform
    '    End Get
    'End Property

#End Region

#Region "setUp() and tearDown()"

    '*
    ' Subclasses can override, but must delegate upwards (using MyBase) to
    ' setup the No framework itself.

    ' <Nfx.SetUp()> _
    Public Sub TheSetup()
        SetUp()
        doAfterSetup()
        ' ac/wi#13010 : Added do method hook to override in subclass.
    End Sub

    ' hook method that can be used by tests to add logici after all setup is completed.
    Public Overridable Sub doAfterSetup() ' ac/wi#13010 : Added hook method to be overriden by subclasses.
    End Sub

    Public Sub SetUp()

        Try
            initSpringContext()
            InitializeNOF()
            initTestObjectFactory(myTestObjectFactoryId)
            initContainer(myContainerId)

            setUpContainer()
            setUpFixtures()
            installFixtures()

            setUpTestClasses()
        Catch ex As Exception
            ExceptionHelper.dumpException(ex)
            Throw
        End Try
    End Sub


    '*
    ' Tears down the session, factory and documentor.
    '
    ' <p>
    ' A new Spring context is created in each setup, so the old NOF container
    ' is simply discarded through garbage collection.
    '
    ' <p>
    ' Subclasses should override if any static (shared) variables are setup, 
    ' blanking them out.  They should then delegate upwards using
    ' <code>MyBase.tearDown()</code>.
    '

    ' EA0805 - Ensure that the teardown is called
    ' <Nfx.TearDown()> _
    <DebuggerStepThrough()>
    Public Sub TheTearDown()
        TearDown()
    End Sub

    Public Sub tearDown()
        'stopDocumenting()

        '----------------------------------
        ' Reset all IResettable components
        '----------------------------------
        '-------------------------------------------------------
        ' RESET components from CLIENT context
        '-------------------------------------------------------
        Dim clientContext As XmlApplicationContext = ClientSpringContext.SharedClientSpringContext
        If Not clientContext Is Nothing Then
            ' reset all components (services and repositories etc) that indicate that they are resettable.
            Dim resettableObjectNames() As String =
                    clientContext.GetObjectNamesForType(GetType(IResettable))
            For Each resettableObjectName As String In resettableObjectNames
                Dim resettable As IResettable = DirectCast(clientContext.GetObject(resettableObjectName), IResettable)
                resettable.reset()
            Next
        End If

        '-----------------------------
        ' shut down NakedObjects 
        ' ac/TODO: check if nakedobjects will be shutdown by Reset loop
        '-----------------------------
        nakedObjects.shutdown()
        nakedObjects.reset()

        DirectCast(myContainer, SdmBusinessObjectContainer).resetStaticHash()
        'myTestObjectFactory.testEnding()
        'myDocumentor.stop()
        'myDocumentor = Nothing

        'myClasses.Clear()
        myTestObjectFactory = Nothing
    End Sub

#End Region

#Region "Helper methods to bootstrap NOF"

    Private Shared myCtx As IConfigurableApplicationContext
    Private Shared myContextInitialized As Boolean
    '*
    ' Instantiate from XML file rather than App.Config since need a complete
    ' new container each time.
    '
    ' <p>
    ' Also, get the opportunity to apply XSD schema files.
    Protected Sub initSpringContext()

        ' TJ: Returning if (myContextInitialized=true) means that spring wont be reinitialised even though the NakedObjects
        ' was reset in the teardown. This was cauing a null pointer exception on the next test fixture setup as NakedObjects 
        ' persistor is null. Commenting out for now to run tests.
        'If myContextInitialized Then Return

        ' Nullify SharedClientSpringContext /LocatorFactory so that during Spring configuration if we get a call to ServiceLocatorFactory
        ' for a component, we wont get stale objects from the previous test.
        ClientSpringContext.SharedClientSpringContext = Nothing
        ServiceLocatorFactory.ServiceLocator = Nothing

        myCtx = New XmlApplicationContext(mySpringConfigFile)

        ' ac: Store Client Spring context in easily accessible Shared property
        ClientSpringContext.SharedClientSpringContext = DirectCast(myCtx, XmlApplicationContext)

        ' ac: TD_SYS_R5#966: Get ServicLocator from context & set it on ServiceLocatorFactory.
        Dim serverSideServiceLocator As IServiceLocator = DirectCast(myCtx.GetObject("ServiceLocator",
                                                                                     GetType(IServiceLocator)),
                                                                     IServiceLocator)
        ServiceLocatorFactory.ServiceLocator = serverSideServiceLocator
        myContextInitialized = True
    End Sub

    Private Sub EnsureContextInitialized()
        If Not myCtx Is Nothing Then Return
        initSpringContext()
    End Sub

    'Private Sub startLogging()
    '    PropertyConfigurator.configure("logging.properties")
    '    Logger.getLogger( _
    '        java.lang.Class.FromType(GetType(AcceptanceTestCase))). _
    '            debug("XAT Logging enabled - new test: " + getName())
    'End Sub


    Private myContainer As FixtureBuilder

    '*
    ' Obtains an implementation of the NOF container (ExplorationSetUp).
    '
    ' Typically the implementation will furnish application-specific 
    ' container functionality, eg dependency injection of services.
    Private Sub initContainer(ByVal containerId As String)
        myContainer = CType(myCtx.GetObject(containerId), FixtureBuilder)
    End Sub

    Public ReadOnly Property Container() As FixtureBuilder
        Get
            Return myContainer
        End Get
    End Property


    Private myTestObjectFactory As TestObjectFactory
    'Private myDocumentor As Documentor
    '*
    ' Obtains an implementation of the TestObjectFactory.
    '
    Private Sub initTestObjectFactory(ByVal testObjectFactoryId As String)
        'myTestObjectFactory = CType(myCtx.GetObject(testObjectFactoryId),  _
        '        TestObjectFactory)

        ''myDocumentor = myTestObjectFactory.getDocumentor()
        ''myDocumentor.start()

        'Dim className As String = Me.GetType().FullName
        '' EA070713 - Check if the method name starts with "test"
        'Dim methodName As String = getName()
        'If methodName.ToLower.StartsWith("test") Then methodName = methodName.Substring(4)
        'myTestObjectFactory.testStarting(className, methodName)
    End Sub

    Protected Sub InitializeNOF()
        nakedObjects = DirectCast(myCtx.GetObject("NakedObjects"), NakedObjects)
    End Sub

    Protected Sub EnsureNOFInitialized()
        If Not nakedObjects Is Nothing Then Return
        EnsureContextInitialized()
        InitializeNOF()
    End Sub


    Private myClasses As Hashtable = New Hashtable

    Private Sub setUpTestClasses()

        'Dim noSpecs As NakedObjectSpecification() = _
        '    nakedObjects.getSpecificationLoader().allSpecifications()

        'For Each noSpec As NakedObjectSpecification In noSpecs
        '    Dim noSpecFullName As String = noSpec.getFullName()
        '    If noSpec.isObject() Then
        '        Dim cls As NakedClass = New NakedClassImpl(noSpec.getFullName())
        '        Dim view As TestClass = _
        '            myTestObjectFactory.createTestClass(cls)
        '        myClasses.put(noSpec.getFullName().ToLower(), view)
        '    End If
        'Next
    End Sub

#End Region

#Region "Methods for subclass to control documenting"

    Protected Sub startDocumenting()
        'myDocumentor.start()
    End Sub

    '*
    ' Gives a story a subtitle in the script documentation.
    '
    Protected Sub title(ByVal text As String)
        'myDocumentor.title(text)
    End Sub

    '*
    ' Gives a story a subtitle in the script documentation.
    '
    Protected Sub subtitle(ByVal text As String)
        'myDocumentor.subtitle(text)
    End Sub

    Protected Sub firstStep()
        startDocumenting()
        nextStep()
    End Sub

    Protected Sub firstStep(ByVal text As String)
        startDocumenting()
        nextStep(text)
    End Sub


    '*
    ' Marks the start of a new step within a story. Adds the specified text to
    ' the script documentation, which will then be followed by the generated
    ' text from the action methods.
    '
    Protected Sub nextStep(ByVal text As String)
        'myDocumentor.step(text)
    End Sub


    '*
    ' Marks the start of a new step within a story.
    Protected Sub nextStep()
        'myDocumentor.step("")
    End Sub


    Protected Sub append(ByVal text As String)
        docln(text)
    End Sub

    Protected Sub note(ByVal text As String)
        docln(text)
    End Sub


    Protected Sub stopDocumenting()
        'myDocumentor.stop()
    End Sub

#Region "helper methods"

    Private Sub docln(ByVal str As String)
        'myDocumentor.docln(str)
    End Sub

#End Region


#End Region

#Region "Methods to allow subclass to setup test"

    '*
    ' Called before setUpFixtures()
    Protected MustOverride Sub setUpContainer()

    '*
    ' Called before setUpContainer()
    Protected MustOverride Sub setUpFixtures()

    '*
    ' Adds fixture to the container
    Protected Sub addFixture(ByVal fixture As Fixture)
        myContainer.addFixture(fixture)
    End Sub

    '*
    ' Installs fixture into the container
    Private Sub installFixtures()
        myContainer.installFixtures()
    End Sub

    '*
    ' If a class hasn't been previously registered (using
    ' <code>ExplorationSetUp#registerType(Type)</code>) then the function will
    ' simply return <code>nothing</code>.
    '
    ' <h3>Historical Note</h3>
    ' We used to throw an exception, but a null pointer later on will be
    ' good enough to indicate to the programmer that they've forgotten to do
    ' something.
    'Protected Overloads Function getTestClass(ByVal systype As Type) _
    '        As TestClass

    '    Dim name As String = systype.FullName
    '    Dim view As TestClass = _
    '        CType(myClasses.get(name.ToLower()), TestClass)

    '    Return view
    'End Function

#End Region

#Region "Methods for subclasses to actually implement their tests"

    'Public Function createParameterTestValue(ByVal value As Object) As TestValue
    '    Return myTestObjectFactory.createParamerTestValue(value)
    'End Function

    'Public Function createNullParameter(ByVal systype As Type) As TestNaked
    '    Return New TestNakedNullParameter(systype.FullName)
    'End Function

    'Public Function createTestObject(ByVal obj As Object) As TestObject
    '    Dim no As org.nakedobjects.object.NakedObject
    '    If TypeOf obj Is org.nakedobjects.object.Naked Then
    '        no = DirectCast(obj, org.nakedobjects.object.NakedObject)
    '    ElseIf TypeOf obj Is sdm.systems.application.SdmObject Then
    '        no = nakedObjects.getObjectLoader().getAdapterFor(obj)
    '    End If
    '    If no Is Nothing Then Return Nothing
    '    Return myTestObjectFactory.createTestObject(no)
    'End Function

    '*
    ' All parameters must be either TestObject (representing a referenced
    ' NakedObject, or a NakedValue.
    '
    ' <p>
    ' Behaviour otherwise is not defined.
    '
    Protected Function params(
                              ByVal p0 As Object) As TestNaked()

        Dim param(0) As TestNaked
        param(0) = asTestNaked(p0)

        Return param
    End Function

    '*
    ' All parameters must be either TestObject (representing a referenced
    ' NakedObject, or a NakedValue.
    '
    ' <p>
    ' Behaviour otherwise is not defined.
    '
    Protected Function params(
                              ByVal p0 As Object,
                              ByVal p1 As Object) As TestNaked()

        Dim param(1) As TestNaked
        param(0) = asTestNaked(p0)
        param(1) = asTestNaked(p1)

        Return param
    End Function

    '*
    ' All parameters must be either TestObject (representing a referenced
    ' NakedObject, or a NakedValue.
    '
    ' <p>
    ' Behaviour otherwise is not defined.
    '
    Protected Function params(
                              ByVal p0 As Object,
                              ByVal p1 As Object,
                              ByVal p2 As Object) As TestNaked()

        Dim param(2) As TestNaked
        param(0) = asTestNaked(p0)
        param(1) = asTestNaked(p1)
        param(2) = asTestNaked(p2)

        Return param
    End Function

    '*
    ' All parameters must be either TestObject (representing a referenced
    ' NakedObject, or a NakedValue.
    '
    ' <p>
    ' Behaviour otherwise is not defined.
    '
    Protected Function params(
                              ByVal p0 As Object,
                              ByVal p1 As Object,
                              ByVal p2 As Object,
                              ByVal p3 As Object) As TestNaked()

        Dim param(3) As TestNaked
        param(0) = asTestNaked(p0)
        param(1) = asTestNaked(p1)
        param(2) = asTestNaked(p2)
        param(3) = asTestNaked(p3)

        Return param
    End Function

    '*
    ' All parameters must be either TestObject (representing a referenced
    ' NakedObject, or a NakedValue.
    '
    ' <p>
    ' Behaviour otherwise is not defined.
    '
    Protected Function params(
                              ByVal p0 As Object,
                              ByVal p1 As Object,
                              ByVal p2 As Object,
                              ByVal p3 As Object,
                              ByVal p4 As Object) As TestNaked()

        Dim param(4) As TestNaked
        param(0) = asTestNaked(p0)
        param(1) = asTestNaked(p1)
        param(2) = asTestNaked(p2)
        param(3) = asTestNaked(p3)
        param(4) = asTestNaked(p4)

        Return param
    End Function

    '*
    ' All parameters must be either TestObject (representing a referenced
    ' NakedObject, or a NakedValue.
    '
    ' <p>
    ' Behaviour otherwise is not defined.
    '
    Protected Function params(
                              ByVal p0 As Object,
                              ByVal p1 As Object,
                              ByVal p2 As Object,
                              ByVal p3 As Object,
                              ByVal p4 As Object,
                              ByVal p5 As Object) As TestNaked()

        Dim param(5) As TestNaked
        param(0) = asTestNaked(p0)
        param(1) = asTestNaked(p1)
        param(2) = asTestNaked(p2)
        param(3) = asTestNaked(p3)
        param(4) = asTestNaked(p4)
        param(5) = asTestNaked(p5)

        Return param
    End Function

    '*
    ' All parameters must be either TestObject (representing a referenced
    ' NakedObject, or a NakedValue.
    '
    ' <p>
    ' Behaviour otherwise is not defined.
    '
    Protected Function params(
                              ByVal p0 As Object,
                              ByVal p1 As Object,
                              ByVal p2 As Object,
                              ByVal p3 As Object,
                              ByVal p4 As Object,
                              ByVal p5 As Object,
                              ByVal p6 As Object) As TestNaked()

        Dim param(6) As TestNaked
        param(0) = asTestNaked(p0)
        param(1) = asTestNaked(p1)
        param(2) = asTestNaked(p2)
        param(3) = asTestNaked(p3)
        param(4) = asTestNaked(p4)
        param(5) = asTestNaked(p5)
        param(6) = asTestNaked(p6)

        Return param
    End Function

    '*
    ' All parameters must be either TestObject (representing a referenced
    ' NakedObject, or a NakedValue.
    '
    ' <p>
    ' Behaviour otherwise is not defined.
    '
    Protected Function params(
                              ByVal p0 As Object,
                              ByVal p1 As Object,
                              ByVal p2 As Object,
                              ByVal p3 As Object,
                              ByVal p4 As Object,
                              ByVal p5 As Object,
                              ByVal p6 As Object,
                              ByVal p7 As Object) As TestNaked()

        Dim param(7) As TestNaked
        param(0) = asTestNaked(p0)
        param(1) = asTestNaked(p1)
        param(2) = asTestNaked(p2)
        param(3) = asTestNaked(p3)
        param(4) = asTestNaked(p4)
        param(5) = asTestNaked(p5)
        param(6) = asTestNaked(p6)
        param(7) = asTestNaked(p7)

        Return param
    End Function

    '*
    ' All parameters must be either TestObject (representing a referenced
    ' NakedObject, or a NakedValue.
    '
    ' <p>
    ' Behaviour otherwise is not defined.
    '
    Protected Function params(
                              ByVal p0 As Object,
                              ByVal p1 As Object,
                              ByVal p2 As Object,
                              ByVal p3 As Object,
                              ByVal p4 As Object,
                              ByVal p5 As Object,
                              ByVal p6 As Object,
                              ByVal p7 As Object,
                              ByVal p8 As Object) As TestNaked()

        Dim param(8) As TestNaked
        param(0) = asTestNaked(p0)
        param(1) = asTestNaked(p1)
        param(2) = asTestNaked(p2)
        param(3) = asTestNaked(p3)
        param(4) = asTestNaked(p4)
        param(5) = asTestNaked(p5)
        param(6) = asTestNaked(p6)
        param(7) = asTestNaked(p7)
        param(8) = asTestNaked(p8)

        Return param
    End Function

    '*
    ' All parameters must be either TestObject (representing a referenced
    ' NakedObject, or a NakedValue.
    '
    ' <p>
    ' Behaviour otherwise is not defined.
    '
    Protected Function params(
                              ByVal p0 As Object,
                              ByVal p1 As Object,
                              ByVal p2 As Object,
                              ByVal p3 As Object,
                              ByVal p4 As Object,
                              ByVal p5 As Object,
                              ByVal p6 As Object,
                              ByVal p7 As Object,
                              ByVal p8 As Object,
                              ByVal p9 As Object) As TestNaked()

        Dim param(9) As TestNaked
        param(0) = asTestNaked(p0)
        param(1) = asTestNaked(p1)
        param(2) = asTestNaked(p2)
        param(3) = asTestNaked(p3)
        param(4) = asTestNaked(p4)
        param(5) = asTestNaked(p5)
        param(6) = asTestNaked(p6)
        param(7) = asTestNaked(p7)
        param(8) = asTestNaked(p8)
        param(9) = asTestNaked(p9)

        Return param
    End Function

    '*
    ' All parameters must be either TestObject (representing a referenced
    ' NakedObject, or a NakedValue.
    '
    ' <p>
    ' Behaviour otherwise is not defined.
    '
    Protected Function params(
                              ByVal p0 As Object,
                              ByVal p1 As Object,
                              ByVal p2 As Object,
                              ByVal p3 As Object,
                              ByVal p4 As Object,
                              ByVal p5 As Object,
                              ByVal p6 As Object,
                              ByVal p7 As Object,
                              ByVal p8 As Object,
                              ByVal p9 As Object,
                              ByVal p10 As Object) As TestNaked()

        Dim param(10) As TestNaked
        param(0) = asTestNaked(p0)
        param(1) = asTestNaked(p1)
        param(2) = asTestNaked(p2)
        param(3) = asTestNaked(p3)
        param(4) = asTestNaked(p4)
        param(5) = asTestNaked(p5)
        param(6) = asTestNaked(p6)
        param(7) = asTestNaked(p7)
        param(8) = asTestNaked(p8)
        param(9) = asTestNaked(p9)
        param(10) = asTestNaked(p10)

        Return param
    End Function

    '*
    ' All parameters must be either TestObject (representing a referenced
    ' NakedObject, or a NakedValue.
    '
    ' <p>
    ' Behaviour otherwise is not defined.
    '
    Protected Function params(
                              ByVal p0 As Object,
                              ByVal p1 As Object,
                              ByVal p2 As Object,
                              ByVal p3 As Object,
                              ByVal p4 As Object,
                              ByVal p5 As Object,
                              ByVal p6 As Object,
                              ByVal p7 As Object,
                              ByVal p8 As Object,
                              ByVal p9 As Object,
                              ByVal p10 As Object,
                              ByVal p11 As Object) As TestNaked()

        Dim param(11) As TestNaked
        param(0) = asTestNaked(p0)
        param(1) = asTestNaked(p1)
        param(2) = asTestNaked(p2)
        param(3) = asTestNaked(p3)
        param(4) = asTestNaked(p4)
        param(5) = asTestNaked(p5)
        param(6) = asTestNaked(p6)
        param(7) = asTestNaked(p7)
        param(8) = asTestNaked(p8)
        param(9) = asTestNaked(p9)
        param(10) = asTestNaked(p10)
        param(11) = asTestNaked(p11)

        Return param
    End Function

    '*
    ' All parameters must be either TestObject (representing a referenced
    ' NakedObject, or a NakedValue.
    '
    ' <p>
    ' Behaviour otherwise is not defined.
    '
    Protected Function params(
                              ByVal p0 As Object,
                              ByVal p1 As Object,
                              ByVal p2 As Object,
                              ByVal p3 As Object,
                              ByVal p4 As Object,
                              ByVal p5 As Object,
                              ByVal p6 As Object,
                              ByVal p7 As Object,
                              ByVal p8 As Object,
                              ByVal p9 As Object,
                              ByVal p10 As Object,
                              ByVal p11 As Object,
                              ByVal p12 As Object) As TestNaked()

        Dim param(12) As TestNaked
        param(0) = asTestNaked(p0)
        param(1) = asTestNaked(p1)
        param(2) = asTestNaked(p2)
        param(3) = asTestNaked(p3)
        param(4) = asTestNaked(p4)
        param(5) = asTestNaked(p5)
        param(6) = asTestNaked(p6)
        param(7) = asTestNaked(p7)
        param(8) = asTestNaked(p8)
        param(9) = asTestNaked(p9)
        param(10) = asTestNaked(p10)
        param(11) = asTestNaked(p11)
        param(12) = asTestNaked(p12)

        Return param
    End Function

    Protected Overridable Function asTestNaked(ByVal o As Object) As TestNaked
        If TypeOf o Is TestNaked Then Return CType(o, TestNaked)
        If TypeOf o Is TestObject Then Return CType(o, TestObject)
        Return Nothing
    End Function

#End Region

#Region "Helper methods: Command line parsing"

    '*
    ' Check if command line argument specifies if the XAT suite is to be
    ' run in text mode. Typically this is used in the automated XAT from
    ' the build process
    Protected Shared Function isTextRunnerType(
                                               ByVal CommandLineArguments As String()) As Boolean
        Dim CommandLineArg As String
        Dim CommandLineArgValue As String

        ' parse the command line for the run mode parameter
        For Each CommandLineArg In CommandLineArguments
            If CommandLineArg.Length > 3 Then
                If CommandLineArg.IndexOf("/t:") = 0 Then
                    ' get command line argument value
                    CommandLineArgValue = CommandLineArg.Substring(3)
                    ' set as true if text mode. default is AWT mode
                    Return (CommandLineArgValue.IndexOf("TextMode") = 0)
                End If
            End If
        Next
        Return False
    End Function

#End Region

#Region "Helper method for PPSN as parameter"

    '*
    ' This method was created to facilitate compatibility between existing
    ' XATs and the revised Customer.actionFindByPPSN() method which now uses
    ' a PPSNString.
    Protected Function ppsnParam(ByVal ppsn As String) As TestNaked()
        Return params(New PPSNString(Nothing, ppsn))
    End Function

#End Region

#Region "Message Broker & User Warnings"

    Protected Sub AssertUserWarningDisplayed(ByVal warning As String)
        Dim warnings As String() = MessageBroker.getWarnings()
        Assert.IsTrue(warnings.Length > 0,
                      String.Format("Expected user Warning '{0}' but no warning was displayed", warning))
        Assert.IsTrue(WarningExists(warnings, warning), String.Format(
            "Expected user Warning '{0}' but got '{1}'", warning, warnings(0)))
    End Sub

    Protected Sub AssertUserWarningNotDisplayed(ByVal warning As String)
        Dim warnings As String() = MessageBroker.getWarnings()
        For Each displayedWarning As String In warnings
            If displayedWarning = warning Then _
                Assert.Fail(String.Format("Unexpected warning '{0}' was displayed.", warning))
        Next
    End Sub

    Protected Sub AssertNoWarningDisplayed()
        Dim warnings As String() = MessageBroker.getWarnings()
        For Each displayedWarning As String In warnings
            Assert.Fail(String.Format("Unexpected warning '{0}' was displayed.", displayedWarning))
        Next
    End Sub


    Private Function WarningExists(ByVal warnings As String(), ByVal target As String) As Boolean
        For Each warning As String In warnings
            If warning = target Then Return True
        Next
        Return False
    End Function

    Private ReadOnly Property MessageBroker() As MessageBroker
        Get
            Return nakedObjects.getMessageBroker()
        End Get
    End Property

#End Region

#Region "NOF Transaction Management"

    Protected Sub StartTran()
        NakedObjectsServer.getObjectPersistor().startTransaction()
    End Sub

    Protected Sub EndTran()
        NakedObjectsServer.getObjectPersistor().endTransaction()
    End Sub

#End Region
End Class
