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
	/// <summary>
	/// Represents an Unreal UProperty with a name, type and value
	/// </summary>
	public abstract class UProperty
    {
        // Map of type names to propery types
        private static readonly Dictionary<string, Type> sTypeMap;

        /// <summary>
        /// Gets a mapping of property type names to property types
        /// </summary>
        public static IReadOnlyDictionary<string, Type> PropertyTypeMap => sTypeMap;

        /// <summary>
        ///  Gets the name of the property
        /// </summary>
        public FString Name { get; }

        /// <summary>
        /// Gets the name of the type of the property's value
        /// </summary>
        public FString Type { get; }

        /// <summary>
        /// Gets or sets the property's value
        /// </summary>
        public object? Value { get; set; }

        /// <summary>
        /// A simple property is one that only serializes its Value property and nothing else.
        /// </summary>
        /// <remarks>
        /// The advantage of a simple property is more efficient allocation of arrays of this property type.
        /// Arrays for simple properties use the value type directly rather than an array of UProperty.
        /// </remarks>
        public virtual bool IsSimpleProperty => false;

        /// <summary>
        /// Type used to allocate arrays of this property when IsSimpleProperty == true
        /// </summary>
        public virtual Type SimpleValueType => typeof(object);

        /// <summary>
        /// Override if a property's size is known prior to serialization to make serializing the property faster
        /// </summary>
        protected virtual long ContentSize { get; }

        protected UProperty(FString name, FString type)
        {
            Name = name;
            Type = type;
            ContentSize = -1;
        }

        static UProperty()
        {
            // Add to this map if a new property type is created
            sTypeMap = new Dictionary<string, Type>()
			{
                // Commented types have no implementation because they have not yet been encountered
				{ nameof(ArrayProperty), typeof(ArrayProperty) },
				{ nameof(BoolProperty), typeof(BoolProperty) },
				{ nameof(ByteProperty), typeof(ByteProperty) },
				//{ nameof(DelegateProperty), typeof(DelegateProperty) },
				{ nameof(DoubleProperty), typeof(DoubleProperty) },
				{ nameof(EnumProperty), typeof(EnumProperty) },
				{ nameof(FloatProperty), typeof(FloatProperty) },
				//{ nameof(Int16Property), typeof(Int16Property) },
				//{ nameof(Int32Property), typeof(Int32Property) },
				{ nameof(Int64Property), typeof(Int64Property) },
				//{ nameof(Int8Property), typeof(Int8Property) },
				{ nameof(IntProperty), typeof(IntProperty) },
				//{ nameof(InterfaceProperty), typeof(InterfaceProperty) },
				//{ nameof(LazyObjectProperty), typeof(LazyObjectProperty) },
				{ nameof(MapProperty), typeof(MapProperty) },
				{ nameof(MulticastDelegateProperty), typeof(MulticastDelegateProperty) },
				{ nameof(NameProperty), typeof(NameProperty) },
				{ nameof(ObjectProperty), typeof(ObjectProperty) },
				//{ nameof(RotatorProperty), typeof(RotatorProperty) },
				{ nameof(SetProperty), typeof(SetProperty) },
				{ nameof(SoftObjectProperty), typeof(SoftObjectProperty) },
				{ nameof(StrProperty), typeof(StrProperty) },
				{ nameof(StructProperty), typeof(StructProperty) },
				{ nameof(TextProperty), typeof(TextProperty) },
				//{ nameof(UInt16Property), typeof(UInt16Property) },
				{ nameof(UInt32Property), typeof(UInt32Property) },
				{ nameof(UInt64Property), typeof(UInt64Property) },
				//{ nameof(VectorProperty), typeof(VectorProperty) }
			};
        }

        /// <summary>
        /// Deserialize data for this property
        /// </summary>
        /// <param name="reader">Reader to read data from</param>
        /// <param name="size">The size of the property (from the propery metadata)</param>
        /// <param name="includeHeader">Whether the serialzied property value includes a header</param>
        /// <param name="engineVersion">The version of Unreal Engine that was used to serialize this property</param>
        public abstract void Deserialize(BinaryReader reader, long size, bool includeHeader, PackageVersion packageVersion);

        /// <summary>
        /// Serialize this property's data
        /// </summary>
        /// <param name="writer">Writer to write data to</param>
        /// <param name="includeHeader">Whether to write a value header</param>
        /// <param name="engineVersion">The version of Unreal Engine to serialize this property for</param>
        /// <returns>The size of the serialized data</returns>
        public abstract long Serialize(BinaryWriter writer, bool includeHeader, PackageVersion packageVersion);

		/// <summary>
		/// Deserializes a new property
		/// </summary>
		/// <param name="reader">The reader to read the property from</param>
		/// <param name="engineVersion">The version of Unreal Engine that was used to serialize this property</param>
		/// <param name="overrideName">If specified, overrides the name of the new property</param>
		/// <returns>The deserialized property</returns>
		public static UProperty Deserialize(BinaryReader reader, PackageVersion packageVersion, FString? overrideName = null)
        {
            FString name = reader.ReadUnrealString() ?? throw new FormatException("Error reading property name");
            if (name == "None") return new NoneProperty();

            FString type = reader.ReadUnrealString() ?? throw new FormatException("Error reading property type");
            long size = reader.ReadInt64();

            UProperty property = (UProperty)Activator.CreateInstance(ResolveType(type), overrideName ?? name, type)!;

            property.Deserialize(reader, size, true, packageVersion);
            return property;
        }

		/// <summary>
		/// Serialize this property
		/// </summary>
		/// <param name="writer">The writer to write the proiperty to</param>
		/// <param name="engineVersion">The version of Unreal Engine to serialize this property for</param>
		/// <returns>The size of this property's data, not including metadata</returns>
		public long Serialize(BinaryWriter writer, PackageVersion packageVersion)
        {
            long size = ContentSize;

            writer.WriteUnrealString(Name);
            writer.WriteUnrealString(Type);

			if (ContentSize >= 0)
            {
                // Optimized path for types that know their size upfront
                writer.Write(ContentSize);
                Serialize(writer, true, packageVersion);
            }
            else
            {
                // We need to write the data size, but we don't know it yet. Write a 0 for now, then come back and change it after writing the data.
                long sizePosition = writer.BaseStream.Position;
                writer.Write((long)0);
                
                size = Serialize(writer, true, packageVersion);
                long returnPosition = writer.BaseStream.Position;

                writer.BaseStream.Seek(sizePosition, SeekOrigin.Begin);
                writer.Write(size);
                writer.BaseStream.Seek(returnPosition, SeekOrigin.Begin);
            }

            return size;
        }

        /// <summary>
        /// Lookup the property type for a given property type name
        /// </summary>
        /// <param name="typeName">The property type name to lookup</param>
        public static Type ResolveType(string typeName)
        {
            if (typeName is null) throw new ArgumentNullException(nameof(typeName));

            Type? type;
            if (sTypeMap.TryGetValue(typeName, out type))
            {
                return type;
            }

            if (typeName == "None")
            {
                throw new FormatException("A property should never have the type \"None\"");
            }

            throw new NotImplementedException($"Property type {typeName} has not been implemented.");
        }

        /// <summary>
        /// Clone this property to a new isntance, optionally with a new name
        /// </summary>
        /// <param name="overrideName">If specified, overrides the name of the new property</param>
        /// <returns>The new property</returns>
        public UProperty Clone(FString? overrideName = null)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                using (BinaryWriter writer = new(stream, Encoding.ASCII, true))
                {
                    Serialize(writer, PackageVersion.LatestTested);
                }
                stream.Seek(0, SeekOrigin.Begin);
                using (BinaryReader reader = new(stream, Encoding.ASCII, true))
                {
                    return Deserialize(reader, PackageVersion.LatestTested, overrideName);
                }
            }
        }

        public override int GetHashCode()
		{
			return HashCode.Combine(Name, Type);
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

    /// <summary>
    /// Base class for properties with a fixed value type
    /// </summary>
    /// <typeparam name="T">The type of the property's value</typeparam>
    public abstract class UProperty<T> : UProperty
    {
        protected UProperty(FString name, FString type)
            : base(name, type)
        {
        }

        public override Type SimpleValueType => typeof(T);

        public new T? Value
        {
            get => (T?)base.Value;
            set => base.Value = value;
        }
    }
}
