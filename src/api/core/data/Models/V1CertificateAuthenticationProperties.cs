namespace Hylo.Api.Core.Data.Models;

/// <summary>
/// Represents the object used to configure a certificate-based authentication
/// </summary>
[DataContract]
public class V1CertificateAuthenticationProperties
{

    /// <summary>
    /// Initializes a new <see cref="V1CertificateAuthenticationProperties"/>
    /// </summary>
    public V1CertificateAuthenticationProperties() { }

    /// <summary>
    /// Initializes a new <see cref="V1CertificateAuthenticationProperties"/>
    /// </summary>
    /// <param name="certificate">The base-64 encoded client certificate</param>
    /// <param name="key">The base-64 encoded client key</param>
    public V1CertificateAuthenticationProperties(string certificate, string key)
    {
        if(string.IsNullOrWhiteSpace(certificate)) throw new ArgumentNullException(nameof(certificate));
        if (string.IsNullOrWhiteSpace(key)) throw new ArgumentNullException(nameof(key));
        this. Certificate = certificate;
        this.Key = key;
    }

    /// <summary>
    /// Gets/sets the base-64 encoded client certificate
    /// </summary>
    [DataMember(Name = "certificate", Order = 1), JsonPropertyName("certificate"), Required, MinLength(1)]
    public virtual string Certificate { get; set; } = null!;

    /// <summary>
    /// Gets/sets the base-64 encoded client key
    /// </summary>
    [DataMember(Name = "key", Order = 2), JsonPropertyName("key"), Required, MinLength(1)]
    public virtual string Key { get; set; } = null!;

}
