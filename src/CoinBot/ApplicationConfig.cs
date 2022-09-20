using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;

namespace CoinBot;

/// <summary>
///     Locator of the application configuration
/// </summary>
[SuppressMessage(category: "ReSharper", checkId: "UnusedType.Global", Justification = "Used in exe code. Not possible to unit test.")]
public static class ApplicationConfig
{
    /// <summary>
    ///     The base path of the folder with the configuration files in them.
    /// </summary>
    [SuppressMessage(category: "ReSharper", checkId: "UnusedMember.Global", Justification = "Used in exe code. Not possible to unit test.")]
    public static string ConfigurationFilesPath { get; } = LookupConfigurationFilesPath();

    private static string LookupConfigurationFilesPath()
    {
        string? path = LookupAppSettingsLocationByAssemblyName();

        if (path == null)
        {
            // https://stackoverflow.com/questions/57222718/how-to-configure-self-contained-single-file-program
            return Environment.CurrentDirectory;
        }

        return path;
    }

    private static string? LookupAppSettingsLocationByAssemblyName()
    {
        string location = AppContext.BaseDirectory;

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
}