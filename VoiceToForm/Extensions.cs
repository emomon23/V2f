using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using VoiceToForm.Model;

namespace VoiceToForm
{
    public static class Extensions
    {
        public static bool IsNull(this string str)
        {
            return string.IsNullOrEmpty(str);
        }

        public static bool IsNotNull(this string str)
        {
            return !str.IsNull();
        }

        public static bool IsGuid(this string str)
        {
            Guid dummy;
            return Guid.TryParse(str, out dummy);
        }

        public static string GetRowId(this Dictionary<string, string> dictionary)
        {
            return dictionary.GetDictionaryValue(FormData.V2f_IdKey);
        }

        public static string GetDateSaved(this Dictionary<string, string> dictionary)
        {
            return dictionary.GetDictionaryValue(FormData.V2f_SaveDateKey);
        }

        public static Dictionary<string, string> Copy(this Dictionary<string, string> dictionary)
        {
            var result = new Dictionary<string, string>();
            foreach (var k in dictionary.Keys)
            {
                result.Add(k, dictionary[k]);
            }

            return result;
        }

        public static void SetDateUploaded(this Dictionary<string, string> dictionary)
        {
            var key = FormData.V2f_UploadedDateKey;
            dictionary.SetValue(key, DateTime.Now.ToString("g"), true);
        }

        public static void ClearDateUploaded(this Dictionary<string, string> dictionary)
        {
            var key = FormData.V2f_UploadedDateKey;
            dictionary.SetValue(key, "");
        }

        public static bool ContainsDuplicates(this List<string> list)
        {
            Dictionary<string, bool> check = new Dictionary<string, bool>();
            foreach (var s in list)
            {
                if (check.ContainsKey(s))
                {
                    return true;
                }

                check.Add(s, true);
            }

            return false;
        }

        public static void SetValue(this Dictionary<string, string> dictionary, string key, string value, bool onlyIfNullOrEmpty = false)
        {
            if (dictionary.ContainsKey(key))
            {
                if (onlyIfNullOrEmpty == false || dictionary[key].IsNull())
                {
                    dictionary[key] = value;
                }
            }
            else
            {
                dictionary.Add(key, value);
            }
        }

        public static string GetDateUploaded(this Dictionary<string, string> dictionary)
        {
            var key = FormData.V2f_UploadedDateKey;
            return dictionary.GetDictionaryValue(key);
        }

        public static bool WasNotUploaded(this Dictionary<string, string> dictionary)
        {
            return dictionary.GetDateUploaded() == "";
        } 

        public static string GetDictionaryValue(this Dictionary<string, string> dictionary, string key, string resultIfNullOrMissing = "")
        {
            return dictionary.ContainsKey(key) ? dictionary[key] : resultIfNullOrMissing;
        }

        public static string CleanStreetForTaxSearch(this string str)
        {
            str = " " + str.ToLower() + " ";
            string streetWord = "";
            string directionWord = "";

            int streetIndex = str.IndexOfAny(out streetWord, " street ", " st ", " ave ", " avenue ", " ln ", " lane ", " rd ", " road ", " tr ", " trail ", " crt ", " court ");
            int directionIndex = str.IndexOfAny(out directionWord, " n ", " s ", " e ", " w ", " north ", " south ", " east ", " west ", " ne ", " nw ", " se ", " sw ", " northeast ", " northwest ", " southeast ", " southwest ");

            if (streetIndex > directionIndex && directionIndex >= 0)
            {
                var words = str.GetWords();
                for (int i = words.Count - 1; i >= 0; i--)
                {
                    if (words[i] == streetWord.Trim() || words[i] == directionWord.Trim())
                    {
                        words.RemoveAt(i);
                    }
                }

                str = $"{string.Join(" ", words)} {streetWord} {directionWord}".Replace("   ", " ").Replace("  ", " ");
            }

            return str.Trim().CapitalizeNormalizeNames();
        }

        public static bool HoldsValue(this string str, string strCheck)
        {
            if (str.IsNull() || strCheck.IsNull())
            {
                return false;
            }

            return str.ToLower().Contains(strCheck.ToLower());
        }

        public static int IndexOfAny(this string str, out string strWord, params string[] lookFor)
        {
            int result = -1;
            strWord = null;

            foreach (var l in lookFor)
            {
                int temp = str.IndexOf(l);
                if (temp != -1 && (result ==-1 || temp < result))
                {
                    result = temp;
                    strWord = l;
                }
            }

            return result;
        }

        public static bool IsNumeric(this string str)
        {
            double d = 0;
            return double.TryParse(str, out d);
        }

        public static bool IsDate(this string str)
        {
            DateTime d;
            return DateTime.TryParse(str, out d);
        }

        public static string CapitalizeNormalizeNames(this string str)
        {
            var result = "";

            if (str.IsNotNull())
            {
                var temp = str.Split(' ');
                foreach (var namePart in temp)
                {
                    if (namePart.IsNotNull())
                    {
                        if (namePart.Length == 1)
                        {
                            result += namePart + " ";
                        }
                        else
                        {
                            result += namePart.CapitalizeNormalizeString() + " ";
                        }
                    }
                }
            }

            return result.StripOffLastChars(1);
            
        }

        public static bool IsBetween(this int intValue, int minRange, int maxRange)
        {
            return intValue >= minRange && intValue <= maxRange;
        }

        public static int? ToNullableInt(this string str)
        {
            if (str.IsNull())
            {
                return null;
            }

            str = str.CleanCrapCharacters().Remove("$", ",");
            if (str.IsNumeric())
            {
                return str.ToInt();
            }

            return null;
        }
        


        public static int ToInt(this string str)
        {
            if (str == null)
            {
                return 0;
            }

            var copy = str.CleanCrapCharacters();

            if (copy.IsNumeric())
            {
                return int.Parse(copy);
            }

            throw new Exception($"ToInt on value '{str}' no good!");
        }

        public static string StripOffLastChars(this string str, int numberToStrip)
        {
            if (str.IsNull() || str.Length < numberToStrip)
            {
                return str;
            }

            return str.Substring(0, str.Length - numberToStrip);
        }

        public static DateTime ToDate(this string str)
        {
            return DateTime.Parse(str);
        }

        public static DateTime? ToNullableDateTime(this string str)
        {
            if (str.IsDate())
            {
                return str.ToDate();
            }

            return null;
        }

        public static string ExtractWordAfter(this string str, params string[] delimiters)
        {
            //"Mike Emo, 25 of Prior Lake Texas, born 1971
            var result = "";

            var words = str.Split(' ');
            var lookForLower = delimiters[0].ToLower();
            var founddFontDelimiter = false;
            var endingDelimitrs = delimiters.ToList();

            endingDelimitrs.RemoveAt(0);
            
            for (int i = 0; i < words.Length; i++)
            {
                if (founddFontDelimiter)
                {
                    if (words[i].ContainsOr(delimiters))
                    {
                        result += " " + words[i].Remove(delimiters);
                        break;
                    }

                    result += " " + words[i];
                }

                if (words[i].ToLower() == lookForLower)
                {
                    founddFontDelimiter = true;
                }       
            }

            return result == "" ? null : result.Trim();
        }

        public static string GetSurroundingText(this string str, int lookFor, int preceedingWordCount = 1,
            int postWordCount = 1)
        {
            var words = str.Split(' ');
            var result = "";
            var lowerLookFor = lookFor.ToString();

            for (int i = 0; i < words.Length; i++)
            {
                if (result != "")
                {
                    break;
                }

                if (words[i].Remove(",", ",") == lowerLookFor)
                {
                    for (int back = i - preceedingWordCount; back < i; back++)
                    {
                        result += words[back] + " ";
                    }

                    result += words[i];

                    for (int post = i + postWordCount; post <= i + postWordCount; post += 1)
                    {
                        result += " " + words[post];
                        if (post == words.Length)
                        {
                            break;
                        }
                    }
                }
            }

            return result.Trim();
        }

        public static string GetSurroundingText(this string str, string lookFor, int preceedingWordCount,
          int postWordCount, bool cleanCrapText)
        {
            var words = str.Split(' ');
            var result = "";
            var lowerLookFor = lookFor.ToLower();

            for (int i = 0; i < words.Length; i++)
            {
                if (result != "")
                {
                    break;
                }

                if (words[i].ToLower().CleanCrapCharacters() == lowerLookFor)
                {
                    if (preceedingWordCount > 0)
                    {
                        for (int back = i - preceedingWordCount; back < i; back++)
                        {
                            result += words[back] + " ";
                        }
                    }

                    result += words[i];

                    if (postWordCount > 0)
                    {
                        for (int post = i + postWordCount; post <= i + postWordCount; post += 1)
                        {
                            if (post >= words.Length)
                            {
                                break;
                            }
                            result += " " + words[post];
                        }
                    }
                }
            }

            return result.Trim();
        }

        public static string CapitalizeNormalizeString(this string str)
        {
            if (str.IsNull())
            {
                return null;
            }

            if (str.Length == 1)
            {
                return str.ToUpper();
            }

            return $"{str.Substring(0, 1).ToUpper()}{str.Substring(1, str.Length - 1).ToLower()}";
        }

        public static bool Matches(this string str1, string str2)
        {
            return String.Equals(str1.CleanCrapCharacters(), str2.CleanCrapCharacters(), StringComparison.CurrentCultureIgnoreCase);
        }
        
        public static bool ContainsAll(this List<string> str, List<string> lookFor)
        {
            bool result = true;

            foreach (var l in lookFor)
            {
                if (!str.Contains(l))
                {
                    result = false;
                    break;
                }
            }

            return result;
        }
     public static bool WithInPercent(this int nbr, int percentCheck, int compareToo)
        {
            double percent = (double) ((double) percentCheck / (double) 100);
            var offset = nbr * percent;

            return nbr > compareToo - offset && nbr < compareToo + offset;
        }

        public static string CleanCity(this string city)
        {
            if (city.IsNull())
            {
                return "";
            }

            city = city.ToLower();
            if (city.Contains("mpls"))
            {
                city = "Minneapolis";
            }

            if (city.ContainsOr("st. paul", "st paul", "st-paul", "st- paul", "saint paul", "saint-paul"))
            {
                city = "St. Paul";
            }

            city = city.Remove(",", "mn").Trim();
            return city;
        }
        
        public static string CleanCrapCharacters(this string str)
        {
            if (str.IsNull())
            {
                return str;
            }
            return str.Remove(",", ".", "$", "<", ">", "\r", "\n").Trim();
        }

        public static string RemoveDigits(this string str)
        {
            var result = "";

            foreach (var character in str.ToCharArray())
            {
                var charStr = character.ToString();

                if (!charStr.IsNumeric())
                {
                    result += charStr;
                }
            }

            return result;
        }

        public static string Remove(this string str, params string[] toRemove)
        {
            if (str.IsNull())
            {
                return str;
            }

            var result = str;
            foreach (var stringToRemove in toRemove)
            {
                result = result.Replace(stringToRemove, "");
            }

            return result;
        }

        public static string ExtractWordAfter(this string str, string lookFor, int indexAfter = 1)
        {
            string result = null;

            if (str.ContainsOr(lookFor))
            {
                var lookForLower = lookFor.ToLower();
                var words = str.Split(' ');
                for (int i = 0; i < words.Length; i++)
                {
                    if (words[i].ToLower() == lookForLower)
                    {
                        if (words.Length > i + indexAfter)
                        {
                            result = words[i + indexAfter];
                            break;
                        }
                    }  
                }
            }

            return result;
        }

        public static bool ContainsOr(this string str, params string [] lookFor)
        {
            bool result = false;
            if (str.IsNull())
            {
                return false;
            }

            var lowerCase = str.ToLower();

            for (int i = 0; i < lookFor.Length; i++)
            {
                if (lowerCase.Contains(lookFor[i].ToLower()))
                {
                    result = true;
                    break;
                }
            }

            return result;

        }

        public static bool ContainsAnd(this string str, params string[] lookfor)
        {
            bool result = true;

            foreach (var check in lookfor)
            {
                if (!str.Contains(check))
                {
                    result = false;
                    break;
                }
            }

            return result;
        }

        public static string Prepend(this string str, string prependStrign)
        {
            return prependStrign + str;
        }

        public static void ToLowerTrim(this string[] strArray)
        {
            for (int i = 0; i < strArray.Length; i++)
            {
                strArray[i] = strArray[i].ToLower().Trim();
            }
        }
        public static List<string> Split(this string str, string splitBy)
        {
            if (str == null)
            {
                return new List<string>();
            }

            char splitByChar;
            var possibleSplitBys = new char[]
            {'~', '`', '!', '@', '#', '$', '%', '^','&', '*', '(',')', '_', '_', '+', '+', '{', '}', '[', ']', '|', '|', '<', '>'
            };

            for (int i = 0; i < possibleSplitBys.Length; i++)
            {
                if (!str.Contains(possibleSplitBys[i].ToString()))
                {
                    splitByChar = possibleSplitBys[i];
                    return str.Replace(splitBy, splitByChar.ToString()).Split(splitByChar).ToList();
                }
            }

            throw new Exception($"Unable to split string by {splitBy}");
        }

        public static string GetSentence(this string str, int startingRawIndex = 0)
        {
            if (startingRawIndex < 0)
            {
                return "";
            }

            var substring = str.Substring(startingRawIndex);
            var indexOf = substring.IndexOf(".");

            if (indexOf == -1)
            {
                return substring;
            }

            return substring.Substring(0, indexOf + 1);
        }

        public static DateTime? ParseDateOutOfString(this string str, int backDays = 10000, int futureDays = 10000)
        {
            DateTime? result = null;
            DateTime backDate = DateTime.Now.AddDays(backDays * -1);
            DateTime futureDate = DateTime.Now.AddDays(futureDays);

            var words = str.Split(' ');

            for (int i=0; i<words.Length; i++)
            {
                var word = words[i];

                if (word.IsDate() && !word.Contains("-") && !word.Contains(":"))
                {
                    result = word.ToDate();
                    if (result > backDate && result < futureDate)
                    {
                        break;
                    }
                    else
                    {
                        result = null;
                    }
                }

                if (word.IsMonth() && words.Length > 1)
                {
                    var dateString = word;
                    var dayOrYer = words[i + 1].CleanCrapCharacters().Remove("th", "rd", "st", "nd").Trim();
                    if (dayOrYer.IsNumeric())
                    {
                        var yearOrNot = words.Length > i+2?  words[i + 2].CleanCrapCharacters() : "";

                        dateString = word + " " + dayOrYer;
                        if (yearOrNot.IsNumeric())
                        {
                            dateString += " " + yearOrNot;
                        }

                        if (dateString.IsDate())
                        {
                            result = dateString.ToDate();
                            if (result > backDate & result < futureDate)
                            {
                                break;
                            }
                            else
                            {
                                result = null;
                            }
                        }
                    }
                }
            }

            //1/1/2900
            // April 15, 2017
            // April 2017

            return result;
        }

        public static bool IsMonth(this string str)
        {
            List<string> months = new List<string>()
            {
                "january", "febuary", "march", "april", "may", "june", "july", "august", "september",
                "october", "november", "december", "jan", "feb", "mar", "apr", "may", "jun", "jul", "aug", "sep", "oct", "nov", "dec"
            };

            return months.Contains(str.ToLower().Trim());
        }
        
        public static int GetWordCount(this string str)
        {
            if (str.IsNull())
            {
                return 0;
            }

            return str.Split(' ').Length;
        }

        public static List<string> GetWords(this string str, bool cleanCrapCharacters = false)
        {
            if (str == null)
            {
                return new List<string>();
            }

            var temp = str.Split(' ');

            if (cleanCrapCharacters)
            {
                var result = new List<string>();
                foreach (string t in temp)
                {
                    result.Add(t.CleanCrapCharacters());
                }

                return result;
            }

            return temp.ToList();
        }

        public static int IndexOfOR(this List<string> list, params string[] lookFor)
        {
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i].ContainsOr(lookFor))
                {
                    return i;
                }
            }

            return -1;
        }

        public static string Merge(this List<string> list, string mergeSeperator, bool cleanCrapCharsFromEachWordBeforeMerging = false, int startIndex = 0, int stopIndex = int.MaxValue)
        {
            string result = "";

            if (startIndex == -1)
            {
                return "";
            }

            if (stopIndex == int.MaxValue)
            {
                stopIndex = list.Count - 1;
            }

            for (int i=startIndex; i<= stopIndex; i++)
            {
                var s = cleanCrapCharsFromEachWordBeforeMerging ? list[i].CleanCrapCharacters() : list[i];
                s += mergeSeperator.IsNotNull() ? mergeSeperator : "";

                result += s;
            }

            if (mergeSeperator.IsNotNull()) { 
                result.StripOffLastChars(mergeSeperator.Length);
            }

            return result;
        }

        public static string GetWord(this string str, int index)
        {
            var words = str.Split(' ');

            if (words.Length > index)
            {
                return words[index];
            }

            return null;
        }

        public static List<int> ParseIntsOutOfString(this string str, int min=-1000, int max=int.MaxValue)
        {
            List<int> result = new List<int>();

            var words = str.Split(' ');
            foreach (var word in words)
            {
                if (word.IsNumeric())
                {
                    var intValue = word.ToInt();
                    if (intValue > min && intValue < max)
                    {
                        result.Add(intValue);
                    }
              
                }
            }

            return result;
        }

        public static int MoveToNextIndex(this string str, string lookFor, int startingIndex)
        {
            return str.IndexOf(lookFor, startingIndex + 1);
        }

        public static string ParseOutFirstSentene(this string str)
        {
            int index = str.IndexOf(".");
            if (index >= 0)
            {
                return str.Substring(0, index + 1);
            }

            return "";
        }
    }
}
