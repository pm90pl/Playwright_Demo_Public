using System;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace BBlog.Tests.Utils;

public static class JsonSerialization
{
    public static T DeserializeCaseInsensitive<T>(string jsonResponse, string responseSection)
    {
        try
        {
            var jsonNode = JsonNode.Parse(jsonResponse);
            var responseSectionNode = jsonNode![responseSection];
            return JsonSerializer.Deserialize<T>(responseSectionNode!.ToJsonString(), new JsonSerializerOptions()
            {
                PropertyNameCaseInsensitive = true
            })!;
        }
        catch
        {
            return default;
        }
       
    }
}