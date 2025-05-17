using System;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using DotNetEnv;

public partial class Program
{
    const string tgtCacheFileName = "tgt-cache.txt";
    static async Task<string> GetTGTAsync()
    {
        long unixTimestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

        if (File.Exists(tgtCacheFileName))
        {
            Console.WriteLine("TGT Cache Found");
            string content = await File.ReadAllTextAsync(tgtCacheFileName);
            var splitted = content.Split('\n');
            long deadline = long.Parse(splitted[0]);
            if (deadline > unixTimestamp) {
                Console.WriteLine("TGT Using cache: Cache is up to date");
                return splitted[1];
            }
            else {
                Console.WriteLine("TGT Unable to use cache: Cache isn't up to date");
            }
        } else {
            Console.WriteLine("TGT Cache Not Found");
        }

        var username = Environment.GetEnvironmentVariable("EPIAS_USERNAME");
        var password = Environment.GetEnvironmentVariable("EPIAS_PASSWORD");

        if (username == null) {
            throw new Exception("Username could not found: Set EPIDAS_USERNAME on '.env' file");
        }
        if (password == null) {
            throw new Exception("Password could not found: Set EPIDAS_PASSWORD on '.env' file");
        }

        var client = new HttpClient();

        var request = new HttpRequestMessage(HttpMethod.Post, "https://giris.epias.com.tr/cas/v1/tickets");
        request.Headers.Add("Accept", "text/plain");

        var body = new StringContent($"username={username}&password={password}", Encoding.UTF8, "application/x-www-form-urlencoded");
        request.Content = body;



        HttpResponseMessage response = await client.SendAsync(request);
        string tgt = await response.Content.ReadAsStringAsync();

        if (!tgt.StartsWith("TGT")) {
            throw new Exception("TGT value not valid");
        }

        try
        {
            // add two hours
            long deadline = unixTimestamp + 60 * 60 * 2;
            var fileData = deadline.ToString() + "\n" + tgt;
            await File.WriteAllTextAsync("tgt-cache.txt", fileData);

            Console.WriteLine("TGT is saved to 'tgt-cache.txt' file");
        }
        catch (Exception ex)
        {
            Console.WriteLine("TGT wasn't saved to 'tgt-cache.txt' file. Exception: " + ex.Message);
        }

        return tgt;
    }

    static async Task Main(string[] args) {
        Env.Load();

        string tgt = await GetTGTAsync();
        Console.WriteLine("TGT: " + tgt);
    }
}
