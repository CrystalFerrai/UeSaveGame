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

namespace UeSaveGame.DataTypes
{
    public struct FVector
    {
        public static readonly FVector Zero;
        public static readonly FVector One;

        public float X;
        public float Y;
        public float Z;

        static FVector()
        {
            Zero = new() { X = 0.0f, Y = 0.0f, Z = 0.0f };
            One = new() { X = 1.0f, Y = 1.0f, Z = 1.0f };
        }

        public override string ToString()
        {
            return $"{X} {Y} {Z}";
        }
    }
}
