using UeSaveGame.DataTypes;

namespace UeSaveGame.StructData
{
	internal class SoftObjectPathStruct : BaseStructData
	{
		public SoftObjectPath Value { get; set; }

		public override IEnumerable<string> StructTypes
		{
			get
			{
				yield return "SoftObjectPath";
			}
		}

		public SoftObjectPathStruct()
		{
			Value = new();
		}

		public override void Deserialize(BinaryReader reader, long size, EngineVersion engineVersion)
		{
			Value.Deserialize(reader, engineVersion);
		}

		public override long Serialize(BinaryWriter writer, EngineVersion engineVersion)
		{
			return Value.Serialize(writer, engineVersion);
		}
	}
}
