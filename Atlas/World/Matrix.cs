using System;

namespace Atlas.World
{
	public sealed class Matrix
	{
		public ushort Width { get; set; } = 10;
		public ushort Height { get; set; } = 10;
		public ushort[,] Cells { get; private set; }

		public Matrix()
		{
			Cells = new ushort[Width, Height];
		}

		public void Resize(ushort width, ushort height)
		{
			var newCells = new ushort[width, height];

			int copyW = Math.Min(width, Width);
			int copyH = Math.Min(height, Height);

			for (int y = 0; y < copyH; y++)
				for (int x = 0; x < copyW; x++)
					newCells[x, y] = Cells[x, y];

			Width = width;
			Height = height;
			Cells = newCells;
		}
	}
}
