using FunctionAppWithRedis.Extensions;
using FunctionAppWithRedis.Interfaces;
using FunctionAppWithRedis.Model;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System.Net.Http;
using System.Threading.Tasks;

namespace FunctionAppWithRedis;

public class MyHttpFunction
{
    public readonly IPeachService _peachService;
    public readonly IConfiguration _configuration;
    public readonly IDistributedCache _cache;

    public MyHttpFunction(IPeachService peachService, IConfiguration configuration, IDistributedCache cache)
    {
        _peachService = peachService;
        _configuration = configuration;
        _cache = cache;
    }

    [FunctionName("Peach")]
    public async Task<HttpResponseMessage> Run(
        [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequestMessage requestMessage
        )
    {
        string requestBody = await requestMessage.Content.ReadAsStringAsync();
        var req = JsonConvert.DeserializeObject<MyRequestMessage>(requestBody);

        var limit = _configuration.GetValue<int>("PeachDictionaryLimit");
        if ( req == null || req.Position > limit)
        {
            return new HttpResponseMessage()
            {
                Content = new StringContent($"Position is required and Max Value of Position is {limit}"),
                StatusCode = System.Net.HttpStatusCode.BadRequest
            };
        }

        var fromCache = await _cache.GetRecordAsync<MyResponseMessage>(req.Position.ToString());
        MyResponseMessage result;
        if ( fromCache is default(MyResponseMessage))
        {
            result = new MyResponseMessage
            {
                Position = req.Position,
                Word = _peachService.GetWord(req.Position)
            };
            await _cache.SetRecordAsync(req.Position.ToString(), result);
        }
        else
        {
            result = fromCache;
        }        
        

        return new HttpResponseMessage()
        {
            Content = new StringContent(JsonConvert.SerializeObject(result)),
            StatusCode = System.Net.HttpStatusCode.OK
        };
    }
}
