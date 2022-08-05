using System.Threading.Tasks;
using BBlog.Tests.AppAbstraction.DtoObjects;
using Microsoft.Playwright;

namespace BBlog.Tests.AppAbstraction.UI.Articles;

public class ArticlesContainer : PageBase
{
    //invalid selectors can hang the operation on ILocator???

    protected const string ArticlePreviewXpathTemplate =
        "xpath=.//*[@class='app-article-preview' and contains(text(),'{0}')]";
    protected ILocator NoArticlesMessage => Page.Locator(string.Format(ArticlePreviewXpathTemplate, "No articles"));
    protected ILocator LoadingArticlesMessage => Page.Locator(string.Format(ArticlePreviewXpathTemplate, "Loading articles"));
    protected ArticlesContainer(UiAppClient uiAppClient) : base(uiAppClient)
    {
    }

    public async Task<ArticlePage> OpenArticle(string slug)
    {
        var articleItem = new ArticleItem(this, slug);
        if (!await articleItem.ArticleExist())
            throw new ArticleNotFoundException(slug);
        return await articleItem.OpenArticle();
    }

    public async Task<Article> GetArticleData(string slug)
    {
        await CheckIfAnyArticleExists(slug);
        var articleItem = new ArticleItem(this, slug);

        if (!await articleItem.ArticleExist())
            throw new ArticleNotFoundException(slug);

        var description = articleItem.Description;
        var articlePage = await articleItem.OpenArticle();
        return new Article()
        {
            Description = description,
            Title = articlePage.Title,
            Body = articlePage.Body,
            Slug = articlePage.Slug,
            TagList = articlePage.Tags,
            Author = await articlePage.GetAuthor()
        };
    }

    private async Task CheckIfAnyArticleExists(string textIdentifier)
    {
        if (await NoArticlesMessage.IsVisibleAsync())
        {
            throw new ArticleNotFoundException(textIdentifier);
        }
    }


    public async Task<bool> ArticleExists(string slug)
    {
        var articleItem = new ArticleItem(this, slug);
        return await articleItem.ArticleExist();
    }


    //Iterator?
    public bool MoveToNextPage()
    {
        //TODO: article pager navigation
        return false;

    }

    private async Task WaitForArticlesToBeLoaded()
    {
        await LoadingArticlesMessage.WaitForAsync(new LocatorWaitForOptions()
        {
            State = WaitForSelectorState.Attached
        });
        await LoadingArticlesMessage.WaitForAsync(new LocatorWaitForOptions()
        {
            State = WaitForSelectorState.Hidden
        });
    }

    protected override async Task PageSpecificWait()
    {
        await WaitForArticlesToBeLoaded();//Works in SPA (no page navigation)
    }
}