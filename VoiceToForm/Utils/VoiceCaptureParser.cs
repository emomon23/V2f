using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoiceToForm.Utils
{
    public class VoiceCaptureParser
    {
        public Dictionary<string, string> ParseVoiceCapture(string voiceCapture, params string[] inputElementIds)
        {
            var result = new Dictionary<string, string>();
            var map = CreateMap(voiceCapture, inputElementIds);

            foreach (var mapItem in map)
            {
                var value =
                    voiceCapture.Substring(mapItem.StartingIndex, mapItem.SubStringLength)
                        .Substring(mapItem.InputElementId.Length)
                        .Trim();

                result.Add(mapItem.InputElementId, value.CaptializeFirstLetter());
            }

            return result;
        }

        private List<VoiceCaptureMap> CreateMap(string voiceCapture, string[] inputElements)
        {
            List<VoiceCaptureMap> map = new List<VoiceCaptureMap>();

            foreach (var inputElmentId in inputElements)
            {
                var searchFor = inputElmentId.Replace("_", " ");
                if (searchFor.IsNotNull())
                {
                    int index = voiceCapture.IndexOf(searchFor, StringComparison.OrdinalIgnoreCase);

                    if (index >= 0)
                    {
                        map.Add(new VoiceCaptureMap() {StartingIndex = index, InputElementId = inputElmentId});
                    }
                }
            }

            map = map.OrderBy(i => i.StartingIndex).ToList();

            for (int i = 0; i < map.Count; i++)
            {
                var mapItem = map[i];

                if (i == map.Count - 1)
                {
                    mapItem.SubStringLength = voiceCapture.Length - mapItem.StartingIndex;
                }
                else
                {
                    mapItem.SubStringLength = map[i + 1].StartingIndex - mapItem.StartingIndex;
                } 
            }

            return map;
        }

        public class VoiceCaptureMap
        {
            public string InputElementId { get; set; }
            public int StartingIndex { get; set; }
            public int SubStringLength { get; set; }
        }
    }
}
