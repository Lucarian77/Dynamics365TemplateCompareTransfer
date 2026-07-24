# Publishing guide

## 1. Create the GitHub repository

Create a public repository:

```text
https://github.com/Lucarian77/Dynamics365TemplateCompareTransfer
```

Recommended settings:

- Repository name: `Dynamics365TemplateCompareTransfer`
- Description: `XrmToolBox tool to compare and safely transfer Word and Excel document templates between Dataverse environments.`
- Visibility: Public
- Do not generate another README, licence, or `.gitignore`; they are included here.
- Default branch: `main`

Upload the contents of this release-kit folder to the repository root.

Recommended topics:

```text
xrmtoolbox
dataverse
dynamics-365
power-platform
document-templates
alm
deployment
migration
csharp
dotnet-framework
```

## 2. Build Release

In Visual Studio:

1. Select `Release`.
2. Run **Clean Solution**.
3. Run **Rebuild Solution**.
4. Confirm zero errors.

## 3. Build the NuGet package

From PowerShell in the repository root:

```powershell
.\Build-ReleasePackage.ps1
```

Review the generated package in NuGet Package Explorer before publishing.

## 4. Publish to NuGet.org

Publish:

```text
artifacts\Dynamics365TemplateCompareTransfer.1.2026.1.2.nupkg
```

Do not upload the source ZIP in place of the `.nupkg`.

## 5. Create the GitHub release

- Tag: `v1.2026.1.2`
- Title: `Dynamics 365 Template Compare & Transfer 1.2026.1.2`
- Notes: copy from `RELEASE_NOTES.md`
- Optional asset: attach the published `.nupkg`

## 6. Register in XrmToolBox

Sign in to the XrmToolBox portal and register the NuGet package after it is visible on NuGet.org. Use `XrmToolBox-LISTING.md` for the title, description, tags, release notes, URLs, and version details.

The XrmToolBox publishing documentation requires:

- A valid NuGet package
- Package and tool assembly versions that match
- An `XrmToolBox` minimum-version dependency
- A unique icon
- A project URL
- Useful release notes and tags

## 7. Final clean-install test

After approval:

1. Remove the manually copied test DLL from the XrmToolBox `Plugins` folder.
2. Restart XrmToolBox.
3. Install the tool from Tool Library.
4. Restart XrmToolBox if requested.
5. Open the tool and run a non-production comparison.
6. Confirm name, author, version, icon, source/target selection, comparison, dry run, and one controlled verified transfer.

