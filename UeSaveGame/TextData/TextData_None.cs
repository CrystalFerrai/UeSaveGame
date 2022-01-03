using UeSaveGame.Util;
using System;
using System.IO;
using UeSaveGame.DataTypes;
using System.Linq;

namespace UeSaveGame.TextData
{
    public class TextData_None : ITextData
    {
        private UString[] mValues;

        public void Deserialize(BinaryReader reader, long size)
        {
            int count = reader.ReadInt32();
            mValues = new UString[count];
            for (int i = 0; i < count; ++i)
            {
                mValues[i] = reader.ReadUnrealString();
            }
        }

        public long Serialize(BinaryWriter writer)
        {
            writer.Write(mValues.Length);
            long len = 4;
            foreach (UString value in mValues)
            {
                writer.WriteUnrealString(value);
                len += 4 + value.SizeInBytes;
            }

            return len;
        }

        public override string ToString()
        {
            return string.Join(",", mValues.Select(v => v.Value));
        }
    }
}
