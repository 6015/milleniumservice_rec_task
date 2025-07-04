using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using System.Text;
using System.Xml;
using StackExchange.Redis;

namespace IntegrationWebApp.Controllers
{
    [ApiController]
    [Route("api/integration/fibonacci")]
    public class FibonacciIntegrationController : ControllerBase
    {
        private readonly IConnectionMultiplexer _redis;
        private readonly IHttpClientFactory _httpClientFactory;

        private const string RestServiceBaseUrl = "https://localhost:7067/api/fibonacci/";

        public FibonacciIntegrationController(IConnectionMultiplexer redis, IHttpClientFactory httpClientFactory)
        {
            _redis = redis;
            _httpClientFactory = httpClientFactory;
        }

        private async Task<int> CallSoapServiceAsync(int n)
        {
            var soapEnvelope = $@"<?xml version=""1.0"" encoding=""utf-8""?>
<soap:Envelope xmlns:soap=""http://schemas.xmlsoap.org/soap/envelope/"">
  <soap:Body>
    <CalculateFibonacci xmlns=""http://tempuri.org/"">
      <n>{n}</n>
    </CalculateFibonacci>
  </soap:Body>
</soap:Envelope>
";

            using var client = new HttpClient();
            var content = new StringContent(soapEnvelope, Encoding.UTF8, "text/xml");
            content.Headers.Add("SOAPAction", "\"http://tempuri.org/IFibonacciService/CalculateFibonacci\"");

            var response = await client.PostAsync("https://localhost:7094/FibonacciService.svc", content);
            response.EnsureSuccessStatusCode();

            var responseContent = await response.Content.ReadAsStringAsync();

            var xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(responseContent);
            var nsmgr = new XmlNamespaceManager(xmlDoc.NameTable);
            nsmgr.AddNamespace("soap", "http://schemas.xmlsoap.org/soap/envelope/");
            nsmgr.AddNamespace("ns", "http://tempuri.org/");

            var resultNode = xmlDoc.SelectSingleNode("//ns:CalculateFibonacciResult", nsmgr);
            if (resultNode == null)
                throw new Exception("Can`t parse responce from SOAP-responce");

            return int.Parse(resultNode.InnerText);
        }


        [HttpGet("{n}")]
        public async Task<IActionResult> GetFibonacci(int n)
        {
            var db = _redis.GetDatabase();
            string cacheKey = $"fibonacci:{n}";

            var cachedValue = await db.StringGetAsync(cacheKey);
            if (cachedValue.HasValue)
            {
                return Ok(new { source = "cache", value = long.Parse(cachedValue) });
            }

            int? restResult = null;

            var httpClient = _httpClientFactory.CreateClient();
            var response = await httpClient.GetAsync(RestServiceBaseUrl + n);
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                restResult = JsonSerializer.Deserialize<int>(json);
            }



            int? soapResult = null;
            soapResult = await CallSoapServiceAsync(n);


            int? result = restResult ?? soapResult;

            if (result == null)
                return StatusCode(503, "Both services unavailable");

            await db.StringSetAsync(cacheKey, result.ToString(), TimeSpan.FromMinutes(5));

            return Ok(new { source = restResult != null ? "rest" : "soap", value = result });
        }
    }
}
