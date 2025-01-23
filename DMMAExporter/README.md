# DMMA Exporter

## Setting up Unity

If you don't have the Unity hub installed already, start by installing it from
here: https://unity.com/download

Create a Project with Unity 2022.3 using Unity Hub.

Drag the DMMA Exporter unitypackge into your Unity files window, and
click "Import" when a small popup window appears.

Go to the top left menu, select Tools > DMMA Exporter.

Add the "Desktop Mate Modded Avatar" script onto any model, and you should see
it appear in the DMMA Exporter window.

You can now select the model and press Export. After a moment you should get an
option to save a .dmma file, which you can load into desktop mate with the
avatar loader mod v1.0.5 or later, like you would load any vrm file.

## Getting Started With Development

Start by deleting the DMMAExporter folder from the Unity project, as we
will add the git version instead.

Open an administrator power shell terminal, as creating symbolic links requires
an admin power shell window, don't ask me why.

Run the following command, and replace `%PATH%` with the path the full path to
the assets folder of your Unity project, mine would for example be:
`D:\VTuber Stuff\UniVRM 1.0\Assets`. Also replace `%VALUE%` with the path of the
DMMAExporter folder in the git repository, mine would for example be:
`D:\Modding\DesktopMate\desktopmate-custom-avatar-loader\DMMAExporter\`

```ps
New-Item -Path "%PATH%\DMMAExporter" -ItemType SymbolicLink -Value "%VALUE%"
```
