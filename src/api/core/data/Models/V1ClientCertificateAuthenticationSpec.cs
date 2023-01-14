using System.Security.Cryptography.X509Certificates;

namespace Hylo.Api.Core.Data.Models;

/// <summary>
/// Represents the object used to configure a certificate-based authentication
/// </summary>
[DataContract]
public class V1ClientCertificateAuthenticationSpec
{

    /// <summary>
    /// Initializes a new <see cref="V1ClientCertificateAuthenticationSpec"/>
    /// </summary>
    public V1ClientCertificateAuthenticationSpec() { }

    /// <summary>
    /// Initializes a new <see cref="V1ClientCertificateAuthenticationSpec"/>
    /// </summary>
    /// <param name="certificate">The base-64 encoded client certificate</param>
    /// <param name="key">The base-64 encoded client key</param>
    public V1ClientCertificateAuthenticationSpec(string certificate, string key)
    {
        if(string.IsNullOrWhiteSpace(certificate)) throw new ArgumentNullException(nameof(certificate));
        if (string.IsNullOrWhiteSpace(key)) throw new ArgumentNullException(nameof(key));
        this.CertificateBase64 = certificate;
        this.Key = key;
    }

    /// <summary>
    /// Gets/sets the base-64 encoded client certificate
    /// </summary>
    [DataMember(Name = "certificate", Order = 1), JsonPropertyName("certificate"), Required, MinLength(1)]
    public virtual string CertificateBase64 { get; set; } = null!;

    /// <summary>
    /// Gets/sets the base-64 encoded client key
    /// </summary>
    [DataMember(Name = "key", Order = 2), JsonPropertyName("key"), Required, MinLength(1)]
    public virtual string Key { get; set; } = null!;

    private X509Certificate2 _Certificate = null!;
    /// <summary>
    /// Gets the client <see cref="X509Certificate2"/>
    /// </summary>
    [IgnoreDataMember, JsonIgnore]
    public virtual X509Certificate2 Certificate
    {
        get
        {
            if (this._Certificate != null) return this._Certificate;
            if (string.IsNullOrWhiteSpace(this.CertificateBase64)) return null!;
            var raw = Convert.FromBase64String(this.CertificateBase64);
            this._Certificate = new X509Certificate2(raw);
            return this._Certificate;
        }
        set
        {
            this._Certificate = value;
            if (this._Certificate == null) this.CertificateBase64 = null!;
            else this.CertificateBase64 = Convert.ToBase64String(this._Certificate.RawData);
        }
    }

}
