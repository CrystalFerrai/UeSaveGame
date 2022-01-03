using System;
using System.Collections.Generic;
using System.IO;

namespace UeSaveGame.StructData
{
    public class GuidStruct : BaseStructData
    {
        public override IEnumerable<string> StructTypes
        {
            get
            {
                yield return "Guid";
            }
        }

        public Guid Value { get; set; }

        public GuidStruct()
        {
        }

        public override void Deserialize(BinaryReader reader, long size)
        {
            Value = new Guid(reader.ReadBytes(16));
        }

        public override long Serialize(BinaryWriter writer)
        {
            writer.Write(Value.ToByteArray());

            return 16;
        }

        public override string ToString()
        {
            return Value.ToString();
        }
    }
}
