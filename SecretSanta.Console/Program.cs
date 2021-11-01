using System;
using System.Diagnostics;
using McMaster.Extensions.CommandLineUtils;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SecretSanta.Console;
using SecretSanta.Domain;

await CreateHostBuilder (args).RunCommandLineApplicationAsync (
        args,
        (app) =>
        {
            app.HelpOption();
            var santasPathOption = app.Option ("-f|--file <SUBJECT>", "Path to csv file containing the santas", CommandOptionType.SingleValue)
                    .IsRequired();
            var debugOption = app.Option<bool> ("-d|--debug", "Launches a debugger when the application starts", CommandOptionType.NoValue);

            app.OnExecuteAsync (
                    async cancellationToken =>
                    {
                        if (debugOption.ParsedValue)
                            Debugger.Launch();
                        
                        var secretSantaApp = app.GetRequiredService<SecretSantaApp>();
                        await secretSantaApp.Run (santasPathOption.Value(), cancellationToken);
                    });
        });

IHostBuilder CreateHostBuilder (string[] args) =>
        Host.CreateDefaultBuilder (args)
                .ConfigureLogging (
                        (context, builder) =>
                        {
                            builder.ClearProviders();
                            builder.AddConsole();
                        })
                .ConfigureServices (
                        (context, services) =>
                        {
                            services.AddTransient<SecretSantaApp>();
                            services.AddTransient<SecretSantaService>();
                        })
                .ConfigureAppConfiguration (
                        (context, builder) =>
                        {
                            if (context.HostingEnvironment.IsDevelopment())
                            {
                                builder.AddUserSecrets<SecretSantaApp>();
                            }
                        });