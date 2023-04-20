using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Socialize
{
    public class MethodHandler
    {
        private HttpClientHandler clientHandler = new HttpClientHandler();
        private HttpClient client = new HttpClient();
        private string prefBearer = string.Empty;

        public string PrefBearer
        {
            set { prefBearer = value; }
        }

        private void ConfigureClient()
        {
            //configure untrusted CA accept for server
            clientHandler.ClientCertificateOptions = ClientCertificateOption.Manual;
            clientHandler.ServerCertificateCustomValidationCallback =
                (httpRequestMessage, cert, cetChain, policyErrors) => true;

            //apply handler + allow any responsetype
            client = new HttpClient(clientHandler);
            client.BaseAddress = URI.ApiURI;
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("*/*"));
        }
        public MethodHandler()
        {
            ConfigureClient();
        }

        public MethodHandler(string bearer)
        {
            ConfigureClient();
            PrefBearer = bearer;
        }

        public void SetBearer(string bearer)
        {
            if (!client.DefaultRequestHeaders.Contains("Authorization"))
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", bearer);
            }
        }

        public void ClearBearer()
        {
            if (client.DefaultRequestHeaders.Contains("Authorization"))
            {
                client.DefaultRequestHeaders.Remove("Authorization");
            }
        }

        public async Task<HttpResponseMessage> Get(string path)
        {
            try
            {
                HttpResponseMessage response = await client.GetAsync(path);
                return response;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public async Task<T> Get<T>(string path)
        {
            try
            {
                HttpResponseMessage response = await Get(path);
                string temp = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<T>(temp);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<HttpResponseMessage> Post(string path) =>
             await client.PostAsync(path, null);


        public async Task<HttpResponseMessage> Post<T>(string path, T obj, string contentType) where T : class
        {
            try
            {
                client.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", contentType);

                object hc = null;

                if (typeof(T) == typeof(string))
                {
                    hc = new StringContent(ConvertToType<string, T>(obj));
                }
                else if (typeof(T) == typeof(byte[]))
                {
                    hc = new StreamContent(new MemoryStream(ConvertToType<byte[], T>(obj)));
                }

                HttpResponseMessage response = await client.PostAsync(path, (HttpContent)hc);
                return response;
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                try
                {
                    client.DefaultRequestHeaders.Remove("Content-Type");
                }
                catch { }
            }
        }

        /// <summary>
        /// Posts data as json
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="path"></param>
        /// <param name="obj"></param>
        /// <returns></returns>
        public async Task<HttpResponseMessage> Post<T>(string path, T obj)
        {
            try
            {
                client.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "application/json");
                HttpResponseMessage response = await client.PostAsJsonAsync(path, obj);
                return response;
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                try
                {
                    client.DefaultRequestHeaders.Remove("Content-Type");
                }
                catch { }
            }
        }

        public async Task<HttpResponseMessage> Put(string path)
        {
            try
            {
                HttpResponseMessage response = await client.PutAsync(path, new StringContent(""));
                return response;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public async Task<HttpResponseMessage> Put<T>(string path, T obj)
        {
            try
            {
                HttpResponseMessage response = await client.PutAsJsonAsync(path, obj);
                return response;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<HttpStatusCode> Delete(string path)
        {
            try
            {
                HttpResponseMessage response = await client.DeleteAsync(path);
                return response.StatusCode;
            }
            catch (Exception)
            {
                throw;
            }
        }

        private R ConvertToType<R, T>(T obj)
        {
            return (R)Convert.ChangeType(obj, typeof(R));
        }
    }
}
