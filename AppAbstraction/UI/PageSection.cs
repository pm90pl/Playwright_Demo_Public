using Microsoft.Playwright;

namespace BBlog.Tests.AppAbstraction.UI;

public class PageSection<T> where T : PageBase
{
    protected T ParentPage { get; }
    protected IPage Page => ParentPage.UiAppClient.Page!;

    protected PageSection(T parentPage)
    {
        ParentPage = parentPage;
    }
}