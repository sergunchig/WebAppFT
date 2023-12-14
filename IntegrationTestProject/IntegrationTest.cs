using Microsoft.AspNetCore.Mvc.Testing;
using System;
using System.Net;
using System.Net.Http.Json;
using WebAppFT.Model;
using Xunit;

namespace IntegrationTestProject
{
    public class IntegrationTest:IClassFixture<CustomWebApplicationFactory<Program>>
    {
        private CustomWebApplicationFactory<Program> _factory;

        public IntegrationTest(CustomWebApplicationFactory<Program> factory) {
            _factory = factory;
        }

        [Theory]
        [InlineData("/WeatherForecast")]
        public async Task GetQueryTest(string url)
        {
            var client = _factory.CreateClient();

            var response = await client.GetAsync(url);

            response.EnsureSuccessStatusCode();
            Assert.Equal("application/json; charset=utf-8",
                response.Content?.Headers?.ContentType?.ToString());
        }
        [Theory]
        [InlineData("/WeatherForecast")]
        public async Task ResponseObjectTest(string url)
        {
            var client = _factory.CreateClient();

            var response = await client.GetAsync(url);
            //var str = await response.Content.ReadAsStringAsync();

            var obj = await response.Content.ReadFromJsonAsync<IEnumerable<WeatherForecast>>();
            response.EnsureSuccessStatusCode();
            Assert.True(obj!.Count() != 0);
            Assert.Contains(obj!, x => x.TemperatureC > 0);
        }
        [Fact]
        public async void PostQueryTest() 
        {
            HttpClient client = _factory.CreateClient();
            JsonContent content = JsonContent.Create(new WeatherForecast());
            var response = await client.PostAsync("/WeatherForecast", content);

            response.EnsureSuccessStatusCode();

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }
    }
}