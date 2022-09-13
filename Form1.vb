Imports System.IO
Imports System.IO.Compression
Imports System.Text
Imports System.Text.RegularExpressions
Imports System.Xml
Public Class Form1
	Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load

		Init()

		'for faster testing
		Dim loadpath As String = "C:\qb64\gx-0.4.0-alpha\games\Tutorial1\kanga.tmx"
		M.map_filename = "kanga.tmx"
		M.map_path = System.IO.Path.GetDirectoryName(loadpath) + "\"
		main()
		MsgBox("Converted", MsgBoxStyle.DefaultButton1)
		Info_text()


	End Sub

	Private Sub Main()

		'SaveMap(My.Computer.FileSystem.CurrentDirectory)
		'Dim loadpath As String = "C:\qb64\gx-0.4.0-alpha\games\Tutorial1\"
		'Dim loadname As String = "untitled.tmx"
		'Dim savename As String = "untitled.gxm"

		Xml_read(M.map_path + M.map_filename)
		'image.source
		M.tileset_image_filename = Xml_read2(M.map_path + M.tileset_source_filename, "image.source")

		'add zero bytes to tiledata
		Dim size = M.mapcol * M.maprow + 1
		Dim zero(size - 1) As Int16
		M.tiledata.InsertRange(0, zero)
		'pack tiledata
		Dim bytes_packed = Convert_int16_to_bytes(M.tiledata)
		'tiledata size after packing
		M.maptile_data_size = bytes_packed.Length


		'Dim filename As String = System.IO.Path.GetFileName(Path)
		Dim savename = Path.GetFileNameWithoutExtension(M.map_filename) + ".gxm"
		'SaveMap(M.map_path + loadname, M.map_path + savename, bytes_packed)
		SaveMap(M.map_path + M.map_filename, M.map_path + savename, bytes_packed)


		'Dim a$ = "2222222222222222222222222222222222222222"
		'Dim buffer As Byte() = Encoding.UTF8.GetBytes(a$)
		'Dim b = Compress(buffer)

		'Dim c = System.Text.Encoding.ASCII.GetString(b)
		'TextBox1.Text = Len(c).ToString
		'For Each i In c
		'	TextBox1.Text += i
		'Next

		'Dim h() As Byte = {&H78, &H9C, &H63, &H60, &HC0, &H6, &H18, &H19, &H98, &H18, &H98, &H19, &H58, &H18, &H58, &H19, &HB8, &H18, &HB8, &H19, &H78, &H18, &H78, &H19, &HF8, &H80, &HA2, &H0, &H2, &HF2, &H0, &H4C}
		'h = Decompress(h)

		'TextBox1.Text += vbCrLf
		'For Each i In h
		'	TextBox1.Text += i
		'Next
	End Sub

	Private Function Convert_int16_to_bytes(listInt16 As List(Of Int16)) As Array

		'convert int16 to bytes
		Dim bytes As New List(Of Byte)
		For Each i In listInt16 'M.tiledata
			bytes.AddRange(BitConverter.GetBytes(i))
		Next
		Return Compress(bytes.ToArray)

	End Function

	Private Sub SaveMap(inputfile As String, outputfile As String, bytes_packed As Byte())
		'Dim CustomerData As Byte() = (From c In customerQuery).ToArray()

		Dim c As Byte()

		' Create the BinaryWriter and use File.Open to create the file.
		Using writer As New BinaryWriter(File.Open(outputfile, FileMode.Create))
			' Write each integer.
			c = BitConverter.GetBytes(M.version)
			writer.Write(c)
			c = BitConverter.GetBytes(M.mapcol)
			writer.Write(c)
			c = BitConverter.GetBytes(M.maprow)
			writer.Write(c)
			c = BitConverter.GetBytes(M.layer)
			writer.Write(c)
			c = BitConverter.GetBytes(M.isometric)
			writer.Write(c)
			c = BitConverter.GetBytes(M.maptile_data_size)
			writer.Write(c)
			'c = M.tiledata
			writer.Write(bytes_packed)
			c = BitConverter.GetBytes(M.tileset_version)
			writer.Write(c)
			M.filename_length = (M.map_path + M.tileset_image_filename).Length
			c = BitConverter.GetBytes(M.filename_length)
			writer.Write(c)
			'c = BitConverter.GetBytes(filename.ToArray)
			writer.Write((M.map_path + M.tileset_image_filename).ToArray)
			c = BitConverter.GetBytes(M.tilewidth)
			writer.Write(c)
			c = BitConverter.GetBytes(M.tileheight)
			writer.Write(c)
			'image file size
			M.tileset_image_filesize = New System.IO.FileInfo(M.map_path + M.tileset_image_filename).Length
			c = BitConverter.GetBytes(M.tileset_image_filesize)
			writer.Write(c)
			'load tileset image file and save it
			Dim image_file As Byte() = My.Computer.FileSystem.ReadAllBytes(M.map_path + M.tileset_image_filename)
			writer.Write(image_file)
			'image_file = Image.FromStream(New IO.MemoryStream(image_file))
			'Dim bytes = My.Computer.FileSystem.ReadAllBytes(
			writer.Write({0, 0, 0, 0, 0, 0})
		End Using
	End Sub

	Private Function Xml_read2(inputfile As String, search_text As String) As String

		Dim s = search_text.Split(".")
		Dim reader As New XmlTextReader(inputfile)
		While reader.Read()
			Select Case reader.NodeType
				Case XmlNodeType.Element And reader.Name = s(0)
					Return reader.GetAttribute(s(1)).Replace("/", "\")

				Case XmlNodeType.Text

					Exit Select
				Case XmlNodeType.EndElement

					Exit Select
			End Select
		End While

	End Function

	Private Sub Xml_read(inputfile)

		Dim xmldoc As New XmlDataDocument()
		Dim xmlnode As XmlNodeList
		Dim i As Integer
		Dim str As String
		Dim fs As New FileStream(inputfile, FileMode.Open, FileAccess.Read)
		xmldoc.Load(fs)
		xmlnode = xmldoc.GetElementsByTagName("map")
		With xmlnode.Item(0)
			M.mapcol = .Attributes("width").Value
			M.maprow = .Attributes("height").Value
			M.tilewidth = .Attributes("tilewidth").Value
			M.tileheight = .Attributes("tileheight").Value
		End With
		xmlnode = xmldoc.GetElementsByTagName("tileset")
		With xmlnode.Item(0)
			M.tileset_source_filename = .Attributes("source").Value
		End With

		'read all layers
		xmlnode = xmldoc.GetElementsByTagName("layer")
		'how many layers
		M.layer = xmlnode.Count

		Dim s As String, z As String()

		For Each layer In xmlnode

			s = layer.InnerText
			z = Regex.Replace(s, "[^,0-9]", "").Split(",")

			For Each j As String In z
				M.tiledata.Add(j)
			Next
			'last add 0 
			M.tiledata.Add(0)
		Next

		fs.Close()

	End Sub

	Public Function Compress(ByVal bytes As Byte()) As Byte()

		Using inputMs As New MemoryStream(bytes)
			Using outputCompressedMs As New MemoryStream

				Using zl As New ZLibStream(outputCompressedMs, CompressionMode.Compress)
					inputMs.CopyTo(zl)
					'inputMs.Write(zl)
				End Using

				Return outputCompressedMs.ToArray

			End Using 'outputCompressedMs
		End Using 'inputMs



	End Function

	Public Function Decompress(ByVal bytes As Byte()) As Byte()

		Using inputMs As New MemoryStream(bytes)
			Using outputCompressedMs As New MemoryStream

				Using zl As New ZLibStream(inputMs, CompressionMode.Decompress, True)
					zl.CopyTo(outputCompressedMs)
					'inputMs.CopyTo(zl)
					'inputMs.write()
				End Using

				Return outputCompressedMs.ToArray

			End Using 'outputCompressedMs
		End Using 'inputMs



	End Function

	Private Sub OpenToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles OpenToolStripMenuItem.Click
		If OpenFileDialog1.ShowDialog = DialogResult.OK Then

			M.map_filename = OpenFileDialog1.SafeFileName
			M.map_path = System.IO.Path.GetDirectoryName(OpenFileDialog1.FileName) + "\"

			main()
			MsgBox("Converted", MsgBoxStyle.DefaultButton1)
			Info_text()
		End If
	End Sub
	Private Sub Info_text()
		'Dim tilefilename = Path.GetFileNameWithoutExtension(M.map_filename) + ".gmx"
		Dim tilefilename = M.map_path + M.tileset_image_filename
		With Txtb
			.Text = ""
			.Text += "Layers        :" + vbTab + M.layer.ToString + vbCrLf
			.Text += "Map size      :" + vbTab + M.mapcol.ToString + " x " + M.maprow.ToString + " tiles" + vbCrLf
			.Text += "Map file      :" + vbTab + M.map_path + M.map_filename + vbCrLf
			.Text += vbCrLf
			.Text += "Tile size     :" + vbTab + M.tilewidth.ToString + " x " + M.tileheight.ToString + " pixels" + vbCrLf
			.Text += "Tile file used:" + vbTab + tilefilename + vbCrLf

			.Select(0, 0)

		End With

	End Sub
	Private Sub OpenFileDialog1_FileOk(sender As Object, e As System.ComponentModel.CancelEventArgs) Handles OpenFileDialog1.FileOk


	End Sub
End Class
