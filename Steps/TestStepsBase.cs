using System.Threading.Tasks;
using BBlog.Tests.AppAbstraction;
using BBlog.Tests.AppAbstraction.DtoObjects;
using BBlog.Tests.AppAbstraction.Enums;
using BoDi;

namespace BBlog.Tests.Steps;

public abstract class TestStepsBase
{
    protected Article? ExpectedArticle;
    protected UiAppClient UiAppClient { get; }
    protected ApiAppClient ApiAppClient { get; }
    public IObjectContainer Container { get; }
    private UserType _userType;

    public UserType UserType
    {
        get => _userType;
        protected set
        {
            _userType = value;
            if (_userType == UserType.Authorized)
            {
                Task.Run(() => UiAppClient.Authenticate()).Wait();
                Task.Run(() => ApiAppClient.Authenticate()).Wait();
            }
            else
            {
                Task.Run(() => UiAppClient.Logout()).Wait();
                Task.Run(() => ApiAppClient.Logout()).Wait();
            }

        }
    }

    protected TestStepsBase(UiAppClient uiAppClient, ApiAppClient apiAppClient, IObjectContainer container)
    {
        UiAppClient = uiAppClient;
        ApiAppClient = apiAppClient;
        Container = container;
    }
}