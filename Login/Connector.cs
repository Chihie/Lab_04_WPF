using System.Configuration;

namespace Login
{
    public static class Connector
    {
        public static string ConnValue(string name) =>
            ConfigurationManager.ConnectionStrings[name].ConnectionString;
    }
}