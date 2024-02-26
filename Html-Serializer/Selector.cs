using System;
using System.Collections.Generic;

using System;
using System.Collections.Generic;
using System.Linq;
using Html_Serializer;

public class Selector
{
    public string TagName { get; set; }
    public string Id { get; set; }
    public List<string> Classes { get; set; }
    public Selector Parent { get; set; }
    public Selector Child { get; set; }

    public Selector(string tagName, string id, List<string> classes, Selector parent)
    {
        TagName = tagName;
        Id = id;
        Classes = classes;
        Parent = parent;
    }

    public static Selector Parse(string[] parts)
    {
        Selector currentSelector = null;
        Selector rootSelector = null;

        foreach (string part in parts)
        {
            if (part.StartsWith("#")) // Id
            {
                string id = part.Substring(1);
                currentSelector = new Selector(null, id, null, currentSelector);
            }
            else if (part.StartsWith(".")) // Class
            {
                string className = part.Substring(1);
                if (currentSelector != null)
                {
                    currentSelector.Classes ??= new List<string>();
                    currentSelector.Classes.Add(className);
                }
                else
                {
                    throw new InvalidOperationException("Class selector must follow a tag or id selector.");
                }
            }
            else // TagName
            {
                if (IsValidTagName(part))
                {
                    currentSelector = new Selector(part, null, null, currentSelector);
                }
                else
                {
                    throw new InvalidOperationException($"Invalid tag name: {part}");
                }
            }

            if (rootSelector == null)
            {
                rootSelector = currentSelector;
            }
        }

        return rootSelector;
    }
    private static bool IsValidTagName(string tagName)
    {
        // Validate tag name by checking if it exists in the list of HTML tags that require closing
        return !string.IsNullOrWhiteSpace(tagName) || HtmlHelper.Instance.AllHtmlTags.Contains(tagName) ||
            HtmlHelper.Instance.SelfClosingHtmlTags.Contains(tagName);
    }
}