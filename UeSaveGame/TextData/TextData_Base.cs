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
	public class TextData_Base : ITextData
    {
        private FString? mNamespace;
        private FString? mKey;
        private FString? mSourceString;

        public void Deserialize(BinaryReader reader, long size)
        {
            mNamespace = reader.ReadUnrealString();
            mKey = reader.ReadUnrealString();
            mSourceString = reader.ReadUnrealString();
        }

        public long Serialize(BinaryWriter writer)
        {
            writer.WriteUnrealString(mNamespace);
            writer.WriteUnrealString(mKey);
            writer.WriteUnrealString(mSourceString);

            return 12 + (mNamespace?.SizeInBytes ?? 0) + (mKey?.SizeInBytes ?? 0) + (mSourceString?.SizeInBytes ?? 0);
        }

        public override string ToString()
        {
            return mSourceString?.Value ?? string.Empty;
        }
    }
}
