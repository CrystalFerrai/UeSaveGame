using System.IO;
using UeSaveGame.DataTypes;

namespace UeSaveGame.PropertyTypes
{
    public class DoubleProperty : UProperty<double>
    {
        protected override long ContentSize => 8;

        public DoubleProperty(UString name, UString type)
            : base(name, type)
        {
        }

        public override void Deserialize(BinaryReader reader, long size, bool includeHeader)
        {
            if (includeHeader) reader.ReadByte();
            Value = reader.ReadDouble();
        }

        public override long Serialize(BinaryWriter writer, bool includeHeader)
        {
            if (includeHeader) writer.Write((byte)0);
            writer.Write(Value);

            return ContentSize;
        }
    }
}
