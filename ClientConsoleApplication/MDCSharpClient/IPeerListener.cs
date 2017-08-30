using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MDCSharpClient
{
    public interface IPeerListener
    {
        void DebugReturn(DebugLevel level, string message);

        void OnEvent(EventData eventData);

        void OnOperationResponse(OperationResponse operationResponse);

        void OnStatusChanged(StatusCode statusCode);
    }
}
