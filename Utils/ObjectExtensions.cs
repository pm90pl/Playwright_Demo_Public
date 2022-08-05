namespace BBlog.Tests.Utils;

public static class ObjectExtensions
{
    public static void SetPropertyValue(this object obj, string property, object? value)
    {
        obj.GetType()!
            .GetProperty(property)!
            .SetValue(obj, value);
    }

}