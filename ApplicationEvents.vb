﻿Namespace My
    ' The following events are available for MyApplication:
    ' Startup: Raised when the application starts, before the startup form is created.
    ' Shutdown: Raised after all application forms are closed.  This event is not raised if the application terminates abnormally.
    ' UnhandledException: Raised if the application encounters an unhandled exception.
    ' StartupNextInstance: Raised when launching a single-instance application and the application is already active. 
    ' NetworkAvailabilityChanged: Raised when the network connection is connected or disconnected.
    Partial Friend Class MyApplication

        Private Sub AppStart(ByVal sender As Object, ByVal e As Microsoft.VisualBasic.ApplicationServices.StartupEventArgs) Handles Me.Startup
            AddHandler AppDomain.CurrentDomain.AssemblyResolve, AddressOf ResolveAssemblies
        End Sub


        Private Function ResolveAssemblies(ByVal sender As Object, ByVal e As System.ResolveEventArgs) As Reflection.Assembly
            Dim desiredAssembly = New Reflection.AssemblyName(e.Name)
            If desiredAssembly.Name = "Microsoft.WindowsAPICodePack.Shell" Then
                Return Reflection.Assembly.Load(My.Resources.Microsoft_WindowsAPICodePack_Shell) 'replace with your assembly's resource name
            ElseIf desiredAssembly.Name = "Microsoft.WindowsAPICodePack" Then
                Return Reflection.Assembly.Load(My.Resources.Microsoft_WindowsAPICodePack) 'replace with your assembly's resource name
            Else
                Return Nothing
            End If
        End Function
    End Class
End Namespace
