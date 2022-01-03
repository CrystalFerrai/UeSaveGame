using System.Collections.Generic;
using System.IO;

namespace UeSaveGame
{
    public abstract class BaseStructData : IStructData
    {
        public virtual ISet<string> KnownPropertyNames => null;

        public abstract IEnumerable<string> StructTypes { get; }

        public abstract void Deserialize(BinaryReader reader, long size);

        public abstract long Serialize(BinaryWriter writer);
    }
}
