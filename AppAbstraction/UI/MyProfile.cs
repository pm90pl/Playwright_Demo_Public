using BBlog.Tests.AppAbstraction.UI.Articles;
using BBlog.Tests.AppAbstraction.UI.Common;

namespace BBlog.Tests.AppAbstraction.UI;

[ElementTextIdentifier("profile", TextType.Attribute)]

public class MyProfile : ArticlesContainer
{
    protected MyProfile(UiAppClient uiAppClient) : base(uiAppClient)
    {
    }



}