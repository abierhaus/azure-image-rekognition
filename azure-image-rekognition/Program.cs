using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace azure_image_rekognition
{
    internal class Program
    {
        // Add your Computer Vision subscription key and endpoint to your environment variables.
        private static readonly string subscriptionKey = "Your API Key here";
        private static readonly string endpoint = "https://yourendpoint.cognitiveservices.azure.com/";

        private static async Task Main(string[] args)
        {
            // Call the API
            await Detect();
        }


        private static async Task Detect()
        {
            var client = new HttpClient();

            // Request headers.
            client.DefaultRequestHeaders.Add(
                "Ocp-Apim-Subscription-Key", subscriptionKey);


            // Add parameters that should be returned. Possible Parameters: Categories, Description, Color
            var requestParameters =
                "visualFeatures=Description";

            // Assemble the URI for the REST API method.
            var uri = $"{endpoint}vision/v3.1/analyze?{requestParameters}";


            // Get picture from local storage
            var photo = "img/sheep.png";
            await using var fs = new FileStream(photo, FileMode.Open, FileAccess.Read);
            var binaryReader = new BinaryReader(fs);
            var byteData = binaryReader.ReadBytes((int) fs.Length);


            HttpResponseMessage response;
            using (var content = new ByteArrayContent(byteData))
            {
                content.Headers.ContentType =
                    new MediaTypeHeaderValue("application/octet-stream");

                // Asynchronously call the REST API method.
                response = await client.PostAsync(uri, content);
            }

            // Call API
            var contentString = await response.Content.ReadAsStringAsync();

            var detectLabelResponse = JsonConvert.DeserializeObject<DetectLabelResponse>(contentString);

            Console.WriteLine("Detected labels for " + photo);
            foreach (var label in detectLabelResponse.Description.Tags) Console.WriteLine(label);
        }


        public class Description
        {
            public List<string> Tags { get; set; }
        }


        public class DetectLabelResponse
        {
            public Description Description { get; set; }
            public string RequestId { get; set; }
        }
    }
}