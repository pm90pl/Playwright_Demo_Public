using System.Threading.Tasks;
using BBlog.Tests.AppAbstraction.DtoObjects;
using BBlog.Tests.AppAbstraction.UI.Common;
using Microsoft.Playwright;

namespace BBlog.Tests.AppAbstraction.UI.Articles;

[ElementTextIdentifier("editor", TextType.Attribute)]
public class ArticleEditionPage : PageBase
{
    protected ArticleEditionPage(UiAppClient uiAppClient) : base(uiAppClient)
    {
    }


    public async Task FillFields(Article article)
    {
        await Page.FillAsync("[formcontrolname='title']", article.Title!);
        await Page.FillAsync("[formcontrolname='description']", article.Description!);
        await Page.FillAsync("[formcontrolname='body']", article.Body!);
    }

    public async Task<ArticlePage> PublishArticle()
    {
        await Page.Locator("button", new PageLocatorOptions()
        {
            HasTextString = "Publish Article"
        }).ClickAsync();
        return await GetPageWhenReady<ArticlePage>(UiAppClient);

    }

    protected override async Task PageSpecificWait()
    {
    }
}