using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using BBlog.Tests.AppAbstraction;
using BBlog.Tests.AppAbstraction.API;
using BBlog.Tests.AppAbstraction.DtoObjects;
using BBlog.Tests.AppAbstraction.Enums;
using BBlog.Tests.AppAbstraction.UI;
using BBlog.Tests.AppAbstraction.UI.Articles;
using BBlog.Tests.Utils;
using BoDi;
using FluentAssertions;
using TechTalk.SpecFlow;

namespace BBlog.Tests.Steps
{
    [Binding]
    public class ArticleSteps : TestStepsBase
    {
        private object? _articleCreationResult;

        public ArticleSteps(UiAppClient uiAppClient, ApiAppClient apiAppClient, IObjectContainer container)
            : base(uiAppClient, apiAppClient, container)
        {
        }

        #region Given

        [Given(@"an article was created")]
        public async Task GivenAnArticleWasCreated()
        {
            await ApiAppClient.Authenticate();
            ExpectedArticle = await ApiAppClient.CreateRandomArticle();
        }

        [Given(@"an Authorized user opened their article")]
        public async Task GivenAnAuthorizedUserOpenedTheirArticle()
        {
            await ApiAppClient.Authenticate();
            ExpectedArticle = await ApiAppClient.CreateRandomArticle();
            await UiAppClient.Authenticate();
            var myProfile = await UiAppClient.GoTo<MyProfile>();
            await myProfile.OpenArticle(ExpectedArticle.Slug!);
        }



        //TODO: change when Bug 1 is fixed
        [Given(@"an Authorized user followed an article author")]
        public async Task GivenAnAuthorizedUserFollowsAnArticleAuthor()
        {
            //A workaround to test YourFeed view because of Bug 1
            ExpectedArticle = (await UiAppClient.MockFeedApiResponse()).First();
        }


        [Given(@"Authorized user has an article ready to be published")]
        public async Task GivenAuthorizedUserStartedNewArticleCreation()
        {
            await UiAppClient.Authenticate();
            var newArticlePage = await UiAppClient.GoTo<ArticleEditionPage>();
            ExpectedArticle = DataGenerator.GenerateData<Article>();
            ExpectedArticle.TagList = new string[] { };//do not fill tags
                                                       //-> TODO: data generator could be a builder?
            await newArticlePage.FillFields(ExpectedArticle);
        }


        #endregion

        #region When

        [When(@"(Authorized|Unauthorized) user opens (Global Feed|Your Feed)")]
        public async Task WhenUserOpensTheFeed(UserType userType, FeedType feed)
        {
            if (userType == UserType.Authorized)
            {
                await UiAppClient.Authenticate();
            }

            await UiAppClient.GoTo<HomePage>();
            await UiAppClient.CurrentViewAs<HomePage>().SwitchFeed(feed);
        }

        [When(@"Authorized API client creates new article")]
        public async Task WhenAuthorizedAPIClientCreatesNewArticle()
        {
            await ApiAppClient.Authenticate();
            _articleCreationResult = await ApiAppClient.CreateRandomArticle();
        }

        [When(@"Authorized API client creates new article with null (.*)")]
        public async Task WhenAuthorizedAPIClientCreatesNewArticleWithoutProperty(string property)
        {
            await ApiAppClient.Authenticate();
            var article = DataGenerator.GenerateData<Article>();
            article.SetPropertyValue(property, null);
            try
            {
                _articleCreationResult = await ApiAppClient.CreatedArticle(article);
            }
            catch (Exception e)
            {
                _articleCreationResult = e;
            }
        }


        [When(@"the user deletes the article")]
        public async Task WhenUserTheUserDeletesTheArticle()
        {
            await UiAppClient.CurrentViewAs<ArticlePage>().DeleteArticle();
        }


        [When(@"the user publishes the article")]
        public async Task WhenTheUserPublishesTheArticle()
        {
            var articlePage = await UiAppClient.CurrentViewAs<ArticleEditionPage>().PublishArticle();
            ExpectedArticle!.Slug = articlePage.Slug;
        }


        #endregion

        #region Then
        [Then(@"the article should be accessible to the user from their profile page")]
        public async Task ThenTheArticleShouldBeAccessibleToTheUserOnProfilePage()
        {
            var actualArticle = await (await UiAppClient.GoTo<MyProfile>()).GetArticleData(ExpectedArticle!.Slug!);
            actualArticle.Should().BeEquivalentTo(ExpectedArticle,
                options => options.Excluding(a => a.Author));
        }

        [Then(@"the article should be accessible to the user")]
        public async Task ThenTheArticleShouldBeAccessibleToTheUser()
        {
            var actualArticle = await UiAppClient.CurrentViewAs<HomePage>().GetArticleData(ExpectedArticle!.Slug!);
            actualArticle.Should().BeEquivalentTo(ExpectedArticle);
        }


        [Then(@"the article should not be created")]
        public void ThenTheArticleShouldNotBeCreated()
        {
            _articleCreationResult.Should()
                .BeOfType<ApiRequestErrorException>("article creation should fail when mandatory field is not provided");
            var creationError = ((ApiRequestErrorException)_articleCreationResult).ApiErrorDetails;
            //creationError.StatusCode
            //    .Should().Be(HttpStatusCode.UnprocessableEntity);
            creationError.MediaType.Should().Be("application/json");
        }

        [Then(@"the article should be accessible to the API user")]
        public async Task ThenTheArticleShouldBeAccessibleToTheAPIUser()
        {
            var expectedArticle = (Article)_articleCreationResult!;
            var unauthorizedApiClient = new ApiAppClient();
            var articles = (await unauthorizedApiClient.GetArticles(expectedArticle.Author!.Username));
            articles.Should().ContainEquivalentOf(expectedArticle);
        }

        [Then(@"the article should not be accessible from the user's profile page")]
        public async Task ThenTheArticleShouldNotBeAccessibleFromTheUserProfilePage()
        {
            var myProfile = await UiAppClient.GoTo<MyProfile>();
            var articleExists = await myProfile.ArticleExists(ExpectedArticle.Slug);
            articleExists.Should().BeFalse("article was removed");
        }
        
        #endregion
    }
}
