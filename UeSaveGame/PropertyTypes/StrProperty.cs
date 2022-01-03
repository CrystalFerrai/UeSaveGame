using UeSaveGame.Util;
using System.IO;
using UeSaveGame.DataTypes;

namespace UeSaveGame.PropertyTypes
{
    public class StrProperty : UProperty<UString>
    {
        protected override long ContentSize => Value == null ? 4 : 4 + Value.SizeInBytes;

        public StrProperty(UString name, UString type)
            : base(name, type)
        {
        }

        public override void Deserialize(BinaryReader reader, long size, bool includeHeader)
        {
            if (includeHeader) reader.ReadByte();
            Value = reader.ReadUnrealString();
        }

        public override long Serialize(BinaryWriter writer, bool includeHeader)
        {
            if (includeHeader) writer.Write((byte)0);
            writer.WriteUnrealString(Value);

            return ContentSize;
        }
    }
}
