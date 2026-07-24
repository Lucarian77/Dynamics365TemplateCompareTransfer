# Dynamics 365 Template Compare & Transfer

![Tool icon](https://raw.githubusercontent.com/Lucarian77/Dynamics365TemplateCompareTransfer/main/Assets/DocumentsTemplateMoverIcon80.png)

An XrmToolBox tool for comparing and safely transferring Microsoft Word and Excel document templates between Microsoft Dataverse environments.

Version: `1.2026.1.2`  
Author: Adrian Lucaci  
Framework: .NET Framework 4.8

## What it does

- Loads Word and Excel document templates from a source and target environment.
- Compares templates using name, associated table, document type, language, status, metadata, and normalized package content.
- Classifies each template as `Identical`, `Different`, `Source Only`, `Target Only`, or `Duplicate`.
- Copies missing templates without updating existing target records.
- Updates existing templates only through a separate, explicit command.
- Detects when a target template is newer and displays a stronger warning.
- Preserves Draft and Activated status.
- Retrieves and verifies every target record after a write.
- Supports dry-run previews, detailed row inspection, search, filtering, CSV evidence export, and an activity log.

## Safety model

| Comparison status | Copy Missing | Update Existing | Reason |
|---|---:|---:|---|
| Source Only | Yes | No | Creates a missing target template. |
| Different | No | Yes | Updates one unambiguous target match. |
| Identical | No | No | No write is required. |
| Target Only | No | No | No source record is available. |
| Duplicate | No | No | Ambiguous matches are never guessed. |

Additional safeguards:

- The source and target cannot be the same Dataverse organization.
- Nothing is deleted.
- Mixed or invalid selections are rejected.
- A write is counted as successful only after target retrieval and verification pass.
- Raw template content, credentials, connection strings, and tokens are not written to the activity log or CSV export.

## Requirements

- Windows
- XrmToolBox `1.2025.7.71` or later
- Access to both Dataverse environments
- Read access to document templates in the source environment
- Create or update access to document templates in the target environment when transfers are performed

For development:

- Visual Studio 2019 or later
- .NET Framework 4.8 Developer Pack
- NuGet package restore enabled


## Basic use

1. Open XrmToolBox and connect to the source environment.
2. Open **Dynamics 365 Template Compare & Transfer**.
3. Select a separate target connection.
4. Select **Load & Compare**.
5. Review the result counts, filters, details, and target modified dates.
6. Use **Dry Run** before any write.
7. Use **Copy Missing** only for `Source Only` records.
8. Use **Update Existing** only for reviewed `Different` records.
9. Confirm `VERIFIED` in the Activity Log.
10. Export the comparison or operation evidence if required.

## Comparison hashes

Dataverse may use different environment-specific entity type codes inside otherwise equivalent Office template packages. For that reason:

- **Raw hash** identifies the exact stored package bytes.
- **Normalized hash** canonicalizes environment-specific references for comparison.

Two templates can be classified as `Identical` when their normalized hashes and compared metadata match even if their raw package hashes differ.

## Build

1. Open `Dynamics365TemplateCompareTransfer.sln`.
2. Restore NuGet packages.
3. Select the `Release` configuration.
4. Run **Build > Rebuild Solution**.
5. Confirm that the build succeeds with no errors.

The release DLL is generated at:

```text
bin\Release\Dynamics365TemplateCompareTransfer.dll
```

See [BUILDING.md](BUILDING.md) for NuGet packaging and validation.

## Privacy and security

The tool operates through the XrmToolBox connections selected by the user and applies the permissions of those Dataverse connections. It does not bypass Dataverse security.

See [PRIVACY.md](PRIVACY.md) and [SECURITY.md](SECURITY.md).

## Support

Use the GitHub issue tracker for reproducible defects and feature requests. Never include credentials, tokens, connection strings, confidential template content, or sensitive exported evidence in a public issue.

## Licence

Released under the [MIT License](LICENSE).
