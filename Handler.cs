using Amazon.Lambda.Core;
using System.Collections;
using Amazon.Lambda.APIGatewayEvents;
using System.Text.Json;
using System.Collections.Generic;
// this does not appear to contain the JsonSerializer - it is in System.Text.Json above
using System.Text.Json.Serialization;
using MySql.Data.MySqlClient;

[assembly:LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]
namespace AwsDotnetCsharp
{
  // need to pull out logic into different classes - e.g. classes to get the data
    public class Handler
    {
      private string DB_HOST = System.Environment.GetEnvironmentVariable("DB_HOST");
      private string DB_NAME = System.Environment.GetEnvironmentVariable("DB_NAME");
      private string DB_USER = System.Environment.GetEnvironmentVariable("DB_USER");
      private string DB_PASSWORD = System.Environment.GetEnvironmentVariable("DB_PASSWORD");

       public APIGatewayProxyResponse GetTasks(APIGatewayProxyRequest request)
       {  
  
         // PathParameters is a library - so access via key: https://github.com/aws/aws-lambda-dotnet/tree/master/Libraries/src/Amazon.Lambda.APIGatewayEvents
          string userId = request.PathParameters["userId"];
          
          // LambdaLogger is in Amazon.Lambda.Core
          LambdaLogger.Log("Getting tasks for: " + userId);


          // need to create an external connection object & extract connection logic out

          //  establish the connection
          MySqlConnection connection = new MySqlConnection($"server={DB_HOST}; port=3306; database={DB_NAME}; uid={DB_USER}; pwd={DB_PASSWORD};");

          // put in a separate class
          // open the connection
          connection.Open();

          // created a command & set up the command with a query & set up parameters - the userId from the request parameters above
          var cmd = connection.CreateCommand();
          cmd.CommandText = @"SELECT * FROM `tasks` WHERE `UserId` = @userId;";
          cmd.Parameters.AddWithValue("@userId", userId);


          // set up a data reader to execute the reader
          MySqlDataReader reader = cmd.ExecuteReader();

          ArrayList tasks = new ArrayList();

          // loop through the results & create a task object for each result
          while (reader.Read()){

            Task task = new Task(reader.GetString("taskId"), reader.GetString("description"), reader.GetBoolean("completed"));

            tasks.Add(task);
            
          }

          // put in a try/catch
          connection.Close();
          
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
