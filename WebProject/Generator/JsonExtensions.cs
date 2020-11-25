
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace WebProject.Generator
{
    public static class JsonExtensions
{
    public static void Capitalize(this JArray jArr)
    {
        foreach(var x in jArr)
        {
            var childObj = x as JObject;
            if(childObj != null)
            {
                childObj.Capitalize();
                continue;
            }
            var childArr = x as JArray;
            if(childArr != null)
            {
                childArr.Capitalize();
                continue;
            }
        }
    }

    public static void Capitalize(this JObject jObj)
    {
        foreach(var kvp in (JObject)jObj.DeepClone())
        {
            jObj.Remove(kvp.Key);
            var newKey = kvp.Key.Capitalize();
            var childObj = kvp.Value as JObject;
            if(childObj != null)
            {
                childObj.Capitalize();
                jObj.Add(newKey, childObj);
                continue;
            }
            var childArr = kvp.Value as JArray;
            if(childArr != null)
            {
                childArr.Capitalize();
                jObj.Add(newKey, childArr);
                continue;
            }
            jObj.Add(newKey, kvp.Value);
        }
    }

    public static string Capitalize(this string str)
    {
        char[] arr = str.ToCharArray();
        arr[0] = char.ToUpper(arr[0]);
        return new string(arr);
    }
}
}