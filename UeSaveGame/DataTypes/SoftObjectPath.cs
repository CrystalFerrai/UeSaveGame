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

using UeSaveGame.Util;

namespace UeSaveGame.DataTypes
{
	/// <summary>
	/// The path to an object within a game asset
	/// </summary>
	public class SoftObjectPath
	{
		public FString? PackageName { get; set; }

		public FString? AssetName { get; set; }

		public FString? SubPathString { get; set; }

		public void Deserialize(BinaryReader reader, PackageVersion packageVersion)
		{
			if (packageVersion < EObjectUE5Version.FSOFTOBJECTPATH_REMOVE_ASSET_PATH_FNAMES)
			{
				FString? path = reader.ReadUnrealString();
				if (path is not null)
				{
					int dotIndex = path.Value.IndexOf('.');
					if (dotIndex > 0)
					{
						PackageName = new(path.Value[..dotIndex]);
						AssetName = new(path.Value[(dotIndex + 1)..]);
					}
					else
					{
						PackageName = path;
					}
				}
			}
			else // Assuming 5 or later
			{
				PackageName = reader.ReadUnrealString();
				AssetName = reader.ReadUnrealString();
			}

			SubPathString = reader.ReadUnrealString();
		}

		public int Serialize(BinaryWriter writer, PackageVersion packageVersion)
		{
			long startPos = writer.BaseStream.Position;

			if (packageVersion < EObjectUE5Version.FSOFTOBJECTPATH_REMOVE_ASSET_PATH_FNAMES)
			{
				FString? path = PackageName;
				if (PackageName is not null && AssetName is not null)
				{
					path = new($"{PackageName},{AssetName}");
				}
				writer.WriteUnrealString(path);
			}
			else
			{
				writer.WriteUnrealString(PackageName);
				writer.WriteUnrealString(AssetName);
			}

			writer.WriteUnrealString(SubPathString);

			return (int)(writer.BaseStream.Position - startPos);
		}

		public override string ToString()
		{
			if (SubPathString is null)
			{
				return $"{PackageName}.{AssetName}";
			}
			return $"{PackageName}.{AssetName}.{SubPathString}";
		}
	}
}
