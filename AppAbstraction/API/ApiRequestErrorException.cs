using System;
using System.Collections.Generic;
using System.Net;
using BBlog.Tests.Utils;

namespace BBlog.Tests.AppAbstraction.API;

public class ApiRequestErrorException : Exception
{

    public ApiErrorDetails ApiErrorDetails { get; }

    public ApiRequestErrorException(HttpStatusCode statusCode, string mediaType, string content, string url) : base($"{url} {content}")
    {
        ApiErrorDetails = new ApiErrorDetails()
        {
            StatusCode = statusCode,
            MediaType = mediaType,
            Errors = JsonSerialization.DeserializeCaseInsensitive<Dictionary<string, object>>(content, "errors"),
            ResponseContent = content
        };
    }

}