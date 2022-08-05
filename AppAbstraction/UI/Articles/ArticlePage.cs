using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using BBlog.Tests.AppAbstraction.DtoObjects;
using BBlog.Tests.Utils;
using Microsoft.Playwright;

namespace BBlog.Tests.AppAbstraction.UI.Articles;

public class ArticlePage : PageBase
{
    public IEnumerable<string> Comments => Page.Locator("app-article-comment .card-text").AllInnerTextsAsync().Result
        .Select(c=>c.Trim());
    public string Slug => Regex.Match(Page.Url, "([^/]+$)").Value;
    public string Body => Page.Locator(".article-content p").InnerTextAsync().Result.Trim();
    public string Title => Page.Locator("h1").InnerTextAsync().Result.Trim();
    public string[] Tags => Page.Locator(".tag-default").AllInnerTextsAsync().Result.Select(t => t.Trim()).ToArray();
    private ILocator PostComment => Page.Locator("xpath=.//button[contains(text(),'Post Comment')]");
    private ILocator CommentContent => Page.Locator("textarea[placeholder='Write a comment...']");
    protected ArticlePage(UiAppClient uiAppClient) : base(uiAppClient)
    {
    }

    public async Task<Author> GetAuthor()
    {
        return new Author()
        {
            Username = (await Page.Locator(".author >> nth=0").InnerTextAsync()).Trim(),
            Image = (await Page.Locator(".article-meta img >> nth=0").GetAttributeAsync("src"))!.Trim(),
            Following =
                (await Page.Locator("xpath=.//i[@class='ion-plus-round']/.. >> nth=0").InnerTextAsync()).Contains("Unfollow")
        };
    }


    public async Task<HomePage> DeleteArticle()
    {
        await Page.Locator(".btn-outline-danger >> nth=0").ClickAsync();
        return await GetPageWhenReady<HomePage>(UiAppClient);
    }

    public async Task<Comment> AddRandomComment()
    {
        var comment = DataGenerator.GenerateData<Comment>();
        await CommentContent.FillAsync(comment.Body);
        await PostComment.ClickAsync();
        await Page.WaitForSelectorAsync("app-article-comment", new PageWaitForSelectorOptions()
        {
            State = WaitForSelectorState.Visible,
            Strict = false,
        });
        return comment;
    }

    protected override async Task PageSpecificWait()
    {
        await Page.Locator(".article-page").WaitForAsync(new LocatorWaitForOptions()
        { State = WaitForSelectorState.Visible });
    }
}