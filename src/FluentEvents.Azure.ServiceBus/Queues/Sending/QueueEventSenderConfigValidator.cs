using FluentEvents.Azure.ServiceBus.Common;
using Microsoft.Extensions.Options;

namespace FluentEvents.Azure.ServiceBus.Queues.Sending
{
    internal class QueueEventSenderConfigValidator : EventSenderConfigValidatorBase
        , IValidateOptions<QueueEventSenderConfig>
    {
        public ValidateOptionsResult Validate(string name, QueueEventSenderConfig options)
        {
            return Validate(options);
        }
    }
}
