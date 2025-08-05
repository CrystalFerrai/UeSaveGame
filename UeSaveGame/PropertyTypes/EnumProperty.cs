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
	public class EnumProperty : FProperty<FString>
	{
		public FPropertyTypeName? EnumType { get; internal set; }

		public override bool IsSimpleProperty => true;

		public EnumProperty(FString name)
			: base(name)
		{
		}

		public EnumProperty(FString name, FPropertyTypeName? enumType)
			: this(name)
		{
			EnumType = enumType;
		}

		protected internal override void ProcessTypeName(FPropertyTypeName typeName, PackageVersion packageVersion)
		{
			if (packageVersion >= EObjectUE5Version.PROPERTY_TAG_COMPLETE_TYPE_NAME)
			{
				if (typeName.Parameters.Count != 2)
				{
					throw new InvalidDataException("Failed to read enum type for EnumProperty");
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
			Value = reader.ReadUnrealString();

			if (Value?.Value is not null && EnumType is null)
			{
				EnumType = new(new(Value.Value.Substring(0, Value.Value.IndexOf(":")), Value.Encoding));
			}
		}

		protected internal override void SerializeHeader(BinaryWriter writer, PackageVersion packageVersion)
		{
			if (packageVersion < EObjectUE5Version.PROPERTY_TAG_COMPLETE_TYPE_NAME)
			{
				if (EnumType == null) throw new InvalidOperationException("Instance is not valid for serialization");
				writer.WriteUnrealString(EnumType.Name);
			}
		}

		protected internal override int SerializeValue(BinaryWriter writer, PackageVersion packageVersion)
		{
			if (Value == null) throw new InvalidOperationException("Instance is not valid for serialization");

			writer.WriteUnrealString(Value);

			return 4 + (Value?.SizeInBytes ?? 0);
		}
	}
}
