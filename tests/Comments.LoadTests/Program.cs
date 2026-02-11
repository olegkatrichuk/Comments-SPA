using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using NBomber.CSharp;
using NBomber.Http.CSharp;

var baseUrl = args.Length > 0 ? args[0] : "http://localhost:5000";

using var httpClient = new HttpClient { BaseAddress = new Uri(baseUrl) };

// Scenario 1: Read comments (paginated)
var readScenario = Scenario.Create("read_comments", async context =>
    {
        var page = Random.Shared.Next(1, 100);
        var request = Http.CreateRequest("GET", $"/api/comments?page={page}&pageSize=25")
            .WithHeader("Accept", "application/json");

        var response = await Http.Send(httpClient, request);
        return response;
    })
    .WithWarmUpDuration(TimeSpan.FromSeconds(5))
    .WithLoadSimulations(
        Simulation.Inject(rate: 100, interval: TimeSpan.FromSeconds(1), during: TimeSpan.FromMinutes(3))
    );

// Scenario 2: Create comments (high load)
var writeScenario = Scenario.Create("create_comments", async context =>
    {
        var userName = $"LoadUser{context.ScenarioInfo.ThreadNumber}_{Random.Shared.Next(10000)}";
        var payload = new
        {
            userName,
            email = $"{userName}@loadtest.com",
            text = $"Load test comment {context.InvocationNumber} from thread {context.ScenarioInfo.ThreadNumber}",
            captchaKey = "load-test-bypass",
            captchaAnswer = "bypass"
        };

        var content = new MultipartFormDataContent
        {
            { new StringContent(payload.userName), "userName" },
            { new StringContent(payload.email), "email" },
            { new StringContent(payload.text), "text" },
            { new StringContent(payload.captchaKey), "captchaKey" },
            { new StringContent(payload.captchaAnswer), "captchaAnswer" }
        };

        var request = Http.CreateRequest("POST", "/api/comments")
            .WithBody(content);

        var response = await Http.Send(httpClient, request);
        return response;
    })
    .WithWarmUpDuration(TimeSpan.FromSeconds(5))
    .WithLoadSimulations(
        Simulation.Inject(rate: 50, interval: TimeSpan.FromSeconds(1), during: TimeSpan.FromMinutes(3))
    );

// Scenario 3: Search comments
var searchTerms = new[] { "comment", "hello", "world", "test", "reply", "user", "example" };
var searchScenario = Scenario.Create("search_comments", async context =>
    {
        var term = searchTerms[Random.Shared.Next(searchTerms.Length)];
        var request = Http.CreateRequest("GET", $"/api/search?q={term}&page=1&pageSize=25")
            .WithHeader("Accept", "application/json");

        var response = await Http.Send(httpClient, request);
        return response;
    })
    .WithWarmUpDuration(TimeSpan.FromSeconds(5))
    .WithLoadSimulations(
        Simulation.Inject(rate: 80, interval: TimeSpan.FromSeconds(1), during: TimeSpan.FromMinutes(3))
    );

NBomberRunner
    .RegisterScenarios(readScenario, writeScenario, searchScenario)
    .WithReportFileName("load_test_report")
    .WithReportFolder("reports")
    .Run();

Console.WriteLine("\nLoad test completed. Report saved to reports/load_test_report.html");
