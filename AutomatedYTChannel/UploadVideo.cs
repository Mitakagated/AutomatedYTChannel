using System.Reflection;
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
            using (var stream = new FileStream($"{Environment.GetFolderPath(Environment.SpecialFolder.Personal)}/client_secrets.json", FileMode.Open, FileAccess.Read))
            {
            credential = await GoogleWebAuthorizationBroker.AuthorizeAsync(
            GoogleClientSecrets.FromStream(stream).Secrets,
            // This OAuth 2.0 access scope allows an application to upload files to the
            // authenticated user's YouTube channel, but doesn't allow other types of access.
            new[] { YouTubeService.Scope.YoutubeUpload },
            "user",
            CancellationToken.None
            );
            }
            var youtubeService = new YouTubeService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = Assembly.GetExecutingAssembly().GetName().Name
            });
        var video = new Video
        {
            Snippet = new VideoSnippet
            {
                Title = "Default Video Title",
                Description = "Default Video Description",
                Tags = new string[] { "tag1", "tag2" },
                CategoryId = "22" // See https://developers.google.com/youtube/v3/docs/videoCategories/list
            },
            Status = new VideoStatus()
            };
            video.Status.PrivacyStatus = "unlisted"; // or "private" or "public"
            var filePath = $"{GetOutputFolder}"; // Replace with path to actual movie file.

            using (var fileStream = new FileStream(filePath, FileMode.Open))
            {
                var videosInsertRequest = youtubeService.Videos.Insert(video, "snippet,status", fileStream, "video/*");
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

