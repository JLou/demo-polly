using Polly;
using Polly.Timeout;
using System.Net.Http;
using System.Threading.Tasks;
using System;
using Polly.CircuitBreaker;
using Polly.Fallback;

namespace demo_polly.Services
{
    public interface INameService
    {
        string GetNameInfo(string name);
        Task<string> GetNameInfoAsync(string name);

    }

    public class NameService : INameService
    {
        private Policy timeoutPolicy;
        private FallbackPolicy<string> fallbackPolicy, fallbackForCircuitBreaker;
        private Policy<string> wrapPolicy, breakerWrap, fallbackForAnyException;

        private Policy breaker;

        private HttpClient client = new HttpClient
        {
            Timeout = TimeSpan.FromSeconds(1)
        };

        private const string fallbackMessage = "Impossible to get the signification at the moment";

        public NameService()
        {
            timeoutPolicy = Policy.Timeout(2, TimeoutStrategy.Pessimistic);
            fallbackPolicy = Policy<string>
                .Handle<TimeoutRejectedException>()
                .Fallback(fallbackMessage);
            wrapPolicy = fallbackPolicy.Wrap(timeoutPolicy);

            breaker = Policy
                .Handle<Exception>()
                .CircuitBreakerAsync(2, TimeSpan.FromMinutes(1),
                (exception, timeout) =>
                {
                    Console.WriteLine("================================= I broke =================================");
                    return;
                },
                () => Console.WriteLine("I reset"));

            fallbackForCircuitBreaker = Policy<string>
                .Handle<BrokenCircuitException>()
                .FallbackAsync("Service unvailable at the moment [broken circuit]");

            fallbackForAnyException = Policy<string>
                .Handle<Exception>()
                .FallbackAsync(fallbackMessage);

            breakerWrap = fallbackForAnyException.WrapAsync(fallbackForCircuitBreaker.WrapAsync(breaker));
        }

        public string GetNameInfo(string name) => wrapPolicy.Execute(() => GetNameInfoCall(name));

        private string GetNameInfoCall(string name)
        {
            System.Threading.Thread.Sleep(5000);

            return $"your name {name} means you are great";

        }

        public async Task<string> GetNameInfoAsync(string name)
        {
                var a = await breakerWrap.ExecuteAsync(() => GetNameInfoCallAsync(name));
                return a;
        }

        private async Task<string> GetNameInfoCallAsync(string name)
        {
            try
            {
                var response = await client.GetStringAsync("http://google.com:81");
                return response;
            }
            catch (Exception e)
            {
                throw e;
            }

        }
    }
}