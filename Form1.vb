Imports System.IO
Imports System.IO.Compression
Imports System.Text
Imports System.Text.RegularExpressions
Imports System.Xml
Public Class Form1
	Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load

		'Init()

		'for faster testing
		'Dim loadpath As String = "C:\qb64\gx-0.4.0-alpha\games\Tutorial1\kanga.tmx"
		'M.map_filename = "kanga.tmx"
		'M.map_path = System.IO.Path.GetDirectoryName(loadpath) + "\"
		'main()
		''MsgBox("Converted", MsgBoxStyle.DefaultButton1)
		'Info_text()


	End Sub

	Private Sub Main()


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

				Using zl As New ZLibStream(outputCompressedMs, CompressionLevel.Fastest) ' CompressionMode.Compress)
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
				End Using

				Return outputCompressedMs.ToArray

			End Using
		End Using

	End Function

	Private Sub OpenToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles OpenToolStripMenuItem.Click

		Init()
		If OpenFileDialog1.ShowDialog = DialogResult.OK Then

			M.map_filename = OpenFileDialog1.SafeFileName
			M.map_path = System.IO.Path.GetDirectoryName(OpenFileDialog1.FileName) + "\"

			Main()
			MsgBox("Converted", MsgBoxStyle.DefaultButton1, "")
			Info_text()
		End If
	End Sub

	Private Sub Info_text()

		Dim tilefilename = M.map_path + M.tileset_image_filename
		Dim map_sixe_pixels() = {M.mapcol * M.tilewidth, M.maprow * M.tileheight}
		Dim s = StrDup(12, "-")

		With Txtb
			.Text = ""
			.Text += "MAP            " + vbCrLf
			.Text += "Layers     :" + vbTab + M.layer.ToString + vbCrLf
			.Text += "Size       :" + vbTab + M.mapcol.ToString + " x " + M.maprow.ToString + " tiles" + "," +
																 map_sixe_pixels(0).ToString + " x " + map_sixe_pixels(1).ToString + " pixels" + vbCrLf
			.Text += "File       :" + vbTab + M.map_path + M.map_filename + vbCrLf
			.Text += s + vbCrLf
			.Text += "TILE           " + vbCrLf
			.Text += "Size       :" + vbTab + M.tilewidth.ToString + " x " + M.tileheight.ToString + " pixels" + vbCrLf
			.Text += "Image used :" + vbTab + tilefilename + vbCrLf

			.Select(0, 0)

		End With

	End Sub


End Class
