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
