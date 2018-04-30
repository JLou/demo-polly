using Polly;
using Polly.Timeout;

namespace demo_polly.Services
{
    public interface INameService
    {
        string GetNameInfo(string name);
    }

    public class NameService : INameService
    {
        private Policy timeoutPolicy;
        private Policy<string> fallbackPolicy;
        private Policy<string> wrapPolicy;
        public NameService()
        {
            timeoutPolicy = Policy.Timeout(2, TimeoutStrategy.Pessimistic);
            fallbackPolicy = Policy<string>
                .Handle<TimeoutRejectedException>()
                .Fallback("Impossible to get the signification at the moment");

            wrapPolicy = fallbackPolicy.Wrap(timeoutPolicy);
        }

        public string GetNameInfo(string name)
        {
            return wrapPolicy.Execute(() => GetNameInfoCall(name));
        }

        private string GetNameInfoCall(string name)
        {
            System.Threading.Thread.Sleep(5000);

            return $"your name {name} means you are great";

        }
    }
}