using FluentEvents.Azure.ServiceBus.Common;
using Microsoft.Extensions.Options;

namespace FluentEvents.Azure.ServiceBus.Topics.Sending
{
    internal class TopicEventSenderConfigValidator
        : EventSenderConfigValidatorBase
            , IValidateOptions<TopicEventSenderConfig>
    {
        public ValidateOptionsResult Validate(string name, TopicEventSenderConfig options)
        {
            return Validate(options);
        }
    }
}