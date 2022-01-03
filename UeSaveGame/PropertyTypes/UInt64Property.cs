using System.IO;
using UeSaveGame.DataTypes;

namespace UeSaveGame.PropertyTypes
{
    public class UInt64Property : UProperty<ulong>
    {
        protected override long ContentSize => 8;

        public UInt64Property(UString name, UString type)
            : base(name, type)
        {
        }

        public override void Deserialize(BinaryReader reader, long size, bool includeHeader)
        {
            if (includeHeader) reader.ReadByte();
            Value = reader.ReadUInt64();
        }

        public override long Serialize(BinaryWriter writer, bool includeHeader)
        {
            if (includeHeader) writer.Write((byte)0);
            writer.Write(Value);

            return ContentSize;
        }
    }
}
