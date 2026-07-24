# Contributing

Contributions and reproducible issue reports are welcome.

## Before opening an issue

- Confirm the issue occurs in the latest version.
- Test with a non-production environment when possible.
- Remove environment names, record identifiers, confidential template names, exported business data, credentials, and tokens.
- Include the XrmToolBox version, Windows version, tool version, and clear reproduction steps.

## Development

1. Fork and clone the repository.
2. Open `Dynamics365TemplateCompareTransfer.sln` in Visual Studio 2019 or later.
3. Restore NuGet packages.
4. Build the Debug configuration.
5. Test against non-production Dataverse environments.
6. Build Release and rerun the relevant regression tests.

## Pull requests

- Keep each pull request focused.
- Explain the user impact and safety implications.
- Add or update documentation when behaviour changes.
- Do not commit binaries, NuGet packages, credentials, connection data, generated exports, or organization-specific test evidence.
- Keep assembly and NuGet package versions aligned for releases.

