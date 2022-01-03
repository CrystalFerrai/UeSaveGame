using UeSaveGame.DataTypes;
using System.Collections.Generic;
using System.IO;

namespace UeSaveGame.StructData
{
    public class ColorStruct : BaseStructData
    {
        public Color Value { get; set; }

        public override IEnumerable<string> StructTypes
        {
            get
            {
                yield return "Color";
            }
        }

        public override void Deserialize(BinaryReader reader, long size)
        {
            Color value = new Color();
            value.R = reader.ReadByte();
            value.G = reader.ReadByte();
            value.B = reader.ReadByte();
            value.A = reader.ReadByte();
            Value = value;
        }

        public override long Serialize(BinaryWriter writer)
        {
            writer.Write(Value.R);
            writer.Write(Value.G);
            writer.Write(Value.B);
            writer.Write(Value.A);

            return 4;
        }
    }
}
