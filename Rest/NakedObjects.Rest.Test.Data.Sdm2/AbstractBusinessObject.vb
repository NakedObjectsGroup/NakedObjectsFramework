Option Strict On

Imports sdm.systems.application.container
Imports sdm.systems.application.persistence
Imports sdm.systems.application.control
Imports sdm.systems.application.action


Namespace RestfulObjects.Test.Data
    <Serializable()>
    Public MustInherit Class AbstractBusinessObject
        Inherits SdmObject

#Region "field order, action order, Menu items"

        Protected Shared Function fieldOrder(ByVal subClassFields As String) _
            As String
            Return subClassFields
        End Function

        Protected Shared Function actionOrder(ByVal subClassActions As String) _
            As String
            Return "Save, " &
                   subClassActions
        End Function

        Public Shared Shadows Function classActionOrder(
                                                        ByVal subClassActions As String) As String

            Return subClassActions
        End Function

        Protected Overloads Shared Sub addMenuItem(ByVal sm As SubMenu, ByVal menuName As String)
            If sm Is Nothing Then Return
            sm.addMenuItem(menuName)
        End Sub

        Protected Overloads Shared Sub addMenuItem(ByVal mm As MainMenu, ByVal menuName As String)
            If mm Is Nothing Then Return
            mm.addMenuItem(menuName)
        End Sub

#End Region


#Region "Last Updated By ID (invisible)"

        Public Overridable Sub aboutLastUpdatedByID(ByVal a As FieldAbout)


            a.invisible()
        End Sub

#End Region

#Region "action: save, makePersistent"

        '*
        ' Simply makes the object persistent.
        Public Overridable Sub actionSave()
            makePersistent()
        End Sub

        '*
        ' Invisible if the object is already persistent
        Public Overridable Sub aboutActionSave(ByVal a As ActionAbout)
            If isPersistent() Then
                a.unusable("This object has already been saved and can be safely closed")
                a.invisible()
            End If
            If TypeOf Me Is ITransient Then
                a.invisible()
            End If
        End Sub

        '*
        ' Programmatic interface that checks to see if the object is persistent, and if not, then saves it.
        Public Sub ensureSaved()
            If Not isPersistent() Then
                actionSave()
            End If
        End Sub

        Public Sub ensureSaved(ByVal tag As String)
            makePersistent(tag)
        End Sub

        '*
        ' Makes the objects persistent
        Public Overloads Overrides Sub makePersistent()
            makePersistent(Nothing)
        End Sub

        Public Overloads Overrides Sub makePersistent(ByVal tag As String)

            If TypeOf Me Is ITransient Then
                Throw New Exception("Cannot make a Transient-Only object persistent")
            End If

            LastActivity.setValue(Container.Clock.Now)
            MyBase.makePersistent(tag)
        End Sub

#End Region


#Region "User Warning, throwError"

        Public Shared Sub userWarning(ByVal msg As String)
            '[JC] Live #1171
            'The framework ignores repeated newline tokenizers, so inject a space before them
            msg = msg.Replace(vbCrLf, " " + vbCrLf).Trim()
            If msg.IndexOf(":"c) = 0 Then
                'The framework uses the text before the first occurance of ":" as the heading
                msg = "Warning:" + msg
                'use 'Warning' as the default heading of the Error message
            End If
            ThreadLocals.Container.addWarningToBroker(msg)
        End Sub

        Protected Shared Sub userError(ByVal msg As String)
            msg = msg.Replace(vbCrLf, " " + vbCrLf).Trim()

            '[MS] The warnings collection are being cleared here for the following reason.
            'If a call to userError occurs after a call to userWarning, then the user warning does not appear on the screen
            'and the original warning message remains in the warnings collection. A subsequent call to userWarning as part of a different process, etc 
            'will then result in two warning messages appearing to the user, the new user warning along with the old warning message which still
            'resides in the warning collection. When a userError is recorded we clear the warning collection so the user is not confused with
            'warning messages that may not relate to an action carried out by them.
            ThreadLocals.Container.clearWarnings()

            Throw New SdmUserException(msg)
        End Sub

        Protected Shared Sub throwError(ByVal msg As String)
            '[JC] Live #1171
            'The framework ignores repeated newline tokenizers, so inject a space before them
            msg = msg.Replace(vbCrLf, " " + vbCrLf).Trim()

            Throw New SystemException(msg)
        End Sub

#End Region
    End Class
End Namespace