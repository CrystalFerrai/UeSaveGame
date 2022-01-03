using System.Collections.Generic;
using System.IO;

namespace UeSaveGame.StructData
{
    public class LinearColorStruct : BaseStructData
    {
        public override IEnumerable<string> StructTypes
        {
            get
            {
                yield return "LinearColor";
            }
        }

        public LinearColor Value { get; set; }

        public LinearColorStruct()
        {
        }

        public override void Deserialize(BinaryReader reader, long size)
        {
            LinearColor c;
            c.R = reader.ReadSingle();
            c.G = reader.ReadSingle();
            c.B = reader.ReadSingle();
            c.A = reader.ReadSingle();
            Value = c;
        }

        public override long Serialize(BinaryWriter writer)
        {
            writer.Write(Value.R);
            writer.Write(Value.G);
            writer.Write(Value.B);
            writer.Write(Value.A);

            return 16;
        }

        public override string ToString()
        {
            return Value.ToString();
        }
    }

    public struct LinearColor
    {
        public float R;
        public float G;
        public float B;
        public float A;
    }
}
