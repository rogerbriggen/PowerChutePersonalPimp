// Roger Briggen license this file to you under the MIT license.
//

using System;
using System.Diagnostics.Contracts;
using System.Globalization;
using System.IO;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RogerBriggen.PowerChutePersonalPimpLib;
using Serilog;

namespace RogerBriggen.PowerChutePersonalPimp;

class Program
{
    static void Main(string[] args)
    {
        System.Threading.Thread.CurrentThread.Name = "MainThread";

        // Setup log with DI
        var serviceCollection = new ServiceCollection();
        ConfigureServices(serviceCollection);
        var serviceProvider = serviceCollection.BuildServiceProvider();
        var logger = serviceProvider.GetService<Microsoft.Extensions.Logging.ILogger<Program>>();
        if (logger == null)
        {
            throw new Exception("Could not get a logger");
        }
        var assembly = typeof(Program).Assembly;
        logger.LogInformation($"Application with version {assembly.GetName().Version}  ({ThisAssembly.AssemblyInformationalVersion}) started...");


        //Do stuff
        //Scan commandline
        if (args.Length == 2)
        {
            CultureInfo ci = new CultureInfo("");
            if (args[0].ToLower(ci) == "exampleproject")
            {
                try
                {

                    logger.LogInformation($"Example project file succesfully written! {args[1]}");
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, $"Example project file could not be written! {args[1]}");
                }

            }
            else
            {
                ShowHelp();
            }
        }
        else
        {
            ShowHelp();
        }

        var pcwService = serviceProvider.GetService<PowerChuteWatcherService>();
        if (pcwService != null)
        {
            pcwService.InitFromFile("PowerChutePersonalPimp.Config.json");
            pcwService.StartWatching();
        } 
        else
        {
            logger.LogError("Could not initialize PowerChuteWatcherService!");
        }
        

        // Finish log
        logger.LogInformation("Application closing...");

        // Use for Microsoft logger.... for serilog, we do it with AppDomain.CurrentDomain.ProcessExit
        // serviceProvider.Dispose();
    }

    private static void ShowHelp()
    {
        System.Console.WriteLine("exampleproject [projectfile.xml]");
    }

    private static void ConfigureServices(IServiceCollection services)
    {
        // we will configure logging here
        // Microsoft logger
        /*
        services.AddLogging(configure => configure.AddConsole(consoleLoggerOptions => consoleLoggerOptions.TimestampFormat = "[dd.MM.yyyy HH:mm:ss.fff]")).Configure<LoggerFilterOptions>(cfg => cfg.MinLevel = LogLevel.Debug).AddTransient<Program>();
        services.AddLogging(configure => configure.AddDebug()).Configure<LoggerFilterOptions>(cfg => cfg.MinLevel = LogLevel.Debug).AddTransient<Program>();
        */

        // Setup Serilog
        if (File.Exists("appsettings.json"))
        {
            // Serilog logger from file
            var configuration = new ConfigurationBuilder()
                                    .AddJsonFile("appsettings.json")
                                    .Build();
            services.AddSerilogServices(new LoggerConfiguration()
                                         .ReadFrom.Configuration(configuration));

            services.AddLogging(loggingBuilder => loggingBuilder.AddSerilog(dispose: true));

            // Get application stuff
            // var RemoteIncrediBuildSec = configuration.GetSection("RemoteIncrediBuild");
            /*
            if (RemoteIncrediBuildSec != null)
            {
                bool? bIsServer = RemoteIncrediBuildSec.GetValue<bool?>("IsServer");
                if (bIsServer == true)
                {
                    ConfigFileAppSettings.Action = AppSettings.EAction.Server;
                }
                else if (bIsServer == false)
                {
                    ConfigFileAppSettings.Action = AppSettings.EAction.Client;
                }

            }
            */
        }
        else
        {
            // Serilog logger from code
            string outputTemplate = "[{Timestamp:dd.MM.yyyy HH:mm:ss.ffff} {Level:u3}] {Message:lj} {Properties}[{ThreadId} {ThreadName}]{NewLine}{Exception}";
            services.AddSerilogServices(new LoggerConfiguration()
                                             .Enrich.WithThreadId()
                                             .Enrich.WithThreadName()
                                             .MinimumLevel.Debug()
                                             .WriteTo.Console(outputTemplate: outputTemplate, restrictedToMinimumLevel: Serilog.Events.LogEventLevel.Information)
                                             .WriteTo.Async(w => w.File("MyLog.log", rollingInterval: RollingInterval.Day, outputTemplate: outputTemplate))
                                             .WriteTo.Debug(outputTemplate: outputTemplate));

            services.AddLogging(loggingBuilder => loggingBuilder.AddSerilog(dispose: true));
        }
        services.AddTransient<PowerChuteWatcherService>();


        Log.Information("**** Opening log... ****");
    }
}

#pragma warning disable SA1402 // File may only contain a single type
public static class MyExtensions
#pragma warning restore SA1402 // File may only contain a single type
{
    public static IServiceCollection AddSerilogServices(this IServiceCollection services, LoggerConfiguration configuration)
    {
        Contract.Requires(configuration != null);
        Log.Logger = configuration!.CreateLogger();
        AppDomain.CurrentDomain.ProcessExit += (s, e) => { Log.Information("**** Closing log... ****"); Log.CloseAndFlush(); };
        return services.AddSingleton(Log.Logger);
    }
}
