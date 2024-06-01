using UeSaveGame.Util;

namespace UeSaveGame.StructData
{
	public class GameplayTagContainerStruct : BaseStructData
	{
		public IList<FString?> Tags { get; }

		public GameplayTagContainerStruct()
		{
			Tags = new List<FString?>();
		}

		public override IEnumerable<string> StructTypes
		{
			get
			{
				yield return "GameplayTagContainer";
			}
		}

		public override void Deserialize(BinaryReader reader, long size, PackageVersion packageVersion)
		{
			Tags.Clear();

			int count = reader.ReadInt32();
			for (int i = 0; i < count; ++i)
			{
				Tags.Add(reader.ReadUnrealString());
			}
		}

		public override long Serialize(BinaryWriter writer, PackageVersion packageVersion)
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
