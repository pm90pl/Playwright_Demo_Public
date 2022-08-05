## Test Report
During the test sessions, ***Articles*** and ***Comments*** features were covered. 
Test cases were documented as feature files:
- [Articles](Features/Articles.feature)
- [Comments](Features/Comments.feature)

> Manual scenarios are marked with tag ***@Manual***

***Findings can be found here: [ExploratoryTestResults](TestResults/ExploratoryTestResults.md)***

## Running Automated Tests - Prerequisits
1. Installed .NET 6 SKD - can be downloaded [here - .net download page](https://dotnet.microsoft.com/en-us/download)
2. In case of running tests on OS other than Windows - Powershell has to be installed - [details](https://docs.microsoft.com/en-us/powershell/scripting/install/installing-powershell?view=powershell-7.2)
3. SpecFlow Living doc global tool installed - [details](https://docs.specflow.org/projects/specflow-livingdoc/en/latest/LivingDocGenerator/Installing-the-command-line-tool.html)
4. Chrome installed (or one of the browsers: Firefox, Chromium, Edge - but the usage of a browser other than Chrome requires modification of [specflow.actions.json](specflow.actions.json) file)

## Configuration
1. Env configuration (app url, credentials) can be modified in [EnvSettings.json](EnvSettings.json)
2. Runtime config (browser context) is possible to set up in [specflow.actions.json](specflow.actions.json)

## Running tests
1. Open Powershell console
2. Navigate to dir containing RunTests.ps1 file
3. Run file ['RunTests.ps1'](RunTests.ps1)

## How to access the generated report
Test run result is available in TestResults dir (file ***'AutomatedTestResults.html'***)
[A test run result example](TestResults/AutomatedTestResults.html)

**!!!Caution!!! Test result report contains also infoabout manual test scenarios**
- **their state = 'Other' (marked with grey dot)**

### Running tests using GitHub Actions
Tests can be executed using workflow ['Run tests'](https://github.com/pm90pl/BBlogTests/actions/workflows/RunTests.yml).

**!!!Caution!!! It was not possible to filter out manual test scenarios - they will be reported as failed**

Test run report can be accessed from the workflow run artifacts (zip file TestRunResults.zip)
- only manual trigger was defined

## Tools
- .NET6 (C#10)
- SpecFlow (BDD)
- SpecFlow+LivingDoc (report generation)
- FluentAssertions
- Playwright