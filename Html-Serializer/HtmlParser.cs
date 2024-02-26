using Html_Serializer;
using System.Text.RegularExpressions;
public class HtmlParser
{
    public HtmlElement ParseHtml(string html)
    {
        HtmlElement root = new HtmlElement();// { Name = "html", Children = new List<HtmlElement>(), Parent = null ,Classes=new List<string>(),Attributes=new List<string>() };
        var currentElement = root;

        string[] htmlTags = HtmlHelper.Instance.AllHtmlTags;
        string[] selfClosingTags = HtmlHelper.Instance.SelfClosingHtmlTags;

        string openingTagPattern = @"<(\w+)([^>]*)>";
        string closingTagPattern = @"<\/(\w+)([^>]*)>";
        string selfClosingTagPattern = @"<(\w+)([^>]*)\/>";
        string contentPattern = @"[^<]+";
        string attributePattern = @"(\w+)=(""|')(.*?)(""|')";
        string combinedPattern = $"({openingTagPattern})|({closingTagPattern})|({selfClosingTagPattern})|({contentPattern})";

        Regex regex = new Regex(combinedPattern);
        var matches = regex.Matches(html);

        foreach (Match match in matches)
        {
                if (match.Groups[1].Success) // Opening tag
               {
                   string tag = match.Groups[1].Value.ToLower();
                   var element = new HtmlElement { Name = tag, Parent = currentElement, Children = new List<HtmlElement>(), InnerHtml = "" };
                   currentElement.Children.Add(element);
                   currentElement = element;

                   // Extract attributes
                   if (match.Groups[2].Success)
                   {
                       string attributes = match.Groups[2].Value.Trim();//לחתוך את המילה הראשונה -שם התגית
                       ProcessAttributes(attributes, currentElement);
                   }
               }
               else if (match.Groups[3].Success) // Closing tag
               {
                   currentElement = currentElement.Parent;
               }
               else if (match.Groups[4].Success) // Self-closing tag
               {
                   string tag = match.Groups[4].Value.ToLower();
                   var element = new HtmlElement { Name = tag, Parent = currentElement, Children = new List<HtmlElement>(), InnerHtml = "" };
                   currentElement.Children.Add(element);

                   // Extract attributes
                   if (match.Groups[5].Success)
                   {
                       string attributes = match.Groups[5].Value.Trim();
                       ProcessAttributes(attributes, currentElement);
                   }
               }
               else if (match.Groups[6].Success) // Content
               {
                   string content = match.Groups[6].Value.Trim();
                   if (!string.IsNullOrWhiteSpace(content))
                   {
                       currentElement.InnerHtml += content;
                   }
               }
        }
        return root;
    }

    private void ProcessAttributes(string attributes, HtmlElement element)
    {
        // string pattern = @"(\w+)=(""|')(.*?)(""|')";
        var pattern = @"(\S+)=[""']?((?:.(?![""']?\s+(?:\S+)=|[>""]))+.)[""']?";
        Regex regex = new Regex(pattern);
        var matches = Regex.Matches(attributes, pattern);
        foreach (Match match in matches)
        {
            string attributeName = match.Groups[1].Value;
            string attributeValue = match.Groups[2].Value;
            if (attributeName.ToLower() == "class")
            {
                element.Classes.AddRange(attributeValue.Split());
            }
            else if (attributeName == "id")
            {
                element.Id = attributeValue;
            }
        }
    }
}