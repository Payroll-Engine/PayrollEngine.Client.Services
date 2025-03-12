using System;
using System.Data;
using System.Linq;
using System.Text.Json;
using System.Globalization;
using System.Collections.Generic;
using PayrollEngine.Client.Model;
using PayrollEngine.Client.Service;
using PayrollEngine.Client.Service.Api;
using PayrollEngine.Client.QueryExpression;

namespace PayrollEngine.Client.Scripting.Runtime.Api;

/// <summary>Runtime for the payroll report function</summary>
public abstract class ReportRuntime : RuntimeBase, IReportRuntime
{
    /// <summary>The report</summary>
    protected ReportSet Report { get; }

    /// <summary>The report request</summary>
    protected ReportRequest ReportRequest { get; }

    /// <summary>The payroll service</summary>
    protected IPayrollService PayrollService { get; }

    /// <summary>Initializes a new instance of the <see cref="PayrollRuntime"/> class</summary>
    /// <param name="httpClient">The Payroll http client</param>
    /// <param name="tenantId">The tenant id</param>
    /// <param name="userId">The user id</param>
    /// <param name="report">The report</param>
    /// <param name="reportRequest">The report request</param>
    protected ReportRuntime(PayrollHttpClient httpClient, int tenantId,
        int userId, ReportSet report, ReportRequest reportRequest) :
        base(httpClient, tenantId, userId)
    {
        Report = report ?? throw new ArgumentNullException(nameof(report));
        ReportRequest = reportRequest ?? throw new ArgumentNullException(nameof(reportRequest));
        PayrollService = new PayrollService(httpClient);
    }

    /// <summary>The log owner type</summary>
    protected override string LogOwner => ReportName;

    /// <inheritdoc />
    public string ReportName => Report.Name;

    /// <summary>The culture by priority: report request > tenant > system</summary>
    public override string UserCulture =>
        ReportRequest.Culture ?? base.UserCulture;

    /// <inheritdoc />
    public object GetReportAttribute(string attributeName) =>
        Report.Attributes?.GetValue<object>(attributeName);

    /// <inheritdoc />
    public void SetReportAttribute(string attributeName, object value)
    {
        if (string.IsNullOrWhiteSpace(attributeName))
        {
            throw new ArgumentException(nameof(attributeName));
        }

        // add/change attribute
        if (value != null)
        {
            // ensure attribute collection
            Report.Attributes ??= new();
            Report.Attributes[attributeName] = value;
        }
        else
        {
            // remove attribute
            if (Report.Attributes != null && Report.Attributes.ContainsKey(attributeName))
            {
                Report.Attributes.Remove(attributeName);
            }
        }
    }

    #region Parameter

    /// <inheritdoc />
    public bool HasParameter(string parameterName)
    {
        if (string.IsNullOrWhiteSpace(parameterName))
        {
            throw new ArgumentException(nameof(parameterName));
        }

        // request
        if (ReportRequest.Parameters != null && ReportRequest.Parameters.ContainsKey(parameterName))
        {
            return true;
        }

        // report
        var parameter = Report.Parameters.FirstOrDefault(x => string.Equals(x.Name, parameterName));
        return parameter?.Value != null;
    }

    /// <inheritdoc />
    public string GetParameter(string parameterName)
    {
        if (string.IsNullOrWhiteSpace(parameterName))
        {
            throw new ArgumentException(nameof(parameterName));
        }

        // request
        if (ReportRequest.Parameters != null && ReportRequest.Parameters.TryGetValue(parameterName, out var requestParameter))
        {
            return requestParameter;
        }

        // report
        var parameter = Report.Parameters.FirstOrDefault(x => string.Equals(x.Name, parameterName));
        return parameter?.Value;
    }

    /// <summary>
    /// Set the report parameter
    /// </summary>
    /// <param name="parameterName">Name of the parameter</param>
    /// <param name="value">The parameter value</param>
    protected void SetParameterInternal(string parameterName, string value)
    {
        if (string.IsNullOrWhiteSpace(parameterName))
        {
            throw new ArgumentException(nameof(parameterName));
        }

        // request parameter
        ReportRequest.Parameters ??= new();
        ReportRequest.Parameters[parameterName] = value;

        // report parameter
        var reportParameter = Report.Parameters?.FirstOrDefault(x => string.Equals(x.Name, parameterName));
        if (reportParameter != null)
        {
            reportParameter.Value = value;
        }
    }

    /// <inheritdoc />
    public object GetParameterAttribute(string parameterName, string attributeName)
    {
        if (string.IsNullOrWhiteSpace(parameterName))
        {
            throw new ArgumentException(nameof(parameterName));
        }
        if (string.IsNullOrWhiteSpace(attributeName))
        {
            throw new ArgumentException(nameof(attributeName));
        }

        // report parameter
        if (Report.Parameters == null)
        {
            throw new ArgumentException($"Invalid report parameter {parameterName}.");
        }
        var reportParameter = Report.Parameters.FirstOrDefault(x => string.Equals(x.Name, parameterName));
        if (reportParameter == null)
        {
            throw new ArgumentException($"Unknown report parameter {parameterName}.");
        }

        // report parameter attribute
        if (reportParameter.Attributes != null && reportParameter.Attributes.TryGetValue(attributeName, out var attribute))
        {
            return attribute;
        }

        return null;
    }

    /// <inheritdoc />
    public void SetParameterAttribute(string parameterName, string attributeName, object value)
    {
        if (string.IsNullOrWhiteSpace(parameterName))
        {
            throw new ArgumentException(nameof(parameterName));
        }
        if (string.IsNullOrWhiteSpace(attributeName))
        {
            throw new ArgumentException(nameof(attributeName));
        }

        // report parameter
        if (Report.Parameters == null)
        {
            throw new ArgumentException($"Invalid report parameter {parameterName}.");
        }
        var reportParameter = Report.Parameters.FirstOrDefault(x => string.Equals(x.Name, parameterName));
        if (reportParameter == null)
        {
            throw new ArgumentException($"Unknown report parameter {parameterName}.");
        }

        // add/change attribute
        if (value != null)
        {
            reportParameter.Attributes ??= new();
            reportParameter.Attributes[attributeName] = value;
        }
        else
        {
            // remove attribute
            if (reportParameter.Attributes != null && reportParameter.Attributes.ContainsKey(attributeName))
            {
                reportParameter.Attributes.Remove(attributeName);
            }
        }
    }

    /// <inheritdoc />
    public bool ParameterHidden(string parameterName)
    {
        if (string.IsNullOrWhiteSpace(parameterName))
        {
            throw new ArgumentException(nameof(parameterName));
        }
        // report parameter
        if (Report.Parameters == null)
        {
            throw new ArgumentException($"Invalid report parameter {parameterName}.");
        }
        var reportParameter = Report.Parameters.FirstOrDefault(x => string.Equals(x.Name, parameterName));
        if (reportParameter == null)
        {
            throw new ArgumentException($"Unknown report parameter {parameterName}.");
        }
        return reportParameter.Hidden;
    }

    #endregion

    #region Execute Query

    /// <inheritdoc />
    public virtual DataTable ExecuteQuery(string tableName, string methodName, string culture, Dictionary<string, string> parameters)
    {
        if (string.IsNullOrWhiteSpace(tableName))
        {
            throw new ArgumentException(nameof(tableName));
        }
        if (string.IsNullOrWhiteSpace(methodName))
        {
            throw new ArgumentException(nameof(methodName));
        }

        // culture
        culture ??= CultureInfo.CurrentCulture.Name;

        try
        {
            // report query
            var payrollDataTable = TenantService.ExecuteReportQueryAsync(TenantId, methodName, culture, parameters).Result;
            if (payrollDataTable == null)
            {
                return null;
            }

            var systemDataTable = Data.DataTableConversionExtensions.ToSystemDataTable(payrollDataTable);
            systemDataTable.TableName = tableName;
            systemDataTable.AcceptChanges();
            return systemDataTable;
        }
        catch (Exception exception)
        {
            throw new ScriptException(exception.GetBaseMessage(), exception);
        }
    }

    /// <inheritdoc />
    public Dictionary<string, string> ExecuteLookupValueQuery(int regulationId, string lookupName,
        string keyAttribute, string valueAttribute)
    {
        if (string.IsNullOrWhiteSpace(lookupName))
        {
            throw new ArgumentException(nameof(lookupName));
        }
        if (string.IsNullOrWhiteSpace(keyAttribute))
        {
            throw new ArgumentException(nameof(keyAttribute));
        }
        if (string.IsNullOrWhiteSpace(valueAttribute))
        {
            throw new ArgumentException(nameof(valueAttribute));
        }

        // lookup
        var query = new Query
        {
            Status = PayrollEngine.ObjectStatus.Active,
            Filter = new Equals(nameof(Lookup.Name), lookupName)
        };
        var lookup = new LookupService(HttpClient).QueryAsync<Lookup>(new(TenantId, regulationId), query).Result.FirstOrDefault();
        if (lookup == null)
        {
            return new();
        }

        // lookup values
        var values = new Dictionary<string, string>();
        var lookupValues = new LookupValueService(HttpClient).QueryAsync<LookupValue>(new(TenantId, regulationId, lookup.Id),
            new() { Status = PayrollEngine.ObjectStatus.Active }).Result;
        foreach (var lookupValue in lookupValues)
        {
            // localized lookup json value
            var value = UserCulture.GetLocalization(lookupValue.ValueLocalizations, lookupValue.Value);
            if (string.IsNullOrWhiteSpace(value))
            {
                continue;
            }

            // deserialize lookup value
            var valueObject = JsonSerializer.Deserialize<Dictionary<string, string>>(value);
            if (valueObject != null && valueObject.ContainsKey(keyAttribute) && valueObject.TryGetValue(valueAttribute, out var attributeValue))
            {
                values.Add(valueObject[keyAttribute], attributeValue);
            }
        }
        return values;
    }

    /// <inheritdoc />
    public DataTable ExecuteGlobalCaseValueQuery(string tableName, Tuple<int?, string, string, string, long?, long?> queryValues)
    {
        var caseValues = new GlobalCaseValueService(HttpClient).QueryAsync<Model.CaseValue>(
            new(TenantId), BuildCaseValueQuery(queryValues)).Result;
        return BuildCaseValueTable(tableName, caseValues);
    }

    /// <inheritdoc />
    public DataTable ExecuteNationalCaseValueQuery(string tableName, Tuple<int?, string, string, string, long?, long?> queryValues)
    {
        var caseValues = new NationalCaseValueService(HttpClient).QueryAsync<Model.CaseValue>(
            new(TenantId), BuildCaseValueQuery(queryValues)).Result;
        return BuildCaseValueTable(tableName, caseValues);
    }

    /// <inheritdoc />
    public DataTable ExecuteCompanyCaseValueQuery(string tableName, Tuple<int?, string, string, string, long?, long?> queryValues)
    {
        var caseValues = new CompanyCaseValueService(HttpClient).QueryAsync<Model.CaseValue>(
            new(TenantId), BuildCaseValueQuery(queryValues)).Result;
        return BuildCaseValueTable(tableName, caseValues);
    }

    /// <inheritdoc />
    public DataTable ExecuteEmployeeCaseValueQuery(string tableName, int employeeId, Tuple<int?, string, string, string, long?, long?> queryValues)
    {
        var caseValues = new EmployeeCaseValueService(HttpClient).QueryAsync<Model.CaseValue>(
            new(TenantId, employeeId), BuildCaseValueQuery(queryValues)).Result;
        return BuildCaseValueTable(tableName, caseValues);
    }

    private static CaseValueQuery BuildCaseValueQuery(Tuple<int?, string, string, string, long?, long?> queryValues)
    {
        var query = QueryValuesToQuery<CaseValueQuery>(queryValues);
        if (string.IsNullOrWhiteSpace(query.OrderBy))
        {
            query.OrderBy = nameof(Model.CaseValue.Start);
        }
        return query;
    }

    private static DataTable BuildCaseValueTable(string tableName, IEnumerable<Model.CaseValue> caseValues)
    {
        // argument check
        if (string.IsNullOrWhiteSpace(tableName))
        {
            throw new ArgumentException(nameof(tableName));
        }

        var dataTable = new DataTable(tableName);

        // setup columns
        dataTable.Columns.Add(nameof(Model.CaseValue.Id), typeof(int));
        dataTable.Columns.Add(nameof(Model.CaseValue.DivisionId), typeof(int));
        dataTable.Columns.Add(nameof(Model.CaseValue.EmployeeId), typeof(int));
        dataTable.Columns.Add(nameof(Model.CaseValue.Status), typeof(int));
        dataTable.Columns.Add(nameof(Model.CaseValue.Created), typeof(DateTime));
        dataTable.Columns.Add(nameof(Model.CaseValue.Updated), typeof(DateTime));
        dataTable.Columns.Add(nameof(Model.CaseValue.CaseName), typeof(string));
        dataTable.Columns.Add(nameof(Model.CaseValue.CaseNameLocalizations), typeof(string));
        dataTable.Columns.Add(nameof(Model.CaseValue.CaseFieldName), typeof(string));
        dataTable.Columns.Add(nameof(Model.CaseValue.CaseFieldNameLocalizations), typeof(string));
        dataTable.Columns.Add(nameof(Model.CaseValue.CaseSlot), typeof(string));
        dataTable.Columns.Add(nameof(Model.CaseValue.Forecast), typeof(string));
        dataTable.Columns.Add(nameof(Model.CaseValue.Start), typeof(DateTime));
        dataTable.Columns.Add(nameof(Model.CaseValue.End), typeof(DateTime));
        dataTable.Columns.Add(nameof(Model.CaseValue.CancellationDate), typeof(DateTime));
        dataTable.Columns.Add(nameof(Model.CaseValue.ValueType), typeof(int));
        dataTable.Columns.Add(nameof(Model.CaseValue.Value), typeof(string));
        dataTable.Columns.Add(nameof(Model.CaseValue.NumericValue), typeof(decimal));
        dataTable.Columns.Add(nameof(Model.CaseValue.Attributes), typeof(string));

        // setup columns
        foreach (var caseValue in caseValues)
        {
            var row = dataTable.NewRow();
            // ids
            row[nameof(Model.CaseValue.Id)] = caseValue.Id;
            if (caseValue.DivisionId.HasValue)
            {
                row[nameof(Model.CaseValue.DivisionId)] = caseValue.DivisionId;
            }
            if (caseValue.EmployeeId.HasValue)
            {
                row[nameof(Model.CaseValue.EmployeeId)] = caseValue.EmployeeId;
            }

            // case value fields
            row[nameof(Model.CaseValue.Status)] = (int)caseValue.Status;
            row[nameof(Model.CaseValue.Created)] = caseValue.Created;
            row[nameof(Model.CaseValue.Updated)] = caseValue.Updated;
            row[nameof(Model.CaseValue.CaseName)] = caseValue.CaseName;
            row[nameof(Model.CaseValue.CaseNameLocalizations)] = JsonSerializer.Serialize(caseValue.CaseNameLocalizations);
            row[nameof(Model.CaseValue.CaseFieldName)] = caseValue.CaseFieldName;
            row[nameof(Model.CaseValue.CaseFieldNameLocalizations)] = JsonSerializer.Serialize(caseValue.CaseFieldNameLocalizations);
            row[nameof(Model.CaseValue.CaseSlot)] = caseValue.CaseSlot;
            row[nameof(Model.CaseValue.Forecast)] = caseValue.Forecast;
            if (caseValue.Start.HasValue)
            {
                row[nameof(Model.CaseValue.Start)] = caseValue.Start;
            }
            if (caseValue.End.HasValue)
            {
                row[nameof(Model.CaseValue.End)] = caseValue.End;
            }
            if (caseValue.CancellationDate.HasValue)
            {
                row[nameof(Model.CaseValue.CancellationDate)] = caseValue.CancellationDate;
            }
            // case value
            row[nameof(Model.CaseValue.ValueType)] = (int)caseValue.ValueType;
            row[nameof(Model.CaseValue.Value)] = caseValue.Value;
            if (caseValue.NumericValue.HasValue)
            {
                row[nameof(Model.CaseValue.NumericValue)] = caseValue.NumericValue;
            }
            row[nameof(Model.CaseValue.Attributes)] = JsonSerializer.Serialize(caseValue.Attributes);
            dataTable.Rows.Add(row);
        }

        // commit changes
        dataTable.AcceptChanges();
        return dataTable;
    }

    /// <inheritdoc />
    public DataTable ExecuteWageTypeQuery(string tableName, int regulationId, Tuple<int?, string, string, string, long?, long?> queryValues)
    {
        if (string.IsNullOrWhiteSpace(tableName))
        {
            throw new ArgumentException(nameof(tableName));
        }

        // query
        var query = QueryValuesToQuery(queryValues);
        // exclude scripting columns
        if (string.IsNullOrWhiteSpace(query.Select))
        {
            query.Select =
                $"{nameof(WageType.Id)}, {nameof(WageType.Status)}, {nameof(WageType.Created)}, " +
                $"{nameof(WageType.Updated)}, {nameof(WageType.Name)}, {nameof(WageType.NameLocalizations)}, " +
                $"{nameof(WageType.WageTypeNumber)}, {nameof(WageType.Description)}";
        }

        // query wage types
        var wageTypes = new WageTypeService(HttpClient).
            QueryAsync<WageType>(new(TenantId, regulationId), query).Result;

        // setup columns
        var dataTable = new DataTable(tableName);
        dataTable.Columns.Add(nameof(WageType.Id), typeof(int));
        dataTable.Columns.Add(nameof(WageType.Status), typeof(int));
        dataTable.Columns.Add(nameof(WageType.Created), typeof(DateTime));
        dataTable.Columns.Add(nameof(WageType.Updated), typeof(DateTime));
        dataTable.Columns.Add(nameof(WageType.Name), typeof(string));
        dataTable.Columns.Add(nameof(WageType.NameLocalizations), typeof(string));
        dataTable.Columns.Add(nameof(WageType.Description), typeof(string));
        dataTable.Columns.Add(nameof(WageType.Attributes), typeof(string));
        dataTable.Columns.Add(nameof(WageType.WageTypeNumber), typeof(decimal));

        // setup rows
        foreach (var wageType in wageTypes)
        {
            var row = dataTable.NewRow();
            row[nameof(WageType.Id)] = wageType.Id;
            row[nameof(WageType.Status)] = (int)wageType.Status;
            row[nameof(WageType.Created)] = wageType.Created;
            row[nameof(WageType.Updated)] = wageType.Updated;
            row[nameof(WageType.Name)] = wageType.Name;
            row[nameof(WageType.NameLocalizations)] = JsonSerializer.Serialize(wageType.NameLocalizations);
            row[nameof(WageType.Description)] = wageType.Description;
            row[nameof(WageType.WageTypeNumber)] = wageType.WageTypeNumber;
            row[nameof(WageType.Attributes)] = JsonSerializer.Serialize(wageType.Attributes);
            dataTable.Rows.Add(row);
        }
        dataTable.AcceptChanges();
        return dataTable;
    }

    /// <inheritdoc />
    public DataTable ExecutePayrollResultQuery(string tableName, Tuple<int?, string, string, string, long?, long?> queryValues)
    {
        // argument check
        if (string.IsNullOrWhiteSpace(tableName))
        {
            throw new ArgumentException(nameof(tableName));
        }

        // query
        var results = new PayrollResultService(HttpClient).
            QueryAsync<PayrollResult>(new(TenantId), QueryValuesToQuery(queryValues)).Result;

        // setup columns
        var dataTable = new DataTable(tableName);
        dataTable.Columns.Add(nameof(PayrollResult.Id), typeof(int));
        dataTable.Columns.Add(nameof(PayrollResult.Status), typeof(int));
        dataTable.Columns.Add(nameof(PayrollResult.Created), typeof(DateTime));
        dataTable.Columns.Add(nameof(PayrollResult.Updated), typeof(DateTime));
        dataTable.Columns.Add(nameof(PayrollResult.PayrollId), typeof(int));
        dataTable.Columns.Add(nameof(PayrollResult.PayrunId), typeof(int));
        dataTable.Columns.Add(nameof(PayrollResult.PayrunJobId), typeof(int));
        dataTable.Columns.Add(nameof(PayrollResult.EmployeeId), typeof(int));
        dataTable.Columns.Add(nameof(PayrollResult.DivisionId), typeof(int));
        dataTable.Columns.Add(nameof(PayrollResult.CycleName), typeof(string));
        dataTable.Columns.Add(nameof(PayrollResult.CycleStart), typeof(DateTime));
        dataTable.Columns.Add(nameof(PayrollResult.CycleEnd), typeof(DateTime));
        dataTable.Columns.Add(nameof(PayrollResult.PeriodName), typeof(string));
        dataTable.Columns.Add(nameof(PayrollResult.PeriodStart), typeof(DateTime));
        dataTable.Columns.Add(nameof(PayrollResult.PeriodEnd), typeof(DateTime));

        // setup rows
        foreach (var result in results)
        {
            var row = dataTable.NewRow();
            row[nameof(PayrollResult.Id)] = result.Id;
            row[nameof(PayrollResult.Status)] = (int)result.Status;
            row[nameof(PayrollResult.Created)] = result.Created;
            row[nameof(PayrollResult.Updated)] = result.Updated;
            row[nameof(PayrollResult.PayrollId)] = result.PayrollId;
            row[nameof(PayrollResult.PayrunId)] = result.PayrunId;
            row[nameof(PayrollResult.PayrunJobId)] = result.PayrunJobId;
            row[nameof(PayrollResult.EmployeeId)] = result.EmployeeId;
            row[nameof(PayrollResult.DivisionId)] = result.DivisionId;
            row[nameof(PayrollResult.CycleName)] = result.CycleName;
            row[nameof(PayrollResult.CycleStart)] = result.CycleStart;
            row[nameof(PayrollResult.CycleEnd)] = result.CycleEnd;
            row[nameof(PayrollResult.PeriodName)] = result.PeriodName;
            row[nameof(PayrollResult.PeriodStart)] = result.PeriodStart;
            row[nameof(PayrollResult.PeriodEnd)] = result.PeriodEnd;
            dataTable.Rows.Add(row);
        }

        // commit changes
        dataTable.AcceptChanges();
        return dataTable;
    }

    /// <inheritdoc />
    public DataTable ExecuteWageTypeResultQuery(string tableName, int payrollResultId, Tuple<int?, string, string, string, long?, long?> queryValues)
    {
        // argument check
        if (string.IsNullOrWhiteSpace(tableName))
        {
            throw new ArgumentException(nameof(tableName));
        }
        // query
        var results = new PayrollResultService(HttpClient).
            QueryWageTypeResultsAsync<Model.WageTypeResult>(new(TenantId, payrollResultId), QueryValuesToQuery(queryValues)).Result;

        // setup columns
        var dataTable = new DataTable(tableName);
        dataTable.Columns.Add(nameof(Model.WageTypeResult.Id), typeof(int));
        dataTable.Columns.Add(nameof(Model.WageTypeResult.Status), typeof(int));
        dataTable.Columns.Add(nameof(Model.WageTypeResult.Created), typeof(DateTime));
        dataTable.Columns.Add(nameof(Model.WageTypeResult.Updated), typeof(DateTime));
        dataTable.Columns.Add(nameof(Model.WageTypeResult.PayrollResultId), typeof(int));
        dataTable.Columns.Add(nameof(Model.WageTypeResult.WageTypeId), typeof(int));
        dataTable.Columns.Add(nameof(Model.WageTypeResult.WageTypeNumber), typeof(decimal));
        dataTable.Columns.Add(nameof(Model.WageTypeResult.WageTypeName), typeof(string));
        dataTable.Columns.Add(nameof(Model.WageTypeResult.WageTypeNameLocalizations), typeof(string));
        dataTable.Columns.Add(nameof(Model.WageTypeResult.ValueType), typeof(int));
        dataTable.Columns.Add(nameof(Model.WageTypeResult.Value), typeof(decimal));
        dataTable.Columns.Add(nameof(Model.WageTypeResult.Start), typeof(DateTime));
        dataTable.Columns.Add(nameof(Model.WageTypeResult.End), typeof(DateTime));
        dataTable.Columns.Add(nameof(Model.WageTypeResult.Tags), typeof(string));
        dataTable.Columns.Add(nameof(Model.WageTypeResult.Attributes), typeof(string));

        // setup rows
        foreach (var result in results)
        {
            var row = dataTable.NewRow();
            row[nameof(Model.WageTypeResult.Id)] = result.Id;
            row[nameof(Model.WageTypeResult.Status)] = (int)result.Status;
            row[nameof(Model.WageTypeResult.Created)] = result.Created;
            row[nameof(Model.WageTypeResult.Updated)] = result.Updated;
            row[nameof(Model.WageTypeResult.PayrollResultId)] = result.PayrollResultId;
            row[nameof(Model.WageTypeResult.WageTypeId)] = result.WageTypeId;
            row[nameof(Model.WageTypeResult.WageTypeNumber)] = result.WageTypeNumber;
            row[nameof(Model.WageTypeResult.WageTypeName)] = result.WageTypeName;
            row[nameof(Model.WageTypeResult.WageTypeNameLocalizations)] = JsonSerializer.Serialize(result.WageTypeNameLocalizations);
            row[nameof(Model.WageTypeResult.ValueType)] = (int)result.ValueType;
            row[nameof(Model.WageTypeResult.Value)] = result.Value;
            row[nameof(Model.WageTypeResult.Start)] = result.Start;
            row[nameof(Model.WageTypeResult.End)] = result.End;
            row[nameof(Model.WageTypeResult.Tags)] = JsonSerializer.Serialize(result.Tags);
            row[nameof(Model.WageTypeResult.Attributes)] = JsonSerializer.Serialize(result.Attributes);
            dataTable.Rows.Add(row);
        }

        // commit changes
        dataTable.AcceptChanges();
        return dataTable;
    }

    /// <inheritdoc />
    public DataTable ExecuteWageTypeCustomResultQuery(string tableName, int payrollResultId, int wageTypeResultId,
        Tuple<int?, string, string, string, long?, long?> queryValues)
    {
        // argument check
        if (string.IsNullOrWhiteSpace(tableName))
        {
            throw new ArgumentException(nameof(tableName));
        }

        // query
        var results = new PayrollResultService(HttpClient).QueryWageTypeCustomResultsAsync<Model.WageTypeCustomResult>(
            new(TenantId), payrollResultId, wageTypeResultId, QueryValuesToQuery(queryValues)).Result;

        // setup columns
        var dataTable = new DataTable(tableName);
        dataTable.Columns.Add(nameof(Model.WageTypeCustomResult.Id), typeof(int));
        dataTable.Columns.Add(nameof(Model.WageTypeCustomResult.Status), typeof(int));
        dataTable.Columns.Add(nameof(Model.WageTypeCustomResult.Created), typeof(DateTime));
        dataTable.Columns.Add(nameof(Model.WageTypeCustomResult.Updated), typeof(DateTime));
        dataTable.Columns.Add(nameof(Model.WageTypeCustomResult.WageTypeResultId), typeof(int));
        dataTable.Columns.Add(nameof(Model.WageTypeCustomResult.WageTypeNumber), typeof(decimal));
        dataTable.Columns.Add(nameof(Model.WageTypeCustomResult.WageTypeName), typeof(string));
        dataTable.Columns.Add(nameof(Model.WageTypeCustomResult.WageTypeNameLocalizations), typeof(string));
        dataTable.Columns.Add(nameof(Model.WageTypeCustomResult.Source), typeof(string));
        dataTable.Columns.Add(nameof(Model.WageTypeCustomResult.ValueType), typeof(int));
        dataTable.Columns.Add(nameof(Model.WageTypeCustomResult.Value), typeof(decimal));
        dataTable.Columns.Add(nameof(Model.WageTypeCustomResult.Start), typeof(DateTime));
        dataTable.Columns.Add(nameof(Model.WageTypeCustomResult.End), typeof(DateTime));
        dataTable.Columns.Add(nameof(Model.WageTypeCustomResult.Tags), typeof(string));
        dataTable.Columns.Add(nameof(Model.WageTypeCustomResult.Attributes), typeof(string));

        // setup rows
        foreach (var result in results)
        {
            var row = dataTable.NewRow();
            row[nameof(Model.WageTypeCustomResult.Id)] = result.Id;
            row[nameof(Model.WageTypeCustomResult.Status)] = (int)result.Status;
            row[nameof(Model.WageTypeCustomResult.Created)] = result.Created;
            row[nameof(Model.WageTypeCustomResult.Updated)] = result.Updated;
            row[nameof(Model.WageTypeCustomResult.WageTypeResultId)] = result.WageTypeResultId;
            row[nameof(Model.WageTypeCustomResult.WageTypeNumber)] = result.WageTypeNumber;
            row[nameof(Model.WageTypeCustomResult.WageTypeName)] = result.WageTypeName;
            row[nameof(Model.WageTypeCustomResult.WageTypeNameLocalizations)] = JsonSerializer.Serialize(result.WageTypeNameLocalizations);
            row[nameof(Model.WageTypeCustomResult.Source)] = result.Source;
            row[nameof(Model.WageTypeCustomResult.ValueType)] = (int)result.ValueType;
            row[nameof(Model.WageTypeCustomResult.Value)] = result.Value;
            row[nameof(Model.WageTypeCustomResult.Start)] = result.Start;
            row[nameof(Model.WageTypeCustomResult.End)] = result.End;
            row[nameof(Model.WageTypeCustomResult.Tags)] = JsonSerializer.Serialize(result.Tags);
            row[nameof(Model.WageTypeCustomResult.Attributes)] = JsonSerializer.Serialize(result.Attributes);
            dataTable.Rows.Add(row);
        }

        // commit changes
        dataTable.AcceptChanges();
        return dataTable;
    }

    /// <inheritdoc />
    public DataTable ExecuteCollectorResultQuery(string tableName, int payrollResultId, Tuple<int?, string, string, string, long?, long?> queryValues)
    {
        // argument check
        if (string.IsNullOrWhiteSpace(tableName))
        {
            throw new ArgumentException(nameof(tableName));
        }

        // query
        var results = new PayrollResultService(HttpClient).QueryCollectorResultsAsync<Model.CollectorResult>(
            new(TenantId, payrollResultId), QueryValuesToQuery(queryValues)).Result;

        // setup columns
        var dataTable = new DataTable(tableName);
        dataTable.Columns.Add(nameof(Model.CollectorResult.Id), typeof(int));
        dataTable.Columns.Add(nameof(Model.CollectorResult.Status), typeof(int));
        dataTable.Columns.Add(nameof(Model.CollectorResult.Created), typeof(DateTime));
        dataTable.Columns.Add(nameof(Model.CollectorResult.Updated), typeof(DateTime));
        dataTable.Columns.Add(nameof(Model.CollectorResult.PayrollResultId), typeof(int));
        dataTable.Columns.Add(nameof(Model.CollectorResult.CollectorId), typeof(int));
        dataTable.Columns.Add(nameof(Model.CollectorResult.CollectorName), typeof(string));
        dataTable.Columns.Add(nameof(Model.CollectorResult.CollectorNameLocalizations), typeof(string));
        dataTable.Columns.Add(nameof(Model.CollectorResult.CollectMode), typeof(int));
        dataTable.Columns.Add(nameof(Model.CollectorResult.Negated), typeof(bool));
        dataTable.Columns.Add(nameof(Model.CollectorResult.ValueType), typeof(int));
        dataTable.Columns.Add(nameof(Model.CollectorResult.Value), typeof(decimal));
        dataTable.Columns.Add(nameof(Model.CollectorResult.Start), typeof(DateTime));
        dataTable.Columns.Add(nameof(Model.CollectorResult.End), typeof(DateTime));
        dataTable.Columns.Add(nameof(Model.CollectorResult.Tags), typeof(string));
        dataTable.Columns.Add(nameof(Model.CollectorResult.Attributes), typeof(string));

        // setup rows
        foreach (var result in results)
        {
            var row = dataTable.NewRow();
            row[nameof(Model.CollectorResult.Id)] = result.Id;
            row[nameof(Model.CollectorResult.Status)] = (int)result.Status;
            row[nameof(Model.CollectorResult.Created)] = result.Created;
            row[nameof(Model.CollectorResult.Updated)] = result.Updated;
            row[nameof(Model.CollectorResult.PayrollResultId)] = result.PayrollResultId;
            row[nameof(Model.CollectorResult.CollectorId)] = result.CollectorId;
            row[nameof(Model.CollectorResult.CollectorName)] = result.CollectorName;
            row[nameof(Model.CollectorResult.CollectorNameLocalizations)] = JsonSerializer.Serialize(result.CollectorNameLocalizations);
            row[nameof(Model.CollectorResult.CollectMode)] = (int)result.CollectMode;
            row[nameof(Model.CollectorResult.Negated)] = result.Negated;
            row[nameof(Model.CollectorResult.ValueType)] = (int)result.ValueType;
            row[nameof(Model.CollectorResult.Value)] = result.Value;
            row[nameof(Model.CollectorResult.Start)] = result.Start;
            row[nameof(Model.CollectorResult.End)] = result.End;
            row[nameof(Model.CollectorResult.Tags)] = JsonSerializer.Serialize(result.Tags);
            row[nameof(Model.CollectorResult.Attributes)] = JsonSerializer.Serialize(result.Attributes);
            dataTable.Rows.Add(row);
        }

        // commit changes
        dataTable.AcceptChanges();
        return dataTable;
    }

    /// <inheritdoc />
    public DataTable ExecuteCollectorCustomResultQuery(string tableName, int payrollResultId, int collectorResultId,
        Tuple<int?, string, string, string, long?, long?> queryValues)
    {
        // argument check
        if (string.IsNullOrWhiteSpace(tableName))
        {
            throw new ArgumentException(nameof(tableName));
        }

        // query
        var results = new PayrollResultService(HttpClient).QueryCollectorCustomResultsAsync<Model.CollectorCustomResult>(
            new(TenantId), payrollResultId, collectorResultId, QueryValuesToQuery(queryValues)).Result;

        // setup columns
        var dataTable = new DataTable(tableName);
        dataTable.Columns.Add(nameof(Model.CollectorCustomResult.Id), typeof(int));
        dataTable.Columns.Add(nameof(Model.CollectorCustomResult.Status), typeof(int));
        dataTable.Columns.Add(nameof(Model.CollectorCustomResult.Created), typeof(DateTime));
        dataTable.Columns.Add(nameof(Model.CollectorCustomResult.Updated), typeof(DateTime));
        dataTable.Columns.Add(nameof(Model.CollectorCustomResult.CollectorResultId), typeof(int));
        dataTable.Columns.Add(nameof(Model.CollectorCustomResult.CollectorName), typeof(string));
        dataTable.Columns.Add(nameof(Model.CollectorCustomResult.CollectorNameLocalizations), typeof(string));
        dataTable.Columns.Add(nameof(Model.CollectorCustomResult.ValueType), typeof(int));
        dataTable.Columns.Add(nameof(Model.CollectorCustomResult.Value), typeof(decimal));
        dataTable.Columns.Add(nameof(Model.CollectorCustomResult.Start), typeof(DateTime));
        dataTable.Columns.Add(nameof(Model.CollectorCustomResult.End), typeof(DateTime));
        dataTable.Columns.Add(nameof(Model.CollectorCustomResult.Tags), typeof(string));
        dataTable.Columns.Add(nameof(Model.CollectorCustomResult.Attributes), typeof(string));

        // setup rows
        foreach (var result in results)
        {
            var row = dataTable.NewRow();
            row[nameof(Model.CollectorCustomResult.Id)] = result.Id;
            row[nameof(Model.CollectorCustomResult.Status)] = (int)result.Status;
            row[nameof(Model.CollectorCustomResult.Created)] = result.Created;
            row[nameof(Model.CollectorCustomResult.Updated)] = result.Updated;
            row[nameof(Model.CollectorCustomResult.CollectorResultId)] = result.CollectorResultId;
            row[nameof(Model.CollectorCustomResult.CollectorName)] = result.CollectorName;
            row[nameof(Model.CollectorCustomResult.CollectorNameLocalizations)] = JsonSerializer.Serialize(result.CollectorNameLocalizations);
            row[nameof(Model.CollectorCustomResult.ValueType)] = (int)result.ValueType;
            row[nameof(Model.CollectorCustomResult.Value)] = result.Value;
            row[nameof(Model.CollectorCustomResult.Start)] = result.Start;
            row[nameof(Model.CollectorCustomResult.End)] = result.End;
            row[nameof(Model.CollectorCustomResult.Tags)] = JsonSerializer.Serialize(result.Tags);
            row[nameof(Model.CollectorCustomResult.Attributes)] = JsonSerializer.Serialize(result.Attributes);
            dataTable.Rows.Add(row);
        }

        // commit changes
        dataTable.AcceptChanges();
        return dataTable;
    }

    /// <inheritdoc />
    public DataTable ExecutePayrunResultQuery(string tableName, int payrollResultId, Tuple<int?, string, string, string, long?, long?> queryValues)
    {
        // argument check
        if (string.IsNullOrWhiteSpace(tableName))
        {
            throw new ArgumentException(nameof(tableName));
        }

        // query
        var results = new PayrollResultService(HttpClient).QueryPayrunResultsAsync<PayrunResult>(
            new(TenantId, payrollResultId), QueryValuesToQuery(queryValues)).Result;

        // setup columns
        var dataTable = new DataTable(tableName);
        dataTable.Columns.Add(nameof(PayrunResult.Id), typeof(int));
        dataTable.Columns.Add(nameof(PayrunResult.Status), typeof(int));
        dataTable.Columns.Add(nameof(PayrunResult.Created), typeof(DateTime));
        dataTable.Columns.Add(nameof(PayrunResult.Updated), typeof(DateTime));
        dataTable.Columns.Add(nameof(PayrunResult.PayrollResultId), typeof(int));
        dataTable.Columns.Add(nameof(PayrunResult.Source), typeof(string));
        dataTable.Columns.Add(nameof(PayrunResult.Name), typeof(string));
        dataTable.Columns.Add(nameof(PayrunResult.NameLocalizations), typeof(string));
        dataTable.Columns.Add(nameof(PayrunResult.Slot), typeof(string));
        dataTable.Columns.Add(nameof(PayrunResult.ValueType), typeof(int));
        dataTable.Columns.Add(nameof(PayrunResult.Value), typeof(string));
        dataTable.Columns.Add(nameof(PayrunResult.NumericValue), typeof(decimal));
        dataTable.Columns.Add(nameof(PayrunResult.Start), typeof(DateTime));
        dataTable.Columns.Add(nameof(PayrunResult.End), typeof(DateTime));
        dataTable.Columns.Add(nameof(PayrunResult.Tags), typeof(string));
        dataTable.Columns.Add(nameof(PayrunResult.Attributes), typeof(string));

        // setup rows
        foreach (var result in results)
        {
            var row = dataTable.NewRow();
            row[nameof(PayrunResult.Id)] = result.Id;
            row[nameof(PayrunResult.Status)] = (int)result.Status;
            row[nameof(PayrunResult.Created)] = result.Created;
            row[nameof(PayrunResult.Updated)] = result.Updated;
            row[nameof(PayrunResult.PayrollResultId)] = result.PayrollResultId;
            row[nameof(PayrunResult.Source)] = result.Source;
            row[nameof(PayrunResult.Name)] = result.Name;
            row[nameof(PayrunResult.NameLocalizations)] = JsonSerializer.Serialize(result.NameLocalizations);
            row[nameof(PayrunResult.Slot)] = result.Slot;
            row[nameof(PayrunResult.ValueType)] = (int)result.ValueType;
            row[nameof(PayrunResult.Value)] = result.Value;
            if (result.NumericValue.HasValue)
            {
                row[nameof(PayrunResult.NumericValue)] = result.NumericValue;
            }
            row[nameof(PayrunResult.Start)] = result.Start;
            row[nameof(PayrunResult.End)] = result.End;
            row[nameof(PayrunResult.Tags)] = JsonSerializer.Serialize(result.Tags);
            row[nameof(PayrunResult.Attributes)] = JsonSerializer.Serialize(result.Attributes);
            dataTable.Rows.Add(row);
        }

        // commit changes
        dataTable.AcceptChanges();
        return dataTable;
    }

    private static Query QueryValuesToQuery(Tuple<int?, string, string, string, long?, long?> queryValues) =>
        QueryValuesToQuery<Query>(queryValues);

    private static T QueryValuesToQuery<T>(Tuple<int?, string, string, string, long?, long?> queryValues)
        where T : Query, new() => new()
        {
            Status = (PayrollEngine.ObjectStatus?)queryValues.Item1,
            Filter = queryValues.Item2,
            OrderBy = queryValues.Item3,
            Select = queryValues.Item4,
            Top = queryValues.Item5,
            Skip = queryValues.Item6
        };

    #endregion

    #region Report Log

    /// <inheritdoc />
    public void AddReportLog(string message, string key = null, DateTime? reportDate = null)
    {
        _ = new ReportLogService(HttpClient).CreateAsync(new(TenantId), new ReportLog
        {
            ReportName = ReportName,
            ReportDate = reportDate ?? Date.Now,
            User = User.Identifier,
            Message = message,
            Key = key
        }).Result;
    }

    #endregion

}