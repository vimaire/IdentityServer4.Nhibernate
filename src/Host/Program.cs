using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;

namespace Host
{
    public class Program
    {
        public static void Main(string[] args)
        {
            HibernatingRhinos.Profiler.Appender.NHibernate.NHibernateProfiler.Initialize();
            var host = BuildWebHost(args);
            host.Run();
        }

        private static IWebHost BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .Build();
    }
}