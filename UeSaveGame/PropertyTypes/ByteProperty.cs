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

using UeSaveGame.Util;

namespace UeSaveGame.PropertyTypes
{
	public class ByteProperty : FProperty
	{
		public FPropertyTypeName? EnumType { get; set; }

		public override bool IsSimpleProperty => true;

		public override Type SimpleValueType => Value is FString ? typeof(FString) : typeof(byte);

		public ByteProperty(FString name)
			: base(name)
		{
		}

		protected internal override void ProcessTypeName(FPropertyTypeName typeName, PackageVersion packageVersion)
		{
			if (packageVersion >= EObjectUE5Version.PROPERTY_TAG_COMPLETE_TYPE_NAME)
			{
				if (typeName.Parameters.Count != 1)
				{
					throw new InvalidDataException("Failed to read enum type for ByteProperty");
				}
				EnumType = typeName.Parameters[0];
			}
		}

		protected internal override void DeserializeHeader(BinaryReader reader, PackageVersion packageVersion)
		{
			if (packageVersion < EObjectUE5Version.PROPERTY_TAG_COMPLETE_TYPE_NAME)
			{
				EnumType = new(reader.ReadUnrealString()!);
			}
		}

		protected internal override void DeserializeValue(BinaryReader reader, int size, PackageVersion packageVersion)
		{
			switch (size)
			{
				case 1:
					Value = reader.ReadByte();
					break;
				default:
					Value = reader.ReadUnrealString();
					break;
			}
		}

		protected internal override void SerializeHeader(BinaryWriter writer, PackageVersion packageVersion)
		{
			if (packageVersion < EObjectUE5Version.PROPERTY_TAG_COMPLETE_TYPE_NAME)
			{
				writer.WriteUnrealString(EnumType!.Name);
			}
		}

		protected internal override int SerializeValue(BinaryWriter writer, PackageVersion packageVersion)
		{
			if (Value is byte b)
			{
				writer.Write(b);
			}
			else if (Value is FString s)
			{
				writer.WriteUnrealString(s);
			}

			return Value is byte ? 1 : 4 + (((FString?)Value)?.SizeInBytes ?? 0);
		}
	}
}
