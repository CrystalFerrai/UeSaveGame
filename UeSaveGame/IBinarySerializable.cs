using System.IO;

namespace UeSaveGame
{
    public interface IBinarySerializable
    {
        void Deserialize(BinaryReader reader, long size);

        long Serialize(BinaryWriter writer);
    }
}
