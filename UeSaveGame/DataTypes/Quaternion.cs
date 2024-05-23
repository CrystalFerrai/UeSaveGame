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
    public struct FQuat
    {
        public static readonly FQuat Identity;

        public double X;
        public double Y;
        public double Z;
        public double W;

        static FQuat()
        {
            Identity.X = 0.0;
            Identity.Y = 0.0;
            Identity.Z = 0.0;
            Identity.W = 1.0;
        }

        public override string ToString()
        {
            return $"{X} {Y} {Z} {W}";
        }
    }
}
