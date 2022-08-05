using System;
using System.Threading.Tasks;
using BBlog.Tests.AppAbstraction.Enums;
using BBlog.Tests.AppAbstraction.UI.Articles;
using BBlog.Tests.AppAbstraction.UI.Common;
using Microsoft.Playwright;

namespace BBlog.Tests.AppAbstraction.UI;

[ElementTextIdentifier("Home", TextType.InnerText)]
public class HomePage : ArticlesContainer
{
    private const string FeedNavLinkLocator = ".feed-toggle .nav-link";
    private const string GlobalFeedText = "Global Feed";
    private const string YourFeedText = "Your Feed";

    private ILocator GlobalFeed => GetFeed(GlobalFeedText);
    private ILocator YourFeed => GetFeed(YourFeedText);
    protected HomePage(UiAppClient uiAppClient) : base(uiAppClient)
    {

    }

    public async Task SwitchFeed(FeedType feed)
    {
        var feedElement = feed switch
        {
            FeedType.GlobalFeed => GlobalFeed,
            FeedType.YourFeed => YourFeed,
            _ => throw new ArgumentOutOfRangeException(nameof(feed), feed, "not handled")
        };
        await feedElement.ClickAsync();
        await WaitForPageToBeReady();
    }


    private ILocator GetFeed(string feedText)
    {
        return Page.Locator(FeedNavLinkLocator, new PageLocatorOptions() { HasTextString = feedText });
    }
}