using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
namespace Html_Serializer
{
    public class HtmlHelper
    {
        private readonly static HtmlHelper instance = new HtmlHelper();
        public static HtmlHelper Instance => instance;
        public string[] AllHtmlTags;
        public string[] SelfClosingHtmlTags;
        private HtmlHelper()
        {
            AllHtmlTags = LoadTagsFromJson("htmlTags.json");
            SelfClosingHtmlTags = LoadTagsFromJson("close.json");
        }
        private string[] LoadTagsFromJson(string jsonFilePath)
        {
            try
            {
                using (StreamReader reader = new StreamReader(jsonFilePath))
                {
                    string jsonString = reader.ReadToEnd();
                    return JsonSerializer.Deserialize<string[]>(jsonString);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading tags from JSON file '{jsonFilePath}': {ex.Message}");
                return null;
            }
        }
    }
}
