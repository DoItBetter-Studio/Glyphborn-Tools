namespace Atlas.World
{
	public sealed class Header
	{
		public ushort HeaderId { get; set; }
		
		// World Positioning
		public short VerticalOffset { get; set; }

		// Map references
		public ushort GeometryId { get; set; }
		public ushort CollisionId { get; set; }

		// Tilesets
		public ushort RegionalTilesetId { get; set; }
		public ushort LocalTilesetId { get; set; }
		public ushort InteriorTilesetId { get; set; }

		public override string ToString()
		{
			return $"Header {HeaderId}";
		}
	}
}
