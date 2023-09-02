using CertificateCreation.Managers;
using EZCAClient.Models;
using Microsoft.AspNetCore.Mvc;

namespace CertificateCreation.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CertificateRegistrationController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<CertificateRegistrationController> _logger;
        private readonly CertificateManager _certificateManager;
        public CertificateRegistrationController(ILogger<CertificateRegistrationController> logger,
            CertificateManager certificateManager)
        {
            _logger = logger;
            _certificateManager = certificateManager;
        }

        [HttpPost(Name = "RegisterNewUser")]
        public async Task<APIResultModel> RegisterNewUserAsync(RegistrationModel registration)
        {
            //ref on how to tell your users to create a CSR
            //https://docs.keytos.io/azure-pki/create-new-certificate/create_csrcert/#create-csr-windows
            if (registration == null)
            {
                HttpContext.Response.StatusCode = 401;
                return new(false, "No registration data provided");
            }

            if (string.IsNullOrWhiteSpace(registration.CSR))
            {
                HttpContext.Response.StatusCode = 400;
                return new(false, "No CSR provided");
            }
            if (string.IsNullOrWhiteSpace(registration.UserName))
            {
                HttpContext.Response.StatusCode = 400;
                return new(false, "No UserName provided");
            }
            try
            {
                return await _certificateManager.RegisterNewUserAsync(registration);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error registering new user");
                return new APIResultModel
                {
                    Success = false,
                    Message = "Error registering new user"
                };
            }
        }
    }
}
