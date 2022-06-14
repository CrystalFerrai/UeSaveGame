using System.IO;
using UeSaveGame.DataTypes;
using UeSaveGame.Util;

namespace UeSaveGame.PropertyTypes
{
	public class SoftObjectProperty : UProperty
	{
        public UString AssetPath { get; private set; }

        public int Unknown { get; private set; } // Maybe an index? Have only seen 0

        protected override long ContentSize => (AssetPath?.SizeInBytes ?? 0) + 8;

        public SoftObjectProperty(UString name, UString type)
            : base(name, type)
        {
        }

        public override void Deserialize(BinaryReader reader, long size, bool includeHeader)
        {
            if (includeHeader) reader.ReadByte();

            AssetPath = reader.ReadUnrealString();
            Unknown = reader.ReadInt32();
        }

        public override long Serialize(BinaryWriter writer, bool includeHeader)
        {
            if (includeHeader) writer.Write((byte)0);

            writer.WriteUnrealString(AssetPath);
            writer.Write(Unknown);

            return ContentSize;
        }
    }
}
