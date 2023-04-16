using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Xabe.FFmpeg;

namespace AutomatedYTChannel
{
    internal class VideoFiles
    {
        internal string[] InputFiles;
        internal string OutputFileName = "/result.mp4";
        internal string OutputFolder;
        public void ReadSettings()
        {
            if (!File.Exists(Environment.GetFolderPath(Environment.SpecialFolder.Personal) + "/videopath.json"))
            {
                var DocumentsFolder = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
                var output = new Output
                {
                    OutputPath = Environment.GetFolderPath(Environment.SpecialFolder.MyVideos)+"\\result.mp4",
                    InputPath = Environment.GetFolderPath(Environment.SpecialFolder.MyVideos)
                };
                var options = new JsonSerializerOptions { WriteIndented = true, IncludeFields = true };
                string fileName = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal)+"/videopath.json");
                string json = JsonSerializer.Serialize(output, options);
                File.WriteAllText(fileName, json);
            }
            string Json = File.ReadAllText(Environment.GetFolderPath(Environment.SpecialFolder.Personal) + "/videopath.json");
            Output output1 = JsonSerializer.Deserialize<Output>(Json);
            OutputFolder = output1.OutputPath;
            InputFiles = Directory.GetFiles(output1.InputPath, "*.mp4");
            return;
        }
        public async Task CreateVideo()
        {
            string output = OutputFolder;
            var conversion = await FFmpeg.Conversions.FromSnippet.Concatenate(output, InputFiles);
            conversion.OnProgress += (sender, args) =>
            {
                var percent = (int)(Math.Round(args.Duration.TotalSeconds / args.TotalLength.TotalSeconds, 2) * 100);
                Debug.WriteLine($"[{args.Duration} / {args.TotalLength}] {percent}%");
            };
            await conversion.Start();
        }
    }
    internal class Output
    {
        [JsonInclude]
        public string OutputPath { get; set; }
        [JsonInclude]
        public string InputPath { get; set; }
    }
}
