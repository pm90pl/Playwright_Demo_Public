using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using BBlog.Tests.AppAbstraction.DtoObjects;
using BBlog.Tests.TestRunConfiguration;
using BBlog.Tests.Utils;

namespace BBlog.Tests.AppAbstraction.Base
{
    public abstract class AppClient : IDisposable
    {
        protected readonly BasicAuth BasicAuth;
        protected readonly string AppBaseUrl;
        protected readonly User TestUser;
        protected readonly User SecondUser;
        protected User UserToUse { get; set; }
        protected readonly ApiAuthenticator Authenticator;
        public LoggedInUser? LoggedInUser { get; protected set; }
        public bool LoggedIn { get; protected set; }
        protected AppClient()
        {
            var settings = Settings.LoadSettings();
            BasicAuth = settings.Environment!.BasicAuth!;
            AppBaseUrl = settings.Environment.AppBaseUrl!;
            TestUser = settings.TestUser!;
            SecondUser = settings.SecondUser!;
            Authenticator = new ApiAuthenticator(this);
        }

        public async Task Authenticate()
        {
            UserToUse = TestUser;
            await LogIn();
        }

        public async Task AuthenticateAsSecondUser()
        {
            UserToUse = SecondUser;
            await LogIn();
        }

        private async Task LogIn()
        {
            if (LoggedIn)
                return;
            LoggedInUser = await Authenticator.LogInUser();
            await AuthenticateImpl();
            LoggedIn = true;
        }

        public async Task Logout()
        {
            if (!LoggedIn)
                return;
            await LogoutImpl();
            LoggedIn = false;
        }


        protected abstract Task AuthenticateImpl();
        protected abstract Task LogoutImpl();
        public sealed class ApiAuthenticator : IDisposable
        {
            private readonly AppClient _parentClient;
            private const string LoginUserEndpoint = "/api/users/login";
            private const string RegisterUserEndpoint = "/api/users";
            public HttpClient BasicAuthHttpClient { get; }

            public ApiAuthenticator(AppClient parentClient)
            {
                _parentClient = parentClient;
                BasicAuthHttpClient = InitBasicAuthHttpClient();
            }

            private HttpClient InitBasicAuthHttpClient()
            {
                var client = new HttpClient()
                {
                    BaseAddress = new Uri(_parentClient.AppBaseUrl),

                };
                client.DefaultRequestHeaders.Authorization = CreateBasicAuthHeader();
                return client;
            }

            public async Task<LoggedInUser> LogInUser()
            {
                HttpResponseMessage authenticationResponse = null;
                try
                {
                    var user = new { User = _parentClient.UserToUse };
                    var requestCredentialsContent = HttpHelper.CreateJsonContent(user);
                    authenticationResponse =
                        await BasicAuthHttpClient.PostAsync(LoginUserEndpoint, requestCredentialsContent);
                    if (authenticationResponse.StatusCode == HttpStatusCode.UnprocessableEntity)
                    {
                        authenticationResponse =
                            await BasicAuthHttpClient.PostAsync(RegisterUserEndpoint, requestCredentialsContent);
                    }

                    authenticationResponse.EnsureSuccessStatusCode();
                    var loggedInUserInfo = await authenticationResponse.Content.ReadAsAsync<LoginResponse>();
                    return loggedInUserInfo.User;
                }
                finally
                {
                    authenticationResponse?.Dispose();
                }
            }

            private AuthenticationHeaderValue CreateBasicAuthHeader()
            {
                var authenticationString = $"{_parentClient.BasicAuth.Login}:{_parentClient.BasicAuth.Password}";
                var base64EncodedAuthenticationString = Convert.ToBase64String(Encoding.ASCII.GetBytes(authenticationString));
                return new AuthenticationHeaderValue("Basic", base64EncodedAuthenticationString);
            }

            public void Dispose()
            {
                BasicAuthHttpClient.Dispose();
            }
        }


        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                Authenticator.Dispose();
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
