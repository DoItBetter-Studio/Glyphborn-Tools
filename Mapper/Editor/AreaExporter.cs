using System;
using System.IO;
using System.Windows.Forms;

using Glyphborn.Mapper.Tiles;

namespace Glyphborn.Mapper.Editor
{
	public static class AreaExporter
	{
		private const uint MAGIC_GEOMETRY = 0x474D4247; // "GBMG"
		private const uint MAGIC_COLLISION = 0x434D4247; // "GBMC"

		private const ushort VERSION = 1;
		private static string DataRoot => Path.Combine(AppContext.BaseDirectory, "../..", "data");
		private static string Maps => Path.Combine(DataRoot, "maps");


		public static bool ExportBinary(AreaDocument doc)
		{
			// Validate format limits so we don't silently truncate values.
			if (doc.Width > byte.MaxValue || doc.Height > byte.MaxValue)
				throw new InvalidDataException("Area dimensions exceed storage limits (max 255).");

			if (!WriteGeometry(doc))
				return false;
			if (!WriteCollision(doc))
				return false;

			return true;
		}

		private static bool WriteGeometry(AreaDocument doc)
		{
			bool result = false;
			try
			{
				for (int areaY = 0; areaY < doc.Height; areaY++)
					for (int areaX = 0; areaX < doc.Width; areaX++)
					{
						string geometryPath = Path.Combine(Maps, $"{doc.Name}_{areaX}_{areaY}", $"geometry.bin");

						var dir = Path.GetDirectoryName(geometryPath) ?? Maps;
						if (!Directory.Exists(dir))
							Directory.CreateDirectory(dir);

						using (var fs = new FileStream(geometryPath, FileMode.Create))
						using (var bw = new BinaryWriter(fs))
						{
							bw.Write(MAGIC_GEOMETRY);
							bw.Write(VERSION);

							var map = doc.GetMap(areaX, areaY);

							if (map == null)
							{
								continue;
							}

							for (int layers = 0; layers < MapDocument.LAYERS; layers++)
								for (int y = 0; y < MapDocument.HEIGHT; y++)
									for (int x = 0; x < MapDocument.WIDTH; x++)
									{
										var tile = map.Tiles[layers][y][x];

										// Pack tileset (2 bits) + tileId (14 bits) into ushort
										ushort packed = (ushort) ((tile.Tileset << 14) | tile.TileId);
										bw.Write(packed);
									}
						}
					}
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message);
				result = false;
			}
			finally
			{
				result = true;
			}

			return result;
		}

		private static bool WriteCollision(AreaDocument doc)
		{
			bool result = false;

			try
			{
				for (int areaY = 0; areaY < doc.Height; areaY++)
					for (int areaX = 0; areaX < doc.Width; areaX++)
					{
						string collisionPath = Path.Combine(Maps, $"{doc.Name}_{areaX}_{areaY}", "collision.bin");
						var dir = Path.GetDirectoryName(collisionPath) ?? Maps;
						if (!Directory.Exists(dir))
							Directory.CreateDirectory(dir);

						using (var fs = new FileStream(collisionPath, FileMode.Create))
						using (var bw = new BinaryWriter(fs))
						{
							bw.Write(MAGIC_COLLISION);
							bw.Write(VERSION);

							var map = doc.GetMap(areaX, areaY);

							if (map == null)
							{
								continue;
							}

							for (int layers = 0; layers < MapDocument.LAYERS; layers++)
								for (int y = 0; y < MapDocument.HEIGHT; y++)
									for (int x = 0; x < MapDocument.WIDTH; x++)
									{
										var tile = map.Tiles[layers][y][x];

										TileDefinition tileDefinition = doc.Tilesets[tile.Tileset].Tiles[tile.TileId];

										bw.Write((byte) tileDefinition.Collision);
									}
						}
					}
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message);
				result = false;
			}
			finally
			{
				result = true;
			}

			return result;
		}
	}
}
