using System;
using System.IO;

namespace Atlas.Editor
{
	internal static class DataPaths
	{
		public static string Root => Path.Combine(AppContext.BaseDirectory, "../..", "data");

		// Layouts Directory
		public static string Layouts => Path.Combine(Root, "layouts");

		// Tilesets Directories
		private static string Tilesets => Path.Combine(Root, "tilesets");
		public static string Regional => Path.Combine(Tilesets, "regional");
		public static string Local => Path.Combine(Tilesets, "local");
		public static string Interior => Path.Combine(Tilesets, "interior");

		public static string GetRegistryPath(DataType type)
		{
			var path = Path.Combine(Root, "registry", type switch
			{
				DataType.Geometry => "geometry.json",
				DataType.Collision => "collision.json",
				DataType.Regional_Tileset => "tileset_regional.json",
				DataType.Local_Tileset => "tileset_local.json",
				DataType.Interior_Tileset => "tileset_interior.json",
				_ => throw new ArgumentOutOfRangeException(nameof(type))
			});

			Directory.CreateDirectory(Path.GetDirectoryName(path)!);

			return path;
		}
	}

	public enum DataType
	{
		Geometry,
		Collision,
		Regional_Tileset,
		Local_Tileset,
		Interior_Tileset
	}
}
