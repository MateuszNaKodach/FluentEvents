﻿using System;
using System.Collections.Generic;
using System.Reflection;

namespace FluentEvents.Utils
{
    internal static class TypeExtensions
    {
        internal static IEnumerable<Type> GetBaseTypesAndInterfacesInclusive(this Type type)
        {
            foreach (var typeInterface in type.GetInterfaces())
                yield return typeInterface;

            while (type != null)
            {
                yield return type;

                type = type.BaseType;
            }
        }

        internal static FieldInfo GetFieldFromBaseTypesInclusive(this Type type, string name, BindingFlags bindingFlags)
        {
            FieldInfo eventFieldInfo = null;
            while (eventFieldInfo == null && type != null)
            {
                eventFieldInfo = type.GetField(name, bindingFlags);

                if (eventFieldInfo == null)
                    type = type.BaseType;
            }

            return eventFieldInfo;
        }
    }
}
