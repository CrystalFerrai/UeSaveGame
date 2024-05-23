using UeSaveGame.Util;

namespace UeSaveGame.StructData
{
	public class GameplayTagContainerStruct : BaseStructData
	{
		List<FString?> Tags { get; }

		public GameplayTagContainerStruct()
		{
			Tags = new();
		}

		public override IEnumerable<string> StructTypes
		{
			get
			{
				yield return "GameplayTagContainer";
			}
		}

		public override void Deserialize(BinaryReader reader, long size, EngineVersion engineVersion)
		{
			Tags.Clear();

			int count = reader.ReadInt32();
			for (int i = 0; i < count; ++i)
			{
				Tags.Add(reader.ReadUnrealString());
			}
		}

		public override long Serialize(BinaryWriter writer, EngineVersion engineVersion)
		{
			long startPos = writer.BaseStream.Position;

			writer.Write(Tags.Count);
			foreach (FString? tag in Tags)
			{
				writer.WriteUnrealString(tag);
			}

			return writer.BaseStream.Position - startPos;
		}
	}
}
