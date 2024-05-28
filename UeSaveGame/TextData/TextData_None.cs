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

using UeSaveGame.Util;

namespace UeSaveGame.TextData
{
	public class TextData_None : ITextData
    {
        public FString?[]? Values { get; set; }

        public void Deserialize(BinaryReader reader, long size)
        {
            int count = reader.ReadInt32();
            Values = new FString[count];
            for (int i = 0; i < count; ++i)
            {
                Values[i] = reader.ReadUnrealString();
            }
        }

        public long Serialize(BinaryWriter writer)
        {
            if (Values == null) throw new InvalidOperationException("Instance is not valid for serialization");

            writer.Write(Values.Length);
            long len = 4;
            foreach (FString? value in Values)
            {
                writer.WriteUnrealString(value);
                len += 4 + (value?.SizeInBytes ?? 0);
            }

            return len;
        }

        public override string ToString()
        {
            return Values == null ? String.Empty : string.Join(",", Values.Select(v => v?.Value));
        }
    }
}
