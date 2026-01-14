using System.Collections.Generic;
using System.IO;
using System.Linq;

using Atlas.Editor.Registry;

namespace Atlas.Editor
{
	public sealed class DataRegistry
	{
		public IReadOnlyList<RegistryEntry> Entries => _registry.Entries;

		private readonly string _jsonPath;
		private readonly RegistryFile _registry;

		public DataRegistry(DataType type)
		{
			_jsonPath = DataPaths.GetRegistryPath(type);
			_registry = RegistrySerializer.Load(_jsonPath);

			_registry.Type = type.ToString();
			Resolve(type);

			RegistrySerializer.Save(_jsonPath, _registry);
		}

		private void Resolve(DataType type)
		{
			var diskLabels = ScanDisk(type);

			// Append new entries
			foreach (var label in diskLabels)
			{
				if (_registry.Entries.Any(e => e.Label == label))
					continue;

				_registry.Entries.Add(new RegistryEntry
				{
					Index = (ushort)_registry.Entries.Count,
					Label = label,
					Valid = true
				});
			}

			// Validate existing entries
			foreach (var entry in _registry.Entries)
			{
				entry.Valid = diskLabels.Contains(entry.Label);
			}
		}

		private static HashSet<string> ScanDisk(DataType type)
		{
			var result = new HashSet<string>();

			switch (type)
			{
				case DataType.Geometry:
					if (Directory.Exists(DataPaths.Layouts))
						foreach (var dir in Directory.EnumerateDirectories(DataPaths.Layouts))
							if (File.Exists(Path.Combine(dir, "geometry.bin")))
								result.Add(Path.GetFileName(dir));
					break;

				case DataType.Collision:
					if (Directory.Exists(DataPaths.Layouts))
						foreach (var dir in Directory.EnumerateDirectories(DataPaths.Layouts))
							if (File.Exists(Path.Combine(dir, "collision.bin")))
								result.Add(Path.GetFileName(dir));
					break;

				case DataType.Regional_Tileset:
					if (Directory.Exists(DataPaths.Regional))
						foreach (var f in Directory.EnumerateFiles(DataPaths.Regional, "*.bin"))
							result.Add(Path.GetFileNameWithoutExtension(f));
					break;

				case DataType.Local_Tileset:
					if (Directory.Exists(DataPaths.Local))
						foreach (var f in Directory.EnumerateFiles(DataPaths.Local, "*.bin"))
							result.Add(Path.GetFileNameWithoutExtension(f));
					break;

				case DataType.Interior_Tileset:
					if (Directory.Exists(DataPaths.Interior))
						foreach (var f in Directory.EnumerateFiles(DataPaths.Interior, "*.bin"))
							result.Add(Path.GetFileNameWithoutExtension(f));
					break;
			}

			return result;
		}
	}
}
