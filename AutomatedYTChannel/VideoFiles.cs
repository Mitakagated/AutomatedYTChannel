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
        public string GetOutputFolder { get { return OutputFolder; } }
        internal string PersonalFolder = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
        internal string DefaultVideosFolder = Environment.GetFolderPath(Environment.SpecialFolder.MyVideos);
        public void ReadSettings()
        {
            if (!File.Exists(PersonalFolder + "/videopath.json"))
            {
                var output = new Output
                {
                    OutputPath = DefaultVideosFolder + "\\result.mp4",
                    InputPath = DefaultVideosFolder
                };
                var options = new JsonSerializerOptions { WriteIndented = true, IncludeFields = true };
                string fileName = Path.Combine(PersonalFolder + "/videopath.json");
                string json = JsonSerializer.Serialize(output, options);
                File.WriteAllText(fileName, json);
            }
            string Json = File.ReadAllText(PersonalFolder + "/videopath.json");
            Output output1 = JsonSerializer.Deserialize<Output>(Json);
            OutputFolder = output1.OutputPath;
            InputFiles = Directory.GetFiles(output1.InputPath, "*.mp4");
            return;
        }
        public async Task CreateVideo()
        {
            string output = OutputFolder;
            if (File.Exists(output))
            {
                var CurrentTime = DateTime.Now.ToString("d.M.yyyy-H-m-ss");
                File.Move(output, $"{DefaultVideosFolder}/{CurrentTime}.mp4");
            }
            var conversion = await FFmpeg.Conversions.FromSnippet.Concatenate(output, InputFiles);
            conversion.OnProgress += (sender, args) =>
            {
                var percent = (int)(Math.Round(args.Duration.TotalSeconds / args.TotalLength.TotalSeconds, 2) * 100);
                Console.WriteLine($"[{args.Duration} / {args.TotalLength}] {percent}%");
            };
            await conversion.Start();
        }
        public async Task Start()
        {
            await CreateVideo();
            Console.WriteLine($"Successfully created the video at '{OutputFolder}'.");
            Console.WriteLine("Press any key to exit.");
            Console.Read();
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
