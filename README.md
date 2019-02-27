# Apkd.UnityDemystifier

Improved stack trace display for Unity, based on [Ben.Demystifier](https://github.com/benaadams/Ben.Demystifier). Work in progress.

#### Before
![before](https://cdn.discordapp.com/attachments/368334636256067597/550024165277696010/before.png)

#### After
![after](https://cdn.discordapp.com/attachments/368334636256067597/550024160558972929/after.png)

## Installation (automatic)

Add a reference to the repository inside the `Packages\manifest.json` file in your project directory:

```json
{
  "dependencies": {
    "com.unity.postprocessing": "2.1.3",
    ...
    "pl.apkd.demystifier": "https://github.com/apkd/Apkd.UnityDemystifier.git"
  }
}
```

## Installation (manual)

Clone/download this repository into the `Assets` directory of your project.

## Compilation symbols

You can customize a few options by defining the following symbols in the project settings:
- `APKD_STACKTRACE_NOFORMAT` - Disables stack trace font formatting. Useful if you prefer your copy-pasted stack traces and log files to be clean.
- `APKD_STACKTRACE_SHOWPARAMS` - Shows the parameter list in the method signature. You can enable this if you have an unnaturally wide monitor.

## Notes

- This package overwrites Unity's built-in stack trace parsing in an unsupported way and can make your editor unstable. All feedback is appreciated.
- Only tested on the 2018.3 (Windows/x86-64) version of the editor.
- This package is editor-only and won't be included in builds.
- Quick benchmarks have shown performance that is competitive or even better than Unity's built-in stack parsing.
