Imports org.nakedobjects.object.fixture
Imports sdm.systems.application.time.programmable
Imports sdm.systems.application.container
Imports sdm.systems.application.services.api.security.authorisation
Imports sdm.systems.application.services.api.configurationservice
Imports sdm.systems.application.services.api.security.certification
Imports sdm.systems.application.services.api.security.authentication
Imports sdm.systems.reflector.container
Imports sdm.utilities

'Imports sdm.common.bom.schemes.repository
'Imports sdm.common.bom.workflow.repository

'Imports sdm.common.bom.businessactivity.repository

#Region "Services Imports"

'Imports sdm.services.shared
'Imports sdm.services.noop.security.certification

#End Region

Public MustInherit Class SdmFixtureAdapter
    Implements ExplorationFixture
    Implements IAuthenticationServiceAware
    Implements ICertificationServiceAware
    Implements IConfigurationServiceAware
    Implements IAuthorisationServiceAware
    'Implements ICustomerCaseRepositoryAware
    'Implements ISchemeRepositoryAware
    Implements IContainerAware
    'Implements IBusinessActivityRepositoryAware

    '*
    ' As a convenience to subclasses, expose the Container's
    ' <code>createTransientInstance</code> method directly.
    Public Function createTransientInstance(ByVal systype As Type) As Object
        Return AppContainer.createTransientInstance(systype)
    End Function


    ''*
    '' Simply returns an empty password-based Identity for current user.
    ''
    '' <p>
    '' Hard-coded to work with no-op certificate manager only.
    ''
    'Protected Function joesPassword() As Identity

    '    If Not TypeOf Me.CertificateManager Is _
    '        sdm.services.noop.security.certification.CertificateManager Then
    '        Throw New SystemException("Only designed to work with no-op Cert Manager")
    '    End If

    '    Dim id As New Identity
    '    id.setValue("abcD5678iJkL") ' still needs to pass BusinessObject#isValidIdentity


    '    Return id

    'End Function


    '*
    ' Checks that the configured clock (as referenced by the
    ' application container) is a programmable clock, and returns.
    '
    ' <p>
    ' If any other implementation of clock is configured, then a
    ' SystemException is thrown.
    '
    Protected Function ProgrammableClock() As ProgrammableClock
        If Not TypeOf Me.AppContainer.Clock Is ProgrammableClock Then
            Throw New SystemException("ProgrammableClock is not configured")
        End If
        Return DirectCast(Me.AppContainer.Clock, ProgrammableClock)
    End Function


#Region "Access to helpers (in utilities)"

    Protected Shared Function th() As TypeHelper
        Return New TypeHelper
    End Function

    Protected Shared Function fh() As FileHelper
        Return New FileHelper
    End Function

    Protected Shared Function xh() As XmlHelper
        Return New XmlHelper
    End Function

    Protected Shared Function sh() As StringHelper
        Return New StringHelper
    End Function

    Protected Shared Function eg() As ExceptionHelper
        Return New ExceptionHelper
    End Function

    Protected Shared Function cnh() As ClassNameHelper
        Return New ClassNameHelper
    End Function

#End Region

#Region "exploration fixture"

    'Private myContainer As Object
    'Private Function getContainer() As Object
    '    Return myContainer
    'End Function
    Public Overridable Sub setContainer(ByVal container As Object) Implements ExplorationFixture.setContainer
        ' ac/TD_SYS_R5#966: get services via service locator instead of injection.
        'myContainer = container
    End Sub

    Private myFixtureBuilder As FixtureBuilder

    Public Function getBuilder() As FixtureBuilder Implements ExplorationFixture.getBuilder
        Return myFixtureBuilder
    End Function

    Public Sub setBuilder(ByVal builder As FixtureBuilder) Implements ExplorationFixture.setBuilder
        myFixtureBuilder = builder
    End Sub


    Public MustOverride Sub install() Implements ExplorationFixture.install

    Public Overridable Function requiresTransaction() _
        As Boolean _
        Implements ExplorationFixture.requiresTransaction
        Return True
    End Function

#End Region

    Protected Function TS(ByVal str As String) As TextString
        Return New TextString(Nothing, str)
    End Function

    Protected Function MLTS(ByVal str As String) As MultilineTextString
        Return New MultilineTextString(Nothing, str)
    End Function

    Protected Function WN(ByVal i As Integer) As WholeNumber
        Return New WholeNumber(Nothing, i)
    End Function

    Protected Function LOG(ByVal b As Boolean) As Logical
        Return New Logical(Nothing, b)
    End Function

    Protected Function emptyWholeNumber() As WholeNumber
        Dim wn As New WholeNumber(Nothing)
        wn.clear()
        Return wn
    End Function

    Protected Function emptyMLTS() As MultilineTextString
        Dim mlts As New MultilineTextString(Nothing)
        mlts.clear()
        Return mlts
    End Function

    Protected Function emptyTextString() As TextString
        Dim ts As New TextString(Nothing)
        ts.clear()
        Return ts
    End Function

#Region "Containers, Repositories, Services"

    'Private myAppContainer As IContainer
    Public Property AppContainer() _
        As IContainer _
        Implements IContainerAware.Container
        Get
            ' ac/TD_SYS_R5#966: get services via service locator instead of injection.
            Return DirectCast(ServiceLocatorFactory.ServiceLocator.SdmBusinessObjectContainer, IContainer)

            '' populated the conventional means
            'Dim asPopulatedByFramework As IContainer = _
            '    CType(getContainer(), IContainer)
            'If Not asPopulatedByFramework Is Nothing Then _
            '    Return asPopulatedByFramework

            '' if injected second hand, by SdmCompositeFixture
            'Return myAppContainer
        End Get
        Set(ByVal Value As IContainer)
            'myContainer = Value
        End Set
    End Property

    '*
    ' Figures out whether we have derived our remaining dependencies.
    '
    ' <p>
    ' The framework furnishes the fixture with its container, but nothing else. 
    ' In particular, the fixture's dependencies will not have been injected.  So
    ' this method helps our accessors determine if they need to self-inject (so
    ' to speak).
    '
    ' <p>
    ' Another option would have been to call
    ' <code>Container.injectDependencies(Me)</code> in the 
    ' <code>install()</code> method.  However, that would have required all
    ' subclasses to call <code>MyBase.install()</code>.  So instead, we lazily
    ' check to see if our dependencies have been satisfied them, and inject 
    ' them ourselves (from the container which we know) if they have not been).
    '
    ' ac/TD_SYS_R5#966: get services via service locator instead of injection.
    'Private Function dependenciesSelfInjected() As Boolean
    '    Return Not mySecurityManager Is Nothing AndAlso _
    '           Not myAuthenticationManager Is Nothing AndAlso _
    '           Not myCertificateManager Is Nothing AndAlso _
    '           Not myConfiguration Is Nothing
    'End Function

    'Private mySecurityManager As IAuthorisationService
    Public Property AuthorisationService() _
        As IAuthorisationService _
        Implements IAuthorisationServiceAware.AuthorisationService
        Get
            'If Not dependenciesSelfInjected() Then _
            '    AppContainer.injectDependencies(Me)
            'Return mySecurityManager

            ' ac/TD_SYS_R5#966: get services via service locator instead of injection.
            Return DirectCast(ServiceLocatorFactory.ServiceLocator.AuthorisationService, IAuthorisationService)
        End Get
        Set(ByVal Value As IAuthorisationService)
            'mySecurityManager = Value
        End Set
    End Property

    'Private myAuthenticationManager As IAuthenticationManager
    Public Property AuthenticationManager() _
        As IAuthenticationManager _
        Implements IAuthenticationServiceAware.AuthenticationManager
        Get
            'If Not dependenciesSelfInjected() Then _
            '    AppContainer.injectDependencies(Me)
            'Return myAuthenticationManager

            ' ac/TD_SYS_R5#966: get services via service locator instead of injection.
            Return DirectCast(ServiceLocatorFactory.ServiceLocator.AuthenticationManager, IAuthenticationManager)
        End Get
        Set(ByVal Value As IAuthenticationManager)
            'myAuthenticationManager = Value
        End Set
    End Property


    'Private myCertificateManager As ICertificateManager
    Public Property CertificateManager() _
        As ICertificateManager _
        Implements ICertificationServiceAware.CertificateManager
        Get
            'If Not dependenciesSelfInjected() Then _
            '    AppContainer.injectDependencies(Me)
            'Return myCertificateManager

            ' ac/TD_SYS_R5#966: get services via service locator instead of injection.
            Return DirectCast(ServiceLocatorFactory.ServiceLocator.CertificateManager, ICertificateManager)
        End Get
        Set(ByVal Value As ICertificateManager)
            'myCertificateManager = Value
        End Set
    End Property

    'Private myConfiguration As IConfiguration
    Public Property Configuration() _
        As IConfiguration _
        Implements IConfigurationServiceAware.Configuration
        Get
            'If Not dependenciesSelfInjected() Then _
            '    AppContainer.injectDependencies(Me)
            'Return myConfiguration

            ' ac/TD_SYS_R5#966: get services via service locator instead of injection.
            Return DirectCast(ServiceLocatorFactory.ServiceLocator.Service(GetType(IConfiguration)), IConfiguration)
        End Get
        Set(ByVal Value As IConfiguration)
            'myConfiguration = Value
        End Set
    End Property

    ''Private myCustomerCaseRepository As ICustomerCaseRepository
    'Public Property CustomerCaseRepository() As ICustomerCaseRepository _
    '        Implements ICustomerCaseRepositoryAware.CustomerCaseRepository
    '    Get
    '        'Return myCustomerCaseRepository

    '        ' ac/TD_SYS_R5#966: get services via service locator instead of injection.
    '        Return DirectCast(sdm.systems.application.ServiceLocatorFactory.ServiceLocator.Repository(GetType(ICustomerCaseRepository)), ICustomerCaseRepository)
    '    End Get
    '    Set(ByVal Value As ICustomerCaseRepository)
    '        'myCustomerCaseRepository = Value
    '    End Set
    'End Property

    ''Private mySchemeRepository As ISchemeRepository
    'Public Property SchemeRepository() As ISchemeRepository _
    '        Implements ISchemeRepositoryAware.SchemeRepository
    '    Get
    '        'Return mySchemeRepository

    '        ' ac/TD_SYS_R5#966: get services via service locator instead of injection.
    '        Return DirectCast(sdm.systems.application.ServiceLocatorFactory.ServiceLocator.Repository(GetType(ISchemeRepository)), ISchemeRepository)

    '    End Get
    '    Set(ByVal Value As ISchemeRepository)
    '        'mySchemeRepository = Value
    '    End Set
    'End Property

    'Public Property BusinessActivityRepository() As IBusinessActivityRepository _
    '        Implements IBusinessActivityRepositoryAware.BusinessActivityRepository
    '    Get

    '        Return DirectCast(sdm.systems.application.ServiceLocatorFactory.ServiceLocator.Repository(GetType(IBusinessActivityRepository)), IBusinessActivityRepository)
    '    End Get
    '    Set(ByVal value As IBusinessActivityRepository)

    '    End Set
    'End Property

#End Region
End Class
