﻿namespace CustomAvatarLoader;

using CustomAvatarLoader.Chara;
using CustomAvatarLoader.Helpers;
using CustomAvatarLoader.Modules;
using Logging;
using MelonLoader;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using UnityEngine;
using Versioning;

public class Core : MelonMod
{
    private const string RepositoryName = "YusufOzmen01/desktopmate-custom-avatar-loader";
    private bool _init;
    
    private Logging.ILogger Logger { get; set; }

    private GitHubVersionChecker VersionChecker { get; set; }
    
    private Updater Updater { get; set; }
    
    private CharaLoader CharaLoader { get; set; }

    private string CurrentVersion { get; set; }

    protected IServiceProvider ServiceProvider { get; private set; }

    protected IEnumerable<IModule> Modules { get; private set; }

    public override void OnInitializeMelon()
    {
        var services = new ServiceCollection();
        ConfigureServices(services);
        ServiceProvider = services.BuildServiceProvider();

        Modules = ServiceProvider.GetServices<IModule>();

        CurrentVersion = Assembly.GetExecutingAssembly().GetName().Version?.ToString() ?? "0";

        Logger = new MelonLoaderLogger(LoggerInstance);
        VersionChecker = new GitHubVersionChecker(RepositoryName, Logger);
        Updater = new Updater(RepositoryName, Logger);

        if (CurrentVersion == "0")
            Logger.Warn("CurrentVersion is 0, faulty module version?");
        
        var hasLatestVersion = VersionChecker.IsLatestVersionInstalled(CurrentVersion);

        if (!hasLatestVersion)
        {
            Updater.ShowUpdateMessageBox();
        }
        else
        {
            Logger.Info("[VersionCheck] Latest version installed");
        }

        foreach (var module in Modules)
        {
            module.OnInitialize();
        }
    }

    private void ConfigureServices(IServiceCollection services)
    {
        services.AddSingleton(typeof(MelonLogger.Instance), LoggerInstance);
        services.AddScoped(typeof(Logging.ILogger), typeof(MelonLoaderLogger));
        services.AddScoped(typeof(IModule), typeof(VrmLoaderModule));
    }

    public override void OnUpdate()
    {
        foreach (var service in Modules)
        {
            service.OnUpdate();
        }
    }
}