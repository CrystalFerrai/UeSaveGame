using UeSaveGame.StructData;
using UeSaveGame.Util;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UeSaveGame.DataTypes;

namespace UeSaveGame.PropertyTypes
{
    public class StructProperty : UProperty<IStructData>
    {
        private static readonly Dictionary<string, Type> sTypeMap;
        private static readonly Dictionary<string, Type> sNameMap;

        public UString StructType { get; set; }

        public Guid StructGuid { get; set; }

        static StructProperty()
        {
            sTypeMap = new Dictionary<string, Type>();
            sNameMap = new Dictionary<string, Type>();

            foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies().Where(a => a.GlobalAssemblyCache == false))
            {
                IEnumerable<Type> types = assembly.GetTypes().Where(t => !t.IsAbstract && t.GetInterfaces().Contains(typeof(IStructData)));
                foreach (Type type in types)
                {
                    IStructData instance = (IStructData)Activator.CreateInstance(type);
                    foreach (string structType in instance.StructTypes)
                    {
                        sTypeMap.Add(structType, type);
                    }
                    if (instance.KnownPropertyNames != null)
                    {
                        foreach (string structType in instance.KnownPropertyNames)
                        {
                            sNameMap.Add(structType, type);
                        }
                    }
                }
            }
        }

        public StructProperty(UString name, UString type)
            : base(name, type)
        {
        }

        public override void Deserialize(BinaryReader reader, long size, bool includeHeader)
        {
            if (includeHeader)
            {
                StructType = reader.ReadUnrealString();
                byte[] guidBytes = reader.ReadBytes(16);
                StructGuid = new Guid(guidBytes);
                reader.ReadByte(); // terminator
            }

            if (size > 0 || StructType == null && !includeHeader)
            {
                IStructData instance;
                Type type;
                if (StructType != null && sTypeMap.TryGetValue(StructType, out type) ||
                    StructType == null && sNameMap.TryGetValue(Name, out type))
                {
                    instance = (IStructData)Activator.CreateInstance(type);
                }
                else
                {
                    instance = new PropertiesStruct();
                }
                instance.Deserialize(reader, size);
                Value = instance;
            }
            else
            {
                Value = null;
            }
        }

        public override long Serialize(BinaryWriter writer,  bool includeHeader)
        {
            if (includeHeader)
            {
                writer.WriteUnrealString(StructType);
                writer.Write(StructGuid.ToByteArray());
                writer.Write((byte)0);
            }

            if (Value != null)
            {
                return Value.Serialize(writer);
            }
            return 0;
        }

        public override string ToString()
        {
            return Value == null ? base.ToString() : $"{Name} [{nameof(StructProperty)} - {StructType??"no type"}] {Value?.ToString() ?? "Null"}";
        }
    }
}
