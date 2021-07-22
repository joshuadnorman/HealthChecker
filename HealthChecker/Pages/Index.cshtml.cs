using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using GraphQL;
using GraphQL.Types;
using GraphQL.SystemTextJson;
using HealthChecker.GraphQL;
using System.Net.Http;

namespace HealthChecker.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly IServiceProvider _provider;


        public IndexModel(ILogger<IndexModel> logger, IServiceProvider provider)
        {
            _logger = logger;
            _provider = provider;
        }

        public async Task OnGet()
        {
            //var schema = new HealthCheckerSchema(_provider);
            //var json = await schema.ExecuteAsync(_ =>
            //{
            //    _.Query = "{ servers { id name status error{body}} }";
            //});
            
           
        }
    }
}
