using System;
using System.Collections.Generic;
using System.Linq;

using Atlas.Editor;

namespace Atlas.World
{
	public sealed class WorldDocument
	{
		public Matrix Matrix { get; set; } = new();
		public List<Header> Headers { get; set; } = new();

		public DataRegistry GeometryRegistry { get; } = new(DataType.Geometry);
		public DataRegistry CollisionRegistry { get; } = new(DataType.Collision);
		public DataRegistry RegionalRegistry { get; } = new(DataType.Regional_Tileset);
		public DataRegistry LocalRegistry { get; } = new(DataType.Local_Tileset);
		public DataRegistry InteriorRegistry { get; } = new(DataType.Interior_Tileset);

		public Header? GetHeader(ushort headerId)
		{
			return Headers.FirstOrDefault(x => x.HeaderId == headerId);
		}

		public Header CreateHeader()
		{
			var header = new Header
			{
				HeaderId = (ushort) Headers.Count
			};

			Headers.Add(header);
			return header;
		}
	}
}
