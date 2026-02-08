using System.Text.RegularExpressions;

namespace Comments.Domain.ValueObjects;

public sealed partial class UserName : IEquatable<UserName>
{
    public string Value { get; }

    private UserName(string value) => Value = value;

    public static UserName Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("User name is required.", nameof(value));

        if (value.Length > 50)
            throw new ArgumentException("User name must not exceed 50 characters.", nameof(value));

        if (!AlphanumericRegex().IsMatch(value))
            throw new ArgumentException("User name must contain only latin letters and digits.", nameof(value));

        return new UserName(value);
    }

    [GeneratedRegex(@"^[a-zA-Z0-9_]+$")]
    private static partial Regex AlphanumericRegex();

    public override string ToString() => Value;
    public override int GetHashCode() => Value.GetHashCode(StringComparison.OrdinalIgnoreCase);
    public override bool Equals(object? obj) => obj is UserName other && Equals(other);
    public bool Equals(UserName? other) => other is not null && string.Equals(Value, other.Value, StringComparison.OrdinalIgnoreCase);
}
