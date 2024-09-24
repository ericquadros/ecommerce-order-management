using Flurl.Http;
using Polly;

namespace EcommerceOrderManagement.Infrastructure.Http;

public class HttpHelper
{
    private readonly string _baseUrl;

    public HttpHelper(string baseUrl)
    {
        _baseUrl = baseUrl;
    }

    public async Task<TResponse> ExecuteAsync<TResponse>(HttpRequestMethod method, string route, object body = null)
    {
        var maxNumberAttempts = 3;
        
        // Configuration of Policy with retries
        var policy = Policy
            .Handle<HttpRequestException>()
            .Or<TaskCanceledException>()
            .WaitAndRetryAsync(
                maxNumberAttempts, // Max Attempts
                retryAttempt => TimeSpan.FromSeconds(5), // Interval between each attempts
                (exception, timeSpan, context) =>
                {
                    Console.WriteLine($"Tentativa falhou. Aguardando {timeSpan} antes da próxima tentativa. Erro: {exception.Message}");
                });

        try
        {
            return await policy.ExecuteAsync(async () =>
            {
                var url = $"{_baseUrl}{route}";
                IFlurlRequest request = url.WithTimeout(TimeSpan.FromSeconds(10)); // Timeout de 10 segundos

                return method switch
                {
                    HttpRequestMethod.GET => await request.GetJsonAsync<TResponse>(),
                    HttpRequestMethod.POST => await request.PostJsonAsync(body).ReceiveJson<TResponse>(),
                    HttpRequestMethod.PUT => await request.PutJsonAsync(body).ReceiveJson<TResponse>(),
                    HttpRequestMethod.DELETE => await request.DeleteAsync().ReceiveJson<TResponse>(),
                    _ => throw new ArgumentException("Método HTTP não suportado")
                };
            });
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro na requisição: {ex.Message}");
            throw; // Re-throw para que o chamador possa tratar o erro
        }
    }
}

public enum HttpRequestMethod
{
    GET,
    POST,
    PUT,
    DELETE
}


