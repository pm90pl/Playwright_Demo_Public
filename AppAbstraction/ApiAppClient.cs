using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using System.Web;
using BBlog.Tests.AppAbstraction.Base;
using BBlog.Tests.AppAbstraction.DtoObjects;
using BBlog.Tests.Utils;

namespace BBlog.Tests.AppAbstraction;

public class ApiAppClient : AppClient
{
    private readonly HttpClient _apiHttpClient;
    private const string ArticlesEndpoint = "/api/articles";
    private const string CommentEndpointTemplate = "/api/articles/{0}/comments";
    private const string JwtAuthHeaderKey = "jwtauthorization";
    private string GetCommentEndpoint(string articleSlug) => string.Format(CommentEndpointTemplate, articleSlug);
    private string LatestResponse { get; set; }
    protected override Task AuthenticateImpl()
    {
        _apiHttpClient.DefaultRequestHeaders.Add(JwtAuthHeaderKey, $"Token {LoggedInUser!.Token}");
        return Task.WhenAll();
    }

    protected override Task LogoutImpl()
    {
        _apiHttpClient.DefaultRequestHeaders.Remove(JwtAuthHeaderKey);
        return Task.WhenAll();
    }

    public ApiAppClient()
    {
        _apiHttpClient = Authenticator.BasicAuthHttpClient;
    }

    public async Task<IEnumerable<Article>> GetArticles(string? author = null)
    {
        var endpoint = author is null
            ? ArticlesEndpoint
            : $"{ArticlesEndpoint}?author={HttpUtility.ParseQueryString(author)}";

        using var httpResponse = await _apiHttpClient.GetAsync(endpoint);
        await HttpHelper.HandleRequestError(httpResponse);
        var articlesInfo = await httpResponse.Content.ReadAsAsync<ArticlesInfo>();
        return articlesInfo.Articles;

    }

    public async Task<Article> CreateRandomArticle()
    {
        var article = DataGenerator.GenerateData<Article>();
        return await CreatedArticle(article);
    }

    public async Task<Article> CreatedArticle(Article article)
    {
        var newArticle = new { Article = article };
        return await PostJson<Article>(newArticle, ArticlesEndpoint, "article");
    }

    public async Task<Comment> CreateUniqueComment(string articleSlug)
    {
        var comment = DataGenerator.GenerateData<Comment>();
        var newComment = new
        {
            Comment = comment
        };
        var endpoint = GetCommentEndpoint(articleSlug);
        var createdComment = await PostJson<Comment>(newComment, endpoint, "comment");
        SaveResponseToMockGetComments();
        return createdComment;
    }

    private void SaveResponseToMockGetComments()
    {
        var commentSectionName = "comment";
        var commentsJson = JsonNode.Parse(LatestResponse).AsObject();
        var commentSection = commentsJson![commentSectionName];
        commentsJson.Remove(commentSectionName);
        commentsJson.Add("comments", new JsonArray(commentSection));
        var commentsJsonString = JsonSerializer.Serialize(commentsJson, new JsonSerializerOptions());
        File.WriteAllText(UiAppClient.CommentsJsonFilePath, commentsJsonString);
    }

    private async Task<T> PostJson<T>(object content, string endpoint, string responseSection)
    {
        var requestContent = HttpHelper.CreateJsonContent(content);
        using var httpResponse = await _apiHttpClient.PostAsync(endpoint, requestContent);
        await HttpHelper.HandleRequestError(httpResponse);
        LatestResponse = await httpResponse.Content.ReadAsStringAsync();
        return JsonSerialization.DeserializeCaseInsensitive<T>(LatestResponse, responseSection);
    }

    public async Task RemoveComment(string articleSlug, string commentId)
    {
        var endpoint = $"{GetCommentEndpoint(articleSlug)}/{commentId}";
        using var httpResponse = await _apiHttpClient.DeleteAsync(endpoint);
        await HttpHelper.HandleRequestError(httpResponse);
    }
}