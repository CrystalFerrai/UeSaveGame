using UeSaveGame.Util;
using System.IO;
using UeSaveGame.DataTypes;

namespace UeSaveGame.TextData
{
    public class TextData_StringTableEntry : ITextData
    {
        private UString mTable;
        private UString mKey;

        public void Deserialize(BinaryReader reader, long size)
        {
            mTable = reader.ReadUnrealString();
            mKey = reader.ReadUnrealString();
        }

        public long Serialize(BinaryWriter writer)
        {
            writer.WriteUnrealString(mTable);
            writer.WriteUnrealString(mKey);

            return 8 + mTable.SizeInBytes + mKey.SizeInBytes;
        }

        public override string ToString()
        {
            return $"{mTable}[{mKey}]";
        }
    }
}
