namespace Hylo.Api.Core.Data.Models;

/// <summary>
/// Represents the object used to configure an OpenID Connect authentication
/// </summary>
[DataContract]
public class V1OpenIDConnectAuthenticationSpec
{

    /// <summary>
    /// Initializes a new <see cref="V1OpenIDConnectAuthenticationSpec"/>
    /// </summary>
    public V1OpenIDConnectAuthenticationSpec() { }

    /// <summary>
    /// Initializes a new <see cref="V1OpenIDConnectAuthenticationSpec"/>
    /// </summary>
    /// <param name="issuer">The issuer of the tokens used to authenticate the user</param>
    /// <param name="clientId">The id of the client to generate the token for</param>
    /// <param name="clientSecret">The secret of the client to generate the token for</param>
    /// <param name="refreshToken">The token used to refresh the id token when it expires</param>
    /// <param name="idToken">The id token</param>
    public V1OpenIDConnectAuthenticationSpec(Uri issuer, string clientId, string? clientSecret, string refreshToken, string idToken)
    {
        if (issuer == null) throw new ArgumentNullException(nameof(issuer));
        if (string.IsNullOrWhiteSpace(clientId)) throw new ArgumentNullException(nameof(clientId));
        if (string.IsNullOrWhiteSpace(clientSecret)) throw new ArgumentNullException(nameof(clientSecret));
        if (string.IsNullOrWhiteSpace(refreshToken)) throw new ArgumentNullException(nameof(refreshToken));
        if (string.IsNullOrWhiteSpace(idToken)) throw new ArgumentNullException(nameof(idToken));
        this.Issuer = issuer;
        this.ClientId = clientId;
        this.ClientSecret = clientSecret;
        this.RefreshToken = refreshToken;
        this.IdToken = idToken;
    }

    /// <summary>
    /// Gets/sets the issuer of the tokens used to authenticate the user
    /// </summary>
    [DataMember(Name = "issuer", Order = 1), JsonPropertyName("issuer"), Required, MinLength(1)]
    public virtual Uri Issuer { get; set; } = null!;

    /// <summary>
    /// Gets/sets the id of the client to generate the token for
    /// </summary>
    [DataMember(Name = "clientId", Order = 2), JsonPropertyName("clientId"), Required, MinLength(1)]
    public virtual string ClientId { get; set; } = null!;

    /// <summary>
    /// Gets/sets the secret of the client to generate the token for
    /// </summary>
    [DataMember(Name = "clientSecret", Order = 3), JsonPropertyName("clientSecret")]
    public virtual string? ClientSecret { get; set; } = null!;

    /// <summary>
    /// Gets/sets the token used to refresh the id token when it expires
    /// </summary>
    [DataMember(Name = "refreshToken", Order = 4), JsonPropertyName("refreshToken"), Required, MinLength(1)]
    public virtual string RefreshToken { get; set; } = null!;

    /// <summary>
    /// Gets/sets the id token
    /// </summary>
    [DataMember(Name = "idToken", Order = 5), JsonPropertyName("idToken"), Required, MinLength(1)]
    public virtual string IdToken { get; set; } = null!;

}
