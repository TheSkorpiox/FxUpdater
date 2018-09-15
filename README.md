# FxUpdater

Fx server updater, for FiveM

## Getting Started

Update take the distribution as first parameter (windows or linux) and can either take an absolute path or a relative path as second parameter. It can also create the directory if needed (in case of a fresh install)

```
/update windows C:\users\MyUser\Desktop\MyServer - Update at the specified path
/update linux MyServer - Update next to the executable
```

You can also clear the console

```
/clear - Clear the console
```

## Patchnotes / Releases

### Version 0.3

You can now use bash command line.

For exemple, instead of doing
```
FxUpdater.exe
/update windows [YourPath]
```

you can do this
```
FxUpdater.exe update windows [YourPath]
```

### Version 0.4

FxUpdater is now running under .Net Core Runtime, for cross-platform compatibility !

You do need to download the .Net Core Runtime in order to run FxUpdater
You can found it [here](https://www.microsoft.com/net/download/dotnet-core/2.0)

You'll also need to change the way you launch FxUpdater. There is no more executable !

```
dotnet FxUpdater.dll update linux [YourPath]
```

It has been tester on Debian 9, no more. But i believe if there is an issue with your distribution,
it's directly related to .Net Core support. No not report me these issues please.