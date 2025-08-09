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

using System.Runtime.Serialization;

namespace UeSaveGame
{
	/// <summary>
	/// Exception thrown when a type is missing a required attribute
	/// </summary>
	[Serializable]
	public class MissingAttributeException : Exception
	{
		private readonly bool mHasMessage;

		public Type ObjectType { get; }

		public Type AttributeType { get; }

		public override string Message => mHasMessage ? base.Message : $"Type '{ObjectType.FullName}' is missing required attribute '{AttributeType.FullName}'.";

		public MissingAttributeException(Type objectType, Type attributeType)
		{
			mHasMessage = false;
			ObjectType = objectType;
			AttributeType = attributeType;
		}

		public MissingAttributeException(Type attributeType, Type objectType, string? message)
			: base(message)
		{
			mHasMessage = true;
			ObjectType = objectType;
			AttributeType = attributeType;
		}

		public MissingAttributeException(Type attributeType, Type objectType, string? message, Exception? innerException)
			: base(message, innerException)
		{
			mHasMessage = true;
			ObjectType = objectType;
			AttributeType = attributeType;
		}
	}

	/// <summary>
	/// Exception thrown when a disallowed duplicate registration is attempted
	/// </summary>
	[Serializable]
	public class DuplicateRegistrationException : Exception
	{
		private readonly bool mHasMessage;

		public object RegisteredObject { get; }

		public override string Message => mHasMessage ? base.Message : $"'{RegisteredObject}' has already been registered";

		public DuplicateRegistrationException(object registeredObject)
		{
			mHasMessage = false;
			RegisteredObject = registeredObject;
		}

		public DuplicateRegistrationException(object registeredObject, string? message)
			: base(message)
		{
			mHasMessage = true;
			RegisteredObject = registeredObject;
		}

		public DuplicateRegistrationException(object registeredObject, string? message, Exception? innerException)
			: base(message, innerException)
		{
			mHasMessage = true;
			RegisteredObject = registeredObject;
		}
	}
}
