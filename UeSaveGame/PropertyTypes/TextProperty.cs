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

using UeSaveGame.DataTypes;

namespace UeSaveGame.PropertyTypes
{
	public class TextProperty : FProperty<FText>
	{
		public TextProperty(FString name)
			: base(name)
		{
		}

		protected internal override void DeserializeValue(BinaryReader reader, int size, PackageVersion packageVersion)
		{
			Value = new FText();
			Value.Deserialize(reader, packageVersion);
		}

		protected internal override int SerializeValue(BinaryWriter writer, PackageVersion packageVersion)
		{
			if (Value is null) throw new InvalidOperationException("TextProperty has no value to serialize");
			return Value.Serialize(writer, packageVersion);
		}
	}
}
