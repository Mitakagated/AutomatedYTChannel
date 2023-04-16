using AutomatedYTChannel;

var videoFiles = new VideoFiles();
videoFiles.ReadSettings();
foreach (var InputFile in videoFiles.InputFiles)
{
    Console.WriteLine(InputFile);
}
Console.WriteLine(videoFiles.OutputFolder);
await videoFiles.CreateVideo();