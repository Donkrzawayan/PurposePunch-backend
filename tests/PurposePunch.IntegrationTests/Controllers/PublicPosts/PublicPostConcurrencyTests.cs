using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using PurposePunch.Domain.Entities;
using PurposePunch.Infrastructure.Persistence;

namespace PurposePunch.IntegrationTests.Controllers.PublicPosts;

public class PublicPostConcurrencyTests : IClassFixture<IntegrationTestWebAppFactory>
{
    private readonly IntegrationTestWebAppFactory _factory;
    private const int NumberOfRequests = 20;

    public PublicPostConcurrencyTests(IntegrationTestWebAppFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task Upvote_ShouldHandleConcurrentRequests()
    {
        // 1. ARRANGE
        var client = _factory.CreateClient();
        int postId = 1;

        using (var scope = _factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var post = new PublicPost
            {
                OriginalDecisionId = null,
                AuthorNickname = "TestUser",
                Title = "Concurrency Test Post",
                Description = "Testing race conditions",
                PublishedAt = DateTime.UtcNow,
                UpvoteCount = 0
            };
            db.Add(post);
            await db.SaveChangesAsync();

            postId = post.Id;
        }

        var tasks = new Task[NumberOfRequests];

        for (int i = 0; i < NumberOfRequests; i++)
            tasks[i] = SendUpvoteRequest(postId, client);

        async Task SendUpvoteRequest(int postId, HttpClient client)
        {
            var deviceId = Guid.NewGuid().ToString();

            using var requestMessage = new HttpRequestMessage(HttpMethod.Post, $"/api/PublicPosts/{postId}/upvote");
            requestMessage.Headers.Add("X-Device-Id", deviceId);

            var response = await client.SendAsync(requestMessage);
            response.EnsureSuccessStatusCode();
        }

        // 2. ACT
        await Task.WhenAll(tasks);

        // 3. ASSERT
        using (var scope = _factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var post = await db.PublicPosts.FindAsync(postId);

            post!.UpvoteCount.Should().Be(NumberOfRequests);
        }
    }
}
