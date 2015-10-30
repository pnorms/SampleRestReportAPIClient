using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Backstop.Samples.RestReports
{
    public static class ReportParser
    {
        public static DataTable CreateDataTable(string json)
        {
            if (string.IsNullOrEmpty(json))
                throw new ArgumentNullException("json");

            var reader = new JsonTextReader(new StringReader(json));
            var serializer = new JsonSerializer();
            var data = serializer.Deserialize(reader) as JObject;

            if (data == null)
                throw new ApplicationException("service returned null result or not in expected format (should not happen)");

            var jsonHeader = data.GetValue("header") as JArray;
            Debug.Assert(jsonHeader != null);

            var dt = new DataTable();
            var header = new List<string>();

            foreach (JObject obj in jsonHeader)
            {
                string name;
                JToken key;
                if (obj.TryGetValue("key", out key))
                    name = key.Value<string>();
                else
                    name = obj.GetValue("name").Value<string>();
                header.Add(name);

                string title = obj.GetValue("title").Value<string>();

                var dc = new DataColumn(name, typeof(string));
                dc.Caption = title;

                dt.Columns.Add(dc);
            }

            var values = data.GetValue("values") as JArray;
            Debug.Assert(values != null);

            foreach (JObject row in values)
            {
                object[] items = new object[header.Count];

                for (int i = 0; i < header.Count; ++i)
                {
                    JToken value = row.GetValue(header[i]);
                    var array = value as JArray;
                    if (array != null)
                    {
                        var sb = new StringBuilder();
                        bool first = true;

                        // NOTE: This may need to be modified later to support other result types. This is for time-serieses
                        foreach (var item in array)
                        {
                            if (first)
                                first = false;
                            else
                                sb.Append('\n');

                            sb.Append(item.Value<string>("date"));
                            sb.Append(": ");
                            sb.Append(item.Value<string>("value"));
                        }
                        items[i] = sb.ToString();
                    }
                    else
                    {
                        items[i] = value.Value<string>();
                    }
                }

                dt.Rows.Add(items);
            }

            return dt;
        }
    }
}
