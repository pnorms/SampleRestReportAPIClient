using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Backstop.Samples.RestReports
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region Properties
        public string BackstopUrl
        {
            get { return (string)GetValue(BackstopUrlProperty); }
            set { SetValue(BackstopUrlProperty, value); }
        }

        // Using a DependencyProperty as the backing store for BackstopUrl.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty BackstopUrlProperty =
            DependencyProperty.Register("BackstopUrl", typeof(string), typeof(MainWindow), new PropertyMetadata("https://yourcompany.backstopsolutions.com"));


        public string Username
        {
            get { return (string)GetValue(UsernameProperty); }
            set { SetValue(UsernameProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Username.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty UsernameProperty =
            DependencyProperty.Register("Username", typeof(string), typeof(MainWindow), new PropertyMetadata(string.Empty));

        public string QueryDefinition
        {
            get { return (string)GetValue(QueryDefinitionProperty); }
            set { SetValue(QueryDefinitionProperty, value); }
        }

        // Using a DependencyProperty as the backing store for QueryDefinition.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty QueryDefinitionProperty =
            DependencyProperty.Register("QueryDefinition", typeof(string), typeof(MainWindow), new PropertyMetadata(string.Empty));



        public string RestrictionExpression
        {
            get { return (string)GetValue(RestrictionExpressionProperty); }
            set { SetValue(RestrictionExpressionProperty, value); }
        }

        // Using a DependencyProperty as the backing store for RestrictionExpression.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty RestrictionExpressionProperty =
            DependencyProperty.Register("RestrictionExpression", typeof(string), typeof(MainWindow), new PropertyMetadata(string.Empty));



        public DateTime AsOfDate
        {
            get { return (DateTime)GetValue(AsOfDateProperty); }
            set { SetValue(AsOfDateProperty, value); }
        }

        // Using a DependencyProperty as the backing store for AsOfDate.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty AsOfDateProperty =
            DependencyProperty.Register("AsOfDate", typeof(DateTime), typeof(MainWindow), new PropertyMetadata(DateTime.Today));



        public List<string> Services
        {
            get { return (List<string>)GetValue(ServicesProperty); }
            set { SetValue(ServicesProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Services.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ServicesProperty =
            DependencyProperty.Register("Services", typeof(List<string>), typeof(MainWindow), new PropertyMetadata(new List<string>()));



        public string SelectedService
        {
            get { return (string)GetValue(SelectedServiceProperty); }
            set { SetValue(SelectedServiceProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SelectedService.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SelectedServiceProperty =
            DependencyProperty.Register("SelectedService", typeof(string), typeof(MainWindow), new PropertyMetadata(new PropertyChangedCallback(SelectedServiceChanged)));



        public List<BackstopRestReportUri> Methods
        {
            get { return (List<BackstopRestReportUri>)GetValue(MethodsProperty); }
            set { SetValue(MethodsProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Methods.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MethodsProperty =
            DependencyProperty.Register("Methods", typeof(List<BackstopRestReportUri>), typeof(MainWindow), new PropertyMetadata(new List<BackstopRestReportUri>()));



        public BackstopRestReportUri SelectedMethod
        {
            get { return (BackstopRestReportUri)GetValue(SelectedMethodProperty); }
            set { SetValue(SelectedMethodProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SelectedMethod.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SelectedMethodProperty =
            DependencyProperty.Register("SelectedMethod", typeof(BackstopRestReportUri), typeof(MainWindow));



        public string Result
        {
            get { return (string)GetValue(ResultProperty); }
            set { SetValue(ResultProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Result.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ResultProperty =
            DependencyProperty.Register("Result", typeof(string), typeof(MainWindow), new PropertyMetadata(string.Empty));



        public System.Data.DataTable DataTable
        {
            get { return (System.Data.DataTable)GetValue(DataTableProperty); }
            set { SetValue(DataTableProperty, value); }
        }

        // Using a DependencyProperty as the backing store for DataTable.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DataTableProperty =
            DependencyProperty.Register("DataTable", typeof(System.Data.DataTable), typeof(MainWindow), new PropertyMetadata(new PropertyChangedCallback(DataTableChanged)));

        #endregion

        public MainWindow()
        {
            this.DataContext = this;

            InitializeComponent();

            this.Services = BackstopRestReportUri.SupportedMethods().Select(u => u.Service).Distinct().ToList();
        }

        static void SelectedServiceChanged(DependencyObject source, DependencyPropertyChangedEventArgs args)
        {
            var this_ = (MainWindow)source;
            this_.SelectedMethod = null;

            string value = (string)args.NewValue;
            if (string.IsNullOrEmpty(value))
            {
                this_.Methods = new List<BackstopRestReportUri>();
            }
            else
            {
                this_.Methods = BackstopRestReportUri.SupportedMethods().Where(u => u.Service == value).ToList();
            }
        }

        private async void Submit(object sender, RoutedEventArgs e)
        {
            buttonSubmit.IsEnabled = false;

            try
            {
                var client = new ReportClient()
                {
                    BackstopUrl = new Uri(this.BackstopUrl),
                    Username = this.Username,
                    Password = this.txtPassword.Password,
                    ReportRestMethod = this.SelectedMethod.Uri,
                    QueryDefinition = this.QueryDefinition,
                    RestrictionExpression = this.RestrictionExpression,
                    AsOfDate = this.AsOfDate
                };

                this.Result = await client.InvokeAsync();
                this.DataTable = ReportParser.CreateDataTable(this.Result);
            }
            catch(Exception ex)
            {
                this.Result = ex.ToString();
            }
            finally
            {
                buttonSubmit.IsEnabled = true;
            }
        }

        static void DataTableChanged(DependencyObject source, DependencyPropertyChangedEventArgs args)
        {
            //var this_ = source as MainWindow;
            //if (this_ == null)
            //    return;

            //var dg = this_.dgParsed;
            //var dt = this_.DataTable;

            //dg.Columns.Clear();

            //foreach(var dc in dt.Columns)
            //{

            //}
        }
        
    }
}
