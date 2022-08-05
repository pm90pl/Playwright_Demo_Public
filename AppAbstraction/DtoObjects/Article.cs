using System.Collections.Generic;
using BBlog.Tests.Utils;

namespace BBlog.Tests.AppAbstraction.DtoObjects;


public class Article
{
    public string? Slug { get; set; }
    public string? Title { get; set; }
    public string? Description { get; set; }
    public string? Body { get; set; }
    public string[]? TagList { get; set; }
    public Author? Author { get; set; }

    public static Article DeserializeArticleFromApiResponse(string jsonResponse)
    {
        return JsonSerialization.DeserializeCaseInsensitive<Article>(jsonResponse, "article");
    }

    public static IEnumerable<Article> DeserializeArticlesFromApiResponse(string jsonResponse)
    {
        return JsonSerialization.DeserializeCaseInsensitive<IEnumerable<Article>>(jsonResponse, "articles");
    }
}

public class Author
{
    public string? Username { get; set; }
    public string? Image { get; set; }
    public bool Following { get; set; }
}