using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backstop.Samples.RestReports
{
    public class BackstopRestReportUri
    {
        public string BasePath { get { return "/backstop/rest/reports"; } }
        public string Service { get; private set; }
        public string Method { get; private set; }
        public Uri Uri
        {
            get
            {
                return new Uri(string.Join("/", BasePath, Service, Method), UriKind.Relative);
            }
        }

        public BackstopRestReportUri(string service, string method)
        {
            if (string.IsNullOrEmpty(service))
                throw new ArgumentNullException("service");
            if (string.IsNullOrEmpty(method))
                throw new ArgumentNullException("method");

            this.Service = service;
            this.Method = method;
        }

        /// <summary>
        ///     Get the list of supported reporting methods supported via the REST API.
        /// </summary>
        /// <returns></returns>
        public static List<BackstopRestReportUri> SupportedMethods()
        {
            var methods = new BackstopRestReportUri[]
            {
                new BackstopRestReportUri("crm", "runPeopleOrgsReport"),
                new BackstopRestReportUri("crm", "runActivityReport"),
                new BackstopRestReportUri("crm", "runOpportunityReport"),
                new BackstopRestReportUri("crm", "runTaskReport"),
                new BackstopRestReportUri("assetGroup", "getAllAssetGroupIds"),
                new BackstopRestReportUri("assetGroup", "getAllAssetTypes"),
                new BackstopRestReportUri("assetGroup", "getAllAssetTypesById"),
                new BackstopRestReportUri("assetGroup", "getPerformanceForAllAssetTypeLevelsByAssetGroup"),
                new BackstopRestReportUri("assetGroup", "getAssetGroupTransactions"),
                new BackstopRestReportUri("assetGroup", "getCumAndAnnBenchmarkValuesAsset"),
                new BackstopRestReportUri("assetGroup", "runAssetGroupReport"),
                new BackstopRestReportUri("assetGroup", "runAssetGroupParticipantTransactionsReport"),
                new BackstopRestReportUri("assetGroup", "runAssetGroupParticipantReport"),
                new BackstopRestReportUri("assetGroup", "runAssetTypeLevelAssetGroupReport"),
                new BackstopRestReportUri("assetGroup", "runAssetGroupWeightedBenchmarkReport"),
                new BackstopRestReportUri("assetGroup", "getAssetGroupWeightedBenchmarkVAMIvalues"),
                new BackstopRestReportUri("assetGroup", "getAssetGroupParticipantBalanceCarryForwardInfo"),
                new BackstopRestReportUri("assetGroup", "runAssetGroupBenchmarksReport"),
                new BackstopRestReportUri("assetGroup", "getAllAssetGroupSubGroupIds"),
                new BackstopRestReportUri("investor", "runAccountsQuery"),
                new BackstopRestReportUri("investor", "runInvestorTransactionsQuery"),
                new BackstopRestReportUri("investor", "runAssetGroupQuery"),
                new BackstopRestReportUri("investor", "runTrancheQuery"),
                new BackstopRestReportUri("investor", "runShareSeriesQuery"),
                new BackstopRestReportUri("portfolio", "runHoldingsReport"),
                new BackstopRestReportUri("portfolio", "runFundsReport"),
                new BackstopRestReportUri("portfolio", "runPortfolioTransactionsReport"),
                new BackstopRestReportUri("portfolio", "runProductsReport"),
                new BackstopRestReportUri("portfolio", "runPortfolioPlansReport"),
                new BackstopRestReportUri("portfolio", "runPortfolioPlanInvestmentsReport"),
                new BackstopRestReportUri("portfolio", "runBenchmarksReport"),
                new BackstopRestReportUri("portfolio", "runGeneralLedgerEntriesReport"),
                new BackstopRestReportUri("portfolio", "runGeneralLedgerSourceAccountReport"),
                new BackstopRestReportUri("portfolio", "runSwapsReport"),
                new BackstopRestReportUri("relationship", "runFundRelationshipReport"),
                new BackstopRestReportUri("relationship", "runGeneralRelationshipReport"),
                new BackstopRestReportUri("relationship", "runRelationshipDeliveryMethodReport"),
            };

            var list = methods.ToList();
            list.Sort((lhs, rhs) => {
                int i = lhs.Service.CompareTo(rhs.Service);
                if (i == 0)
                    return lhs.Method.CompareTo(rhs.Method);
                return i;
            });
            return list;
        }
    }
}
