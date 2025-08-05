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

using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace UeSaveGame.DataTypes
{
	public struct FDateTime : IFormattable
	{
		// FDateTime and System.DateTime use the same underlying Ticks value
		private DateTime mValue;

		public DateTime Value
		{
			readonly get => mValue;
			set => mValue = value;
		}

		public long Ticks
		{
			readonly get => mValue.Ticks;
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

		public static bool TryParse([NotNullWhen(true)] string? s, out FDateTime result)
		{
			if (s is null)
			{
				result = default;
				return false;
			}

			return TryParse(s.AsSpan(), DateTimeFormatInfo.CurrentInfo, DateTimeStyles.None, out result);
		}

		public static bool TryParse(ReadOnlySpan<char> s, out FDateTime result)
		{
			return TryParse(s, DateTimeFormatInfo.CurrentInfo, DateTimeStyles.None, out result);
		}

		public static bool TryParse([NotNullWhen(true)] string? s, IFormatProvider? provider, DateTimeStyles styles, out FDateTime result)
		{
			if (s is null)
			{
				result = default;
				return false;
			}

			return TryParse(s.AsSpan(), provider, styles, out result);
		}

		public static bool TryParse(ReadOnlySpan<char> s, IFormatProvider? provider, DateTimeStyles styles, out FDateTime result)
		{
			if (DateTime.TryParse(s, provider, styles, out DateTime dt))
			{
				result = new FDateTime(dt);
				return true;
			}
			result = default;
			return false;
		}

		public override readonly string ToString()
		{
			return mValue.ToString();
		}

		public readonly string ToString(string? format)
		{
			return mValue.ToString(format);
		}

		public readonly string ToString(IFormatProvider? formatProvider)
		{
			return mValue.ToString(formatProvider);
		}

		public readonly string ToString(string? format, IFormatProvider? formatProvider)
		{
			return mValue.ToString(format, formatProvider);
		}
	}
}
