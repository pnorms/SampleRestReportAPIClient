using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RestSharp;
using Newtonsoft.Json;

namespace Backstop.Samples.RestReports
{
    public class ReportClient
    {
        public Uri BackstopUrl { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }

        public Uri ReportRestMethod { get; set; }
        public string QueryDefinition { get; set; }
        public string RestrictionExpression { get; set; }
        public DateTime AsOfDate { get; set; }
        public string Grouping { get; set; }

        public ReportClient()
        {
        }

        RestClient CreateClient()
        {
            if (this.BackstopUrl == null)
                throw new InvalidOperationException("BackstopUrl has not been initialized.");
            if (string.IsNullOrEmpty(this.Username))
                throw new InvalidOperationException("Username has not been initialized.");
            if (string.IsNullOrEmpty(this.Password))
                throw new InvalidOperationException("Password has not been initialized.");

            return new RestClient(this.BackstopUrl)
            {
                Authenticator = new HttpBasicAuthenticator(this.Username, this.Password),
                Timeout = 2000000
                
            };
        }

        RestRequest CreateRequest()
        {
            if (this.ReportRestMethod == null)
                throw new InvalidOperationException("ReportRestMethod has not been initialized.");

            var request = new RestRequest(this.ReportRestMethod, Method.POST);
            request.AddParameter("queryDefinition", this.QueryDefinition);
            request.AddParameter("restrictionExpression", this.RestrictionExpression);
            request.AddParameter("asOf", this.AsOfDate.ToString("yyyy-MM-dd"));
            request.AddParameter("grouping", this.Grouping);
            return request;
        }

        public string Invoke()
        {
            var client = CreateClient();
            var request = CreateRequest();
            var response = client.Execute(request);

            if (response.ErrorException != null)
                throw new ApplicationException("Error executing report. Check inner details for more info.", response.ErrorException);

            return response.Content;
        }

        public Task<string> InvokeAsync()
        {
            var client = CreateClient();
            var request = CreateRequest();
            var tcs = new TaskCompletionSource<string>();

            client.ExecuteAsync(request, response =>
                {
                    if (response.ErrorException == null)
                        tcs.SetResult(response.Content);
                    else
                        tcs.SetException(new ApplicationException("Error executing report. Check inner details for more info.", response.ErrorException));
                });

            return tcs.Task;
        }

        public static Uri ConvertSoapPathToRest(string path, string method)
        {
            const string root = "/backstop/rest/reports/";
            string service;

            if (path.Contains("CrmQuery"))
                service = "crm";
            else if (path.Contains("PortfolioQuery"))
                service = "portfolio";
            else if (path.Contains("InvestorQuery"))
                service = "investor";
            else if (path.Contains("RelationshipQuery"))
                service = "relationship";
            else if (path.Contains("AssetGroup"))
                service = "assetGroup";
            else
                throw new ArgumentException(String.Format("{0} is not a valid path", path));

            return new Uri(root + service + "/" + method, UriKind.Relative);
        }
    }
}
