using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace EbookProvider
{
    internal class RequestsSession
    {
        HttpClient client;
        int delay;
        Random r;
        internal RequestsSession(string burl, int delay=0)
        {
            client = new HttpClient();
            //https://httpbin.org/anything
            client.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/106.0.0.0 Safari/537.36");
            //client.DefaultRequestHeaders.Add("Upgrade-Insecure-Requests", "1");
            //client.DefaultRequestHeaders.Add("Accept-Language", "hu-HU,hu;q=0.9,en-US;q=0.8,en;q=0.7");
            //client.DefaultRequestHeaders.Add("Accept-Encoding","gzip, deflate, br");
            client.BaseAddress = new Uri(burl);
            this.delay = delay;
            r=new Random();
        }
        void Delay()
        {
            if(delay > 0)
            {
                Thread.Sleep(r.Next(2, delay));
            }   
        }
        public async Task<byte[]> DownloadFile(string url
     , string outputPath="")
        {
            if (!File.Exists(outputPath) && outputPath!="")
            {
                File.Create(outputPath);
            }
            byte[] fileBytes = await client.GetByteArrayAsync(url);
            if (outputPath != "")
            {
                File.WriteAllBytes(outputPath, fileBytes);
            }
            return fileBytes;
        }
        public async Task<string> PostMultipartForm(string url, Dictionary<string, string> formdata)
            {
                Delay();
                string item = "";
                MultipartFormDataContent form = new MultipartFormDataContent();
                foreach (string key in formdata.Keys)
                {
                    form.Add(new StringContent(formdata[key]), key);
                }
                HttpResponseMessage response = await client.PostAsync(url, form);
                response.EnsureSuccessStatusCode();
                item = await response.Content.ReadAsStringAsync();
                return item;
            }
            public async Task<T> PostMultipartForm<T>(string url, Dictionary<string, string> formdata)
            {
                Delay();
                T item = default(T);
                MultipartFormDataContent form = new MultipartFormDataContent();
                foreach (string key in formdata.Keys)
                {
                    form.Add(new StringContent(formdata[key]), key);
                }
                HttpResponseMessage response = await client.PostAsync(url, form);
                response.EnsureSuccessStatusCode();
                item = await response.Content.ReadFromJsonAsync<T>();
                return item;
            }
            public async Task<string> Get(string url)
            {
                Delay();
                string item = "";
                HttpResponseMessage response = await client.GetAsync(url);
                response.EnsureSuccessStatusCode();
                item = await response.Content.ReadAsStringAsync();
                return item;
            }
            public async Task<T> GetJson<T>(string url)
            {
                Delay();
                T item = default(T);
                HttpResponseMessage response = await client.GetAsync(url);
                response.EnsureSuccessStatusCode();
                item = await response.Content.ReadFromJsonAsync<T>();
                return item;
            }
            public async Task<T> PostJson<T>(string url)
            {
                Delay();
                T item = default(T);
                HttpResponseMessage response = await client.PostAsJsonAsync(url, item);
                response.EnsureSuccessStatusCode();;
                item = await response.Content.ReadFromJsonAsync<T>();
                return item;
            }
            public async Task<T> PostJson<T, E>(string url, E data)
            {
                Delay();
                T item = default(T);
                HttpResponseMessage response = await client.PostAsJsonAsync(url, data);
                response.EnsureSuccessStatusCode();
                item = await response.Content.ReadFromJsonAsync<T>();
                return item;
            }
            public async Task<string> PostJson<E>(string url, E data)
            {
                Delay();
                string item = "";
                HttpResponseMessage response = await client.PostAsJsonAsync(url, data);
                response.EnsureSuccessStatusCode();
                item = await response.Content.ReadAsStringAsync();
                return item;
            }

    }
}
