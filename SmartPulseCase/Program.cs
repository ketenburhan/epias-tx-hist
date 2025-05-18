using System;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using DotNetEnv;
using SmartPulseCase;
using SmartPulseCase.EpiasClient;

public partial class Program
{
    static async Task Main(string[] args) {
        Env.Load();

        EpiasClient client = new EpiasClient();

        string tgt = await client.GetTGTAsync();
        Console.WriteLine("TGT: " + tgt);

        // TODO: get today
        string todayDate = "2025-05-18T00:00:00+03:00";
        string rawTxHist = await client.GetTransactionHistoryAsync(todayDate, todayDate, tgt);

        Console.WriteLine(rawTxHist);
    }
}
