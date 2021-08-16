Imports System.IO
Imports Microsoft.WindowsAPICodePack
Imports Microsoft.WindowsAPICodePack.Shell
Imports Microsoft.WindowsAPICodePack.Taskbar
Public Class Form1

    Dim args As String() = Environment.GetCommandLineArgs()

    'Private Minimode As ThumbnailToolbarButton

    'Keep a reference to the Taskbar instance
    Private windowsTaskbar As TaskbarManager = TaskbarManager.Instance

    Private WithEvents tmr As New Timer
    Private currentValue As Integer = 0
    Private Var_JumpListItems_Maximum As Integer = 0
    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles Me.Load

        'get or set JumpListItems_Maximum from Reg
        If My.Computer.Registry.GetValue("HKEY_CURRENT_USER\SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\Advanced", "JumpListItems_Maximum", Nothing) Is Nothing Then
            My.Computer.Registry.CurrentUser.CreateSubKey("SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\Advanced\JumpListItems_Maximum")
            My.Computer.Registry.SetValue("HKEY_CURRENT_USER\SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\Advanced", "JumpListItems_Maximum", 35, Microsoft.Win32.RegistryValueKind.DWord)
            Var_JumpListItems_Maximum = 35
        Else
            Var_JumpListItems_Maximum = My.Computer.Registry.GetValue("HKEY_CURRENT_USER\SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\Advanced", "JumpListItems_Maximum", 35)
        End If
        TbListMax.Text = Var_JumpListItems_Maximum

        'Dim MyFile As String = System.IO.Path.GetDirectoryName(args(0)) & "\jump_list.txt"
        'If System.IO.File.Exists(MyFile) = True Then
        '    Button3_Click()
        '    'MsgBox("Done")
        '    Application.Exit()
        'End If
    End Sub
    Private Sub Form1_Shown(sender As Object, e As EventArgs) Handles MyBase.Shown
        'needes to show form befor it is able to add jumplists
        If Environment.GetCommandLineArgs().Count > 1 Then
            If Environment.GetCommandLineArgs(1) = "update" Then Call Button3_Click()
        End If


        'Minimode = New ThumbnailToolbarButton(My.Resources.Tech_Icon_32, "Next Image")
        'AddHandler Minimode.Click, AddressOf Minimode_Click

        'TaskbarManager.Instance.ThumbnailToolbars.AddButtons(Me.Handle, Minimode)
        ' windowsTaskbar.SetProgressState(TaskbarProgressBarState.NoProgress)
        'tmr.Interval = 500
        'tmr.Start()
    End Sub
    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        Me.Text = ""
        Dim MyFile As String = System.IO.Path.GetDirectoryName(args(0)) & "\jump_list.txt"
        If System.IO.File.Exists(MyFile) = True Then
            Process.Start(MyFile)
        Else
            'MsgBox("jump_list.txt Does Not Exist!!", MsgBoxStyle.Critical)
            Dim newfile As System.IO.StreamWriter
            newfile = My.Computer.FileSystem.OpenTextFileWriter(MyFile, True)
            newfile.WriteLine("CategoryName=Enter Name")
            newfile.WriteLine("Display Name=Link to file")
            newfile.WriteLine("Sample internet link=https://translate.google.com/")
            newfile.WriteLine("sample program=Link|Argument if needed")
            newfile.WriteLine("Sample Cmd command=cmd.exe|/k systeminfo")
            newfile.WriteLine()
            newfile.WriteLine("CategoryName=Jump List")
            newfile.WriteLine("JumpList File=C:\Your File location\jump_list.txt")
            newfile.WriteLine("JumpList Update=D:\Your Exe location\Jump_List.exe|update")
            newfile.WriteLine()
            newfile.WriteLine("CategoryName=leave Last Category Empty")
            newfile.Close()
            Process.Start(MyFile)
        End If
    End Sub
    Private Sub Button3_Click() Handles Button3.Click


        Dim MyFile As String = System.IO.Path.GetDirectoryName(args(0)) & "\jump_list.txt"
        If System.IO.File.Exists(MyFile) = False Then
            MsgBox("jump_list.txt Does Not Exist!!", MsgBoxStyle.Critical)
            Exit Sub
        End If

        'anble recent Jump Lists items in Registry
        If My.Computer.Registry.GetValue("HKEY_CURRENT_USER\SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\Advanced", "Start_TrackDocs", Nothing) Is Nothing Then _
            My.Computer.Registry.CurrentUser.CreateSubKey("SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\Advanced\Start_TrackDocs")
        My.Computer.Registry.SetValue("HKEY_CURRENT_USER\SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\Advanced", "Start_TrackDocs", 1, Microsoft.Win32.RegistryValueKind.DWord)


        Dim JList As JumpList
        JList = JumpList.CreateJumpList()
        JList.ClearAllUserTasks()
        Dim Category As JumpListCustomCategory
        Dim Link As JumpListLink


        For Each Line As String In File.ReadLines(MyFile)
            If String.IsNullOrWhiteSpace(Line) Then GoTo A
            Dim Split_string As String() = Line.Split("="c)
            ' Debug.Print(Split_string(1).Split("|"c).Length.ToString & " - " & Split_string2(0))
            If Split_string(0).Contains("CategoryName") = True Then
                If Not Category Is Nothing Then JList.AddCustomCategories(Category)
                Category = New JumpListCustomCategory(Split_string(1))
                GoTo A
            End If
            If Split_string(1).Split("|"c).Length > 1 Then  ' has a Argument after |
                Link = New JumpListLink(Split_string(1).Split("|"c)(0), Split_string(0)) With {.Arguments = Split_string(1).Split("|"c)(1)}
            Else
                Link = New JumpListLink(Split_string(1), Split_string(0)) 'With {.IconReference = New IconReference("Notepad.exe", 0)}
            End If
            Category.AddJumpListItems(Link)
A:
        Next

        'to control which category Recent/Frequent is displayed 
        'JList.KnownCategoryToDisplay = JumpListKnownCategoryType.Frequent
        'Dim Link0 As New JumpListLink("cmd.exe", "Cmd") With {.IconReference = New IconReference("C:\Program Files\Microsoft Office\Root\VFS\Windows\Installer\{90160000-000F-0000-1000-0000000FF1CE}\xlicons.exe", 1)}
        'Dim Link1 As New JumpListLink("Calc.exe", "Calculator") With {.IconReference =
        '  New IconReference("Calc.exe", 0)}
        'JList.AddUserTask(Link0)
        'JList.AddUserTasks(New JumpListSeparator())
        'JList.AddUserTasks(Link1)

        JList.Refresh()

        If Environment.GetCommandLineArgs().Count > 1 Then
            If Environment.GetCommandLineArgs(1) = "update" Then ' this will close form after update
                'Me.Hide()
                tmr.Interval = 800 ' will close 
                tmr.Start()
            End If
        End If
        Me.Text = "Jump List Was Updated!"
        Label2.Text = "Updated At:" & DateAndTime.Now.ToString("h:mm:ss")
        'Application.Exit()
    End Sub

    Private Sub tmr_Tick(sender As Object, e As EventArgs) Handles tmr.Tick
        Application.Exit()

        '    currentValue += 5
        '    windowsTaskbar.SetProgressValue(currentValue, 100)
        '    If currentValue = 100 Then
        '        tmr.Stop()
        '        Threading.Thread.Sleep(500)
        '        windowsTaskbar.SetProgressValue(0, 100)
        '    End If
    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        Me.Close()
    End Sub
    Private Sub TbListMax_KeyPress(sender As Object, e As KeyPressEventArgs) Handles TbListMax.KeyPress
        'If e.KeyChar = Chr(13) Then TbListMax_Was_changed()
        If Not Char.IsNumber(e.KeyChar) AndAlso Not Char.IsControl(e.KeyChar) Then e.KeyChar = ""
    End Sub
    Private Sub TbListMax_TextChanged(sender As Object, e As EventArgs) Handles TbListMax.TextChanged
        Timer1.Start()
    End Sub
    Private Sub Timer1_Tick(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Timer1.Tick
        Timer1.Stop()
        Did_JumpListItems_Maximum_change()
    End Sub
    Sub Did_JumpListItems_Maximum_change()
        If Var_JumpListItems_Maximum <> TbListMax.Text Then
            Var_JumpListItems_Maximum = TbListMax.Text
            My.Computer.Registry.SetValue("HKEY_CURRENT_USER\SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\Advanced", "JumpListItems_Maximum", TbListMax.Text, Microsoft.Win32.RegistryValueKind.DWord)
        End If
    End Sub
    Private Sub Form1_Closed(sender As Object, e As EventArgs) Handles Me.Closed
        Did_JumpListItems_Maximum_change()
    End Sub
End Class
