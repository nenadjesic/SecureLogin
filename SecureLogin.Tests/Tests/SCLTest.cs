using System.Net;
using System.Net.Http.Json;
using SecureLogin.Model;

namespace SecureLogin.Tests.Tests;

public class SclTests : IntegrationTestBase
{
    public SclTests(MsSqlContainerFixture fixture) : base(fixture)
    {
    }

    [Fact]
    public async Task AddUser_ReturnsCreated()
    {
        using var client = CreateClient();
        client.DefaultRequestHeaders.Add("X-API-KEY", "F91BDAB9-14FC-40FF-B9E8-7ACE33EE4EE2");

        var newUser = new User
        {
            UserName = "test",
            FullName = "Test User",
            Email = "t1@nenadjesic.com",
            Password = "1234rewq",
            MobileNumber = "123123",
            Language = "Slovenian",
            Culture = "sl-SI"
        };

        var response = await client.PostAsJsonAsync("/api/users", newUser);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }

    [Fact]
    public async Task Update_User_Data()
    {
        using var client = CreateClient();
        client.DefaultRequestHeaders.Add("X-API-KEY", "F91BDAB9-14FC-40FF-B9E8-7ACE33EE4EE2");

        var user = new User
        {
            UserName = "test3",
            Password = "1234rewq",
            Email = "t3@nenadjesic.com",
            FullName = "Test User",
            Language = "Slovenian",
            Culture = "sl-SI"
        };
        var createRes = await client.PostAsJsonAsync("/api/users", user);
        createRes.EnsureSuccessStatusCode();
        var created = await createRes.Content.ReadFromJsonAsync<User>();

        created.FullName = "New Name";
        var updateRes = await client.PutAsJsonAsync($"/api/users/{created.Id}", created);

        Assert.Equal(HttpStatusCode.NoContent, updateRes.StatusCode);
    }

    [Fact]
    public async Task Delete_User_Test()
    {
        using var client = CreateClient();
        client.DefaultRequestHeaders.Add("X-API-KEY", "F91BDAB9-14FC-40FF-B9E8-7ACE33EE4EE2");

        var user = new User
        {
            UserName = "test2",
            Password = "1234rewq",
            Email = "t2@nenadjesic.com",
            FullName = "Test User",
            Language = "Slovenian",
            Culture = "sl-SI"
        };
        var createRes = await client.PostAsJsonAsync("/api/users", user);
        createRes.EnsureSuccessStatusCode();
        var created = await createRes.Content.ReadFromJsonAsync<User>();

        var deleteRes = await client.DeleteAsync($"/api/users/{created.Id}");
        Assert.Equal(HttpStatusCode.OK, deleteRes.StatusCode);

        var getRes = await client.GetAsync($"/api/users/{created.Id}");
        Assert.Equal(HttpStatusCode.NotFound, getRes.StatusCode);
    }

    [Fact]
    public async Task Validate_Password_Success_And_Failure()
    {
        using var client = CreateClient();
        client.DefaultRequestHeaders.Add("X-API-KEY", "F91BDAB9-14FC-40FF-B9E8-7ACE33EE4EE2");

        var user = new User
        {
            UserName = "login_test",
            Password = "SecretPassword",
            Email = "t3@nenadjesic.com",
            FullName = "Test User",
            Language = "Slovenian",
            Culture = "sl-SI"
        };
        await client.PostAsJsonAsync("/api/users", user);


        var validLogin = new { UserName = "login_test", Password = "SecretPassword" };
        var resOk = await client.PostAsJsonAsync("/api/users/validate", validLogin);
        Assert.Equal(HttpStatusCode.OK, resOk.StatusCode);


        var invalidLogin = new { UserName = "login_test", Password = "WrongPassword" };
        var resFail = await client.PostAsJsonAsync("/api/users/validate", invalidLogin);
        Assert.Equal(HttpStatusCode.Unauthorized, resFail.StatusCode);
    }

    [Fact]
    public async Task Unauthorized_When_ApiKey_Is_Wrong()
    {
        using var client = CreateClient();
        client.DefaultRequestHeaders.Add("X-API-KEY", "wrong-key");

        var response = await client.GetAsync("/users/any-id");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }
}
