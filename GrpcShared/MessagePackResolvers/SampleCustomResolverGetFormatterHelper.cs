using MessagePack.Formatters;
using System;
using System.Collections.Generic;

namespace GrpcShared.MessagePackResolvers
{
	internal static class SampleCustomResolverGetFormatterHelper
	{
		// If type is concrete type, use type-formatter map
		static readonly Dictionary<Type, object> formatterMap = new Dictionary<Type, object>
		{
			[typeof(HelloReply)] = new HelloReplyFormatter()
		};

		internal static object GetFormatter(Type t)
		{
			object formatter;
			if (formatterMap.TryGetValue(t, out formatter))
			{
				return formatter;
			}

			// If target type is generics, use MakeGenericType.
			if (t.IsGenericParameter && t.GetGenericTypeDefinition() == typeof(ValueTuple<,>))
			{
				return Activator.CreateInstance(typeof(ValueTupleFormatter<,>).MakeGenericType(t.GenericTypeArguments));
			}

			// If type can not get, must return null for fallback mechanism.
			return null;
		}
	}

}

