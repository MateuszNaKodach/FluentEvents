using FluentEvents.Azure.ServiceBus.Common;
using Microsoft.Extensions.Options;

namespace FluentEvents.Azure.ServiceBus.Queues.Receiving
{
    internal class QueueEventReceiverConfigValidator : EventReceiverConfigValidatorBase
        , IValidateOptions<QueueEventReceiverConfig>
    {
        public ValidateOptionsResult Validate(string name, QueueEventReceiverConfig options)
        {
            return Validate(options);
        }
    }
}
