using MessagePack;
using MessagePack.Formatters;

namespace GrpcShared.MessagePackResolvers
{
	public class CustomResolver : IFormatterResolver
	{
		public static readonly IFormatterResolver Instance = new CustomResolver();
		public static readonly MessagePackSerializerOptions Options = MessagePackSerializerOptions.Standard.WithResolver(Instance);

		private CustomResolver()
		{

		}

		public IMessagePackFormatter<T> GetFormatter<T>()
			=> FormatterCache<T>.Formatter;

		private static class FormatterCache<T>
		{
			public static readonly IMessagePackFormatter<T> Formatter;

			// generic's static constructor should be minimized for reduce type generation size!
			// use outer helper method.
			static FormatterCache()
			{
				Formatter = (IMessagePackFormatter<T>)SampleCustomResolverGetFormatterHelper.GetFormatter(typeof(T));
			}
		}
	}

}

