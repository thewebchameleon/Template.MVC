using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Template.Common.Helpers
{
    public static class JsonHelper
    {
        public static string ObfuscateFieldValues(this string jsonData, List<string> fieldNames)
        {
            // props to this person
            // https://stackoverflow.com/questions/33130489/find-and-replace-a-string-in-a-json-response-in-net#answer-33130706

            var propsToMask = new HashSet<string>(fieldNames);
            var jObj = JObject.Parse(jsonData);

            var fieldValuesToObfuscate = new List<string>();

            foreach (var p in jObj.Descendants()
                                 .OfType<JProperty>()
                                 .Where(p => propsToMask.Contains(p.Name)))
            {
                fieldValuesToObfuscate.Add(p.Value.ToString());
            }

            foreach (var fieldValue in fieldValuesToObfuscate)
            {
                jsonData = jsonData.Replace(fieldValue, "******");
            }

            return jsonData;
        }
    }
}
