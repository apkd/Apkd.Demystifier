# Apkd.UnityDemystifier

Improved stack trace display for Unity, based on [Ben.Demystifier](https://github.com/benaadams/Ben.Demystifier). Work in progress.

#### Before
![before](https://cdn.discordapp.com/attachments/368334636256067597/550672599319969822/before.png)

#### After
![after](https://cdn.discordapp.com/attachments/368334636256067597/550672619532320778/after.png)

## Requirements

- The [.NET 4.X Scripting Runtime](https://docs.unity3d.com/Manual/ScriptingRuntimeUpgrade.html) needs to be enabled in the project.
- Only tested on the 2018.3 (Windows/x86_64) version of the editor.

## Installation

### Unity Package Manager

Add a reference to the repository in the [`Packages\manifest.json`](https://docs.unity3d.com/Packages/com.unity.package-manager-ui@1.8/manual/index.html#project-manifests) file in your project directory:

```json
{
  "dependencies": {
    "com.unity.postprocessing": "2.1.3",
    "pl.apkd.demystifier": "https://github.com/apkd/Apkd.UnityDemystifier.git"
  }
}
```


### Manual

Clone/download this repository into the `Assets` directory of your project.

## Configuration

You can customize the stack trace by defining the following compilation symbols in the project settings:
- `APKD_STACKTRACE_HIDEPARAMS` - Hide the method parameter list completely. The most compact option. 
- `APKD_STACKTRACE_SHORTPARAMS` - Display the parameter list compactly (first letter of each parameter name only).
- `APKD_STACKTRACE_FULLPARAMS` - Shows an expanded parameter list in the method signature. You can enable this if you have an unnaturally wide monitor.
- `APKD_STACKTRACE_NOFORMAT` - Disables stack trace font formatting. Useful if you prefer your copy-pasted stack traces and log files to be clean.
- `APKD_STACKTRACE_DISABLE` - Restores default Unity stack traces.
- `APKD_STACKTRACE_FILEPATH_FONTSIZE_XX` - Change the font size of the source filename/line number string to `XX` (available values: 7-11, default: 8).
- `APKD_STACKTRACE_NOCACHE` - Disable caching. Less memory usage, higher performance impact.
- `APKD_STACKTRACE_LAMBDAORDINALS` - Shows lambda expression ordinals.

## Notes

- This package overrides Unity's built-in stack trace parsing in an unsupported way and can make your editor unstable. All feedback is appreciated.
- This package is editor-only and won't be included in builds.
