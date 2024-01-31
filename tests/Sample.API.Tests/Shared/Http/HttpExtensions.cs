using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Sample.API.Tests.Shared.Http;

public static class HttpExtensions
{
    public static async Task<CustomHttpResponseMessage<T>> DeserializeAsync<T>(
        this Task<HttpResponseMessage> task)
    {
        var response = await task.ConfigureAwait(false);
        return await response.DeserializeAsync<T>().ConfigureAwait(false);
    }

    public static async Task<CustomHttpResponseMessage<T>> DeserializeAsync<T>(this HttpResponseMessage response) =>
        new(response,
            JsonConvert.DeserializeObject<T>(await response.Content.ReadAsStringAsync().ConfigureAwait(false)));
}
