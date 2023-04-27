using System.Reflection;
using System.Text.Json;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Upload;
using Google.Apis.YouTube.v3;
using Google.Apis.YouTube.v3.Data;

namespace AutomatedYTChannel
{
    internal class UploadVideo : VideoFiles
    {
        public async Task Run()
        {
            UserCredential credential;
            using (var stream = new FileStream($"{PersonalFolder}/client_secrets.json", FileMode.Open, FileAccess.Read))
            {
                credential = await GoogleWebAuthorizationBroker.AuthorizeAsync(
                GoogleClientSecrets.FromStream(stream).Secrets,
                new[] { YouTubeService.Scope.YoutubeUpload },
                "Dimitar Petrov",
                CancellationToken.None
                );
            }
            var youtubeService = new YouTubeService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = Assembly.GetExecutingAssembly().GetName().Name
            });

            if (!File.Exists(PersonalFolder + "/videooptions.json"))
            {
                var video = new Video
                {
                    Snippet = new VideoSnippet
                    {
                        Title = "Default Video Title",
                        Description = "Default Video Description",
                        Tags = new string[] { "tag1", "tag2" },
                        CategoryId = "20" // See https://developers.google.com/youtube/v3/docs/videoCategories/list
                    },
                    Status = new VideoStatus()
                };
                var options = new JsonSerializerOptions { WriteIndented = true, IncludeFields = true };
                var fileName = Path.Combine(PersonalFolder + "/videooptions.json");
                var json = JsonSerializer.Serialize(video, options);
                File.WriteAllText(fileName, json);
            }

            var Json = File.ReadAllText(PersonalFolder + "/videooptions.json");
            var video1 = JsonSerializer.Deserialize<Video>(Json);
            ReadSettings();

            video1.Status.PrivacyStatus = "private"; // or "private" or "public"
            var filePath = $"{OutputFolder}";

            using (var fileStream = new FileStream(filePath, FileMode.Open))
            {
                var videosInsertRequest = youtubeService.Videos.Insert(video1, "snippet,status", fileStream, "video/*");
                videosInsertRequest.ProgressChanged += VideosInsertRequest_ProgressChanged;
                videosInsertRequest.ResponseReceived += VideosInsertRequest_ResponseReceived;

            await videosInsertRequest.UploadAsync();
            }
        }

    void VideosInsertRequest_ProgressChanged(IUploadProgress progress)
    {
      switch (progress.Status)
      {
        case UploadStatus.Uploading:
          Console.WriteLine("{0} bytes sent.", progress.BytesSent);
          break;

        case UploadStatus.Failed:
          Console.WriteLine("An error prevented the upload from completing.\n{0}", progress.Exception);
          break;
      }
    }

    void VideosInsertRequest_ResponseReceived(Video video)
    {
      Console.WriteLine("Video id '{0}' was successfully uploaded.", video.Id);
    }
  }
}

