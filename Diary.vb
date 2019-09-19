Imports System.Data
Imports System.Data.SqlClient
Imports System.IO
Imports System.Xml

Public Class Diary

    Public Sub New()
        InitializeComponent()
        Call PrepareDatas()
        ComboBox1.SelectedIndex = 0
        ComboBox1_SelectedIndexChanged(Nothing, Nothing)
    End Sub

    Private Sub PrepareDatas()
        Dim cmd As SqlCommand
        Dim con = New SqlConnection("server = localhost; database = test; integrated security = true")
        Dim reader As SqlDataReader
        Call con.Open()
        cmd = New SqlCommand("select name from diary", con)
        reader = cmd.ExecuteReader()
        Call ComboBox1.Items.Clear()
        While reader.Read()
            ComboBox1.Items.Add(reader.GetValue(0).ToString)
        End While
        con.Close()
    End Sub
    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        RichTextBox2.Focus()
    End Sub

    Private Sub ComboBox1_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ComboBox1.SelectedIndexChanged
        Dim cmd As SqlCommand
        Dim con = New SqlConnection("server = localhost; database = test; integrated security = true")
        Dim reader As SqlDataReader
        Call con.Open()
        Dim a = ComboBox1.SelectedItem.ToString
        cmd = New SqlCommand("select name, content from diary where name like '" & a.Replace("'", "''") & "'", con)
        reader = cmd.ExecuteReader()
        If reader.Read() Then
            RichTextBox1.Text = reader.GetValue(1).ToString
        End If
        con.Close()
    End Sub

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        Dim cmd As SqlCommand
        Dim con = New SqlConnection("server = localhost; database = test; integrated security = true")
        Call con.Open()
        cmd = New SqlCommand("update diary set content = '" & RichTextBox1.Text.Replace("'", "''") & "' where name like '" & ComboBox1.SelectedItem.ToString.Replace("'", "''") & "'", con)
        Call cmd.ExecuteNonQuery()
        con.Close()
        MessageBox.Show("Updated")
    End Sub

    Private Sub Button3_Click(sender As Object, e As EventArgs) Handles Button3.Click
        Dim cmd As SqlCommand
        Dim con = New SqlConnection("server = localhost; database = test; integrated security = true")
        Call con.Open()
        cmd = New SqlCommand("insert into diary (name, content) values ('" & TextBox1.Text.Replace("'", "''") & "', '" & RichTextBox2.Text.Replace("'", "''") & "')", con)
        Call cmd.ExecuteNonQuery()
        con.Close()
        PrepareDatas()
    End Sub

    Private Sub Button4_Click(sender As Object, e As EventArgs) Handles Button4.Click
        Try
            ComboBox1.SelectedIndex = ComboBox1.SelectedIndex - 1
            ComboBox1_SelectedIndexChanged(Nothing, Nothing)
        Catch Ex As Exception
        End Try
    End Sub

    Private Sub Button5_Click(sender As Object, e As EventArgs) Handles Button5.Click
        Try
            ComboBox1.SelectedIndex = ComboBox1.SelectedIndex + 1
            ComboBox1_SelectedIndexChanged(Nothing, Nothing)
        Catch Ex As Exception
        End Try
    End Sub

    Private Sub Button6_Click(sender As Object, e As EventArgs) Handles Button6.Click
        Dim a As String = ""
        Dim firstline As String = ""
        Try
            OpenFileDialog1.Title = "Open File..."
            OpenFileDialog1.ShowDialog()
            Using SR As New System.IO.StreamReader(OpenFileDialog1.OpenFile())
                Do
                    Try
                        firstline = SR.ReadLine
                        a = a & firstline
                    Catch Ex As Exception
                    End Try
                Loop Until firstline Is Nothing
            End Using
        Catch Ex As Exception
        End Try
        Dim xml As XDocument = XDocument.Parse(a)
        Dim listRows As List(Of XElement) = xml.Descendants("title").ToList()
        For Each node As XElement In listRows
            Dim values As List(Of XElement) = node.Descendants("content").ToList()
            Dim ip As String = values(0)
            Dim bytes As String = values(1)
            Dim cmd As SqlCommand
            Dim con = New SqlConnection("server = localhost; database = test; integrated security = true")
            Call con.Open()
            Try
                cmd = New SqlCommand("insert into diary (name, content) values ('" & ip.Replace("'", "''") & "', '" & bytes.Replace("'", "''") & "')", con)
                Call cmd.ExecuteNonQuery()
            Catch Ex As Exception
                MessageBox.Show(Ex.Message)
            End Try
            con.Close()
            PrepareDatas()
        Next
        ComboBox1.SelectedIndex = 0
        ComboBox1_SelectedIndexChanged(Nothing, Nothing)
    End Sub

    Private Sub Button8_Click(sender As Object, e As EventArgs) Handles Button8.Click
        RichTextBox2.Text = ""
    End Sub

    Private Sub Button7_Click(sender As Object, e As EventArgs) Handles Button7.Click
        Dim myDoc As Xml.XmlDocument = New Xml.XmlDocument
        Dim docNode = myDoc.CreateXmlDeclaration("1.0", "UTF-8", Nothing)
        Dim productsNode = myDoc.CreateElement("datas")
        Dim root As XmlNode = myDoc.DocumentElement
        myDoc.AppendChild(docNode)
        myDoc.AppendChild(productsNode)
        For i = 0 To ComboBox1.Items.Count - 1
            Dim myTitle As XmlElement = myDoc.CreateElement("title")
            Dim myContent1 As XmlElement = myDoc.CreateElement("content")
            Dim myContent2 As XmlElement = myDoc.CreateElement("content")
            Dim cmd As SqlCommand
            Dim con = New SqlConnection("server = localhost; database = test; integrated security = true")
            Call con.Open()
            cmd = New SqlCommand("select name, content from diary where name like '" & ComboBox1.Items(i).ToString.Replace("'", "''") & "'", con)
            Dim reader As SqlDataReader
            reader = cmd.ExecuteReader()
            If reader.Read() Then
                myContent1.InnerText = reader.GetValue(0).ToString()
                myContent2.InnerText = reader.GetValue(1).ToString()
            End If
            Call con.Close()
            productsNode.AppendChild(myTitle)
            myTitle.AppendChild(myContent1)
            myTitle.AppendChild(myContent2)
        Next
        myDoc.Save("D:\\backup.xml")
        MsgBox("Xml File GeneratedUpdated")
    End Sub

    Private Sub Button9_Click(sender As Object, e As EventArgs) Handles Button9.Click
        Application.Exit()
    End Sub
End Class