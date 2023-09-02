using System.Runtime.ConstrainedExecution;

namespace CertificateCreation.Models
{
    public class CSRInfoModel
    {
        public CSRInfoModel()
        {
        }
        public CSRInfoModel(bool isValid, string reason)
        {
            IsValid = isValid;
            CSR = reason;
        }

        public bool IsValid { get; set; }
        public string CSR { get; set; }
        public byte[] CSRBytes { get; set; }
        public string SubjectName { get; set; }
        public List<string> SubjectAlternateNames { get; set; } = new();
        public int KeyUsage { get; set; }
        public List<string> EKUs { get; set; } = new();
    }
}
