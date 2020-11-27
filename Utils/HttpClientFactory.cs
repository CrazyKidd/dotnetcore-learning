using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace NetNote.Utils
{
    public class HttpClientFactory
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<HttpClientFactory> _logger;
        public HttpClientFactory(IConfiguration configuration, IHttpClientFactory iHttpClientFactory, ILogger<HttpClientFactory> logger)
        {
            _httpClientFactory = iHttpClientFactory;
            _logger = logger;
        }


        public async Task<string> PostAsync<T>(string requestUri, T postContent)
        {
            try
            {
                var _httpclient = _httpClientFactory.CreateClient();
                _httpclient.DefaultRequestHeaders.Accept.Clear();
                _httpclient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                HttpContent httpContent = new StringContent(JsonConvert.SerializeObject(postContent));
                var response = await _httpclient.PostAsync(requestUri, httpContent);
                if (response.IsSuccessStatusCode)
                {
                    //读取content
                    var responseStr = await response.Content.ReadAsStringAsync();
                    //对结果序列化
                    //var responseObj = JsonConvert.DeserializeObject<>(responseStr);
                    return responseStr;
                }
                else
                {
                    _logger.LogError(requestUri + $" error");
                    return "error";
                }
            }
            catch (Exception ex)
            {
                _logger.LogInformation($"{requestUri}：{ex.Message + ex?.InnerException}");
                throw new Exception("HttpClientFactory");
            }
        }

        public async Task<string> GetAsync<T>(string requestUri, T postContent)
        {
            try
            {
                var _httpclient = _httpClientFactory.CreateClient();
                var request = new HttpRequestMessage(HttpMethod.Get, requestUri);
                HttpResponseMessage response = await _httpclient.SendAsync(request);
                if (response.IsSuccessStatusCode)
                {
                    var responseStr = await response.Content.ReadAsStringAsync();
                    return responseStr;
                }
                else
                {
                    _logger.LogError(requestUri + $" error");
                    return "error";
                }
            }
            catch (Exception ex)
            {
                _logger.LogInformation($"{requestUri}：{ex.Message + ex?.InnerException}");
                return "error";
            }
        }
    }
}
