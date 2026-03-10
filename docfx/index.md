# Client Services

The **PayrollEngine.Client.Services** library is the .NET SDK for Payroll Engine client development.
It provides the complete runtime infrastructure for executing payroll scripting functions locally
against a live backend — enabling local script development and testing without deploying to the server.

---

## Service Layer

The service layer provides typed access to every Payroll Engine REST API resource.

### Service Interfaces

| Namespace | Description |
|:--|:--|
| [PayrollEngine.Client.Service](api/PayrollEngine.Client.Service.html) | Service interfaces for all payroll resources: `ICaseService`, `IEmployeeService`, `IWageTypeService`, `ICollectorService`, `IPayrunService`, `IReportService`, and more. Also contains service context types (`CaseServiceContext`, `EmployeeServiceContext`, …) |

### Service Implementations

| Namespace | Description |
|:--|:--|
| [PayrollEngine.Client.Service.Api](api/PayrollEngine.Client.Service.Api.html) | Concrete HTTP-based implementations of all service interfaces. Handles serialization, query parameters, and REST communication with the backend |

---

## Scripting Runtime

The runtime layer bridges the scripting function contract with the live backend.

| Namespace | Description |
|:--|:--|
| [PayrollEngine.Client.Scripting](api/PayrollEngine.Client.Scripting.html) | Function classes, runtime interfaces, No-Code action attributes and infrastructure (from `PayrollEngine.Client.Scripting`) |
| [PayrollEngine.Client.Scripting.Runtime.Api](api/PayrollEngine.Client.Scripting.Runtime.Api.html) | Concrete runtime implementations for every scripting function type: `CaseAvailableRuntime`, `CaseBuildRuntime`, `WageTypeValueRuntime`, `CollectorApplyRuntime`, `ReportBuildRuntime`, and all payrun lifecycle runtimes. Each runtime implements the corresponding `IRuntime` interface and handles all backend API calls on behalf of the executing script |

---

## Test Infrastructure

| Namespace | Description |
|:--|:--|
| [PayrollEngine.Client.Test](api/PayrollEngine.Client.Test.html) | Base classes and utilities for scripting tests: `TestBase`, `TestRunnerBase`, `FileTestRunner`, assertion helpers, and test result types |
| [PayrollEngine.Client.Test.Case](api/PayrollEngine.Client.Test.Case.html) | Test runner for case function tests (available, build, validate) |
| [PayrollEngine.Client.Test.Payrun](api/PayrollEngine.Client.Test.Payrun.html) | Test runner for payrun and wage type function tests |
| [PayrollEngine.Client.Test.Report](api/PayrollEngine.Client.Test.Report.html) | Test runner for report function tests |

---

## Client Infrastructure

| Namespace | Description |
|:--|:--|
| [PayrollEngine.Client](api/PayrollEngine.Client.html) | HTTP client (`PayrollHttpClient`), REST API endpoint definitions for all resources, console base classes (`ConsoleProgram<TApp>`, `ConsoleToolBase`), and connection configuration |
| [PayrollEngine.Client.Model](api/PayrollEngine.Client.Model.html) | Payroll domain model: `Tenant`, `Employee`, `Division`, `Regulation`, `Case`, `CaseField`, `WageType`, `Collector`, `Payrun`, `PayrunJob`, `Report`, and all result types |
| [PayrollEngine.Client.Exchange](api/PayrollEngine.Client.Exchange.html) | Exchange import/export model and visitor pattern for bulk regulation data transfer |
| [PayrollEngine.Client.QueryExpression](api/PayrollEngine.Client.QueryExpression.html) | Fluent query expression builder for constructing REST API filter, order, and pagination parameters |
| [PayrollEngine.Client.Command](api/PayrollEngine.Client.Command.html) | CLI command base classes used by Payroll Engine command-line tools |
| [PayrollEngine.Client.Script](api/PayrollEngine.Client.Script.html) | Script parsers for extracting and injecting function source code in regulation objects |

---

## Shared Core

| Namespace | Description |
|:--|:--|
| [PayrollEngine](api/PayrollEngine.html) | Fundamental shared types: enumerations, calendar utilities (`CalendarTimeUnit`, `CalendarWeekMode`), value types, and extension methods used across all Payroll Engine libraries |

---

## Links

- [Documentation](https://payrollengine.org/roles/automator/client-services)
- [Repository](https://github.com/Payroll-Engine/PayrollEngine.Client.Services)
- [NuGet](https://www.nuget.org/packages/PayrollEngine.Client.Services)
