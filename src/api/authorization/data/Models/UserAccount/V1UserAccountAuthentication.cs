namespace Hylo.Api.Authorization.Data.Models;

/// <summary>
/// Represents the object used to configure a <see cref="V1UserAccount"/>'s authentication
/// </summary>
[DataContract]
public class V1UserAccountAuthentication
{

    /// <summary>
    /// Gets/sets the object used to configure the user's Basic authentication
    /// </summary>
    [DataMember(Name = "basic", Order = 1), JsonPropertyName("basic"), Required, MinLength(1)]
    public virtual V1BasicAuthenticationSpec? Basic { get; set; }

    /// <summary>
    /// Gets/sets the object used to configure the user's certificate-based authentication
    /// </summary>
    [DataMember(Name = "certificate", Order = 2), JsonPropertyName("certificate"), Required, MinLength(1)]
    public virtual V1ClientCertificateAuthenticationSpec? ClientCertificate { get; set; }

    /// <summary>
    /// Gets/sets the object used to configure the user's OpenID Connect authentication
    /// </summary>
    [DataMember(Name = "oidc", Order = 2), JsonPropertyName("oidc"), Required, MinLength(1)]
    public virtual V1OpenIDConnectAuthenticationSpec? OpenIDConnect { get; set; }

}
