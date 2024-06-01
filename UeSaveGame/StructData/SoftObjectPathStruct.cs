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

		public override void Deserialize(BinaryReader reader, long size, PackageVersion packageVersion)
		{
			Value.Deserialize(reader, packageVersion);
		}

		public override long Serialize(BinaryWriter writer, PackageVersion packageVersion)
		{
			return Value.Serialize(writer, packageVersion);
		}
	}
}
