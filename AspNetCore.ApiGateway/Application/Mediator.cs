﻿using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace AspNetCore.ApiGateway
{
    public enum GatewayVerb
    {
        GET,
        POST,
        PUT,
        DELETE
    }

    public class HttpClientConfig
    {
        public Func<HttpClient> HttpClient { get; set; }

        public Func<HttpContent> HttpContent { get; set; }
    }

    public class GatewayRouteInfo
    {
        public GatewayVerb Verb { get; set; }

        public RouteInfo Route { get; set; }
    }

    public class RouteInfo
    {
        public string Path { get; set; }
        public Type ResponseType { get; set; }
        public Type RequestType { get; set; }
        public Func<ApiInfo, RouteInfo, HttpRequest, Task<object>> Exec { get; set; }
        public HttpClientConfig HttpClientConfig { get; set; }
    }


    public class Mediator : IMediator
    {
        readonly IApiOrchestrator _apiOrchestrator;
        Dictionary<string, GatewayRouteInfo> paths = new Dictionary<string, GatewayRouteInfo>();
        public Mediator(IApiOrchestrator apiOrchestrator)
        {
            _apiOrchestrator = apiOrchestrator;
        }

        public IMediator AddRoute(string key, GatewayVerb verb, RouteInfo routeInfo)
        {
            var gatewayRouteInfo = new GatewayRouteInfo
            {
                Verb = verb,
                Route = routeInfo
            };

            paths.Add(key.ToLower(), gatewayRouteInfo);

            return this;
        }

        public IMediator AddRoute(string key, GatewayVerb verb, Func<ApiInfo, RouteInfo, HttpRequest, Task<object>> exec)
        {
            var gatewayRouteInfo = new GatewayRouteInfo
            {
                Verb = verb,
                Route = new RouteInfo
                {
                    Exec = exec
                }
            };

            paths.Add(key.ToLower(), gatewayRouteInfo);

            return this;
        }

        public IApiOrchestrator ToOrchestrator()
        {
            return _apiOrchestrator;
        }

        public GatewayRouteInfo GetRoute(string key)
        {
            return paths[key.ToLower()];
        }

    }

}
