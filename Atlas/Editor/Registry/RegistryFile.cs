using System;
using System.Collections.Generic;

namespace Atlas.Editor.Registry
{
	public sealed class RegistryFile
	{
		public string Type { get; set; } = String.Empty;
		public int Version { get; set; } = 1;
		public List<RegistryEntry> Entries { get; set; } = new();
	}

	public sealed class RegistryEntry
	{
		public ushort Index { get; set; }
		public string Label { get; set; } = String.Empty;
		public bool Valid { get; set; }

		public override string ToString()
		{
			return Valid
				? $"{Index} - {Label}"
				: $"{Index} - Invalid";
		}
	}
}
