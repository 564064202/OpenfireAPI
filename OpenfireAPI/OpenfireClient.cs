﻿using OpenfireAPI.util;
using RestSharp;
using System.Collections.Generic;
using System.Net;

namespace OpenfireAPI
{
    public class OpenfireClient
    {
        public OpenfireAuthenticator Authenticator { get; private set; }
        public RestClient Client { get; private set; }
        public string OpenfirePlugin { get; private set; }

        public OpenfireClient(string url, int port, OpenfireAuthenticator authenticator)
        {
            Authenticator = authenticator;
            Client = new RestClient(url + ":" + port) {Authenticator = authenticator};
            OpenfirePlugin = "/plugins/restapi/v1/";
        }

        public IRestResponse get(string restPath, Dictionary<string, string> queryParams)
        {
            return call(Method.GET, restPath, null, queryParams);
        }

        public IRestResponse post(string restPath, object payload, Dictionary<string, string> queryParams)
        {
            return call(Method.POST, restPath, payload, queryParams);
        }

        public IRestResponse put(string restPath, object payload, Dictionary<string, string> queryParams)
        {
            return call(Method.PUT, restPath, payload, queryParams);
        }

        public IRestResponse delete(string restPath, object payload, Dictionary<string, string> queryParams)
        {
            return call(Method.DELETE, restPath, payload, queryParams);
        }

        private IRestResponse call(Method method, string restPath, object payload,
            Dictionary<string, string> queryParams)
        {
            var request = new RestRequest(OpenfirePlugin + restPath, method);
            foreach (var entry in queryParams)
            {
                request.AddParameter(entry.Key, entry.Value);
            }
            request.RequestFormat = DataFormat.Json;
            request.AddHeader("Accept", "application/json");

            if (payload == null) return Client.Execute(request);

            request.AddHeader("Content-Type", "application/json");
            request.JsonSerializer = new RestSharpJsonNetSerializer();
            request.AddJsonBody(payload);

            return Client.Execute(request);
        }

        public bool isStatusCodeOK(IRestResponse response)
        {
            //Console.WriteLine(response.StatusCode);
            return response.StatusCode == HttpStatusCode.OK || response.StatusCode == HttpStatusCode.Created;
        }
    }
}