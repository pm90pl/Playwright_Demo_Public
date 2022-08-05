using System.Collections.Generic;
using System.Net;

namespace BBlog.Tests.AppAbstraction.API;

public class ApiErrorDetails
{
    public HttpStatusCode StatusCode { get; set; }
    public IDictionary<string, object>? Errors { get; set; }
    public string? MediaType { get; set; }
    public string ResponseContent { get; set; }
}