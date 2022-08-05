using System;
using System.Net;
using System.Threading.Tasks;
using BBlog.Tests.AppAbstraction;
using BBlog.Tests.AppAbstraction.API;
using BBlog.Tests.AppAbstraction.DtoObjects;
using BBlog.Tests.AppAbstraction.Enums;
using BBlog.Tests.AppAbstraction.UI;
using BBlog.Tests.AppAbstraction.UI.Articles;
using BoDi;
using FluentAssertions;
using Microsoft.VisualBasic.CompilerServices;
using TechTalk.SpecFlow;

namespace BBlog.Tests.Steps
{
    [Binding]
    public class CommentSteps : TestStepsBase
    {
        private Comment _expectedComment;
        private object _actionResult;

        public CommentSteps(UiAppClient uiAppClient, ApiAppClient apiAppClient, Article article, IObjectContainer container) :
            base(uiAppClient, apiAppClient, container)
        {
            ExpectedArticle = article;
            if (article.Slug is null)
                throw new IncompleteInitialization();
        }

        //TODO: change when Bug 6 is fixed
        [Given(@"an article was commented")]
        public async Task GivenAnArticleWasCommented()
        {
            await ApiAppClient.Authenticate();
            _expectedComment = await ApiAppClient.CreateUniqueComment(ExpectedArticle!.Slug);
            await UiAppClient.MockCommentsApiResponse(ExpectedArticle!.Slug);
        }

        [Given(@"an article was commented by another user")]
        public async Task GivenAnArticleWasCommentedByAnotherUser()
        {
            await ApiAppClient.Logout();
            await ApiAppClient.AuthenticateAsSecondUser();
            _expectedComment = await ApiAppClient.CreateUniqueComment(ExpectedArticle.Slug);
            await ApiAppClient.Logout();
        }


        [When(@"(.*) opens the article")]
        public async Task WhenUserOpensTheArticle(UserType user)
        {
            UserType = user;
            await OpenArticle();
        }

        [When(@"Unauthorized API user comments the article")]
        public async Task WhenUnauthorizedAPIUserCommentsTheArticle()
        {
            UserType = UserType.Unauthorized;

            try
            {
                _actionResult = await ApiAppClient.CreateRandomArticle();
            }
            catch (Exception e)
            {
                _actionResult = e;
            }

        }


        private async Task OpenArticle()
        {
            var home = await UiAppClient.GoTo<HomePage>();
            await home.SwitchFeed(FeedType.GlobalFeed);
            await home.OpenArticle(ExpectedArticle!.Slug!);
        }

        [When(@"Authorized user comments the article")]
        public async Task WhenAuthorizedUserCommentsTheArticle()
        {
            UserType = UserType.Authorized;
            await OpenArticle();
            _expectedComment = await UiAppClient.CurrentViewAs<ArticlePage>().AddRandomComment();
        }


        [When(@"Authorized API user removes the comment")]
        public async Task WhenAuthorizedAPIUserRemovesTheComment()
        {
            try
            {
                await ApiAppClient.Authenticate();
                await ApiAppClient.RemoveComment(ExpectedArticle.Slug, _expectedComment.Id);

            }
            catch (Exception e)
            {
                _actionResult = e;
            }
        }



        [Then(@"the comment should be accessible to the user")]
        public void ThenTheCommentShouldBeAccessibleToTheAuthorized()
        {
            UiAppClient.CurrentViewAs<ArticlePage>().Comments.Should().Contain(_expectedComment.Body);
        }


        [Then(@"the comment should not be created via API")]
        public void ThenTheCommentShouldNotBeCreatedViaAPI()
        {
            _actionResult.Should().BeOfType<ApiRequestErrorException>();
            var errorDetails = (_actionResult as ApiRequestErrorException).ApiErrorDetails;
            errorDetails.MediaType.Should().BeEquivalentTo("application/json");
            errorDetails.StatusCode.Should().Be(HttpStatusCode.Unauthorized);

        }

        [Then(@"the comment should not be removed via API")]
        public void ThenTheCommentShouldNotBeRemovedViaAPI()
        {
            _actionResult.Should().BeOfType<ApiRequestErrorException>();
            var errorDetails = (_actionResult as ApiRequestErrorException).ApiErrorDetails;
            errorDetails.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        }


    }
}
