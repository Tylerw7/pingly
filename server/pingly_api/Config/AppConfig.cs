using DotNetEnv;

namespace pingly_api.Config;

public class AppConfig
{
    public required string DatabaseConnectionString { get; init; }
    public int Port { get; init; } = 8080;

    public static AppConfig Load()
    {
        // Load .env file into environment variables (dev convenience).
        // Docker Compose already injects env vars in production, so this
        // is a no-op there. Env.Load() also doesn't overwrite existing
        // variables, so production values always win.
        Env.TraversePath().Load();

        var databaseUrl = Environment.GetEnvironmentVariable("DATABASE_URL")
            ?? throw new InvalidOperationException(
                "DATABASE_URL is required. Set it in your .env file or environment.");

        var portString = Environment.GetEnvironmentVariable("PORT") ?? "8080";
        if (!int.TryParse(portString, out var port) || port <= 0)
            throw new InvalidOperationException(
                $"PORT must be a positive integer, got: '{portString}'");

        return new AppConfig
        {
            DatabaseConnectionString = ConvertPostgresUrlToNpgsql(databaseUrl),
            Port = port,
        };
    }

    // Neon gives you a URL like postgresql://user:pass@host/db?sslmode=require
    // Npgsql prefers key=value syntax. Convert once, cache the result.
    private static string ConvertPostgresUrlToNpgsql(string url)
    {
        Uri uri;
        try
        {
            uri = new Uri(url);
        }
        catch (UriFormatException ex)
        {
            throw new InvalidOperationException(
                $"DATABASE_URL is not a valid URL: {ex.Message}");
        }

        var userInfo = uri.UserInfo.Split(':', 2);
        var username = Uri.UnescapeDataString(userInfo[0]);
        var password = userInfo.Length > 1 ? Uri.UnescapeDataString(userInfo[1]) : "";
        var database = uri.AbsolutePath.TrimStart('/');
        var host = uri.Host;
        var port = uri.Port > 0 ? uri.Port : 5432;

        // Neon requires SSL. These pool settings are tuned for Neon's
        // serverless nature: keep the pool small and let connections
        // die off quickly so the compute can scale to zero when idle.
        return
            $"Host={host};Port={port};" +
            $"Username={username};Password={password};Database={database};" +
            "SSL Mode=Require;Trust Server Certificate=true;" +
            "Pooling=true;Minimum Pool Size=0;Maximum Pool Size=5;" +
            "Connection Idle Lifetime=60;";
    }
}