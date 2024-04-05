using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Serilog.Formatting.Compact;
using Serilog;

namespace Infrastructure.LogConfiguration
{
    public static class LoggerConfigurationExtensions
    {
        public static LoggerConfiguration ConfigureLogging(this LoggerConfiguration configuration)
        {
            return configuration
                .MinimumLevel.Information()
                .MinimumLevel.Override("Microsoft", Serilog.Events.LogEventLevel.Warning)
                .Enrich.WithEnvironmentName()
                .Enrich.WithMachineName()
                .WriteTo.Console(outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm} [{Level}] {Message}{NewLine}{Exception}");

        }


    }
}
