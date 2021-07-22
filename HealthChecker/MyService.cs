using HealthChecker.GraphQL;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace HealthChecker
{
    public class MyService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IDistributedCache _disCache;

        public MyService(IHttpClientFactory httpClientFactory,IDistributedCache disCache)
        {
            _httpClientFactory = httpClientFactory;
            _disCache = disCache;
        }

        public async Task<String> GetStatus(string id,string uri)
        {

            var res = await _disCache.GetAsync(id);

            if (res != null)
            {
                return Encoding.UTF8.GetString(res);

            } else {
                var client = _httpClientFactory.CreateClient();
                var result = await client.GetAsync(uri);
                var finalRes="";
                if (result.IsSuccessStatusCode)
                {
                    finalRes = "UP";

                }
                else
                {
                    ErrorDetail error = new ErrorDetail()
                    {
                        status = ((int)result.StatusCode).ToString(),
                        body = result.ReasonPhrase
                    };

                    finalRes = JsonConvert.SerializeObject(error);
                }

                var options = new DistributedCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromMinutes(5));
                await _disCache.SetAsync(id, Encoding.UTF8.GetBytes(finalRes), options);

                return finalRes;
            }
            
            
            

        }
    }

   

    
}
