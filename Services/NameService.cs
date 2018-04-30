namespace demo_polly.Services
{
    public interface INameService
    {
        string GetNameInfo(string name);
    }

    public class NameService : INameService
    {
        public string GetNameInfo(string name)
        {
            return GetNameInfoCall(name);
        }

        private string GetNameInfoCall(string name) {
            System.Threading.Thread.Sleep(5000);

            return $"your name {name} means you are great";

        }
    }
}