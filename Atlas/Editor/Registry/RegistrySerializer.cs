using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Atlas.Editor.Registry
{
	public static class RegistrySerializer
	{
		private static readonly JsonSerializerOptions Options = new()
		{
			WriteIndented = true,
			AllowTrailingCommas = true,
			ReadCommentHandling = JsonCommentHandling.Skip,
			DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
		};

		public static RegistryFile Load(string path)
		{
			if (!File.Exists(path))
				return new RegistryFile();

			var json = File.ReadAllText(path);
			return JsonSerializer.Deserialize<RegistryFile>(json, Options) ?? new RegistryFile();
		}

		public static void Save(string path, RegistryFile registryFile)
		{
			var json = JsonSerializer.Serialize(registryFile, Options);

			var dir = Path.GetDirectoryName(path);
			if (!string.IsNullOrEmpty(dir))
				Directory.CreateDirectory(dir);

			File.WriteAllText(path, json);
		}
	}
}
