using System.Collections.Generic;
using System.IO;

namespace UeSaveGame.StructData
{
    public class IntPointStruct : BaseStructData
    {
        IntPoint Value { get; set; }

        public override IEnumerable<string> StructTypes
        {
            get
            {
                yield return "IntPoint";
            }
        }

        public override void Deserialize(BinaryReader reader, long size)
        {
            IntPoint value = new IntPoint();
            value.X = reader.ReadInt32();
            value.Y = reader.ReadInt32();
            Value = value;
        }

        public override long Serialize(BinaryWriter writer)
        {
            writer.Write(Value.X);
            writer.Write(Value.Y);

            return 8;
        }

        public override string ToString()
        {
            return Value.ToString();
        }
    }

    public struct IntPoint
    {
        public int X;
        public int Y;

        public override string ToString()
        {
            return $"{X},{Y}";
        }
    }
}
