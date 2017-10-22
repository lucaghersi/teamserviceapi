using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using StatlerWaldorfCorp.TeamService.Models;

namespace StatlerWaldorfCorp.TeamService.LocationClient
{
    public class HttpLocationClient : ILocationClient
    {
        public HttpLocationClient(string url)
        {
            Url = url;
        }

        public String Url { get; set; }

        public async Task<LocationRecord> GetLatestForMember(Guid memberId)
        {
            LocationRecord locationRecord = null;

            using (var httpClient = new HttpClient())
            {
                httpClient.BaseAddress = new Uri(Url);
                httpClient.DefaultRequestHeaders.Accept.Clear();
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                HttpResponseMessage response =
                    await httpClient.GetAsync(String.Format("/locations/{0}/latest", memberId));

                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    locationRecord = JsonConvert.DeserializeObject<LocationRecord>(json);
                }
            }

            return locationRecord;
        }

        public async Task<LocationRecord> AddLocation(Guid memberId, LocationRecord locationRecord)
        {
            using (var httpClient = new HttpClient())
            {
                httpClient.BaseAddress = new Uri(Url);
                httpClient.DefaultRequestHeaders.Accept.Clear();
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                var jsonString = JsonConvert.SerializeObject(locationRecord);
                var uri = String.Format("/locations/{0}", memberId);
                HttpResponseMessage response =
                    await httpClient.PostAsync(uri, new StringContent(jsonString, Encoding.UTF8, "application/json"));

                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                }
            }

            return locationRecord;
        }
    }
}