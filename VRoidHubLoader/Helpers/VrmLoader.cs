using Il2Cpp;
using Il2CppUniGLTF;
using Il2CppUniVRM10;
using Il2CppUniGLTF;
using UnityEngine;
using ILogger = CustomAvatarLoader.Logging.ILogger;

namespace CustomAvatarLoader.Helpers;

public class VrmLoader
{
    ILogger _logger;

    public VrmLoader(ILogger logger)
    {
        _logger = logger;
    }

    public GameObject LoadVrmIntoScene(string path)
    {
        try
        {
            var data = new GlbFileParser(path).Parse();
            var vrmdata = Vrm10Data.Parse(data);
            if (vrmdata == null)
            {
                _logger.Warn("VRM data is null, assuming it's VRM 0.0 avatar. Starting migration");
                vrmdata = MigrateVrm0to1(data);
                if (vrmdata == null)
                {
                    _logger.Error("VRM migration attempt failed. The avatar file might be corrupt or incompatible.");
                }
                
                _logger.Debug("VRM data migration succeeded!");
            }

            var context = new Vrm10Importer(vrmdata);
            var loaded = context.Load();

            loaded.EnableUpdateWhenOffscreen();
            loaded.ShowMeshes();
            loaded.gameObject.name = "VRMFILE";
            
            return loaded.gameObject;
        }
        catch (Exception ex)
        {
            _logger.Error("Error trying to load the VRM file!", ex);
            return null;
        }
    }

    public Vrm10Data MigrateVrm0to1(GltfData data)
    {
        Vrm10Data vrmdata = null;
        Vrm10Data.Migrate(data, out vrmdata, out _);
        if (vrmdata == null) _logger.Error("VRM migration attempt failed. The avatar file might be corrupt or incompatible.");
        
        return vrmdata;
    }
}