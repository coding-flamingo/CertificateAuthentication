using System.Security.Cryptography.X509Certificates;
using System.Text.RegularExpressions;
using Azure.Identity;
using CertificateCreation.Models;
using EZCAClient.Managers;
using EZCAClient.Models;
using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.Pkcs;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Pkcs;

namespace CertificateCreation.Managers
{
    public class CertificateManager
    {
        private readonly IEZCAClient _ezcaClient;
        private readonly ILogger _logger;
        public CertificateManager(HttpClient httpClient, ILogger<CertificateManager> logger)
        {
            _logger = logger;
            _ezcaClient = new EZCAClientClass(httpClient, logger,
                "https://portal.ezca.io/", new DefaultAzureCredential());
        }

        public async Task<APIResultModel> RegisterNewUserAsync(RegistrationModel registrationModel)
        {
            if (registrationModel == null)
            {
                throw new ArgumentNullException(nameof(registrationModel));
            }
            //check CSR matches the user name
            CSRInfoModel validationCheck = ValidateCSR(registrationModel);
            if (!validationCheck.IsValid)
            {
                return new(false, validationCheck.CSR);
            }
            //Todo add code to register the user in your application database
            AvailableCAModel[]? availableCAs = await _ezcaClient.GetAvailableCAsAsync();
            AvailableCAModel? selectedCA = availableCAs?.FirstOrDefault(x =>
                x.CAFriendlyName == "SAME WestUS");
            if (selectedCA == null)
            {
                _logger.LogError("No CA found");
                return new APIResultModel
                {
                    Success = false,
                    Message = "No CA found"
                };
            }
            APIResultModel registrationResult = await _ezcaClient.RegisterDomainAsync(
                selectedCA, registrationModel.UserName);
            if (!registrationResult.Success)
            {
                Console.WriteLine($"Could not register new device in EZCA {registrationResult.Message}");
                return registrationResult;
            }
            // Request Certificate
            X509Certificate2? certificateResult = await _ezcaClient.RequestCertificateAsync(
                selectedCA, registrationModel.CSR, validationCheck.SubjectName, 30);
            if (certificateResult == null)
            {
                return new APIResultModel
                {
                    Success = false,
                    Message = "Could not request certificate"
                };
            }
            return new(true, certificateResult.ExportCertificatePem());
        }

        private CSRInfoModel ValidateCSR(RegistrationModel registrationModel)
        {
            CSRInfoModel csrInfo = DecomposeCSR(registrationModel.CSR);
            if (csrInfo.IsValid == false)
            {
                return new(false, csrInfo.CSR);
            }
            string[] separatedSubjectName = csrInfo.SubjectName.Split('=');
            if (separatedSubjectName.Length != 2)
            {
                return new(false, "Invalid subject name format");
            }
            if (separatedSubjectName[1] != registrationModel.UserName)
            {
                return new(false, "Invalid subject name");
            }
            return csrInfo;
        }

        private CSRInfoModel DecomposeCSR(string csr)
        {
            if (string.IsNullOrWhiteSpace(csr))
            {
                return new(false, "CSR cannot be empty");
            }
            CSRInfoModel csrModel = new(true, csr);
            if (!ConvertCSRToBytes(csrModel))
            {
                return csrModel;
            }
            Pkcs10CertificationRequest pk10Holder = new(csrModel.CSRBytes);
            csrModel.IsValid = pk10Holder.Verify();
            if (csrModel.IsValid == false)
            {
                csrModel.CSR = "Error: Invalid CSR";
                return csrModel;
            }
            CertificationRequestInfo requestInfo = pk10Holder.GetCertificationRequestInfo();
            csrModel.SubjectName = requestInfo.Subject.ToString() ?? string.Empty;
            return csrModel;
        }

        private static bool ConvertCSRToBytes(CSRInfoModel csrModel)
        {
            try
            {
                csrModel.CSR = ScrubHeaders(csrModel.CSR);
                csrModel.CSRBytes = Convert.FromBase64String(csrModel.CSR);
                return true;
            }
            catch
            {
                csrModel.IsValid = false;
                csrModel.CSR = "Error: Invalid CSR";
                return false;
            }
        }

        public static string ScrubHeaders(string cert)
        {
            return Regex.Replace(cert, @"-----[a-z A-Z]+-----", "").Trim();
        }
    }
}
