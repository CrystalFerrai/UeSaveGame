// Copyright 2022 Crystal Ferrai
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//    http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System.Text;
using UeSaveGame.PropertyTypes;
using UeSaveGame.Util;

namespace UeSaveGame
{
	public abstract class UProperty
    {
        private static Dictionary<string, Type> sTypeMap;

        public FString Name { get; }

        public FString Type { get; }

        public object? Value { get; set; }

        protected virtual long ContentSize { get; }

        protected UProperty(FString name, FString type)
        {
            Name = name;
            Type = type;
            ContentSize = -1;
        }

        static UProperty()
        {
            sTypeMap = new Dictionary<string, Type>()
            {
                { nameof(ArrayProperty), typeof(ArrayProperty) },
                { nameof(BoolProperty), typeof(BoolProperty) },
                { nameof(ByteProperty), typeof(ByteProperty) },
                { nameof(DoubleProperty), typeof(DoubleProperty) },
                { nameof(EnumProperty), typeof(EnumProperty) },
                { nameof(FloatProperty), typeof(FloatProperty) },
                { nameof(IntProperty), typeof(IntProperty) },
                { nameof(Int64Property), typeof(Int64Property) },
                { nameof(MapProperty), typeof(MapProperty) },
                { "NameProperty", typeof(StrProperty) },
                { nameof(ObjectProperty), typeof(ObjectProperty) },
                { nameof(SetProperty), typeof(SetProperty) },
                { nameof(SoftObjectProperty), typeof(SoftObjectProperty) },
                { nameof(StrProperty), typeof(StrProperty) },
                { nameof(StructProperty), typeof(StructProperty) },
                { nameof(TextProperty), typeof(TextProperty) },
                { nameof(MulticastDelegateProperty), typeof(MulticastDelegateProperty) },
                { nameof(UInt32Property), typeof(UInt32Property) },
                { nameof(UInt64Property), typeof(UInt64Property) }
            };
        }

        public abstract void Deserialize(BinaryReader reader, long size, bool includeHeader);

        public abstract long Serialize(BinaryWriter writer, bool includeHeader);

        public static UProperty Deserialize(BinaryReader reader, FString? overrideName = null)
        {
            FString name = reader.ReadUnrealString() ?? throw new FormatException("Error reading property name");
            if (name == "None") return new NoneProperty();

            FString type = reader.ReadUnrealString() ?? throw new FormatException("Error reading property type");
            long size = reader.ReadInt64();

            UProperty property = (UProperty)Activator.CreateInstance(ResolveType(type), overrideName ?? name, type)!;

            property.Deserialize(reader, size, true);
            return property;
        }

        public long Serialize(BinaryWriter writer)
        {
            long size = ContentSize;

            writer.WriteUnrealString(Name);
            writer.WriteUnrealString(Type);

            if (ContentSize >= 0)
            {
                // Optimized path for types that know their size upfront
                writer.Write(ContentSize);
                Serialize(writer, true);
            }
            else
            {
                // We need to write the data size, but we don't know it yet. Write a 0 for now, then come back and change it after writing the data.
                long sizePosition = writer.BaseStream.Position;
                writer.Write((long)0);
                
                size = Serialize(writer, true);
                long returnPosition = writer.BaseStream.Position;

                writer.BaseStream.Seek(sizePosition, SeekOrigin.Begin);
                writer.Write(size);
                writer.BaseStream.Seek(returnPosition, SeekOrigin.Begin);
            }

            return size;
        }

        public static Type ResolveType(FString typeName)
        {
            if (typeName.Value == null) throw new ArgumentNullException(nameof(typeName));

            Type? type;
            if (sTypeMap.TryGetValue(typeName.Value, out type))
            {
                return type;
            }

            if (typeName == "None")
            {
                throw new FormatException("A property should never have the type \"None\"");
            }

            throw new NotImplementedException($"Property type {typeName} has not been implemented.");
        }

        public UProperty Clone(FString? overrideName = null)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                using (BinaryWriter writer = new BinaryWriter(stream, Encoding.ASCII, true))
                {
                    Serialize(writer);
                }
                stream.Seek(0, SeekOrigin.Begin);
                using (BinaryReader reader = new BinaryReader(stream, Encoding.ASCII, true))
                {
                    return Deserialize(reader, overrideName);
                }
            }
        }

        public override int GetHashCode()
        {
            int hash = 17;
            hash = hash * 23 + Name.GetHashCode();
            hash = hash * 23 + Type.GetHashCode();
            return hash;
        }

        public override bool Equals(object? obj)
        {
            return obj is UProperty up && Name.Equals(up.Name) && Type.Equals(up.Type);
        }

        public override string ToString()
        {
            return $"{Name} [{Type}] {Value}";
        }
    }

    public abstract class UProperty<T> : UProperty
    {
        protected UProperty(FString name, FString type)
            : base(name, type)
        {
        }

        public new T? Value
        {
            get => (T?)base.Value;
            set => base.Value = value;
        }
    }
}
