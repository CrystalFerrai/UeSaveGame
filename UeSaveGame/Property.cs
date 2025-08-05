// Copyright 2025 Crystal Ferrai
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

namespace UeSaveGame
{
	/// <summary>
	/// Represents an Unreal property with a name, type and value
	/// </summary>
	public abstract class FProperty
	{
		// Map of type names to propery types
		private static readonly Dictionary<string, Type> sTypeMap;

		// Name of the property tag that owns this property
		protected readonly FString mPropertyName;

		/// <summary>
		/// Gets a mapping of property type names to property types
		/// </summary>
		public static IReadOnlyDictionary<string, Type> PropertyTypeMap => sTypeMap;

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

		protected FProperty(FString name)
		{
			mPropertyName = name;
		}

		static FProperty()
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
				{ nameof(MulticastInlineDelegateProperty), typeof(MulticastInlineDelegateProperty) },
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

		public static FProperty Create(FString name, FPropertyTypeName type)
		{
			return (FProperty)Activator.CreateInstance(ResolveType(type.Name), name)!;
		}

		protected internal virtual void ProcessTypeName(FPropertyTypeName typeName, PackageVersion packageVersion)
		{
		}

		/// <summary>
		/// Deserialize custom header for this property
		/// </summary>
		/// <param name="reader">Reader to read data from</param>
		/// <param name="packageVersion">The engine package serialization version</param>
		protected internal virtual void DeserializeHeader(BinaryReader reader, PackageVersion packageVersion)
		{
		}

		/// <summary>
		/// Serialize this property's custom header
		/// </summary>
		/// <param name="writer">Writer to write data to</param>
		/// <param name="packageVersion">The engine package serialization version</param>
		/// <returns>The size of the serialized data</returns>
		protected internal virtual void SerializeHeader(BinaryWriter writer, PackageVersion packageVersion)
		{
		}

		/// <summary>
		/// Deserialize data for this property
		/// </summary>
		/// <param name="reader">Reader to read data from</param>
		/// <param name="size">The size of the property (from the propery metadata)</param>
		/// <param name="packageVersion">The engine package serialization version</param>
		protected internal abstract void DeserializeValue(BinaryReader reader, int size, PackageVersion packageVersion);

		/// <summary>
		/// Serialize this property's data
		/// </summary>
		/// <param name="writer">Writer to write data to</param>
		/// <param name="packageVersion">The engine package serialization version</param>
		/// <returns>The size of the serialized data</returns>
		protected internal abstract int SerializeValue(BinaryWriter writer, PackageVersion packageVersion);

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

		public override string? ToString()
		{
			return Value?.ToString();
		}
	}

	/// <summary>
	/// Base class for properties with a fixed value type
	/// </summary>
	/// <typeparam name="T">The type of the property's value</typeparam>
	public abstract class FProperty<T> : FProperty
	{
		protected FProperty(FString name)
			: base(name)
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
