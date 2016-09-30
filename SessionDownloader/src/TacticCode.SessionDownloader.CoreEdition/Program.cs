using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using YoutubeExtractorCore;

namespace TacticCode.SessionDownloader.CoreEdition
{
    public class Program
    {
        public static void Main(string[] args)
        {
            string videoUrl = args[1];

            var downloadInfo = GetDownloadInfo(videoUrl).GetAwaiter().GetResult();

            HttpClient httpClient = new HttpClient();

            httpClient.BaseAddress = new Uri(downloadInfo.DownloadUrl);

            DownloadVideoDataToFile(downloadInfo.DownloadUrl, args[0], downloadInfo.Title);
        }

        static async void DownloadVideoDataToFile(string url, string filePath, string fileName)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(url);
                    client.Timeout = TimeSpan.FromMinutes(5);
                    string requestUrl = url;

                    var request = new HttpRequestMessage(HttpMethod.Get, requestUrl);
                    var getTask = client.GetAsync(url);
                    var response = getTask.Result.EnsureSuccessStatusCode();
                    var httpStream = await response.Content.ReadAsStreamAsync();

                    string OutputDirectory = filePath;

                    if (!Directory.Exists(OutputDirectory))
                    {
                        Directory.CreateDirectory(OutputDirectory);
                    }

                    var fullFilePath = Path.Combine(OutputDirectory, $"{fileName}.mp4");
                    using (var fileStream = File.Create(fullFilePath))
                    using (var reader = new StreamReader(httpStream))
                    {
                        httpStream.CopyTo(fileStream);
                        fileStream.Flush();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

        private static async Task<VideoInfo> GetDownloadInfo(string videoUrl)

        {
            VideoInfo videoWithAudio;
            try

            {

                var videoInfos = await DownloadUrlResolver.GetDownloadUrlsAsync(videoUrl, false);

                videoWithAudio =

                    videoInfos.FirstOrDefault(video => video.Resolution > 0 && video.AudioBitrate > 0);



                if (videoWithAudio != null)

                {
                    return videoWithAudio;

                }

                else

                {
                    throw new Exception("Unable to find video with audio");

                }

            }

            catch (YoutubeVideoNotAvailableException e)

            {
                throw e;
            }

            catch (YoutubeParseException e)

            {
                throw e;

            }

            catch (Exception ex)

            {

                throw ex;

            }
            return videoWithAudio;
        }
    }
}
