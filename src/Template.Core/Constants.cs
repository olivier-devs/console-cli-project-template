namespace Template.Core;

public static class Constants
{
    public const int WaitDebuggerTimeout = 400;

    public static class Environments
    {
        public const string AspNetCoreEnvironment = "ASPNETCORE_ENVIRONMENT";
        public const  string DotNetEnvironment = "DOTNET_ENVIRONMENT";
        public const string Development = "Development";
        public const string Production = "Production";
        public const string Staging = "Staging";
        public const string Qa = "Qa";

        public const int ExitCodeSuccess = 0;
        public const int ExitCodeError = -1;
        public const int ExitCodeCancel = -2;

        public static class Keys
        {
            public const string AppName = "AppName";
            public const string AppVersion = "AppVersion";
        }
    }
}