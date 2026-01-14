using System.IO;

using Atlas.World;

namespace Atlas.Editor
{
	public static class Serializer
	{
		private const uint MATRIX_MAGIC = 0x4D574247; // "GBWM"
		private const uint HEADER_MAGIC = 0x20484247; // "GBH "
		private const ushort VERSION = 1;

		public static void Save(WorldDocument doc)
		{
			string path = Path.Combine(DataPaths.Root, "world_matrix.mtx");

			// Ensure root folder exists
			var dir = Path.GetDirectoryName(path) ?? DataPaths.Root;
			if (!Directory.Exists(dir))
				Directory.CreateDirectory(dir);

			using (var fs = new FileStream(path, FileMode.Create))
			using (var bw = new BinaryWriter(fs))
			{
				bw.Write(MATRIX_MAGIC);
				bw.Write(VERSION);

				bw.Write(doc.Matrix.Width);
				bw.Write(doc.Matrix.Height);

				for (int y = 0; y < doc.Matrix.Height; y++)
					for (int x = 0; x < doc.Matrix.Width; x++)
					{
						bw.Write(doc.Matrix.Cells[x, y]);
					}
			}

			path = Path.Combine(DataPaths.Root, "world_headers.hdr");


			// Ensure root folder exists
			dir = Path.GetDirectoryName(path) ?? DataPaths.Root;
			if (!Directory.Exists(dir))
				Directory.CreateDirectory(dir);

			using (var fs = new FileStream(path, FileMode.Create))
			using (var bw = new BinaryWriter(fs))
			{
				bw.Write(HEADER_MAGIC);
				bw.Write(VERSION);

				bw.Write((ushort)doc.Headers.Count);

				for (int i = 0; i < doc.Headers.Count; i++)
				{
					Header header = doc.Headers[i];

					bw.Write(header.HeaderId);
					bw.Write(header.VerticalOffset);
					bw.Write(header.GeometryId);
					bw.Write(header.CollisionId);
					bw.Write(header.RegionalTilesetId);
					bw.Write(header.LocalTilesetId);
					bw.Write(header.InteriorTilesetId);
					bw.Write((ushort) 0);
				}
			}
		}

		public static void Load(WorldDocument doc)
		{
			string path = Path.Combine(DataPaths.Root, "world_matrix.mtx");

			if (!File.Exists(path))
				return;

			using (var fs = new FileStream(path, FileMode.Open))
			using (var br = new BinaryReader(fs))
			{
				uint magic = br.ReadUInt32();
				ushort version = br.ReadUInt16();

				if (magic != MATRIX_MAGIC)
					throw new InvalidDataException("Invalid matrix magic");

				if (version != VERSION)
					throw new InvalidDataException("Unsupported matrix version");

				ushort width = br.ReadUInt16();
				ushort height = br.ReadUInt16();

				doc.Matrix.Resize(width, height);

				for (int y = 0; y < height; y++)
					for (int x = 0; x < width; x++)
					{
						doc.Matrix.Cells[x, y] = br.ReadUInt16();
					}
			}

			path = Path.Combine(DataPaths.Root, "world_headers.hdr");

			if (!File.Exists(path))
				return;

			using (var fs = new FileStream(path, FileMode.Open))
			using (var br = new BinaryReader(fs))
			{
				uint magic = br.ReadUInt32();
				ushort version = br.ReadUInt16();

				if (magic != HEADER_MAGIC)
					throw new InvalidDataException("Invalid matrix magic");

				if (version != VERSION)
					throw new InvalidDataException("Unsupported matrix version");

				ushort count = br.ReadUInt16();

				doc.Headers.Clear();

				for (int i = 0; i < count; i++)
				{
					var header = new Header
					{
						HeaderId = br.ReadUInt16(),
						VerticalOffset = br.ReadInt16(),
						GeometryId = br.ReadUInt16(),
						CollisionId = br.ReadUInt16(),
						RegionalTilesetId = br.ReadUInt16(),
						LocalTilesetId = br.ReadUInt16(),
						InteriorTilesetId = br.ReadUInt16()
					};

					br.ReadUInt16(); // reserved

					doc.Headers.Add(header);
				}
			}
		}
	}
}
