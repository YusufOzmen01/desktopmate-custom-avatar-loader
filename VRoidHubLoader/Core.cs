namespace CustomAvatarLoader;

using CustomAvatarLoader.Helpers;
using CustomAvatarLoader.Modules;
using Logging;
using MelonLoader;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using Versioning;

public class Core : MelonMod
{
    protected const string RepositoryName = "YusufOzmen01/desktopmate-custom-avatar-loader";
    
    protected virtual ILogger Logger { get; private set; }

    protected virtual IServiceProvider ServiceProvider { get; private set; }

    protected virtual IEnumerable<IModule> Modules { get; private set; }

    public override void OnInitializeMelon()
    {
        var services = new ServiceCollection();
        ConfigureServices(services);
        ServiceProvider = services.BuildServiceProvider();

        Modules = ServiceProvider.GetServices<IModule>();
        Logger = ServiceProvider.GetService<ILogger>();

        var versionChecker = new GitHubVersionChecker(RepositoryName, Logger);
        var updater = new Updater(RepositoryName, Logger);

        var currentVersion = Assembly.GetExecutingAssembly().GetName().Version?.ToString() ?? "0";

        if (currentVersion == "0")
        {
            Logger.Warn("CurrentVersion is 0, faulty module version?");
        }
        
        var hasLatestVersion = versionChecker.IsLatestVersionInstalled(currentVersion);

        if (!hasLatestVersion)
        {
            updater.ShowUpdateMessageBox();
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

    protected virtual void ConfigureServices(IServiceCollection services)
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