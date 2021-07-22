using System;
using System.Collections.Generic;
using System.Net;
using System.Web;
using System.Net.Http;
using System.Threading.Tasks;
using GraphQL;
using GraphQL.Types;
using Newtonsoft.Json;
using Microsoft.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Distributed;

namespace HealthChecker.GraphQL
{

    public class Server
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string HealthCheckUri { get; set; }
        public DateTime LastTimeUp { get; set; }
        public ErrorDetail Error { get; set; } 
    }

    public class ErrorDetail
    {
        public string status { get; set; }
        public string body { get; set; }
    }

    public class ServerType : ObjectGraphType<Server>
    {
        
        public ServerType(IHttpClientFactory factory,IDistributedCache disCache)
        {
            Name = "Server";
            Description = "A server to monitor";


            Field(h => h.Id);
            Field(h => h.Name);
            Field(h => h.HealthCheckUri);
            Field(h => h.LastTimeUp);


            FieldAsync<StringGraphType>(
                "status",
                // TODO: replace with health check code
                resolve: async(context) => {
                    MyService m = new MyService(factory,disCache);
                    var status = await m.GetStatus(context.Source.Id,context.Source.HealthCheckUri);
                    if (status != "UP")
                    {
                        context.Source.Error = JsonConvert.DeserializeObject<ErrorDetail>(status);
                        return "DOWN";
                        
                    }
                    context.Source.LastTimeUp = System.DateTime.Now;
                    return status;
                }
            ) ;

            Field<ErrorDetailType>(
                "error",
                resolve:  context => context.Source.Error
            );



        }
    }

    public class ErrorDetailType : ObjectGraphType<ErrorDetail>
    {
        public ErrorDetailType()
        {
            Name = "Error";
            Description = "Error pertaining to server";
            Field(h => h.body);
            Field(h => h.status);
        }
    }


    

    public class HealthCheckerQuery : ObjectGraphType<object>
    {
        private List<Server> servers = new List<Server>{
            new Server{
                Id = "1",
                Name = "stackworx.io",
                HealthCheckUri = "https://www.stackworx.io",
                Error = new ErrorDetail(){body="",status=""}
            },
            new Server{
                Id = "2",
                Name = "prima.run",
                HealthCheckUri = "https://prima.run",
                 Error = new ErrorDetail(){body="",status=""}
            },
            new Server{
                Id = "3",
                Name = "google",
                HealthCheckUri = "https://www.google.com",
                 Error = new ErrorDetail(){body="",status=""}
            },
        };

        public HealthCheckerQuery()
        {
            Name = "Query";



            Func<ResolveFieldContext, string, object> serverResolver = (context, id) => this.servers;

            FieldDelegate<ListGraphType<ServerType>>(
                "servers",
                arguments: new QueryArguments(
                    new QueryArgument<StringGraphType> { Name = "id", Description = "id of server" }
                ),
                resolve: serverResolver
            );

            Field<StringGraphType>(
                "hello",
                resolve: context => "world"
            );

            Field<StringGraphType>(
                "test",
                resolve: context => "What is up my guy! This is me testing the worlds most basic resolve besides hello world"
            );

        }
    }

    public class HealthCheckerSchema : Schema
    {
        public HealthCheckerSchema(IServiceProvider provider) : base(provider)
        {
            Query = new HealthCheckerQuery();
            RegisterType<ServerType>();
            RegisterType<ErrorDetailType>();
        }
    }

    

}
