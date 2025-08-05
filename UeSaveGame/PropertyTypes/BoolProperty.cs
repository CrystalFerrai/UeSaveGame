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

namespace UeSaveGame.PropertyTypes
{
	public class BoolProperty : FProperty<bool>
	{
		public override bool IsSimpleProperty => true;

		public BoolProperty(FString name)
			: base(name)
		{
		}

		protected internal override void DeserializeValue(BinaryReader reader, int size, PackageVersion packageVersion)
		{
			Value = reader.ReadByte() != 0;
		}

		protected internal override int SerializeValue(BinaryWriter writer, PackageVersion packageVersion)
		{
			writer.Write((byte)(Value ? 1 : 0));
			return 1;
		}
	}
}
