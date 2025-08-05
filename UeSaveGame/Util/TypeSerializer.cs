// Copyright 2025 Crystal Ferrai
//
// Licensed under the Apache License, Version 2.0 (the "License";
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

using UeSaveGame.PropertyTypes;

namespace UeSaveGame.Util
{
	/// <summary>
	/// Converts UProperty instances to and from managed types
	/// </summary>
	public class TypeSerializer
	{
		private static Dictionary<Type, Type> sPropertyTypesToManagedTypes;

		static TypeSerializer()
		{
			sPropertyTypesToManagedTypes = new()
			{
				// Commented types have no implementation because they have not yet been encountered
				//{ typeof(ArrayProperty), typeof(Array) },
				//{ typeof(BoolProperty), typeof(bool) },
				//{ typeof(ByteProperty), typeof(byte) },
				////typeof(DelegateProperty)
				//{ typeof(DoubleProperty), typeof(double) },
				//{ typeof(FloatProperty), typeof(float) },
				////{ typeof(Int16Property), typeof(short) },
				////{ typeof(Int32Property), typeof(int) },
				//{ typeof(Int64Property), typeof(long) },
				////{ typeof(Int8Property), typeof(sbyte) },
				//{ typeof(IntProperty), typeof(int) },
				////typeof(InterfaceProperty)
				////typeof(LazyObjectProperty)
				//{ typeof(MapProperty), typeof(Dictionary) },
				//{ typeof(MulticastDelegateProperty), typeof(UDelegate) },
				//{ typeof(NameProperty), typeof(FName) },
				//{ typeof(ObjectProperty), typeof(UObject) },
				////{ typeof(RotatorProperty), typeof(FRotator) },
				//{ typeof(SetProperty), typeof(HashSet) },
				//{ typeof(SoftObjectProperty), typeof(FSoftObjectPtr) },
				//{ typeof(StrProperty), typeof(string) },
				//{ typeof(StructProperty), typeof(F) },
				//{ typeof(TextProperty), typeof(FText) },
				////{ typeof(UInt16Property), typeof(ushort) },
				//{ typeof(UInt32Property), typeof(uint) },
				//{ typeof(UInt64Property), typeof(ulong) },
				////{ typeof(VectorProperty), typeof(FVector) }
			};
		}
	}
}
