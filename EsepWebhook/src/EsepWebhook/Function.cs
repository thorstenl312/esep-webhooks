using System.Text;
using Amazon.Lambda.Core;
using Newtonsoft.Json;


// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace EsepWebhook;

public class Function
{   
    /// <summary>
    /// A simple function that takes a string and does a ToUpper
    /// </summary>
    /// <param name="input"></param>
    /// <param name="context"></param>
    /// <returns></returns>
    public string FunctionHandler(object input, ILambdaContext context)
    {
        dynamic json = JsonConvert.DeserializeObject<dynamic>(input.ToString());
        string payload = "";

        if ($"{json.issue.state}" == "open")
        {
          payload = $"{{'text':'Issue Opened: {json.issue.html_url}'}}";
        }
        else if ($"{json.issue.state}" == "closed") {
          payload = $"{{'text':'Issue Closed: {json.issue.html_url}'}}";
        }
        
        var client = new HttpClient();
        var webRequest = new HttpRequestMessage(HttpMethod.Post, "{do not check in this URL}")
        {
            Content = new StringContent(payload, Encoding.UTF8, "application/json")
        };

        var response = client.Send(webRequest);
        using var reader = new StreamReader(response.Content.ReadAsStream());
            
        return reader.ReadToEnd();
    }
}