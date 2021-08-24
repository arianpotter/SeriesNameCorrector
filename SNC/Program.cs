using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using Newtonsoft.Json.Linq;

namespace SNC
{
    class Program
    {
        private static readonly string _apiKey = "k_mncz41m2";
        private static readonly HttpClient _client = new HttpClient();
        private static readonly Regex _numberingRegex = new Regex("[S-s][0-9]+[E-e][0-9]+");
        private static readonly Regex _episodeRegex = new Regex("[0-9]+$");
        private static readonly Regex _seasonRegex = new Regex("[0-9]+");
        
        static void Main(string[] args)
        {
            
            var folderPath = args[0];
            
            var imdbId = args[1];
            var seasonId = args[2];
            var request = WebRequest.Create($"https://imdb-api.com/en/API/SeasonEpisodes/{_apiKey}/{imdbId}/{seasonId}");
            var response = request.GetResponse().GetResponseStream();
            var reader = new StreamReader(response);
            var query = JObject.Parse(reader.ReadToEnd());
            var seriesName = query["title"].ToString().Replace(" ",".");
            
            
            var files = Directory.GetFiles(folderPath);
            foreach (var file in files)
            {
                
                var numbering = _numberingRegex.Match(Path.GetFileName(file)).ToString();
                var episode = int.Parse(_episodeRegex.Match(numbering).ToString());
                var season = int.Parse(_seasonRegex.Match(numbering).ToString());
                var titleWithExtension = query["episodes"][episode - 1]["title"]
                    .ToString()
                    .Replace(" ",".") + Path.GetExtension(file);
                
                var tempStr = new StringBuilder();
                foreach (var c in titleWithExtension.ToCharArray())
                {
                    if (tempStr.Length == 0 || tempStr[tempStr.Length - 1] != c || c != '.')
                        tempStr.Append(c);
                }
                titleWithExtension = tempStr.ToString();

                var editedName = seriesName + ".S" + season + ".E" + episode + "." + titleWithExtension;
                
                File.Move(file,folderPath + "/" + editedName);
            }
            
            


        }
    }
}