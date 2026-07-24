# Release checklist

## Code and testing

- [x] Version is `1.2026.1.2` in assembly metadata and About.
- [x] Author is `Adrian Lucaci`.
- [x] No prohibited branding remains.
- [x] Debug build passes.
- [x] Word-template copy passes.
- [x] Excel-template transfer passes.
- [x] Existing-template update passes.
- [x] Post-write verification passes.
- [x] Automatic refresh passes.
- [x] Search by name, status, hash, and record ID passes.
- [x] Status filters pass.
- [x] Mixed-selection validation passes.
- [x] Same-environment blocking passes.
- [x] Duplicate blocking passes.
- [x] Dry-run scenarios pass.
- [x] Newer-target warning passes.
- [x] Copy Log and Clear Log pass.
- [x] Cancellation and rejected confirmation pass.
- [x] Connection-failure handling passes.
- [x] CSV formula-injection escaping passes.
- [x] High-contrast selection passes.

## GitHub

- [ ] Create public repository `Lucarian77/Dynamics365TemplateCompareTransfer`.
- [ ] Use `main` as the default branch.
- [ ] Upload the release-kit contents.
- [ ] Confirm README icon and links render correctly.
- [ ] Enable Issues.
- [ ] Enable private vulnerability reporting if available.
- [ ] Add repository topics from `XrmToolBox-LISTING.md`.
- [ ] Create GitHub release tag `v1.2026.1.2`.
- [ ] Paste `RELEASE_NOTES.md` into the GitHub release.

## Release build and NuGet

- [ ] Rebuild the Release configuration.
- [ ] Confirm zero build errors.
- [ ] Run `Build-ReleasePackage.ps1`.
- [ ] Confirm package validation passes.
- [ ] Record the generated `.nupkg` SHA-256 value.
- [ ] Inspect the package with NuGet Package Explorer.
- [ ] Confirm the DLL is under `lib/net48/Plugins`.
- [ ] Confirm package and assembly versions match.
- [ ] Confirm icon, licence, project URL, dependency, tags, and release notes.
- [ ] Publish the `.nupkg` to NuGet.org.

## XrmToolBox portal

- [ ] Sign in to the XrmToolBox portal.
- [ ] Register the published NuGet package.
- [ ] Use the listing text in `XrmToolBox-LISTING.md`.
- [ ] Submit the tool for validation.
- [ ] Install the approved package from Tool Library on a clean XrmToolBox instance.
- [ ] Run a final non-production smoke test.

