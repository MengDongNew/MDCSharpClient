using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MDCSharpClient
{
    class EventDataTool
    {
        public static EventData CreateEventData(ServerPacket serverPacket)
        {
            var reader = serverPacket.arrByteReader;
            EventData eventData = new EventData()
            {
                Code = serverPacket.Code,
            };
            Dictionary<byte, object> parameters = new Dictionary<byte, object>();
            eventData.Parameters = parameters;
            while (reader.ReadLen<serverPacket.len)
            {
                byte key = reader.ReadByte();
                string value = reader.ReadUTF8String();
                parameters.Add(key, value);
            }

            return eventData;
        }
    }
}
