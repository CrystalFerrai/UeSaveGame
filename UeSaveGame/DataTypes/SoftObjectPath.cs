using UeSaveGame.Util;

namespace UeSaveGame.DataTypes
{
	/// <summary>
	/// The path to an object within a game asset
	/// </summary>
	public class SoftObjectPath
	{
		public FString? PackageName { get; set; }

		public FString? AssetName { get; set; }

		public FString? SubPathString { get; set; }

		public void Deserialize(BinaryReader reader, EngineVersion engineVersion)
		{
			if (engineVersion.Major == 4)
			{
				FString? path = reader.ReadUnrealString();
				if (path is not null)
				{
					int dotIndex = path.Value.IndexOf('.');
					if (dotIndex > 0)
					{
						PackageName = new(path.Value[..dotIndex]);
						AssetName = new(path.Value[(dotIndex + 1)..]);
					}
					else
					{
						PackageName = path;
					}
				}
			}
			else // Assuming 5 or later
			{
				PackageName = reader.ReadUnrealString();
				AssetName = reader.ReadUnrealString();
			}

			SubPathString = reader.ReadUnrealString();
		}

		public long Serialize(BinaryWriter writer, EngineVersion engineVersion)
		{
			long startPos = writer.BaseStream.Position;

			if (engineVersion.Major == 4)
			{
				FString? path = PackageName;
				if (PackageName is not null && AssetName is not null)
				{
					path = new($"{PackageName},{AssetName}");
				}
				writer.WriteUnrealString(path);
			}
			else // Assuming 5 or later
			{
				writer.WriteUnrealString(PackageName);
				writer.WriteUnrealString(AssetName);
			}

			writer.WriteUnrealString(SubPathString);

			return writer.BaseStream.Position - startPos;
		}

		public override string ToString()
		{
			if (SubPathString is null)
			{
				return $"{PackageName}.{AssetName}";
			}
			return $"{PackageName}.{AssetName}.{SubPathString}";
		}
	}
}
