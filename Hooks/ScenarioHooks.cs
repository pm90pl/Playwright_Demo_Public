using System;
using System.IO;
using System.Threading.Tasks;
using BBlog.Tests.AppAbstraction;
using BBlog.Tests.AppAbstraction.DtoObjects;
using BBlog.Tests.Utils;
using BoDi;
using Microsoft.Playwright;
using NUnit.Framework;
using TechTalk.SpecFlow;
using TechTalk.SpecFlow.Infrastructure;

namespace BBlog.Tests.Hooks
{
    [Binding]
    public class ScenarioHooks
    {
        [BeforeScenario("UITest")]
        public async Task BeforeUiScenario(IObjectContainer container)
        {
            await container.Resolve<UiAppClient>().Init();
        }


        [BeforeScenario]
        public static void RegisterDependencies(IObjectContainer container)
        {
            container.RegisterFactoryAs((c) => Task.Run(() => CreateRandomArticle(c)).Result);
        }

        private static async Task<Article> CreateRandomArticle(IObjectContainer container)
        {
            var apiClient = container.Resolve<ApiAppClient>();
            await apiClient.Authenticate();
            return await apiClient.CreateRandomArticle();
        }

        [BeforeScenario("Manual")]
        public void BeforeManualScenario()
        {
            Assert.Ignore("manual test");
        }

        [AfterScenario("UITest")]
        public void AfterTest(IObjectContainer container, ScenarioContext context, ISpecFlowOutputHelper outputHelper)
        {
            var projectDir = IOHelpers.GetRootProjectDir();
            var relativeScreenshotPath = Path.Combine("Screenshots"
                , $"{context.ScenarioInfo.Title}{DateTime.Now:HH_mm_ss_fff}.png");
            var absoluteScreenshotPath = Path.Combine(projectDir, "TestResults", relativeScreenshotPath);
            var appClient = container.Resolve<UiAppClient>();
            var screenshots = appClient.Page?.ScreenshotAsync(new PageScreenshotOptions()
            {
                Path = absoluteScreenshotPath,
                Type = ScreenshotType.Png
            }).Result;
            //In case of browser initialization error - it would not be possible to get a screenshots
            if (screenshots is not null)
            {
                outputHelper.AddAttachment(Path.Combine(".",relativeScreenshotPath));
            }
        }
    }
}
