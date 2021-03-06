using Amazon.Lambda.Core;
using System.Collections;
using Amazon.Lambda.APIGatewayEvents;
using System.Text.Json;
using System.Collections.Generic;
// this does not appear to contain the JsonSerializer - it is in System.Text.Json above
using System.Text.Json.Serialization;

[assembly:LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]
namespace AwsDotnetCsharp
{
    public class Handler
    {
       public APIGatewayProxyResponse GetTasks(APIGatewayProxyRequest request)
       {  
         // PathParameters is a library - so access via key: https://github.com/aws/aws-lambda-dotnet/tree/master/Libraries/src/Amazon.Lambda.APIGatewayEvents
          string userId = request.PathParameters["userId"];
          ArrayList tasks = new ArrayList();

          // LambdaLogger is in Amazon.Lambda.Core
          LambdaLogger.Log("Getting tasks for: " + userId);
          
          if (userId == "abc123") {
            Task t1 = new Task("abc1234", "Buy milk", false);
            tasks.Add(t1);
          } else {
            Task t2 = new Task("abc4567", "Get Newspaper", false);
            tasks.Add(t2);
          }
          
          return new APIGatewayProxyResponse(){
            Body = JsonSerializer.Serialize(tasks), 
            Headers = new Dictionary<string, string>
              { 
                { "Content-Type", "application/json" }, 
                { "Access-Control-Allow-Origin", "*" } 
              },
            StatusCode = 200
          };
       }

       public APIGatewayProxyResponse SaveTask(APIGatewayProxyRequest request)
       {  

          string requestBody = request.Body;
          Task t = JsonSerializer.Deserialize<Task>(requestBody);
          LambdaLogger.Log("Saving task: " + t.Description);

         return new APIGatewayProxyResponse(){
            Body = "Task Saved", 
            Headers = new Dictionary<string, string>
              { 
                { "Content-Type", "application/json" }, 
                { "Access-Control-Allow-Origin", "*" } 
              },
            StatusCode = 200
          };
       }
    }

    public class Task
    {
      public string TaskId {get; set;}
      public string Description {get; set;}
      public bool Completed {get; set;}

      public Task(){}
      public Task(string taskId, string description, bool completed){
        TaskId = taskId;
        Description = description;
        Completed = completed;
      }
    }

    public class Request
    {
      public string Key1 {get; set;}
      public string Key2 {get; set;}
      public string Key3 {get; set;}
    }
}
