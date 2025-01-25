namespace CustomAvatarLoader.Helpers;

#if MELON
using Il2CppUniGLTF;
using Il2CppUniVRM10;
#endif
#if BEPINEX
using UniGLTF;
using UniVRM10;
#endif
using UnityEngine;

public class VrmLoader
{
    public GameObject? LoadVrmIntoScene(string path)
    {
        try
        {
            var data = new GlbFileParser(path).Parse();
            var vrmdata = Vrm10Data.Parse(data);
            if (vrmdata == null)
            {
                Core.Warn("VRM data is null, assuming it's VRM 0.0 avatar. Starting migration");
                vrmdata = MigrateVrm0to1(data);
                if (vrmdata == null)
                {
                    Core.Error("VRM migration attempt failed. The avatar file might be corrupt or incompatible.");
                }
                
                Core.Msg("VRM data migration succeeded!");
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
            Core.Error("Error trying to load the VRM file!\n" + ex);
            return null;
        }
    }

    public Vrm10Data? MigrateVrm0to1(GltfData data)
    {
        Vrm10Data.Migrate(data, out Vrm10Data vrmdata, out _);
        if (vrmdata == null) Core.Error("VRM migration attempt failed. The avatar file might be corrupt or incompatible.");     
        return vrmdata;
    }
}