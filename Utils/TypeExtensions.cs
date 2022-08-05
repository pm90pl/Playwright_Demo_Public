using System;

namespace BBlog.Tests.Utils;

public static class TypeExtensions
{
    public static T GetAttribute<T>(this Type type) where T : Attribute
    {
        return Attribute.IsDefined(type, typeof(T))
            ? (T)Attribute.GetCustomAttribute(type, typeof(T), false)!
            : throw new AttributeWasNotDefinedException();
    }

}