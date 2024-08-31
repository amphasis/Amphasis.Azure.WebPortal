﻿using System.Reflection;
using System.Runtime.Serialization;

namespace Amphasis.Azure.Common.Extensions;

public static class EnumExtensions
{
	public static string? GetEnumMemberValue<T>(this T value) where T : struct, IConvertible
	{
		return typeof(T)
			.GetTypeInfo()
			.DeclaredMembers
			.SingleOrDefault(x => x.Name == value.ToString())
			?.GetCustomAttribute<EnumMemberAttribute>(false)
			?.Value;
	}
}