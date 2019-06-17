﻿using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace FluentEvents.Azure.ServiceBus
{
    /// <inheritdoc />
    /// <summary>
    ///     An exception thrown by the FluentEvents.Azure.ServiceBus plugin.
    /// </summary>
    public abstract class FluentEventsServiceBusException : FluentEventsException
    {
        /// <inheritdoc />
        /// <summary>
        ///     Initializes a new instance of the <see cref="FluentEventsServiceBusException" /> class.
        /// </summary>
        protected FluentEventsServiceBusException(string message) : base(message)
        {
        }

        /// <inheritdoc />
        /// <summary>
        ///     Initializes a new instance of the <see cref="FluentEventsServiceBusException" /> class.
        /// </summary>
        protected FluentEventsServiceBusException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}
