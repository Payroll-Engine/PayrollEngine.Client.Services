# Payroll Engine Client Services
👉 This repository is part of the [Payroll Engine](https://github.com/Payroll-Engine/PayrollEngine/wiki).

Main library for Payroll Engine clients, including all required dependencies:
- Function controllers (local script development)
- Function invokers
- JSON schemas

## HTML documentantion
The client scripting library contains static HTML documentation for client developers. This is created using [docx](https://github.com/dotnet/docfx) [MIT]. The following commands are available in the `docfx` folder:
- `Static.Build` - builds the static HTML content (output is the `_site` subdirectory)
- `Static.Static` - opens the static help (`_site\index.html`)
- `Server.Start` - start the static help on http://localhost:4037/ (dark mode support)

## Third party components
- Documentation generation with [docfx](https://github.com/dotnet/docfx/) - licence `MIT`

## Build
Supported runtime environment variables:
- *PayrollEnginePackageDir* - the NuGet package destination directory (optional)