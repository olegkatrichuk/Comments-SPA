namespace Comments.Domain.ValueObjects;

public sealed class HomePage : IEquatable<HomePage>
{
    public string Value { get; }

    private HomePage(string value) => Value = value;

    public static HomePage? Create(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return null;

        if (value.Length > 2048)
            throw new ArgumentException("Home page URL must not exceed 2048 characters.", nameof(value));

        if (!Uri.TryCreate(value, UriKind.Absolute, out var uri) ||
            (uri.Scheme != Uri.UriSchemeHttp && uri.Scheme != Uri.UriSchemeHttps))
            throw new ArgumentException("Home page must be a valid HTTP or HTTPS URL.", nameof(value));

        return new HomePage(value);
    }

    public override string ToString() => Value;
    public override int GetHashCode() => Value.GetHashCode(StringComparison.OrdinalIgnoreCase);
    public override bool Equals(object? obj) => obj is HomePage other && Equals(other);
    public bool Equals(HomePage? other) => other is not null && string.Equals(Value, other.Value, StringComparison.OrdinalIgnoreCase);
}
