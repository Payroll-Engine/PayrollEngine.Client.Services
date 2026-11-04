# Workflow: Deploy DocFX to GitHub Pages

Builds the client API reference with [DocFX](https://dotnet.github.io/docfx/) and deploys it to GitHub Pages.

## Trigger

| Event | Condition |
|:--|:--|
| `release: published` | Runs automatically on every published release |
| `workflow_dispatch` | Manual trigger via GitHub Actions UI |

## Steps

| Step | Description |
|:--|:--|
| Checkout | Checks out the repository |
| Setup .NET | Installs .NET 10 SDK |
| Configure GitHub Packages | Adds the Payroll-Engine NuGet feed using `PAT_DISPATCH` |
| Build | `dotnet publish Client.Services/PayrollEngine.Client.Services.csproj -c Release -o publish` |
| Add PayrollEngine.Lex | Publishes the `PayrollEngine.Lex` package (same version) from a temporary project and copies `PayrollEngine.Lex.dll` and `PayrollEngine.Lex.xml` to `publish/`. Lex is no dependency of Client.Services |
| Install DocFX | Installs the latest DocFX global tool |
| Build DocFX | Runs `docfx docfx/docfx.json` — generates HTML into `docfx/_site/` |
| Upload Pages artifact | Uploads `docfx/_site/` as the GitHub Pages artifact |
| Deploy to GitHub Pages | Deploys the artifact to the `github-pages` environment |

## Output

👉 https://payroll-engine.github.io/PayrollEngine.Client.Services/

## Permissions

| Permission | Reason |
|:--|:--|
| `contents: read` | Checkout |
| `pages: write` | Deploy to GitHub Pages |
| `id-token: write` | OIDC token for Pages deployment |

## Secrets

| Secret | Usage |
|:--|:--|
| `PAT_DISPATCH` | Read access to the Payroll-Engine GitHub Packages NuGet feed |

## Configuration

DocFX is configured in `docfx/docfx.json`. The metadata source is the compiled assembly:

```
publish/PayrollEngine.*.dll
```

For a local build, `publish/` must contain the same assemblies, including `PayrollEngine.Lex.dll` and `PayrollEngine.Lex.xml`.
