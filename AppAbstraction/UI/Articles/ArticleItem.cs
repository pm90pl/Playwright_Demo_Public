using System.Threading.Tasks;
using Microsoft.Playwright;

namespace BBlog.Tests.AppAbstraction.UI.Articles;

public class ArticleItem : PageSection<ArticlesContainer>
{
    private readonly string _articleSelector;
    private ILocator ArticleLocator => Page.Locator(_articleSelector);
    public string Description => ArticleLocator.Locator("p").InnerTextAsync().Result;

    public ArticleItem(ArticlesContainer articleContainer, string slug) : base(articleContainer)
    {
        _articleSelector = $"[href='/article/{slug}']";
    }

    public async Task<ArticlePage> OpenArticle()
    {
        await ArticleLocator.ClickAsync();
        return await PageBase.GetPageWhenReady<ArticlePage>(ParentPage.UiAppClient);
    }

    public async Task<bool> ArticleExist()
    {
        var canMoveToTheNextPage = true;
        while (!await ArticleLocator.IsVisibleAsync() && canMoveToTheNextPage)
        {
            canMoveToTheNextPage = ParentPage.MoveToNextPage();
        }

        var articleExists = await ArticleLocator.IsVisibleAsync();
        if (articleExists)
            await ArticleLocator.ScrollIntoViewIfNeededAsync();
        return articleExists;
    }
}