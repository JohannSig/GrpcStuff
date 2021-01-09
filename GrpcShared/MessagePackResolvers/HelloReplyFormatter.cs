using MessagePack;
using MessagePack.Formatters;
using System.Text;

namespace GrpcShared.MessagePackResolvers
{

	public class HelloReplyFormatter : IMessagePackFormatter<HelloReply>
	{
		public HelloReply Deserialize(ref MessagePackReader reader, MessagePackSerializerOptions options)
		{
			return new HelloReply { Message = reader.ReadString() };
		}

		public void Serialize(ref MessagePackWriter writer, HelloReply value, MessagePackSerializerOptions options)
		{
			writer.WriteString(Encoding.UTF8.GetBytes(value.Message));
		}
	}

}

