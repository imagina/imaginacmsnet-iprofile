using Core.Events;
using Core.Events.Interfaces;

namespace Iprofile.Events.Handlers
{
    public class URlRequestParseHandler : EventHandlerBase
    {

        IEventBase _eventBase;

        public URlRequestParseHandler(IEventBase eventBase) : base(eventBase)
        {
            _eventBase = eventBase;
        }


        public override void EventBase_URLRequestBeingParsed(object? sender, object? e)
        {
            base.EventBase_URLRequestBeingParsed(sender, e);
        }



    }
}
