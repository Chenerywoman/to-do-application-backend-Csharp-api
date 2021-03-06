using Amazon.Lambda.Core;

[assembly:LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]
namespace AwsDotnetCsharp
{
    public class Handler
    {
       public Task Hello(Request request)
       {
          Task t1 = new Task("abc1234", "Buy milk", false);
           return t1;
       }
    }

    public class Task
    {
      public string TaskId {get;}
      public string Description {get;}
      public bool Completed {get;}

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
