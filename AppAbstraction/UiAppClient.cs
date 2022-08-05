using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using BBlog.Tests.AppAbstraction.Base;
using BBlog.Tests.AppAbstraction.DtoObjects;
using BBlog.Tests.AppAbstraction.UI;
using BBlog.Tests.AppAbstraction.UI.Common;
using BBlog.Tests.Utils;
using Microsoft.Playwright;
using SpecFlow.Actions.Playwright;

namespace BBlog.Tests.AppAbstraction;

public class UiAppClient : AppClient
{
    public PageBase CurrentView { get; set; }
    public T CurrentViewAs<T>() where T : PageBase => (T)CurrentView;
    internal IPage? Page { get; set; }
    private readonly BrowserDriver _browserDriver;
    private bool _wasInitialized;
    private ILocator NavBar => Page!.Locator(".navbar-nav");
    private const string JwtTokenLocalStorageName = "jwtToken";
    public const string CommentsJsonFilePath = "Resources\\MockedApiResponses\\Comments.json";
    public UiAppClient(BrowserDriver browserDriver)
    {
        _browserDriver = browserDriver;
    }

    public async Task Init()
    {
        _wasInitialized = true;
        var browser = await _browserDriver.Current;
        var context = await browser.NewContextAsync(
            new BrowserNewContextOptions()
            {
                HttpCredentials = new HttpCredentials()
                {
                    Password = BasicAuth.Password!,
                    Username = BasicAuth.Login!
                }
            });
        Page = await context.NewPageAsync();
        await Page.GotoAsync(AppBaseUrl);
        CurrentView = await PageBase.GetPageWhenReady<HomePage>(this);
    }

    protected override async Task AuthenticateImpl()
    {
        await RunJsScriptAndRefresh($"window.localStorage.setItem('{JwtTokenLocalStorageName}', '{LoggedInUser!.Token}')");
    }

    protected override async Task LogoutImpl()
    {
        await RunJsScriptAndRefresh($"window.localStorage.removeItem('{JwtTokenLocalStorageName}')");
    }

    private async Task RunJsScriptAndRefresh(string script)
    {
        if (!_wasInitialized)
            return;
        await Page!.EvaluateAsync(script);
        _ = await Page.ReloadAsync();
    }

    public async Task<IEnumerable<Article>> MockFeedApiResponse()
    {
        var feedResponseJson = await File.ReadAllTextAsync(Path.Combine("Resources","MockedApiResponses","FeedResponse.json"));
        await Page!.RouteAsync(url => url.Contains("/api/articles/feed"),
            async r => await r.FulfillAsync(GetJsonSuccessfulResponseOptions(feedResponseJson)));
        return Article.DeserializeArticlesFromApiResponse(feedResponseJson);
    }

    public async Task MockCommentsApiResponse(string articleSlug)
    {
        var commentsJson = await File.ReadAllTextAsync(CommentsJsonFilePath);
        await Page!.RouteAsync(url => url.Contains($"/api/articles/{articleSlug}/comments"),
            async r => await r.FulfillAsync(
                GetJsonSuccessfulResponseOptions(commentsJson)));
    }

    private static RouteFulfillOptions GetJsonSuccessfulResponseOptions(string content)
    {
        return new RouteFulfillOptions
        {
            Status = 200,
            ContentType = "text/json",
            Body = content
        };
    }


    public async Task<T> GoTo<T>() where T : PageBase
    {
        var pageTextLocator = typeof(T).GetAttribute<ElementTextIdentifier>();

        var selector = pageTextLocator.Type switch
        {
            TextType.Attribute => $"a[href*='{pageTextLocator.Value}']",
            TextType.InnerText => $"xpath=.//a[contains(text(),'{pageTextLocator.Value}')]",
            _ => throw new ArgumentOutOfRangeException($"Text type wasnot implemented:{pageTextLocator.Type}")
        };

        await NavBar
            .Locator(selector)
            .ClickAsync();

        return await PageBase.GetPageWhenReady<T>(this);
    }
}