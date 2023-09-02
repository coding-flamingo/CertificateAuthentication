// Ignore Spelling: CSR

using System.Text.Json.Serialization;

namespace CertificateCreation
{
    public class RegistrationModel
    {

        [JsonPropertyName("CSR")]
        public string CSR { get; set; } = string.Empty;

        [JsonPropertyName("UserName")]
        public string UserName { get; set; } = string.Empty;
    }
}
