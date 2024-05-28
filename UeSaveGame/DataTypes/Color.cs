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
    public struct FColor
    {
        public byte R;
        public byte G;
        public byte B;
        public byte A;

        public static bool TryParse(string? s, out FColor result)
        {
            if (s is null)
            {
                result = default;
                return false;
            }

            if (s.StartsWith('#'))
            {
                s = s[1..];
            }

            int value = 0;

            if (s.Length == 6 || s.Length == 8)
            {
                FColor instance = new();
                if (int.TryParse(s, out value))
                {
                    switch (s.Length)
                    {
                        case 6:
                            instance.R = (byte)((value >> 16) & 0xff);
                            instance.G = (byte)((value >> 8) & 0xff);
                            instance.B = (byte)(value & 0xff);
                            instance.A = 0xff;
                            break;
                        case 8:
							instance.R = (byte)((value >> 24) & 0xff);
							instance.G = (byte)((value >> 16) & 0xff);
							instance.B = (byte)((value >> 8) & 0xff);
							instance.A = (byte)(value & 0xff);
							break;
                    }
                }
                result = instance;
                return true;
            }

            result = default;
            return false;
        }

        public readonly override string ToString()
        {
            return $"#{R:x2}{G:x2}{B:x2}{A:x2}";
        }
    }
}
