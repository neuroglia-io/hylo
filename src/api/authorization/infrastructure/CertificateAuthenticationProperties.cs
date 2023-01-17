using Microsoft.AspNetCore.Authentication.Certificate;
using System.Security.Cryptography.X509Certificates;

namespace Hylo.Api.Authorization.Infrastructure.Services;

/// <summary>
/// Holds information about a <see cref="CertificateAuthenticationDefaults.AuthenticationScheme"/> scheme authentication attempt
/// </summary>
public class CertificateAuthenticationProperties
{

    /// <summary>
    /// Initializes a new <see cref="CertificateAuthenticationProperties"/>
    /// </summary>
    /// <param name="certificate">The <see cref="X509Certificate2"/> to authenticate with</param>
    public CertificateAuthenticationProperties(X509Certificate2 certificate)
    {
        if(certificate == null) throw new ArgumentNullException(nameof(certificate));
        this.Certificate = certificate;
    }

    /// <summary>
    /// Gets the <see cref="X509Certificate2"/> to authenticate with
    /// </summary>
    public X509Certificate2 Certificate { get; }

}