using System.IO;

namespace UeSaveGame
{
    public interface ITextData
    {
        void Deserialize(BinaryReader reader, long size);

        long Serialize(BinaryWriter writer);
    }
}
