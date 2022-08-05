using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace BBlog.Tests.Utils;

public static class IOHelpers
{
    public static string? GetRootProjectDir()
    {
        var dir = Directory.GetCurrentDirectory();
        FileInfo? projectFileInfo = null;
        while (projectFileInfo is null && dir is not null)
        {
            var parent = Directory.GetParent(dir);
            dir = parent?.FullName;
            projectFileInfo = parent?
                .GetFiles("BBlog.Tests.csproj", SearchOption.TopDirectoryOnly)
                .FirstOrDefault();
        }
        return dir;
    }
}