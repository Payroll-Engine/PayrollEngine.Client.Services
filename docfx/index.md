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
| [PayrollEngine.Client.Service](xref:PayrollEngine.Client.Service) | Service interfaces for all payroll resources: `ICaseService`, `IEmployeeService`, `IWageTypeService`, `ICollectorService`, `IPayrunService`, `IReportService`, and more. Also contains service context types (`CaseServiceContext`, `EmployeeServiceContext`, …) |

### Service Implementations

| Namespace | Description |
|:--|:--|
| [PayrollEngine.Client.Service.Api](xref:PayrollEngine.Client.Service.Api) | Concrete HTTP-based implementations of all service interfaces. Handles serialization, query parameters, and REST communication with the backend |

---

## Scripting Runtime

The runtime layer bridges the scripting function contract with the live backend.

| Namespace | Description |
|:--|:--|
| [PayrollEngine.Client.Scripting](xref:PayrollEngine.Client.Scripting) | Function classes, runtime interfaces, No-Code action attributes and infrastructure (from `PayrollEngine.Client.Scripting`) |
| [PayrollEngine.Client.Scripting.Runtime.Api](xref:PayrollEngine.Client.Scripting.Runtime.Api) | Concrete runtime implementations for every scripting function type: `CaseAvailableRuntime`, `CaseBuildRuntime`, `WageTypeValueRuntime`, `CollectorApplyRuntime`, `ReportBuildRuntime`, and all payrun lifecycle runtimes. Each runtime implements the corresponding `IRuntime` interface and handles all backend API calls on behalf of the executing script |

---

## Test Infrastructure

| Namespace | Description |
|:--|:--|
| [PayrollEngine.Client.Test](xref:PayrollEngine.Client.Test) | Base classes and utilities for scripting tests: `TestBase`, `TestRunnerBase`, `FileTestRunner`, assertion helpers, and test result types |
| [PayrollEngine.Client.Test.Case](xref:PayrollEngine.Client.Test.Case) | Test runner for case function tests (available, build, validate) |
| [PayrollEngine.Client.Test.Payrun](xref:PayrollEngine.Client.Test.Payrun) | Test runner for payrun and wage type function tests |
| [PayrollEngine.Client.Test.Report](xref:PayrollEngine.Client.Test.Report) | Test runner for report function tests |

---

## Client Infrastructure

| Namespace | Description |
|:--|:--|
| [PayrollEngine.Client](xref:PayrollEngine.Client) | HTTP client (`PayrollHttpClient`), REST API endpoint definitions for all resources, console base classes (`ConsoleProgram<TApp>`, `ConsoleToolBase`), and connection configuration |
| [PayrollEngine.Client.Model](xref:PayrollEngine.Client.Model) | Payroll domain model: `Tenant`, `Employee`, `Division`, `Regulation`, `Case`, `CaseField`, `WageType`, `Collector`, `Payrun`, `PayrunJob`, `Report`, and all result types |
| [PayrollEngine.Client.Exchange](xref:PayrollEngine.Client.Exchange) | Exchange import/export model and visitor pattern for bulk regulation data transfer |
| [PayrollEngine.Client.QueryExpression](xref:PayrollEngine.Client.QueryExpression) | Fluent query expression builder for constructing REST API filter, order, and pagination parameters |
| [PayrollEngine.Client.Command](xref:PayrollEngine.Client.Command) | CLI command base classes used by Payroll Engine command-line tools |
| [PayrollEngine.Client.Script](xref:PayrollEngine.Client.Script) | Script parsers for extracting and injecting function source code in regulation objects |

---

## Shared Core

| Namespace | Description |
|:--|:--|
| [PayrollEngine](xref:PayrollEngine) | Fundamental shared types: enumerations, calendar utilities (`CalendarTimeUnit`, `CalendarWeekMode`), value types, and extension methods used across all Payroll Engine libraries |

---

## Links

- [Documentation](https://payrollengine.org/roles/automator/client-services)
- [Repository](https://github.com/Payroll-Engine/PayrollEngine.Client.Services)
- [NuGet](https://www.nuget.org/packages/PayrollEngine.Client.Services)
