// See https://aka.ms/new-console-template for more information
using Html_Serializer;
using System.Text.RegularExpressions;
using System.Xml;
using System.Collections.Generic;
using Newtonsoft.Json;
using Formatting = Newtonsoft.Json.Formatting;

async Task<string> Load(string url)
{
    HttpClient client = new HttpClient();
    var response = await client.GetAsync(url);
    var html = await response.Content.ReadAsStringAsync();
    return html;
}
var html = await Load("https://learn.malkabruk.co.il/practicode/projects/pract-2/#-htmlelement");

var cleanHtml = new Regex("\\s").Replace(html, "");
var HtmlLines = new Regex("<(.*?)>").Split(cleanHtml).Where(s => s.Length > 0);
var htmlElement = "<div id=\"my-id\" class=\"my-class-1 my-class-2\" width=\"100%\">text</div>";
var attributes = new Regex("([^\\s]*?)=\"(.*?)\"").Match(htmlElement);
var close = File.ReadAllText("close.json");
var htmlTags = File.ReadAllText("htmlTags.json");

HtmlParser parser = new HtmlParser();

HtmlElement htmlTree = parser.ParseHtml(html);

string json = JsonConvert.SerializeObject(htmlTree, Formatting.Indented);
Console.Write(json);
List<string> nonClosingTags = HtmlHelper.Instance.AllHtmlTags.ToList();
PrintHtml(htmlTree, nonClosingTags);
static void PrintHtml(HtmlElement root, List<string> selfClosingTags)
{
    // Print the current node
    Console.WriteLine("Name: " + root.Name);

    // Print InnerHtml if present
    if (!string.IsNullOrEmpty(root.InnerHtml))
    {
        Console.WriteLine("InnerHtml: " + root.InnerHtml);
    }

    // Print Id if present
    if (!string.IsNullOrEmpty(root.Id))
    {
        Console.WriteLine("Id: " + root.Id);
    }

    // Print Classes if present
    if (root.Classes.Count > 0)
    {
        Console.WriteLine("Classes: " + string.Join(", ", root.Classes));
    }

    // Print children recursively
    foreach (var child in root.Children)
    {
        PrintHtml(child, selfClosingTags);
    }
    // Check if the tag is self-closing or if it's in the list of self-closing tags
    if (!HtmlHelper.Instance.SelfClosingHtmlTags.Contains(root.Name) && !selfClosingTags.Contains(root.Name))
    {
        Console.WriteLine("Closing tag: " + root.Name);
    }
    string query1 = "div #md-header__title";
    string[] parts = query1.Split(' ', StringSplitOptions.RemoveEmptyEntries);
    Selector selector1 = Selector.Parse(parts);
    Console.WriteLine("-------------------------query-------------------------------------------");
    Console.WriteLine("Parsed Selector:");
    Console.WriteLine($"TagName: {selector1.TagName}");
    Console.WriteLine($"Id: {selector1.Id}");
    Console.Write("Classes: ");
    if (selector1.Classes != null)
    {
        Console.WriteLine(string.Join(", ", selector1.Classes));
    }
    else
    {
        Console.WriteLine("None");
    }
    Console.WriteLine("Parent: " + (selector1.Parent != null ? "Exists" : "None"));
    // Call the Descendants and Ancestors functionsConsole.WriteLine("Descendants:");
    // Call the Descendants and Ancestors functions
    HtmlElement htmle = new HtmlElement();
    Console.WriteLine("Descendants:");
    var e = htmle.Descendants();
    Console.WriteLine(e.ToString());
    /* foreach (var descendant in htmle.Descendants())
     {
         Console.WriteLine(descendant.Name);
     }*/
    Console.WriteLine("Ancestors:");
    foreach (var ancestor in htmle.Ancestors())
    {
        Console.WriteLine(ancestor.Name);
    }
    Console.WriteLine("FindElements:");
    // Call the FindElements function with some criteria
    var elements = htmle.FindElements(selector1);
}