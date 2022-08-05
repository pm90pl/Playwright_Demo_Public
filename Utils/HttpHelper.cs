using System.Net.Http;
using System.Runtime.ExceptionServices;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using BBlog.Tests.AppAbstraction.API;

namespace BBlog.Tests.Utils;

public static class HttpHelper
{
    public static StringContent CreateJsonContent(object body)
    {
        var serializeOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true
        };

        var authenticationRequestBody = JsonSerializer.Serialize(body, serializeOptions);
        return new StringContent(authenticationRequestBody, Encoding.UTF8, "application/json");
    }


    public static async Task HandleRequestError(HttpResponseMessage httpResponse)
    {
        try
        {
            httpResponse.EnsureSuccessStatusCode();
        }
        catch
        {
            var responseContentMediaType = httpResponse.Content?.Headers?.ContentType?.MediaType;
            var responseContent = await httpResponse.Content?.ReadAsStringAsync();
            var responseCode = httpResponse.StatusCode;
            ExceptionDispatchInfo.Throw(new ApiRequestErrorException(responseCode, responseContentMediaType,
                responseContent, httpResponse.RequestMessage.RequestUri.AbsoluteUri));
        }
    }
}