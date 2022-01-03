using System.Collections.Generic;

namespace UeSaveGame
{
    public interface IStructData : IBinarySerializable
    {
        IEnumerable<string> StructTypes { get; }

        // Only needed for cases where type name is not saved for a custom struct type (due to being in a map or something)
        ISet<string> KnownPropertyNames { get; }
    }
}
