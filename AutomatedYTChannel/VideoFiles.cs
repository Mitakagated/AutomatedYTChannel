using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace AutomatedYTChannel
{
    internal class VideoFiles
    {
        string[] InputFiles;
        string OutputFileName = "/result.mp4";
        public static void ReadSettings()
        {
            if (!File.Exists(Environment.GetFolderPath(Environment.SpecialFolder.Personal) + "/videopath.json"))
            {
                var DocumentsFolder = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
                var output = new Output
                {
                    OutputPath = Environment.GetFolderPath(Environment.SpecialFolder.MyVideos)+"\\result.mp4"
                };
                var options = new JsonSerializerOptions { WriteIndented = true, IncludeFields = true };
                string fileName = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal)+"/videopath.json");
                string json = JsonSerializer.Serialize(output, options);
                File.WriteAllText(fileName, json);
            }
            string Json = File.ReadAllText(Environment.GetFolderPath(Environment.SpecialFolder.Personal) + "/videopath.json");
            Output output1 = JsonSerializer.Deserialize<Output>(Json);
            Console.WriteLine(output1.OutputPath);

            //sled kato se izbere papkata, se gledat vsichkite filove koito matchvat video filove i se dobavqt kym InputFiles
            //sled tova shte callne SaveVideo funkciqta koqto da zadade ime i lokaciq na novoto video
        }
    }
    internal class Output
    {
        [JsonInclude]
        public string OutputPath { get; set; }
    }
}
