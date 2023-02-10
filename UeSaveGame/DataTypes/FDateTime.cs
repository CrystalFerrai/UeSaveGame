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
    public struct FDateTime : IFormattable
    {
        // FDateTime and System.DateTime use the same underlying Ticks value
        private DateTime mValue;

        public DateTime Value
		{
            get => mValue;
            set => mValue = value;
		}

        public long Ticks
		{
            get => mValue.Ticks;
            set => mValue = new DateTime(value);
		}

        public FDateTime(DateTime value)
		{
            mValue = value;
		}

        public FDateTime(long ticks)
		{
            mValue = new DateTime(ticks);
		}

        public override string ToString()
        {
            return Ticks.ToString();
        }

        public string ToString(string? format)
        {
            return mValue.ToString(format);
        }

        public string ToString(IFormatProvider? formatProvider)
        {
            return mValue.ToString(formatProvider);
        }

        public string ToString(string? format, IFormatProvider? formatProvider)
		{
            return mValue.ToString(format, formatProvider);
		}
	}
}
