using backend.Controllers.API_Bodys.NextPosition;
using System.Text.Json;

namespace backend.Controllers
{
    public class Client
    {
        private readonly HttpClient client = new();
        private readonly string? URL = "";

        public Client(string URL) 
        {
            this.URL = URL;
        }

        public async Task<NextPositionRequestBody?> SetTarget(NextPositionRequestBody request)
        {
            if (request == null)
                throw new Exception("body == null at SendBotRoute()");

            StringContent postBody = GetStringContentfromJsonObject(request);

            var response = await client.PostAsync(URL + "/api/setTarget", postBody);
            response.EnsureSuccessStatusCode();


            string jsonString = await response.Content.ReadAsStringAsync();
            NextPositionRequestBody? responseBody = JsonSerializer.Deserialize<NextPositionRequestBody>(jsonString);

            return responseBody;
        }

        public async void NextPosition(NextPositionRequestBody request)
        {
            if (request == null)
                throw new Exception("body == null at SendBotRoute()");

            StringContent postBody = GetStringContentfromJsonObject(request);

            var response = await client.PostAsync(URL + "/api/nextPosition", postBody);
            response.EnsureSuccessStatusCode();
        }

        private StringContent GetStringContentfromJsonObject(object json) 
        {
            string jsonString = JsonSerializer.Serialize(json);
            return new StringContent(jsonString);
        }
    }
}
