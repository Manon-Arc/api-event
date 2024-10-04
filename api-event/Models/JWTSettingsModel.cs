public class JwtSettings
{
    // Émetteur du token JWT (généralement l'API)
    public string Issuer { get; set; } = string.Empty;

    // Audience prévue pour le token (API elle-même ou d'autres services)
    public string Audience { get; set; } = string.Empty;

    // Clé secrète utilisée pour signer et valider le token JWT
    public string Key { get; set; } = string.Empty;

    // Durée d'expiration du token en minutes
    public int ExpirationMinutes { get; set; }
}