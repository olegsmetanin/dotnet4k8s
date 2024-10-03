using Microsoft.Extensions.Configuration;

class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine($"Environment.OSVersion: '{Environment.OSVersion}'");
        Console.WriteLine($"CWD: '{Directory.GetCurrentDirectory()}'");
        var builder = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
            .AddEnvironmentVariables(prefix: "CONTAINERIZED_APP_");
        var config = builder.Build();
        var message = config["message"];
        Console.WriteLine(message);
    }
}
