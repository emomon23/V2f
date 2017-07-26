using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Xml.Linq;
using VoiceToForm.Model;
using VoiceToForm.Pages;
using Xamarin.Forms;

namespace VoiceToForm.Utils
{
    public static class Extensions
    {
        private static Random _rnd = new Random(DateTime.Now.Millisecond);
        public static string GetValue(this XElement element, string xPathQuery)
        {
            var resultElement = XPathQuery(element, xPathQuery);
            if (resultElement != null)
            {
                return resultElement.Value;
            }

            return null;
        }

        public static Label AddLabel(this StackLayout stackLayout, string labelText, double fontSize, double height, bool isBinding = true)
        {
            StackLayout lblConainer = new StackLayout()
            {
                HorizontalOptions = LayoutOptions.CenterAndExpand
            };
            
            Label lbl = new Label() {FontSize = fontSize, HeightRequest = height, HorizontalTextAlignment = TextAlignment.Center};
            if (isBinding)
            {
                lbl.SetBinding(Label.TextProperty, labelText);
            }
            else
            {
                lbl.Text = labelText;
            }

            lblConainer.Children.Add(lbl);
            stackLayout.Children.Add(lblConainer);

            return lbl;
        }

        public static void CreateV2F_Id_andDateSaved(this Dictionary<string, string> dictionary)
        {
            if (!dictionary.ContainsKey(FormData.V2f_IdKey))
            {
                dictionary.Add(FormData.V2f_IdKey, Guid.NewGuid().ToString());
            }

            var now = DateTime.Now.ToString("g");
            if (dictionary.ContainsKey(FormData.V2f_SaveDateKey))
            {
                dictionary[FormData.V2f_SaveDateKey] = now;
            }
            else
            {
                dictionary.Add(FormData.V2f_SaveDateKey, now);
            }
        }

        public static bool ContainsOr(this string str, params string[] lookFor)
        {
            if (str.IsNullOrEmpty())
            {
                return false;
            }

            var temp = str.ToLower();
            return lookFor.Any(l => temp.Contains(l.ToLower()));
        }

        public static Dictionary<string, string> GetDisplayFields(this Dictionary<string, string> main)
        {
            Dictionary<string, string> result = new Dictionary<string, string>();

            foreach (var d in main)
            {
                if (!d.Key.ContainsOr(FormData.V2f_SaveDateKey, FormData.V2f_IdKey, FormData.V2f_UploadedDateKey))
                {
                    result.Add(d.Key, d.Value);
                }

                if (d.Key == FormData.V2f_UploadedDateKey && d.Value.IsNotNull())
                {
                    result.Add("Uploaded", d.Value);
                }
            }

            return result;
        }

        public static string SerializeV2fFields(this Dictionary<string, string> dictionary)
        {
            var id = dictionary.GetV2fId();
            var dateSaved = dictionary.GetV2fDateSaved();
            var dataUploaed = dictionary.GetDateUploaded();

            var nl = Environment.NewLine;

            dateSaved = dateSaved.IsNotNullOrEmpty() ? dateSaved.Replace(":", "_") : "";
            dataUploaed = dataUploaed.IsNotNull() ? dataUploaed.Replace(":", "_") : "";

            return $"{FormData.V2f_IdKey}:{id},{FormData.V2f_SaveDateKey}:{dateSaved},{FormData.V2f_UploadedDateKey}:{dataUploaed},";
        }

        public static string DeserializeV2fDateSaved(this string str)
        {
            return str.Replace("_", ":");
        }

        public static string GetV2fId(this Dictionary<string, string> dictionary)
        {
            if (dictionary.ContainsKey(FormData.V2f_IdKey))
            {
                return dictionary[FormData.V2f_IdKey];
            }

            return "";
        }

        public static string GetV2fDateSaved(this Dictionary<string, string> dictionary)
        {
            if (dictionary.ContainsKey(FormData.V2f_SaveDateKey))
            {
                return dictionary[FormData.V2f_SaveDateKey];
            }

            return "";
        }

        public static List<string> SplitOnNLCR(this string str)
        {
            return str.Replace(Environment.NewLine, "^").Split('^').ToList();
        }

        public static bool IsPropertyValue(this string str, string lookFor, out string fieldValue)
        {
            if (str.StartsWith(lookFor))
            {
                fieldValue = str.Remove(lookFor);
                return true;
            }

            fieldValue = "";
            return false;
        }

        public static string Remove(this string str, params string [] remove)
        {
            if (str.IsNullOrEmpty())
            {
                return str;
            }

            var result = str;
            foreach (var r in remove)
            {
                result = result.Replace(r, "");
            }

            return result;
        }

        public static string PickOne(this string[] strArray)
        {
            if (strArray == null || strArray.Length == 0)
            {
                return "";
            }

            if (strArray.Length == 1)
                return strArray[0];

            var index = _rnd.Next(0, strArray.Length - 1);
            return strArray[index];
        }

        public static XElement XPathQuery(this XElement element, string xPathQuery)
        {
            var queryParts = xPathQuery.Split('/');
            // FavoriteMovies/Movie[@category='Drama']/Title/Value

            var currentElement = element;

            foreach (var part in queryParts)
            {
                if (currentElement == null)
                {
                    break;
                }

                var nodeName = part;
                var attrName = "";
                var attrValue = "";

                if (part.Contains("[@"))
                {
                    //Movie[@cateogyr='Drama']   
                    nodeName = part.Substring(0, part.IndexOf("["));
                    var temp = part.Substring(nodeName.Length).Split('=');
                    attrName = temp[0].Replace("[", "").Replace("@", "");
                    attrValue = temp[1].Replace("'", "").Replace("]", "");
                }

                if (!string.IsNullOrEmpty(attrValue))
                {
                    currentElement =
                        currentElement.Elements(nodeName).FirstOrDefault(e => e.Attribute(attrName).Value == attrValue);
                }
                else
                {
                    currentElement = currentElement.Element(nodeName);
                }
            }
            return currentElement;

        }

        public static int ToInt(this string str)
        {
            int result = 0;
            int.TryParse(str, out result);
            return result;
        }

        public static string Capitalize(this string str)
        {
            if (str.IsNullOrEmpty())
            {
                return str;
            }

            if (str.Length == 1)
            {
                return str.ToUpper();
            }

            return $"{str.Substring(0, 1).ToUpper()}{str.Substring(1)}";
        }

        public static bool ToBool(this string str)
        {
            var temp = str.ToLower();
            if (temp == "true" || temp == "t" || temp == "yes" || temp == "y")
            {
                return true;
            }

            return false;
        }

        public static Page FindPage(this List<Page> pages, string pageTitle)
        {
            return pages.SingleOrDefault(p => p.Title == pageTitle);
        }

        public static double ToDouble(this string str)
        {
            double result = 0;
            double.TryParse(str, out result);
            return result;
        }

        public static bool IsNullOrEmpty(this string str)
        {
            return string.IsNullOrEmpty(str);
        }

        public static bool IsOlderThanMinutes(this DateTime date, int mins)
        {
            var timeSpan = DateTime.Now - date;
            return timeSpan.TotalMinutes > mins;
        }

        public static bool IsNotNullOrEmpty(this string str)
        {
            return !string.IsNullOrEmpty(str);
        }

        public static string LeadWithUpperCase(this string str)
        {
            if (str.IsNullOrEmpty())
            {
                return str;
            }

            if (str.Length == 1)
            {
                return str.ToUpper();
            }

            var result = $"{str.Substring(0, 1).ToUpper()}{str.Substring(1)}";

            return result;
        }

        public static BaseModelView GoBackOnePage(this INavigation navigation)
        {
            var currentPage = navigation.NavigationStack[navigation.NavigationStack.Count - 1];
            navigation.RemovePage(currentPage);
            currentPage = navigation.NavigationStack[navigation.NavigationStack.Count - 1];
            return (BaseModelView)currentPage.BindingContext;
        }

        public static bool IsDate(this string str)
        {
            DateTime d;

            return DateTime.TryParse(str, out d);
        }

        public static DateTime ToDate(this string str)
        {
            DateTime result;

            DateTime.TryParse(str, out result);
            return result;
        }

        public static void CreateAddToolbarButton(this ContentPage page, Func<Task> addFunction)
        {
            page.ToolbarItems.Add(new ToolbarItem("Add", null, async () =>
            {
                await addFunction();
            }));
        }

        public static void CreateAddToolbarButton(this ContentPage page, Func<Func<Task>, Task> addFunction, Func<Task> callBack)
        {
            page.ToolbarItems.Add(new ToolbarItem("Add", null, async () =>
            {
                await addFunction(callBack);
            }));

            //Add a space
            page.ToolbarItems.Add(new ToolbarItem(" ", null, () =>
            {

            }));
        }

        public static void CreateSaveToolbarButton(this ContentPage page, Func<Task> saveFunction , Func<Task> deleteFunction = null)
        {
            if (deleteFunction != null)
            {
                page.ToolbarItems.Add(new ToolbarItem("Delete", null, async () =>
                {
                    await deleteFunction();
                }));


                //Add a space
                page.ToolbarItems.Add(new ToolbarItem(" ", null, () =>
                {

                }));
            }

            page.ToolbarItems.Add(new ToolbarItem("Save", null, async () =>
            {
                await saveFunction();
            }));

            //Add a space
            page.ToolbarItems.Add(new ToolbarItem(" ", null, () =>
            {

            }));
        }

        public static void CreateCancelButton(this ContentPage page, Func<Task> cancelFunction)
        {
            CreateToolbarButton(page, cancelFunction, "Cancel");
        }

        public static List<string> SplitAndTrim(this string str, char split)
        {
            var result = new List<string>();
            var temp = str.Split(split);

            foreach (var t in temp)
            {
                var trimmed = t.Trim();

                if (trimmed.Replace(" ", "").IsNotNullOrEmpty())
                {
                    result.Add(trimmed);
                }
            }
            return result;
        }

        public static void CreateToolbarButton(this ContentPage page, Func<Task> callBack, string buttonText, bool addASpace = true)
        { 
            page.ToolbarItems.Add(new ToolbarItem(buttonText, null, async () =>
            {
                await callBack();
            }));

            //Add a space
            if (addASpace)
            {
                page.ToolbarItems.Add(new ToolbarItem(" ", null, () =>
                    { }));
            }
        }
        
        public static void AddItems(this Picker picker, IEnumerable<string> items)
        {
            foreach (var item in items)
            {
                picker.Items.Add(item);
            }
        }

        public static string CaptializeFirstLetter(this string str)
        {
            if (string.IsNullOrEmpty(str))
            {
                return str;
            }

            if (str.Length == 1)
            {
                return str.ToUpper();
            }

            return $"{str.Substring(0, 1).ToUpper()}{str.Substring(1)}";

        }
    }
}
