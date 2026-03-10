# Payroll Engine Client Services

> Part of the [Payroll Engine](https://github.com/Payroll-Engine/PayrollEngine) open-source payroll automation framework.
> Full documentation at [payrollengine.org](https://payrollengine.org).

The main SDK for Payroll Engine client development. It provides the complete runtime infrastructure for executing payroll scripting functions locally, enabling local script development and testing against a live backend without deploying to the server.

Dependencies included:
- [PayrollEngine.Client.Scripting](https://github.com/Payroll-Engine/PayrollEngine.Client.Scripting) — function API and No-Code actions
- [PayrollEngine.Client.Test](https://github.com/Payroll-Engine/PayrollEngine.Client.Test) — case, payrun, and report test runners

---

## Architecture

The library is organized into three areas:

### `Scripting`

Period and cycle implementations used for local function execution:

| Type | Description |
|:--|:--|
| `MonthPayrollPeriod` | Monthly payroll period |
| `WeekPayrollPeriod` | Weekly payroll period |
| `YearPayrollCycle` | Annual payroll cycle |
| `WeekPayrollCycle` | Weekly payroll cycle |
| `ScriptCalendar` | Combines configuration, calendar, and culture for script execution |
| `ScriptContext` | Container for `ScriptCalendar` and regulation namespace |
| `ScriptConfiguration` | Evaluation date, period date, regulation date, culture, and calendar |

`PayrollPeriodExtensions` adds `GetDatePeriod()`, `GetOffsetDatePeriod()`, and `GetContinuePeriods()` to `IPayrollPeriod`.

### `Scripting.Function.Api`

Controllers and invokers for executing scripting functions locally against a live backend via `PayrollHttpClient`:

| Pattern | Role |
|:--|:--|
| `*Controller<TFunc>` | Instantiates the runtime, creates the function instance, invokes the script method, and returns a typed result |
| `*FunctionInvoker<TFunc>` | Thin wrapper around a controller for simplified call sites |
| `*FunctionResult` | Typed result carrying the function output and resolved context objects |

Supported function types:

| Function | Controller | Result |
|:--|:--|:--|
| `CaseAvailable` | `CaseAvailableController` | `CaseAvailableFunctionResult` |
| `CaseBuild` | `CaseBuildController` | `CaseBuildFunctionResult` |
| `CaseValidate` | `CaseValidateController` | `CaseValidateFunctionResult` |
| `CaseRelationBuild` | `CaseRelationBuildController` | `CaseRelationBuildFunctionResult` |
| `CaseRelationValidate` | `CaseRelationValidateController` | `CaseRelationValidateFunctionResult` |
| `ReportBuild` | `ReportBuildController` | `ReportBuildFunctionResult` |
| `ReportStart` | `ReportStartController` | `ReportStartFunctionResult` |
| `ReportEnd` | `ReportEndController` | `ReportEndFunctionResult` |

`ReportParameterParser` resolves identifier-based report parameters (employee, regulation, payroll, payrun, webhook, report) to their corresponding database IDs before execution.

### `Scripting.Runtime.Api`

Concrete runtime implementations for every scripting function type. Each runtime implements the corresponding `IRuntime` interface from the Scripting library and handles API calls to the backend:

- **Base runtimes:** `RuntimeBase` → `PayrollRuntime` → `CaseRuntimeBase`, `PayrunRuntimeBase`, `CollectorRuntimeBase`, `WageTypeRuntimeBase`, `ReportRuntime`
- **Leaf runtimes:** one per function event — `CaseAvailableRuntime`, `CaseBuildRuntime`, `CaseValidateRuntime`, `CaseRelationBuildRuntime`, `CaseRelationValidateRuntime`, `CollectorStartRuntime`, `CollectorApplyRuntime`, `CollectorEndRuntime`, `WageTypeValueRuntime`, `WageTypeResultRuntime`, `PayrunStartRuntime`, `PayrunEndRuntime`, `PayrunEmployeeAvailableRuntime`, `PayrunEmployeeStartRuntime`, `PayrunEmployeeEndRuntime`, `PayrunWageTypeAvailableRuntime`, `ReportBuildRuntime`, `ReportStartRuntime`, `ReportEndRuntime`

Culture and calendar resolution follows the standard priority: employee → division → tenant.

> Webhooks are not supported through the client runtime. Calls to `InvokeWebhook` throw `NotSupportedException`.

---

## Local Script Development

The SDK is the foundation for running function scripts outside the backend. Usage pattern:

```csharp
// 1. Configure the scripting context
var config = new ScriptConfiguration
{
    EvaluationDate = DateTime.UtcNow,
    PeriodDate = new DateTime(2026, 3, 1, 0, 0, 0, DateTimeKind.Utc),
    RegulationDate = DateTime.UtcNow,
    CultureName = "de-CH"
};

// 2. Invoke via the function invoker
var httpClient = new PayrollHttpClient(backendUrl);
var invoker = new CaseAvailableFunctionInvoker<MyCaseAvailableFunction>(httpClient, config);
var result = invoker.Available("MySalaryCase");

Console.WriteLine($"Available: {result.Available}");
```

For report parameter resolution before invoking:

```csharp
var parser = new ReportParameterParser(httpClient, tenantId, regulationId);
await parser.ParseParametersAsync(parameters); // resolves names → IDs
```

---

## Developer Reference

The client API reference is published at:
👉 [payroll-engine.github.io/PayrollEngine.Client.Services](https://payroll-engine.github.io/PayrollEngine.Client.Services/)

The HTML reference is generated with [docfx](https://github.com/dotnet/docfx). Commands in the `docfx` folder:

| Command | Description |
|:--|:--|
| `Static.Build` | Build static HTML content (output: `_site/`) |
| `Static.Start` | Open the static help (`_site/index.html`) |
| `Server.Start` | Serve on `http://localhost:4037/` with dark mode support |

---

## NuGet Package

Available on [NuGet.org](https://www.nuget.org/profiles/PayrollEngine):

```sh
dotnet add package PayrollEngine.Client.Services
```

---

## Build

Environment variable used during build:

| Variable | Description |
|:--|:--|
| `PayrollEnginePackageDir` | Output directory for the NuGet package (optional) |

---

## Third Party Components
- Documentation generation with [docfx](https://github.com/dotnet/docfx/) — license `MIT`

---

## See Also

- [Client Services](https://payrollengine.org/roles/automator/client-services) — Automator role documentation
- [Client Scripting](https://github.com/Payroll-Engine/PayrollEngine.Client.Scripting) — scripting function API
- [Client Test](https://github.com/Payroll-Engine/PayrollEngine.Client.Test) — test library
- [Payroll Console](https://github.com/Payroll-Engine/PayrollEngine.PayrollConsole) — CLI tool using this SDK
