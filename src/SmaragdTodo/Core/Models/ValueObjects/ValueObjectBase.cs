namespace Core.Models.ValueObjects;

public class ValueObjectBase<TSelf, TValue>
    where TSelf : ValueObjectBase<TSelf, TValue>, new()
{
    public TValue Value { get; private init; } = default!;

    public static TSelf From(TValue value) => new TSelf
    {
        Value = value
    };

    public override int GetHashCode()
    {
        ArgumentNullException.ThrowIfNull(Value);

        return Value.GetHashCode();
    }

    public override string? ToString()
    {
        ArgumentNullException.ThrowIfNull(Value);

        return Value.ToString();
    }

    public override bool Equals(object? obj)
    {
        if (obj is not ValueObjectBase<TSelf, TValue> casted)
        {
            return false;
        }

        var value = Value ?? throw new ArgumentNullException(nameof(Value));

        return value.Equals(casted.Value);
    }

    public static bool operator ==(ValueObjectBase<TSelf, TValue> left, ValueObjectBase<TSelf, TValue> right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(ValueObjectBase<TSelf, TValue> left, ValueObjectBase<TSelf, TValue> right)
    {
        return !left.Equals(right);
    }
}