using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace EbookProvider
{
    public static class Requests
    {
        static HttpClient ClientFactory(string url)
        {
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (compatible; AcmeInc/1.0)");
            client.BaseAddress = new Uri(url);
            return client;
        }
        public async static Task<string> PostMultipartForm(string url, Dictionary<string, string> formdata)
        {
            string item="";
            HttpClient client = ClientFactory(url);
            MultipartFormDataContent form = new MultipartFormDataContent();
            foreach (string key in formdata.Keys)
            {
                form.Add(new StringContent(formdata[key]), key);
            }
            HttpResponseMessage response = await client.PostAsync("/", form);
            response.EnsureSuccessStatusCode();
            client.Dispose();
            item = await response.Content.ReadAsStringAsync();
            return item;
        }
        public async static Task<T> PostMultipartForm<T>(string url, Dictionary<string, string> formdata)
        {
            T item = default(T);
            HttpClient client = ClientFactory(url);
            MultipartFormDataContent form = new MultipartFormDataContent();
            foreach (string key in formdata.Keys)
            {
                form.Add(new StringContent(formdata[key]), key);
            }
            HttpResponseMessage response = await client.PostAsync("/", form);
            response.EnsureSuccessStatusCode();
            client.Dispose();
            item = await response.Content.ReadFromJsonAsync<T>();
            return item;
        }
        public async static Task<string> Get(string url)
        {
            HttpClient client = ClientFactory(url);
            string item = "";
            HttpResponseMessage response = await client.GetAsync("/");
            response.EnsureSuccessStatusCode();
            client.Dispose();
            item = await response.Content.ReadAsStringAsync();
            return item;
        }
        public async static Task<T> GetJson<T>(string url)
        {
            HttpClient client = ClientFactory(url);
            T item = default(T);
            HttpResponseMessage response = await client.GetAsync("/");
            response.EnsureSuccessStatusCode();
            client.Dispose();
            item = await response.Content.ReadFromJsonAsync<T>();
            return item;
        }
        public async static Task<T> PostJson<T>(string url)
        {
            HttpClient client = ClientFactory(url);
            T item = default(T);
            HttpResponseMessage response = await client.PostAsJsonAsync("/", item);
            response.EnsureSuccessStatusCode();
            client.Dispose();
            item = await response.Content.ReadFromJsonAsync<T>();
            return item;
        }
        public async static Task<T> PostJson<T,E>(string url, E data)
        {
            HttpClient client = ClientFactory(url);
            T item = default(T);
            HttpResponseMessage response = await client.PostAsJsonAsync("/", data);
            response.EnsureSuccessStatusCode();
            client.Dispose();
            item = await response.Content.ReadFromJsonAsync<T>();
            return item;
        }
        public async static Task<string> PostJson<E>(string url, E data)
        {
            HttpClient client = ClientFactory(url);
            string item = "";
            HttpResponseMessage response = await client.PostAsJsonAsync("/", data);
            response.EnsureSuccessStatusCode();
            client.Dispose();
            item = await response.Content.ReadAsStringAsync();
            return item;
        }

    }
}
