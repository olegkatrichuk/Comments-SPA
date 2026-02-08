using System.Text.RegularExpressions;

namespace Comments.Domain.ValueObjects;

public sealed partial class Email : IEquatable<Email>
{
    public string Value { get; }

    private Email(string value) => Value = value;

    public static Email Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Email is required.", nameof(value));

        if (value.Length > 254)
            throw new ArgumentException("Email must not exceed 254 characters.", nameof(value));

        if (!EmailRegex().IsMatch(value))
            throw new ArgumentException("Email format is invalid.", nameof(value));

        return new Email(value.ToLowerInvariant());
    }

    [GeneratedRegex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.IgnoreCase)]
    private static partial Regex EmailRegex();

    public override string ToString() => Value;
    public override int GetHashCode() => Value.GetHashCode(StringComparison.OrdinalIgnoreCase);
    public override bool Equals(object? obj) => obj is Email other && Equals(other);
    public bool Equals(Email? other) => other is not null && string.Equals(Value, other.Value, StringComparison.OrdinalIgnoreCase);
}
