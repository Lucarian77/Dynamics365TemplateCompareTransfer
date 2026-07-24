# Build and package

## Build the tool

1. Open `Dynamics365TemplateCompareTransfer.sln` in Visual Studio 2019 or later.
2. Restore NuGet packages.
3. Select `Release` and `Any CPU`.
4. Run **Build > Rebuild Solution**.
5. Confirm there are no build errors.

Expected DLL:

```text
Dynamics365TemplateCompareTransfer\bin\Release\Dynamics365TemplateCompareTransfer.dll
```

## Build the NuGet package

The XrmToolBox Tool Library distributes tools through NuGet packages.

1. Download `nuget.exe` from the official NuGet site and either place it in the repository root or make it available on `PATH`.
2. Build the project in Release.
3. Open PowerShell in the repository root.
4. Run:

```powershell
.\Build-ReleasePackage.ps1
```

The script:

- Confirms the Release DLL exists.
- Confirms the DLL version is `1.2026.1.2`.
- Packs `Dynamics365TemplateCompareTransfer.nuspec`.
- Confirms the package contains the plugin DLL under `lib/net48/Plugins`.
- Calculates the package SHA-256 hash.

Output:

```text
artifacts\Dynamics365TemplateCompareTransfer.1.2026.1.2.nupkg
```

## Important release rule

The NuGet package version and the DLL assembly version must remain identical. Otherwise, XrmToolBox may continually report that an update is available.

