using Fido2NetLib.Objects;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Text.Json;

namespace DevSpaceWeb.Fido2;

/// <summary>
/// Represents a WebAuthn credential.
/// </summary>
public class FidoStoredCredential
{
    public string Name { get; set; }

    /// <summary>
    /// Gets or sets the public key for this user.
    /// </summary>
    public virtual byte[]? PublicKey { get; set; }

    /// <summary>
    /// Gets or sets the user handle for this user.
    /// </summary>
    public byte[]? UserHandle { get; set; }


    /// <summary>
    /// Gets or sets the registration date for this user.
    /// </summary>
    public DateTime CreatedAt { get; set; }

    public DateTime? LastUsedAt { get; set; }

    /// <summary>
    /// Gets or sets the Authenticator Attestation GUID (AAGUID) for this user.
    /// </summary>
    /// <remarks>
    /// An AAGUID is a 128-bit identifier indicating the type of the authenticator.
    /// </remarks>
    [BsonGuidRepresentation(GuidRepresentation.CSharpLegacy)]
    public Guid AaGuid { get; set; }

    [BsonIgnore]
    public PublicKeyCredentialDescriptor? Descriptor
    {
        get { return string.IsNullOrWhiteSpace(DescriptorJson) ? null : JsonSerializer.Deserialize<PublicKeyCredentialDescriptor>(DescriptorJson); }
        set { DescriptorJson = JsonSerializer.Serialize(value); }
    }

    public string? DescriptorJson { get; set; }
}
