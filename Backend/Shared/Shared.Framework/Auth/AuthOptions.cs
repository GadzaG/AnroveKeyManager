namespace Shared.Framework.Auth;

public record AuthOptions
{
    public const string SECTION_NAME = "AuthOptions";

    public string SecretKey { get; init; } = string.Empty;

    public int TokenLifeTimeInMinutes { get; init; } = 5;

    public string Issuer { get; init; } = string.Empty;

    public string Audience { get; init; } = "All";
}