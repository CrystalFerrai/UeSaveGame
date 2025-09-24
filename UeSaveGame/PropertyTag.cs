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

using System.Reflection.PortableExecutable;
using UeSaveGame.PropertyTypes;
using UeSaveGame.Util;

namespace UeSaveGame
{
	public class FPropertyTag
	{
		private long mSizeOffset;

		/// <summary>
		/// Gets the name of the property
		/// </summary>
		public FString Name { get; }

		/// <summary>
		/// Gets the name of the type of the property's value
		/// </summary>
		public FPropertyTypeName Type { get; }

		public FProperty? Property { get; set; }

		internal int Size { get; private set; }

		internal int ArrayIndex { get; }

		/// <summary>
		/// Property serialization flags
		/// </summary>
		internal EPropertyTagFlags Flags { get; set; }

		public bool IsNone { get; private set; }

		public static FPropertyTag NoneProperty = new(new("None"), new(new("None")), 0, 0, null, EPropertyTagFlags.None) { IsNone = true };

		public FPropertyTag(FString name, FPropertyTypeName type, EPropertyTagFlags flags)
			: this(name, type, 0, 0, null, flags)
		{
		}

		internal FPropertyTag(FString name, FPropertyTypeName type, int size, int arrayIndex, FProperty? property, EPropertyTagFlags flags)
		{
			mSizeOffset = 0;

			Name = name;
			Type = type;
			Property = property;
			Size = size;
			ArrayIndex = arrayIndex;
			Flags = flags;

			IsNone = false;
		}

		/// <summary>
		/// Creates a new property tag using metadata from an existing property tag. The new
		/// tag will not contain any property unless it is later assigned one.
		/// </summary>
		/// <param name="other">The tag to clone metadata from</param>
		public FPropertyTag(FPropertyTag other)
		{
			Name = other.Name;
			Type = other.Type.Clone();
			Property = null;
			Size = 0;
			ArrayIndex = other.ArrayIndex;
			Flags = other.Flags;
		}

		/// <summary>
		/// Deserializes a new property tag
		/// </summary>
		/// <param name="reader">The reader to read the property from</param>
		/// <param name="packageVersion">The engine package serialization version</param>
		/// <param name="overrideName">If specified, overrides the name of the new property</param>
		/// <returns>The deserialized property</returns>
		public static FPropertyTag Deserialize(BinaryReader reader, PackageVersion packageVersion, FString? overrideName = null)
		{
			FString name = reader.ReadUnrealString() ?? throw new InvalidDataException("Error reading property name");
			if (name == "None") return NoneProperty;

			if (overrideName is not null)
			{
				name = overrideName;
			}

			FPropertyTypeName type = FPropertyTypeName.Deserialize(reader, packageVersion);

			int size = reader.ReadInt32();
			int arrayIndex = 0;
			if (packageVersion < EObjectUE5Version.PROPERTY_TAG_COMPLETE_TYPE_NAME)
			{
				arrayIndex = reader.ReadInt32();
			}

			FProperty property = FProperty.Create(name, type);
			property.ProcessTypeName(type, packageVersion);

			property.DeserializeHeader(reader, packageVersion);

			EPropertyTagFlags flags = EPropertyTagFlags.None;
			if (packageVersion >= EObjectUE5Version.PROPERTY_TAG_COMPLETE_TYPE_NAME)
			{
				flags = (EPropertyTagFlags)reader.ReadByte();
				if (flags.HasFlag(EPropertyTagFlags.HasArrayIndex))
				{
					arrayIndex = reader.ReadInt32();
				}
				if (flags.HasFlag(EPropertyTagFlags.HasPropertyGuid))
				{
					throw new NotImplementedException();
				}
				if (flags.HasFlag(EPropertyTagFlags.HasPropertyExtensions))
				{
					throw new NotImplementedException();
				}

				if (property is BoolProperty bp)
				{
					bp.Value = flags.HasFlag(EPropertyTagFlags.BoolTrue);
				}
			}
			else
			{
				byte b = reader.ReadByte();
				if (property is BoolProperty bp)
				{
					bp.Value = b == 1;
				}
			}

			return new(name, type, size, arrayIndex, property, flags);
		}

		/// <summary>
		/// Serialize this property tag
		/// </summary>
		/// <param name="writer">The writer to write the proiperty to</param>
		/// <param name="engineVersion">The version of Unreal Engine to serialize this property for</param>
		public int Serialize(BinaryWriter writer, PackageVersion packageVersion)
		{
			long startPos = writer.BaseStream.Position;

			writer.WriteUnrealString(Name);
			Type.Serialize(writer, packageVersion);

			mSizeOffset = writer.BaseStream.Position;
			writer.Write(0);
			if (packageVersion < EObjectUE5Version.PROPERTY_TAG_COMPLETE_TYPE_NAME)
			{
				writer.Write(ArrayIndex);
			}

			Property?.SerializeHeader(writer, packageVersion);

			if (packageVersion >= EObjectUE5Version.PROPERTY_TAG_COMPLETE_TYPE_NAME)
			{
				if (Property is BoolProperty bp)
				{
					if (bp.Value)
					{
						Flags |= EPropertyTagFlags.BoolTrue;
					}
					else
					{
						Flags &= ~EPropertyTagFlags.BoolTrue;
					}
				}

				writer.Write((byte)Flags);
				if (Flags.HasFlag(EPropertyTagFlags.HasArrayIndex))
				{
					writer.Write(ArrayIndex);
				}
				if (Flags.HasFlag(EPropertyTagFlags.HasPropertyGuid))
				{
					throw new NotImplementedException();
				}
				if (Flags.HasFlag(EPropertyTagFlags.HasPropertyExtensions))
				{
					throw new NotImplementedException();
				}
			}
			else
			{
				if (Property is BoolProperty bp && bp.Value)
				{
					writer.Write((byte)1);
				}
				else
				{
					writer.Write((byte)0);
				}
			}

			return (int)(writer.BaseStream.Position - startPos);
		}

		public void DeserializeProperty(BinaryReader reader, PackageVersion packageVersion)
		{
			if (Property is BoolProperty bp)
			{
				if (packageVersion < EObjectUE5Version.PROPERTY_TAG_COMPLETE_TYPE_NAME)
				{
					reader.ReadByte();
				}
				return;
			}

			Property?.DeserializeValue(reader, Size, packageVersion);
		}

		public int SerializeProperty(BinaryWriter writer, PackageVersion packageVersion)
		{
			if (Property is BoolProperty bp)
			{
				if (packageVersion < EObjectUE5Version.PROPERTY_TAG_COMPLETE_TYPE_NAME)
				{
					writer.Write((byte)0);
				}
				return 0;
			}

			int size = Property?.SerializeValue(writer, packageVersion) ?? 0;
			WriteSize(writer, size, packageVersion);
			return size;
		}

		internal void WriteSize(BinaryWriter writer, int size, PackageVersion packageVersion)
		{
			Size = size;

			long offset = writer.BaseStream.Position;
			writer.BaseStream.Seek(mSizeOffset, SeekOrigin.Begin);

			writer.Write(Size);

			writer.BaseStream.Seek(offset, SeekOrigin.Begin);
		}

		public override string ToString()
		{
			return Name;
		}
	}

	public enum EPropertyTagFlags : byte
	{
		None = 0x00,
		HasArrayIndex = 0x01,
		HasPropertyGuid = 0x02,
		HasPropertyExtensions = 0x04,
		HasBinaryOrNativeSerialize = 0x08,
		BoolTrue = 0x10,
		SkippedSerialize = 0x20
	};
}
