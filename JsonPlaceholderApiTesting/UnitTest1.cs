using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Net;
using System.Text;
using FluentAssertions;

namespace JsonPlaceholderApiTesting
{
    public class Tests
    {
        private const string getBlogPostUrl = "https://jsonplaceholder.typicode.com/posts/1";
        private const string postBlogPostUrl = "https://jsonplaceholder.typicode.com/posts";

        readonly BlogPost _expectedBlogContent = new()
        {
            UserId = 1,
            Id = 1,
            Title = "sunt aut facere repellat provident occaecati excepturi optio reprehenderit",
            Body = "quia et suscipit\nsuscipit recusandae consequuntur expedita et cum\nreprehenderit molestiae ut ut quas totam\nnostrum rerum est autem sunt rem eveniet architecto"
        };

        readonly BlogPost _postBlogContent = new()
        {
            UserId = 1,
            Title = "hi",
            Body = "Sheila was here"
        };

        [Test]
        public async Task GetBlogPostReturnsSuccessful()
        {
            using var client = new HttpClient();
            var response = await client.GetAsync(getBlogPostUrl);
            response.Should().BeSuccessful();
        }

        [Test]
        public async Task VerifyGetBlogPostResponseHeader()
        {
            using var client = new HttpClient();
            var response = await client.GetAsync(getBlogPostUrl);
            response.Headers.Connection.Should().Contain("keep-alive");
        }

        [Test]
        public async Task GetBlogPostReturnsTitle()
        {
            using var client = new HttpClient();
            var response = await client.GetAsync(getBlogPostUrl);
            var content = await DeserializeContent(response);
            content.Title.Should().Be(_expectedBlogContent.Title);
        }

        [Test]
        public async Task PostBlogPostReturnsSuccessful()
        {
            using var client = new HttpClient();
            var response = await client.PostAsync(postBlogPostUrl, SerializeContent(_postBlogContent));
            response.StatusCode.Should().Be(HttpStatusCode.Created);
        }

        private async Task<BlogPost> DeserializeContent(HttpResponseMessage response)
        {
            var content = await response.Content.ReadAsStringAsync();
            var contentDeserialized = JsonConvert.DeserializeObject<BlogPost>(content);
            return contentDeserialized;
        }

        private StringContent SerializeContent(BlogPost blog)
        {
            var body = JsonConvert.SerializeObject(blog, Formatting.None);
            var content = new StringContent(body, Encoding.UTF8, "application/json");
            content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/json");
            return content;
        }
    }
}