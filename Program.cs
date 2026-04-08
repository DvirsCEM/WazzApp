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
          var token = request.GetParams<string?>();
          var user = token == null ? null : database.Users.FirstOrDefault(u => u.Token == token);
          request.Respond(user);
        }
        else if (request.Name == "signUp")
        {
          var (username, password, imgUrl) = request.GetParams<(string, string, string)>();

          if (database.Users.Any(u => u.Name == username))
          {
            continue;
          }

          var token = Guid.NewGuid().ToString();
          var user = new User(token, username, password, imgUrl);
          database.Users.Add(user);
          database.SaveChanges();

          request.Respond(token);
        }
        else if (request.Name == "logIn")
        {
          var (username, password) = request.GetParams<(string, string)>();

          var user = database.Users.FirstOrDefault(u => u.Name == username && u.Password == password);

          request.Respond(user?.Token);
        }
        else if (request.Name == "addMessage")
        {
          var (token, text) = request.GetParams<(string, string)>();
          var user = database.Users.FirstOrDefault(u => u.Token == token)!;

          var message = new Message(text, user.Id);
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

class User(string token, string name, string password, string imgUrl)
{
  public int Id { get; set; } = default!;
  [JsonIgnore] public string Token { get; set; } = token;
  public string Name { get; set; } = name;
  [JsonIgnore] public string Password { get; set; } = password;
  public string ImgUrl { get; set; } = imgUrl;
}

class Message(string text, int userId)
{
  public int Id { get; set; } = default!;
  public string Text { get; set; } = text;
  public int UserId { get; set; } = userId;
  public User User { get; set; } = default!;
}
