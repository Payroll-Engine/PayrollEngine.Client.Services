using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using PayrollEngine.Client.Model;
using PayrollEngine.Client.Scripting.Function;

namespace PayrollEngine.Client.Scripting.Runtime.Api;

/// <summary>Runtime for the report end function</summary>
public class ReportEndRuntime : ReportRuntime, IReportEndRuntime
{
    /// <inheritdoc />
    public DataSet DataSet { get; }

    /// <summary>Initializes a new instance of the <see cref="ReportEndRuntime"/> class</summary>
    /// <param name="httpClient">The Payroll http client</param>
    /// <param name="tenantId">The tenant id</param>
    /// <param name="userId">The user id</param>
    /// <param name="report">The report</param>
    /// <param name="reportRequest">The report request</param>
    /// <param name="dataSet">The report data set</param>
    public ReportEndRuntime(PayrollHttpClient httpClient, int tenantId,
        int userId, ReportSet report, ReportRequest reportRequest, DataSet dataSet = null) :
        base(httpClient, tenantId, userId, report, reportRequest)
    {
        DataSet = dataSet ?? new DataSet();
    }

    /// <summary>The log owner type</summary>
    protected override string LogOwnerType => nameof(ReportEndFunction);

    /// <inheritdoc />
    public DataTable ExecuteQuery(string tableName, string methodName, string culture,
        Dictionary<string, string> parameters, bool resultQuery)
    {
        var table = ExecuteQuery(tableName, methodName, culture, parameters);
        if (resultQuery && table != null)
        {
            // result table
            if (DataSet.Tables.Contains(tableName))
            {
                // remove previous table
                DataSet.Tables.Remove(tableName);
            }
            // new table
            DataSet.Tables.Add(table);
        }
        return table;
    }

    /// <inheritdoc />
    public DataTable ExecuteMergeQuery(string tableName, string methodName, string culture, string mergeColumn,
        Dictionary<string, string> parameters, int schemaChange)
    {
        if (string.IsNullOrWhiteSpace(tableName))
        {
            throw new ArgumentException(nameof(tableName));
        }
        if (string.IsNullOrWhiteSpace(methodName))
        {
            throw new ArgumentException(nameof(methodName));
        }
        // schema change
        if (!Enum.IsDefined((DataMergeSchemaChange)schemaChange))
        {
            throw new PayrollException($"Invalid schema change: {schemaChange}");
        }

        // culture
        culture ??= CultureInfo.CurrentCulture.Name;

        // report query
        var queryTable = TenantService.ExecuteReportQueryAsync(TenantId, methodName, culture, parameters).Result;
        if (queryTable == null)
        {
            if (!DataSet.Tables.Contains(tableName))
            {
                DataSet.Tables.Add(tableName);
            }
            return DataSet.Tables[tableName];
        }

        var resultTable = Data.DataTableConversionExtensions.ToSystemDataTable(queryTable);
        resultTable.TableName = tableName;
        if (resultTable.Rows.Count > 0 && !string.IsNullOrWhiteSpace(mergeColumn))
        {
            if (!resultTable.Columns.Contains(mergeColumn))
            {
                throw new ScriptException($"Unknown merge column {mergeColumn}");
            }
            resultTable.PrimaryKey = new[] { resultTable.Columns[mergeColumn] };
        }
        resultTable.AcceptChanges();

        // result table
        var dataTable = DataSet.Tables[tableName];
        if (dataTable == null)
        {
            // new table
            DataSet.Tables.Add(resultTable);
            dataTable = DataSet.Tables[tableName];
        }
        if (dataTable != null && resultTable.Rows.Count > 0)
        {
            // merge with existing table with schema change support
            dataTable.Merge(resultTable, false, (MissingSchemaAction)schemaChange);
            dataTable.AcceptChanges();
        }

        return resultTable;
    }
}