using System;
using System.Linq;
using PayrollEngine.Client.Model;
using PayrollEngine.Client.Service.Api;

namespace PayrollEngine.Client.Scripting.Runtime.Api;

/// <summary>Runtime for the case change function</summary>
public abstract class CaseChangeRuntimeBase : CaseRuntimeBase, ICaseChangeRuntime
{
    /// <summary>The case set</summary>
    protected new CaseSet Case => (CaseSet)base.Case;

    /// <summary>Initializes a new instance of the <see cref="CaseChangeRuntimeBase"/> class</summary>
    /// <param name="httpClient">The Payroll http client</param>
    /// <param name="calendar">The calendar</param>
    /// <param name="tenantId">The tenant id</param>
    /// <param name="userId">The user id</param>
    /// <param name="payrollId">The payroll id</param>
    /// <param name="caseSet">The runtime case set</param>
    /// <param name="employeeId">The employee id</param>
    protected CaseChangeRuntimeBase(PayrollHttpClient httpClient, ScriptCalendar calendar, int tenantId,
        int userId, int payrollId, CaseSet caseSet, int? employeeId = null) :
        base(httpClient, calendar, tenantId, userId, payrollId, caseSet, employeeId)
    {
    }

    #region Case

    /// <inheritdoc />
    public DateTime? CancellationDate => Case.CancellationDate;

    /// <inheritdoc />
    public bool CaseAvailable(string caseName) =>
        GetCaseSet(caseName) != null;

    /// <inheritdoc />
    public void SetCaseAttribute(string caseName, string attributeName, object value)
    {
        if (string.IsNullOrWhiteSpace(caseName))
        {
            throw new ArgumentException(nameof(caseName));
        }
        if (string.IsNullOrWhiteSpace(attributeName))
        {
            throw new ArgumentException(nameof(attributeName));
        }

        // ensure attribute collection
        Case.Attributes ??= new();

        // set or update attribute value
        Case.Attributes[attributeName] = value ?? throw new ArgumentNullException(nameof(value));
    }

    /// <inheritdoc />
    public bool RemoveCaseAttribute(string caseName, string attributeName)
    {
        if (string.IsNullOrWhiteSpace(caseName))
        {
            throw new ArgumentException(nameof(caseName));
        }
        if (string.IsNullOrWhiteSpace(attributeName))
        {
            throw new ArgumentException(nameof(attributeName));
        }

        // missing attribute
        if (Case.Attributes == null || !Case.Attributes.ContainsKey(attributeName))
        {
            return false;
        }

        // remove attribute
        return Case.Attributes.Remove(attributeName);
    }

    /// <summary>
    /// Get case by name
    /// </summary>
    /// <param name="caseName">The name of the case</param>
    /// <returns>The case set matching the name, script exception on missing case</returns>
    protected CaseSet GetCaseSet(string caseName)
    {
        if (string.IsNullOrWhiteSpace(caseName))
        {
            throw new ArgumentException(nameof(caseName));
        }
        // cache or search
        return Case.FindCase(caseName) ?? PayrollService.BuildCaseAsync<CaseSet>(
            new(TenantId, PayrollId), caseName, UserId, EmployeeId).Result;
    }

    #endregion

    #region Case Fields

    /// <inheritdoc />
    public string[] GetFieldNames() =>
        Case.Fields?.Select(x => x.Name).ToArray();

    /// <inheritdoc />
    public bool HasFields() =>
        Case.Fields?.Any() ?? false;

    /// <inheritdoc />
    public bool HasField(string caseFieldName) =>
        Case.Fields?.Any(x => string.Equals(caseFieldName, x.Name)) ?? false;

    /// <inheritdoc />
    public bool IsFieldComplete(string caseFieldName) =>
        GetCaseFieldSet(caseFieldName).IsComplete();

    /// <inheritdoc />
    public bool IsFieldEmpty(string caseFieldName) =>
        !HasStart(caseFieldName) && !HasEnd(caseFieldName) && !HasValue(caseFieldName);
    /// <inheritdoc />
    public bool FieldAvailable(string caseFieldName) =>
        GetCaseFieldSet(caseFieldName).Status == PayrollEngine.ObjectStatus.Active;

    /// <inheritdoc />
    public void FieldAvailable(string caseFieldName, bool available) =>
        GetCaseFieldSet(caseFieldName).Status = available ?
            PayrollEngine.ObjectStatus.Active :
            PayrollEngine.ObjectStatus.Inactive;

    /// <inheritdoc />
    public bool HasStart(string caseFieldName) =>
        GetCaseFieldSet(caseFieldName).Start != null;

    /// <inheritdoc />
    public DateTime? GetStart(string caseFieldName) =>
        GetCaseFieldSet(caseFieldName).Start;

    /// <inheritdoc />
    public void SetStart(string caseFieldName, DateTime? start) =>
        GetCaseFieldSet(caseFieldName).Start = start;

    /// <inheritdoc />
    public void InitStart(string caseFieldName, DateTime? start)
    {
        CaseFieldSet caseFieldSet = GetCaseFieldSet(caseFieldName, true);
        caseFieldSet.Start ??= start;
    }

    /// <inheritdoc />
    public bool HasEnd(string caseFieldName) =>
        GetCaseFieldSet(caseFieldName).End != null;

    /// <inheritdoc />
    public DateTime? GetEnd(string caseFieldName) =>
        GetCaseFieldSet(caseFieldName).End;

    /// <inheritdoc />
    public void SetEnd(string caseFieldName, DateTime? end) =>
        GetCaseFieldSet(caseFieldName).End = end;

    /// <inheritdoc />
    public void InitEnd(string caseFieldName, DateTime? end)
    {
        CaseFieldSet caseFieldSet = GetCaseFieldSet(caseFieldName, true);
        caseFieldSet.End ??= end;
    }

    /// <inheritdoc />
    public int GetValueType(string caseFieldName) =>
        (int)GetCaseFieldSet(caseFieldName).ValueType;

    /// <inheritdoc />
    public bool HasValue(string caseFieldName) =>
        GetCaseFieldSet(caseFieldName).HasValue;

    /// <inheritdoc />
    public object GetValue(string caseFieldName) =>
        GetCaseFieldSet(caseFieldName).GetValue(TenantCulture);

    /// <inheritdoc />
    public void SetValue(string caseFieldName, object value) =>
        GetCaseFieldSet(caseFieldName).SetValue(value);

    /// <inheritdoc />
    public void InitValue(string caseFieldName, object value)
    {
        CaseFieldSet caseFieldSet = GetCaseFieldSet(caseFieldName, true);
        if (caseFieldSet.Value == null)
        {
            caseFieldSet.SetValue(value);
        }
    }

    /// <inheritdoc />
    public bool AddCaseValueTag(string caseFieldName, string tag)
    {
        if (string.IsNullOrWhiteSpace(tag))
        {
            throw new ArgumentException(nameof(tag));
        }

        CaseFieldSet caseFieldSet = GetCaseFieldSet(caseFieldName);
        if (caseFieldSet == null)
        {
            return false;
        }
        caseFieldSet.Tags ??= [];
        if (!caseFieldSet.Tags.Contains(tag))
        {
            caseFieldSet.Tags.Add(tag);
        }
        return caseFieldSet.Tags.Contains(tag);
    }

    /// <inheritdoc />
    public bool RemoveCaseValueTag(string caseFieldName, string tag)
    {
        if (string.IsNullOrWhiteSpace(tag))
        {
            throw new ArgumentException(nameof(tag));
        }

        CaseFieldSet caseFieldSet = GetCaseFieldSet(caseFieldName);
        if (caseFieldSet?.Tags == null || !caseFieldSet.Tags.Contains(tag))
        {
            return false;
        }
        caseFieldSet.Tags.Remove(tag);
        return !caseFieldSet.Tags.Contains(tag);
    }

    /// <inheritdoc />
    public override object GetCaseFieldAttribute(string caseFieldName, string attributeName) =>
        GetCaseFieldSet(caseFieldName)?.Attributes?.GetValue<object>(attributeName);

    /// <inheritdoc />
    public void SetCaseFieldAttribute(string caseFieldName, string attributeName, object value)
    {
        if (string.IsNullOrWhiteSpace(caseFieldName))
        {
            throw new ArgumentException(nameof(caseFieldName));
        }
        if (string.IsNullOrWhiteSpace(attributeName))
        {
            throw new ArgumentException(nameof(attributeName));
        }

        // case field
        var caseField = GetCaseFieldSet(caseFieldName);
        if (caseField == null)
        {
            throw new ArgumentException($"unknown case field {caseFieldName}.");
        }
        // ensure case field attribute collection
        caseField.Attributes ??= new();

        // set or update case field attribute value
        caseField.Attributes[attributeName] = value ?? throw new ArgumentNullException(nameof(value));
    }

    /// <inheritdoc />
    public bool RemoveCaseFieldAttribute(string caseFieldName, string attributeName)
    {
        if (string.IsNullOrWhiteSpace(caseFieldName))
        {
            throw new ArgumentException(nameof(caseFieldName));
        }
        if (string.IsNullOrWhiteSpace(attributeName))
        {
            throw new ArgumentException(nameof(attributeName));
        }

        // case field
        var caseField = GetCaseFieldSet(caseFieldName);
        if (caseField == null)
        {
            throw new ArgumentException($"unknown case field {caseFieldName}.");
        }
        if (caseField.Attributes == null || !caseField.Attributes.ContainsKey(attributeName))
        {
            return false;
        }

        // remove case field attribute
        return caseField.Attributes.Remove(attributeName);
    }

    /// <inheritdoc />
    public override object GetCaseValueAttribute(string caseFieldName, string attributeName) =>
        GetCaseFieldSet(caseFieldName)?.ValueAttributes?.GetValue<object>(attributeName);

    /// <inheritdoc />
    public void SetCaseValueAttribute(string caseFieldName, string attributeName, object value)
    {
        if (string.IsNullOrWhiteSpace(caseFieldName))
        {
            throw new ArgumentException(nameof(caseFieldName));
        }
        if (string.IsNullOrWhiteSpace(attributeName))
        {
            throw new ArgumentException(nameof(attributeName));
        }

        // case field
        var caseField = GetCaseFieldSet(caseFieldName);
        if (caseField == null)
        {
            throw new ArgumentException($"unknown case field {caseFieldName}.");
        }
        // ensure  case value attribute collection
        caseField.ValueAttributes ??= new();

        // set or update case value attribute value
        caseField.ValueAttributes[attributeName] = value ?? throw new ArgumentNullException(nameof(value));
    }

    /// <inheritdoc />
    public bool RemoveCaseValueAttribute(string caseFieldName, string attributeName)
    {
        if (string.IsNullOrWhiteSpace(caseFieldName))
        {
            throw new ArgumentException(nameof(caseFieldName));
        }
        if (string.IsNullOrWhiteSpace(attributeName))
        {
            throw new ArgumentException(nameof(attributeName));
        }

        // case field
        var caseField = GetCaseFieldSet(caseFieldName);
        if (caseField == null)
        {
            throw new ArgumentException($"unknown case field {caseFieldName}.");
        }
        if (caseField.ValueAttributes == null || !caseField.ValueAttributes.ContainsKey(attributeName))
        {
            return false;
        }

        // remove case value attribute
        return caseField.ValueAttributes.Remove(attributeName);
    }

    /// <summary>
    /// Get case field by name
    /// </summary>
    /// <param name="caseFieldName">The name of the case field</param>
    /// <param name="addField">Add unknown field</param>
    /// <returns>The derived case field</returns>
    protected CaseFieldSet GetCaseFieldSet(string caseFieldName, bool addField = false)
    {
        var caseFieldSet = Case.FindCaseField(caseFieldName);
        if (caseFieldSet == null)
        {
            var caseField = new PayrollService(HttpClient).GetCaseFieldsAsync<CaseField>(
                new(TenantId, PayrollId), [caseFieldName]).Result.FirstOrDefault();
            if (caseField == null)
            {
                throw new ScriptException($"Unknown case field {caseFieldName}.");
            }
            caseFieldSet = new(caseField);
            if (addField)
            {
                Case.Fields.Add(caseFieldSet);
            }
        }
        return caseFieldSet;
    }

    #endregion

}