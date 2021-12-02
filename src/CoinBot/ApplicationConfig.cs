using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;

namespace CoinBot;

/// <summary>
///     Application configuration helpers
/// </summary>
public static class ApplicationConfig
{
    // really should be using AppContext.BaseDirectory, but this seems to break sometimes when running unit tests with dotnet-xuint.

    /// <summary>
    ///     The base path of the folder with the configuration files in them.
    /// </summary>
    public static string ConfigurationFilesPath { get; } = LookupConfigurationFilesPath();

    private static string LookupConfigurationFilesPath()
    {
        string? path = LookupAppSettingsLocationByAssemblyName();

        if (path == null)
        {
            // https://stackoverflow.com/questions/57222718/how-to-configure-self-contained-single-file-program
            path = Environment.CurrentDirectory;
        }

        Console.WriteLine($"Loading Appsettings from {path}");

        return path;
    }

    private static string? LookupAppSettingsLocationByAssemblyName()
    {
        string location = AppLocation();

        string? path = Path.GetDirectoryName(location);

        if (string.IsNullOrWhiteSpace(path))
        {
            return null;
        }

        if (!File.Exists(Path.Combine(path1: path, path2: @"appsettings.json")))
        {
            return null;
        }

        return path;
    }

    [SuppressMessage(category: "FunFair.CodeAnalysis", checkId: "FFS0008: Don't disable warnings", Justification = "TODO: Review")]
    private static string AppLocation()
    {
#pragma warning disable IL3000
        return typeof(ApplicationConfig).Assembly.Location;
#pragma warning restore IL3000
    }
}