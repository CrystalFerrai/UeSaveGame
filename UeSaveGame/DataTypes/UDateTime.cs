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
    public struct UDateTime : IFormattable
    {
		private DateTime mDateTime;

        public static UDateTime Now => new UDateTime(DateTime.Now);

        public static UDateTime UtcNow => new UDateTime(DateTime.UtcNow);

        public DateTime Value
        {
            get => mDateTime;
            set => mDateTime = value;
        }

        public long Ticks
		{
			get => mDateTime.Ticks;
			set => mDateTime = new DateTime(value);
		}

        public UDateTime(long ticks)
		{
            mDateTime = new DateTime(ticks);
		}

        public UDateTime(DateTime value)
		{
            mDateTime = value;
		}

        public override string ToString()
        {
            return mDateTime.ToString();
        }

        public string ToString(string format)
		{
            return mDateTime.ToString(format);
        }

        public string ToString(IFormatProvider provider)
        {
            return mDateTime.ToString(provider);
        }

        public string ToString(string format, IFormatProvider provider)
        {
            return mDateTime.ToString(format, provider);
        }
    }
}
