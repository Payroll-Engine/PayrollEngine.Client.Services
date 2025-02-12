using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PayrollEngine.Client.Model;
using PayrollEngine.Client.Service.Api;

namespace PayrollEngine.Client.Scripting.Function.Api
{
    /// <summary>Report parameter parser</summary>
    public class ReportParameterParser
    {
        /// <summary>The Payroll http client</summary>
        public PayrollHttpClient HttpClient { get; }

        /// <summary>The tenant id</summary>
        public int TenantId { get; }

        /// <summary>The regulation id</summary>
        public int? RegulationId { get; }

        /// <summary>Parser constructor</summary>
        /// <param name="httpClient">The Payroll http configuration</param>
        /// <param name="tenantId">The tenant id</param>
        /// <param name="regulationId">The regulation id</param>
        public ReportParameterParser(PayrollHttpClient httpClient, int tenantId,
            int? regulationId = null)
        {
            HttpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            TenantId = tenantId;
            RegulationId = regulationId;
        }

        /// <summary>Get report</summary>
        /// <param name="parameters">The report parameters</param>
        public async System.Threading.Tasks.Task ParseParametersAsync(Dictionary<string, string> parameters)
        {
            if (parameters == null)
            {
                throw new ArgumentNullException(nameof(parameters));
            }

            if (parameters.Any())
            {
                return;
            }

            // employee
            if (parameters.TryGetValue("EmployeeId", out var employeeParameter) &&
                !int.TryParse(employeeParameter, out _))
            {
                parameters["EmployeeId"] = (await GetEmployeeAsync(employeeParameter)).Id.ToString();
            }

            // regulation
            if (parameters.TryGetValue("RegulationId", out var regulationParameter) &&
                !int.TryParse(regulationParameter, out _))
            {
                parameters["RegulationId"] = (await GetRegulationAsync(regulationParameter)).Id.ToString();
            }

            // payroll
            if (parameters.TryGetValue("PayrollId", out var payrollParameter) &&
                !int.TryParse(payrollParameter, out _))
            {
                parameters["PayrollId"] = (await GetPayrollAsync(payrollParameter)).Id.ToString();
            }

            // payrun
            if (parameters.TryGetValue("PayrunId", out var payrunParameter) &&
                !int.TryParse(payrunParameter, out _))
            {
                parameters["PayrunId"] = (await GetPayrunAsync(payrunParameter)).Id.ToString();
            }

            // webhook
            if (parameters.TryGetValue("WebhookId", out var webhookParameter) &&
                !int.TryParse(webhookParameter, out _))
            {
                parameters["WebhookId"] = (await GetWebhookAsync(webhookParameter)).Id.ToString();
            }

            // report
            if (RegulationId.HasValue)
            {
                if (parameters.TryGetValue("ReportId", out var reportParameter) &&
                    !int.TryParse(reportParameter, out _))
                {
                    parameters["ReportId"] = (await GetReportAsync(reportParameter)).Id.ToString();
                }
            }
        }

        private async Task<Employee> GetEmployeeAsync(string employeeIdentifier)
        {
            var employee = await new EmployeeService(HttpClient).GetAsync<Employee>(new(TenantId), employeeIdentifier);
            if (employee == null)
            {
                throw new ScriptException($"Unknown employee {employeeIdentifier}.");
            }
            return employee;
        }

        private async Task<Regulation> GetRegulationAsync(string regulationName)
        {
            var regulation = await new RegulationService(HttpClient).GetAsync<Regulation>(new(TenantId), regulationName);
            if (regulation == null)
            {
                throw new ScriptException($"Unknown regulation {regulationName}.");
            }
            return regulation;
        }

        private async Task<Payroll> GetPayrollAsync(string payrollName)
        {
            var payroll = await new PayrollService(HttpClient).GetAsync<Payroll>(new(TenantId), payrollName);
            if (payroll == null)
            {
                throw new ScriptException($"Unknown payroll {payrollName}.");
            }
            return payroll;
        }

        private async Task<Payrun> GetPayrunAsync(string payrunName)
        {
            var payrun = await new PayrunService(HttpClient).GetAsync<Payrun>(new(TenantId), payrunName);
            if (payrun == null)
            {
                throw new ScriptException($"Unknown payrun {payrunName}.");
            }
            return payrun;
        }

        private async Task<Webhook> GetWebhookAsync(string webhookName)
        {
            var webhook = await new WebhookService(HttpClient).GetAsync<Webhook>(new(TenantId), webhookName);
            if (webhook == null)
            {
                throw new ScriptException($"Unknown webhook {webhookName}.");
            }
            return webhook;
        }

        private async Task<Model.Report> GetReportAsync(string reportName)
        {
            // report
            if (!RegulationId.HasValue)
            {
                return null;
            }

            var report = await new ReportService(HttpClient).GetAsync<Model.Report>(new(TenantId, RegulationId.Value), reportName);
            if (report == null)
            {
                throw new ScriptException($"Unknown report {reportName}.");
            }
            return report;
        }
    }
}
