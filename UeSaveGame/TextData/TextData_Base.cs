using UeSaveGame.Util;
using System.IO;
using UeSaveGame.DataTypes;

namespace UeSaveGame.TextData
{
    public class TextData_Base : ITextData
    {
        private UString mNamespace;
        private UString mKey;
        private UString mSourceString;

        public void Deserialize(BinaryReader reader, long size)
        {
            mNamespace = reader.ReadUnrealString();
            mKey = reader.ReadUnrealString();
            mSourceString = reader.ReadUnrealString();
        }

        public long Serialize(BinaryWriter writer)
        {
            writer.WriteUnrealString(mNamespace);
            writer.WriteUnrealString(mKey);
            writer.WriteUnrealString(mSourceString);

            return 12 + mNamespace.SizeInBytes + mKey.SizeInBytes + mSourceString.SizeInBytes;
        }

        public override string ToString()
        {
            return mSourceString;
        }
    }
}
