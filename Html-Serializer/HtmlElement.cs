using Newtonsoft.Json;

using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Html_Serializer
{
    //הפונקציות במחלקה זו בדוקות ומתאימות לדרישות
    public class HtmlElement
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string InnerHtml { get; set; }
        public List<HtmlElement> Children { get; set; }
        public List<string> Attributes { get; set; }
        public List<string> Classes { get; set; }
        [JsonIgnore] // איגור משתנה זה במהלך הסידור ל JSON
        public HtmlElement Parent { get; set; }
        public HtmlElement()
        {
            Id =null;
            Name = null;
            InnerHtml = null;
            Children = new List<HtmlElement>();
            Attributes = new List<string>();
            Classes = new List<string>();
        }
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

            // Printing inner HTML of script tags
            if (root.Name.Equals("script", StringComparison.OrdinalIgnoreCase))
            {
                Console.WriteLine("Inner HTML of script tag:");
                Console.WriteLine(root.InnerHtml);
                Console.WriteLine("---------------------------------------");
            }

            // Print selector details
            string query1 = "div .md-header__title .md-header__ellipsis";
            string[] parts = query1.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            Selector selector1 = Selector.Parse(parts);

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
        }
        public IEnumerable<HtmlElement> Descendants()
        {
            Queue<HtmlElement> queue = new Queue<HtmlElement>();
            queue.Enqueue(this);

            while (queue.Count > 0)
            {
                var current = queue.Dequeue();
                yield return current;

                foreach (var child in current.Children)
                {
                    queue.Enqueue(child);
                }
            }
        }
        public IEnumerable<HtmlElement> Ancestors()
        {
            HtmlElement current = this;

            while (current != null)
            {
                yield return current;
                current = current.Parent;
            }
        }
        private static bool MatchSelector(HtmlElement htmlElement, Selector selector)
        {
            if (!string.IsNullOrEmpty(selector.TagName) && htmlElement.Name != selector.TagName)
            {
                return false;
            }
            if (!string.IsNullOrEmpty(selector.Id) && htmlElement.Id != selector.Id)
            {
                return false;
            }
            if (selector.Classes != null && selector.Classes.Any() && !selector.Classes.All(cls => htmlElement.Classes.Contains(cls)))
            {
                return false;
            }
            return true;
        }//פונקציה עזר לפונקציה FindElementsRecursive  

        public IEnumerable<HtmlElement> FindElements(Selector selector)
        {
            HashSet<HtmlElement> resultSet = new HashSet<HtmlElement>();
            FindElementsRecursive(this, selector, resultSet);
            return resultSet;
        }

        private void FindElementsRecursive(HtmlElement element, Selector selector, HashSet<HtmlElement> resultSet)//FindElements פונקציה עזר לפונקציה
        {
            if (element == null)
                return;

            if (selector == null || MatchSelector(element, selector))
            {
                // Check if the element is already in the result set before adding it
                if (!resultSet.Contains(element))
                {
                    resultSet.Add(element);
                }
            }

            foreach (var child in element.Children)
            {
                FindElementsRecursive(child, selector, resultSet);
            }
        }
    }
}