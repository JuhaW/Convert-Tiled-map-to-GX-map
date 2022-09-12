Module Module1



	Public Structure Moi
		Dim version As Int16 '= 2
		Dim mapcol As Int16
		Dim maprow As Int16
		Dim layer As Int16 '= 1
		Dim tile As Int16
		Dim depth As Int16
		Dim isometric As Int16 '= 1
		Dim maptile_data_size As Int32

		Dim tilewidth As Int16
		Dim tileheight As Int16

		Dim tiledata As List(Of Int16)
		Dim tileset_version As Int16
		Dim filename_length As Int32
		'tilemap info file
		Dim tileset_source_filename As String
		Dim tileset_image_filename As String
		Dim tileset_image_filesize As Int32
		Dim map_filename As String
		Dim map_path As String

	End Structure
	Public M As New Moi


	Public Sub Init()
		M.version = 2
		M.layer = 1
		M.isometric = 0
		M.maptile_data_size = 0
		M.tiledata = New List(Of Int16)
		M.tiledata.Clear()
		M.tileset_version = 1
		'M.tiledata.AddRange({1, 2, 3, 4, 1, 2, 3, 4})
		'M.tiledata.AddRange({1, 2, 3, 4, 1, 2, 3, 4})
		'M.tiledata.AddRange({1, 2, 3, 4, 1, 2, 3, 4})
		'M.tiledata.AddRange({1, 2, 3, 4, 1, 2, 3, 4})
		''M.tiledata.Add(2)


	End Sub
End Module
