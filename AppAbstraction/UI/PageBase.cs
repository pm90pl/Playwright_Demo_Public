using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Playwright;

namespace BBlog.Tests.AppAbstraction.UI;

public abstract class PageBase
{
    internal UiAppClient UiAppClient { get; }
    protected IPage Page => UiAppClient.Page!;

    public async Task WaitForPageToBeReady()
    {
        //https://dev.to/checkly/avoiding-hard-waits-in-playwright-and-puppeteer-272
        //WaitForNavigationAsync hangs ?? when to use?
        //WaitForLoadStateAsync seems to help with few problems with waiting for articles to be loaded
        //Works when navigation occurs -> but it might be only a fluke
        //-> the delay caused by Wait makes the test work but it's like almost like hardcoded wait :(
        await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);
        await PageSpecificWait();
    }

    protected abstract Task PageSpecificWait();

    protected PageBase(UiAppClient uiAppClient)
    {
        UiAppClient = uiAppClient;
        uiAppClient.CurrentView = this;
    }

    public static async Task<T> GetPageWhenReady<T>(UiAppClient uiAppClient) where T : PageBase
    {
        var pageConstructor = typeof(T).GetConstructors(BindingFlags.Instance | BindingFlags.NonPublic).FirstOrDefault();
        var page = (T)pageConstructor!.Invoke(new object?[] { uiAppClient });
        await page.WaitForPageToBeReady();
        return page;
    }
}