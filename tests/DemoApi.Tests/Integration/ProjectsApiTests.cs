using System.Net;
using System.Net.Http.Json;
using DemoApi.Domain;
using Microsoft.AspNetCore.Mvc.Testing;

namespace DemoApi.Tests.Integration;

[Collection(ApiCollection.Name)]
public class ProjectsApiTests(WebApplicationFactory<Program> factory)
{
    private readonly HttpClient _client = factory.CreateClient();

    [Fact]
    public async Task Create_ReturnsCreatedWithLocation()
    {
        var response = await _client.PostAsJsonAsync("/api/projects", new { name = TestApi.Unique("project") });

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.NotNull(response.Headers.Location);
    }

    [Fact]
    public async Task Create_DuplicateName_ReturnsConflict()
    {
        var name = TestApi.Unique("duplicate");
        (await _client.PostAsJsonAsync("/api/projects", new { name })).EnsureSuccessStatusCode();

        var response = await _client.PostAsJsonAsync("/api/projects", new { name });

        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
    }

    [Fact]
    public async Task Create_EmptyName_ReturnsBadRequest()
    {
        var response = await _client.PostAsJsonAsync("/api/projects", new { name = "" });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Rename_ReturnsOkWithNewName()
    {
        var oldName = TestApi.Unique("rename-old");
        var newName = TestApi.Unique("rename-new");

        var createResponse = await _client.PostAsJsonAsync(
            "/api/projects",
            new { name = oldName });

        createResponse.EnsureSuccessStatusCode();

        var created = await createResponse.Content.ReadFromJsonAsync<Project>();
        Assert.NotNull(created);

        var response = await _client.PutAsJsonAsync(
            $"/api/projects/{created.Id}",
            new { name = newName });

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var renamed = await response.Content.ReadFromJsonAsync<Project>();
        Assert.NotNull(renamed);
        Assert.Equal(newName, renamed.Name);
    }

    [Fact]
    public async Task Rename_DuplicateName_ReturnsConflict()
    {
        var firstName = TestApi.Unique("rename-first");
        var secondName = TestApi.Unique("rename-second");

        var firstResponse = await _client.PostAsJsonAsync(
            "/api/projects",
            new { name = firstName });
        firstResponse.EnsureSuccessStatusCode();

        var first = await firstResponse.Content.ReadFromJsonAsync<Project>();
        Assert.NotNull(first);

        (await _client.PostAsJsonAsync(
            "/api/projects",
            new { name = secondName }))
            .EnsureSuccessStatusCode();

        var response = await _client.PutAsJsonAsync(
            $"/api/projects/{first.Id}",
            new { name = secondName });

        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
    }

    [Fact]
    public async Task Get_Unknown_ReturnsNotFound()
    {
        var response = await _client.GetAsync($"/api/projects/{int.MaxValue}");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}
