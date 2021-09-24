using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace Common.Common
{
    public static class Extensions
    {
        public static T XmlDeserialize<T>(this string s)
        {
            var locker = new object();
            var stringReader = new StringReader(s);
            var reader = new XmlTextReader(stringReader);
            try
            {
                var xmlSerializer = new XmlSerializer(typeof(T));
                lock (locker)
                {
                    var datasource = (T)xmlSerializer.Deserialize(reader);
                    reader.Close();
                    return datasource;
                }
            }
            catch
            {
                return default(T);
            }
            finally
            {
                reader.Close();
            }
        }

        public static string RemoveInvalidCharacters(this string text)
        {
            if (string.IsNullOrEmpty(text)) return text;
            var c = text.ToCharArray();
            return string.Join(string.Empty, c.Where(t => t != '\u202c').ToList());
        }

        public static int GetNewIntIdX<T>(this List<T> list, int startId = 1)
        {
            if (list == null || list.Count() == 0) return startId;
            var avaiableIds = new List<int>();
            foreach (var item in list)
            {
                int oldId = (int)item.GetType().GetProperty("Id")?.GetValue(item, null);
                avaiableIds.Add(oldId);
            }
            avaiableIds.RemoveAll(t => t == 0);
            int maxId = avaiableIds.Max(t => t);
            return maxId + 1;
        }

        public static Task<HttpResponseMessage> CreatePostRequestAsync(this HttpClient client, string url, object data)
        {
            var json = Helper.SerializeObject(data);
            var dataContent = new StringContent(json, Encoding.UTF8, "application/json");
            var request = new HttpRequestMessage()
            {
                RequestUri = new Uri(url),
                Method = HttpMethod.Post,
                Content = dataContent
            };
            return client.SendAsync(request);
        }

        public static T GetDataX<T>(this Task<T> response)
        {
            return response.GetAwaiter().GetResult();
        }

        public static string ToValidFileName(this string fileName)
        {
            return fileName.Trim().Replace(".", "").Replace(" ", "");
        }

        public static int GetIdMaxWithStringProperty<T>(this List<T> list, string propertyId = "Id", int startId = 1)
        {
            if (list == null || list.Count() == 0) return startId;
            var avaiableIds = new List<int>();
            foreach (var item in list)
            {
                int oldId = (int)item.GetType().GetProperty(propertyId)?.GetValue(item, null);
                avaiableIds.Add(oldId);
            }
            avaiableIds.RemoveAll(t => t == 0);
            int maxId = avaiableIds.Max(t => t);
            return maxId + 1;
        }

        public static List<T> LoadModelsFromFolderX<T>(this string dir, string extFile = ".json", string pathProp = "PhysicalPath", bool orderByCreateTime = false)
        {
            if (!Directory.Exists(dir)) return default(List<T>);
            if (!extFile.Contains("*")) extFile = "*" + extFile;
            try
            {
                IList<T> list = new List<T>();
                Dictionary<string, PropertyInfo> _propertyMap = typeof(T)
                .GetProperties()
                .ToDictionary(
                    prop => prop.Name.ToLower(),
                    prop => prop
                );
                PropertyInfo p;
                _propertyMap.TryGetValue(pathProp.ToLower(), out p);
                var files = Directory.EnumerateFiles(dir, extFile);
                if (orderByCreateTime)
                {
                    files = files.OrderBy(r => new FileInfo(r).CreationTime);
                }
                files.ToList().ForEach(t =>
                {
                    try
                    {
                        string text = File.ReadAllText(t);
                        var objT = JsonConvert.DeserializeObject<T>(text);
                        if (p != null) p.SetValue(objT, t, null);
                        list.Add(objT);
                    }
                    catch
                    {
                        //Abandon record ERROR!
                    }
                });
                var ret = list.ToList();
                return ret;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return default(List<T>);
            }
        }

        public static string TryGetValueX(this Dictionary<int, string> dic, int id)
        {
            return dic.ContainsKey(id) ? dic[id] : string.Empty;
        }

        public static IEnumerable<T> RecursiveSelector<T>(this IEnumerable<T> nodes, Func<T, IEnumerable<T>> selector)
        {
            if (nodes.Any())
                return nodes.Concat(nodes.SelectMany(selector).RecursiveSelector(selector));

            return nodes;
        }

        public static bool ContainsAbsolute(this string textContain, string textInput)
        {
            bool isExist = false;
            textContain = textContain.Replace("\r", " ");
            textContain = textContain.Replace("\n", " ");
            isExist = (" " + textContain + " ").Contains(" " + textInput + " ");
            return isExist;
        }

        public static List<string> SplitStringToList(this string text, string stringSplit)
        {
            var listResults = new List<string>();
            if (!text.Contains(stringSplit))
            {
                listResults.Add(text.Trim());
                return listResults;
            }
            foreach (var t in text.Split(new string[] { stringSplit }, StringSplitOptions.None))
            {
                if (string.IsNullOrEmpty(t.Trim()))
                {
                    continue;
                }
                listResults.Add(t.Trim());
            }
            return listResults;
        }

        

    }
}
