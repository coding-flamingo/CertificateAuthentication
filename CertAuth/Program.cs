using Microsoft.AspNetCore.Authentication.Certificate;
using Microsoft.AspNetCore.HttpOverrides;
using System.Security.Cryptography.X509Certificates;
using Microsoft.AspNetCore.Server.Kestrel.Https;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.WebHost.ConfigureKestrel(serverOptions =>
{
    serverOptions.ConfigureHttpsDefaults(q =>
    {
        q.ClientCertificateMode = ClientCertificateMode.RequireCertificate;
        q.SslProtocols = System.Security.Authentication.SslProtocols.Tls12;

    });
    
});
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders =
        ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
    // Only loopback proxies are allowed by default. Clear that restriction to enable this explicit configuration.
    options.KnownNetworks.Clear();
    options.KnownProxies.Clear();
});

builder.Services.AddCertificateForwarding(
    options => { options.CertificateHeader = "X-ARR-ClientCert"; });

byte[] issuingCABytes = Convert.FromBase64String("MIIFYTCCA0mgAwIBAgIIB97Q6eyzGXMwDQYJKoZIhvcNAQELBQAwFDESMBAGA1UEAwwJU0FNRSBSb290MB4XDTIyMDkyMjAwMDAwMFoXDTI3MDkyMjAwMDAwMFowFDESMBAGA1UEAwwJU0FNRSBSb290MIICIjANBgkqhkiG9w0BAQEFAAOCAg8AMIICCgKCAgEAvA3TC9CblFD8rSwP0xljBeEXd61Rnfi1hdZgidzWgvOe12XLb50XJ2r92MmYq0Tr5Y1hZJ6TjHjHveTgzW7lsDdITUAzB6vyi+Hk0ltmqGhAke9wk/14EDMlGoVJBb0dNrDBrBgKxjWh4FHKaCdFPRwZMcnaRyXAY6Jv/qhLuEoNUYWQiy4KEi9u91NoDAJzggZTm6WQS8rQrqfXpRCTkorwTTV1GXs2IAKT+kRZxHycsWSppFRrVj50ySsfCqETGVllCpN5bDcfhjQLPKIZNZpJsPNZ9qubQoZ4rSD9ztmSYzpCrH+kMInMsj9pn63/bMZx/rOJXAwpnsYfSqw4tJe+iQ7xL1PS0WlHuwlIvm3Y4j+lMMtyvAEPOlqKt44EXwZgKgDL15GLq7y+5oLoNHwcT5saRHBNglf4ETCm0zp8LpTrGIzzWvKY7v/Fy3Kl/BaGbWMWz7E2pwAqABUptjWlF9TFgfqj9Xb/RgR1uoKS8osLRglGjqdTTHfOQ+aVDyKnrYzbUGul+UY9dsZoIWvBhSIti0nTpnXH6uLobTCripIazUYYvOiu/EkOyOSPMvXaavDByPBqIAQHe7lCssRAVWSGG9c9+LbrMDG9CMXQOGZFSHZxvHMyruSU5wdXb4gfCGfCVK/eQt5ZgJcVCjj6aZM4HAQRsXtklZP5/5cCAwEAAaOBtjCBszAfBgNVHQ4EGAQWBBSAa3jm2kLoevTfvgNvSAe8aZ6BYTAOBgNVHQ8BAf8EBAMCAYYwbwYDVR0lBGgwZgYIKwYBBQUHAwEGCCsGAQUFBwMCBggrBgEFBQcDAwYIKwYBBQUHAwQGCCsGAQUFBwMFBggrBgEFBQcDBgYIKwYBBQUHAwkGCCsGAQUFBwMBBgorBgEEAYI3FAICBggrBgEFBQcDCDAPBgNVHRMBAf8EBTADAQH/MA0GCSqGSIb3DQEBCwUAA4ICAQA6ZshnZwshfzEOGqfQaREwdrSuplY5WkhKge7OO5Phaxcow2mslj6w2HOAYUBmPWpQXLG/JvfuvGLcpJlf9oUevC5PtdrjxNwf2kclrMMBjtkqT5K92XNLvZe1yf26p9Rt14LjY8Wli4Lq88sFinahMOXSaZXJzV/amIgXxH7HDFFp6BFfLtpMALYw3uXuRnQ7h2vyVL3KUGWPcabco1ZTyhSGviLi0V8MkpY4iw5uPjZRDEBXJ7MwtPKmxNuWIsfYtA+PN0crN3ycLSMyM/lNiVhjA2YAS+q5LLtSwK5v7DGYVl/uwCOl4uLSM0DWzfYyNR5fnaVUfIEwhpCqVm+f+3XYmaMcaVqW4aX7RQT+79YlHUZKq/lfrAVHVHufNEUcyAm/ydtZzZwjUJDTzQz77d9qA/w1dNlvCgGS7ETOTnuqtB1YWBCWTkpdjJlJ1RMo6q83HzpMFSsCpuqHy9aVp4onC4cpBhDCG6arKnxLSo19WFlg6yXcUHxX81EgW0SuZX1bIbk687k/5kdc/7/3t9S1jTzhnW1Mth3h2o9B/a+NWVqWBOiwp/HH+KtwCxsrMjJJECvHWlospQ9GX3pUltawSm+39dHpfHwIuBB3JyKqftfReugQf6YwfSMMKui2mOhtmOY9QXuBWps2Kurqn3n7Y9jZR8jJQzYsBYEIXw==");
byte[] rootCABytes = Convert.FromBase64String("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");
X509Certificate2 rootCA = new(rootCABytes);
X509Certificate2 issuingCA = new(issuingCABytes);
builder.Services.AddAuthentication(
        CertificateAuthenticationDefaults.AuthenticationScheme)
    .AddCertificate(options => {

        options.ValidateValidityPeriod = true;
        options.CustomTrustStore.Add(rootCA);
        options.CustomTrustStore.Add(issuingCA);
        options.AdditionalChainCertificates.Add(rootCA);
        options.AdditionalChainCertificates.Add(issuingCA);
        options.ChainTrustValidationMode = X509ChainTrustMode.CustomRootTrust;
        options.RevocationMode = System.Security.Cryptography.X509Certificates.X509RevocationMode.Online;
        options.RevocationFlag = System.Security.Cryptography.X509Certificates.X509RevocationFlag.EntireChain;
        //options.Events = new CertificateAuthenticationEvents
        //{
        //    OnCertificateValidated = context => {
                
        //        context.Success();
        //        return Task.CompletedTask;
        //    },
        //    OnAuthenticationFailed = context => {
        //        context.Fail("Invalid certificate" + context.Exception.Message);
        //        return Task.CompletedTask;
        //    }
        //};
    });
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseForwardedHeaders();
app.UseCertificateForwarding();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();


