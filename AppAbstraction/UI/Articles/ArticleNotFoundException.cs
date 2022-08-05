using System;

namespace BBlog.Tests.AppAbstraction.UI.Articles;

public class ArticleNotFoundException : Exception
{
    public ArticleNotFoundException(string slug) : base($"Cannot find article with slug {slug}")
    {

    }
}