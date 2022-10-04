namespace EventBus.Library.Attributes;

[AttributeUsage(AttributeTargets.Property)]
public class IncludeInHeaderAttribute : Attribute
{
    public string? Key { get; }
    public IncludeInHeaderAttribute()
    {
    }

    public IncludeInHeaderAttribute(string key)
    {
        Key = key;
    }
}
