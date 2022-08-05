dotnet test .\BBlog.Tests.csproj --filter "TestCategory=UITest | TestCategory=APITest"
$testRunReport = (Get-ChildItem -Path .\ -Recurse -Filter "TestExecution.json" | Select-Object -First 1).FullName
$testAssembly = (Get-ChildItem -Path .\ -Recurse -Filter "BBlog.Tests.dll" | Select-Object -First 1).FullName

livingdoc test-assembly $testAssembly -t $testRunReport --output .\TestResults\AutomatedTestResults.html