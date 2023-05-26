namespace NetCoreConf2023.BlogApp.Api
{
    public record UserDetails(string UserName, string Email);

    /// <summary>
    /// This is a Third Party Http-based Service (Non-managed service ) 
    /// </summary>
    /// 
    public interface IUsersService
    {
        Task<UserDetails> GetDetailsFor(int userId);
    }

    public class UsersService : IUsersService
    {
        private readonly HttpClient _httpClient;

        public UsersService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri(configuration.GetSection("UsersService:url").Value);
        }

        public async Task<UserDetails> GetDetailsFor(int userId)
        {
            return await _httpClient.GetFromJsonAsync<UserDetails>($"userDetails/{userId}");
            //... call Http Endpoint
            //return await Task.FromResult(new UserDetails("joliverd", "j.olive.rodriguez@gmail.com"));
        }
    }
}
