'INSTANT VB NOTE: This code snippet uses implicit typing. You will need to set 'Option Infer On' in the VB file or set 'Option Infer' at the project level.

' Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
' All Rights Reserved. This code released under the terms of the 
' Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 
Namespace RestfulObjects.Test.Data
    Public Class RestDataFixture
        Inherits SdmFixtureAdapter

        Public Overrides Sub install()

            Dim ms1 = MostSimple.SetUp(1)
            Dim ms2 = MostSimple.SetUp(2)

            Dim wr1 = WithReference.SetUp(1, ms1, ms1)
            Dim wr2 = WithReference.SetUp(2, ms1, ms1)

            Dim wv1 = WithValue.SetUp(1, 100, 200)

            Dim wa1 = WithActionObject.SetUp(1)

            Dim wc1 = WithCollection.SetUp(1)
            wc1.ACollection.add(ms1)
            wc1.ACollection.add(ms2)
            wc1.ADisabledCollection.add(ms1)
            wc1.ADisabledCollection.add(ms2)
            wc1.AHiddenCollection.add(ms1)
            wc1.AHiddenCollection.add(ms2)

            Dim we1 = WithError.SetUp(1)

            Dim wge1 = WithGetError.SetUp(1)

            Dim i1 = Immutable.SetUp(1)
        End Sub
    End Class
End Namespace
