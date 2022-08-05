using System.IO;
using System.Text;
using System.Text.Json;
using BBlog.Tests.AppAbstraction.DtoObjects;

namespace BBlog.Tests.TestRunConfiguration
{
    public class Settings
    {
        public Environment? Environment { get; set; }
        public User? TestUser { get; set; }
        public User? SecondUser { get; set; }

        public static Settings LoadSettings()
        {
            var configContent = File.ReadAllText("EnvSettings.json", Encoding.UTF8);
            return
                 JsonSerializer.Deserialize<Settings>(configContent)!;
        }
    }
}
