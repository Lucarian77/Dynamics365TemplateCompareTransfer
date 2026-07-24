# Security policy

## Supported version

Security updates are applied to the latest published version.

| Version | Supported |
|---|---:|
| 1.2026.1.2 | Yes |
| Earlier builds | No |

## Reporting a vulnerability

Use GitHub private vulnerability reporting when it is available for this repository. If private reporting is unavailable, contact the maintainer through a private channel before disclosing technical details.

Do not place credentials, tokens, connection strings, organization URLs, confidential template content, customer data, or security evidence in a public issue.

Include:

- A concise description of the issue
- Affected version
- Reproduction steps using non-sensitive test data
- Expected and actual behaviour
- Potential impact
- Suggested mitigation, if known

## Security boundaries

- The tool uses the permissions of the selected XrmToolBox Dataverse connections.
- It does not bypass Dataverse authorization.
- It does not delete templates.
- It does not log or export raw template content or connection secrets.
- Users remain responsible for validating their source, target, permissions, and business approval before performing a transfer.

