# Privacy

Dynamics 365 Template Compare & Transfer processes document-template records through the XrmToolBox connections selected by the user.

## Data processed

The tool retrieves template names, identifiers, associated tables, document types, language codes, statuses, descriptions, client metadata, modified dates, version diagnostics, and template package content needed for comparison and transfer.

## Local display and export

- Template content is processed in memory to calculate hashes and perform transfers.
- Raw template content is not written to the Activity Log.
- Raw template content and client metadata are not included in CSV evidence exports.
- CSV exports can contain names, identifiers, environment names, statuses, dates, notes, and SHA-256 hashes. Review exported files before sharing them.

## Credentials

The tool uses XrmToolBox-managed connections. It does not intentionally log or export passwords, access tokens, client secrets, or connection strings.

## Network activity

The tool communicates with the source and target Dataverse organizations chosen by the user. No separate telemetry or analytics service is implemented by this project.

