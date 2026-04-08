using System;
using System.Linq;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using Project.DatabaseUtilities;
using Project.LoggingUtilities;
using Project.ServerUtilities;

class Program
{
  static void Main()
  {
    int port = 5000;

    var server = new Server(port);
    var database = new Database();

    Console.WriteLine("The server is running");
    Console.WriteLine($"Local:   http://localhost:{port}/website/pages/index.html");
    Console.WriteLine($"Network: http://{Network.GetLocalNetworkIPAddress()}:{port}/website/pages/index.html");

    while (true)
    {
      var request = server.WaitForRequest();

      Console.WriteLine($"Recieved a request: {request.Name}");

      try
      {
        if (request.Name == "getUser")
        {
          var userId = request.GetParams<string?>();
          var user = database.Users.Find(userId);
          request.Respond(user);
        }
        else if (request.Name == "signUp")
        {
          var (username, password, imgUrl) = request.GetParams<(string, string, string)>();

          if (database.Users.Any(u => u.Name == username))
          {
            continue;
          }

          var userId = Guid.NewGuid().ToString();
          var user = new User(userId, username, password, imgUrl);
          database.Users.Add(user);
          database.SaveChanges();

          request.Respond(userId);
        }
        else if (request.Name == "logIn")
        {
          var (username, password) = request.GetParams<(string, string)>();

          var user = database.Users.FirstOrDefault(u => u.Name == username && u.Password == password);

          request.Respond(user?.Id);
        }
        else if (request.Name == "addMessage")
        {
          var (userId, text) = request.GetParams<(string, string)>();

          var message = new Message(text, userId);
          database.Messages.Add(message);
          database.SaveChanges();
        }
        else if (request.Name == "getMessages")
        {
          request.Respond(database.Messages.Include(m => m.User));
        }
        else if (request.Name == "getMessagesCount")
        {
          request.Respond(database.Messages.Count());
        }
      }
      catch (Exception exception)
      {
        request.SetStatusCode(500);
        Log.WriteException(exception);
      }
    }
  }
}

class Database() : DatabaseCore("database")
{
  public DbSet<User> Users { get; set; } = default!;
  public DbSet<Message> Messages { get; set; } = default!;
}

class User(string id, string name, string password, string imgUrl)
{
  [JsonIgnore] public string Id { get; set; } = id;
  public string Name { get; set; } = name;
  [JsonIgnore] public string Password { get; set; } = password;
  public string ImgUrl { get; set; } = imgUrl;
}

class Message(string text, string userId)
{
  public int Id { get; set; } = default;
  public string Text { get; set; } = text;
  public string UserId { get; set; } = userId;
  public User User { get; set; } = default!;
}
