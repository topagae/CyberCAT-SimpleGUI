using System.Linq;
using System.Reflection;
using System.Windows.Forms;

namespace CP2077SaveEditor.Utils;

public static class AppInfo
{
    /// <summary>
    /// The version shown to users, like "0.29c". Set by DisplayVersion in CP2077SaveEditor.csproj.
    /// Kept apart from InformationalVersion, which WolvenKit parses as strict SemVer.
    /// </summary>
    public static string DisplayVersion { get; } =
        typeof(AppInfo).Assembly.GetCustomAttributes<AssemblyMetadataAttribute>()
            .FirstOrDefault(a => a.Key == "DisplayVersion")?.Value
        ?? Application.ProductVersion;
}
