using EZCAClient.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CertificateCreation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CertificateAuthenticationController : ControllerBase
    {
        [HttpGet]
        public APIResultModel Get()
        {
            string? headerValue = Request.Headers["X-Client-Cert-Subject"];

            if (string.IsNullOrEmpty(headerValue))
            {
                HttpContext.Response.StatusCode = 401;
                return new(false, "X-Client-Cert-Subject header is missing");
            }
            return new(true, $"Hello {headerValue.Replace("CN=", "")}!");
        }
    }
}
