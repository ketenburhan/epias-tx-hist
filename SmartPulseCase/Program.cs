using System;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;
using DotNetEnv;
using ConsoleTables;
using SmartPulseCase;
using SmartPulseCase.EpiasClient;
using SmartPulseCase.TransactionHistory;
using System.Text.Json;

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

        // Console.WriteLine(rawTxHist);

        TransactionHistory? txHist = JsonSerializer.Deserialize<TransactionHistory>(rawTxHist);

        var groups = txHist.items.GroupBy(tx => tx.contractName);

        var table = new ConsoleTable("Tarih", "Toplam İşlem Tutarı", "Toplam İşlem Miktarı", "Ağırlıklı Ortalama Fiyat");

        foreach (var group in groups) {
            double totalTxCost = 0;
            int totalTxQuantity = 0;
            foreach (var tx in group) {
                totalTxCost += (double)tx.price * (double)tx.quantity;
                totalTxQuantity += (int)tx.quantity;
            }
            totalTxCost /= 10;
            totalTxQuantity /= 10;

            double weightedAveragePrice = totalTxCost / (double)totalTxQuantity;

            string y = group.Key.Substring(2, 2);
            string m = group.Key.Substring(4, 2);
            string d = group.Key.Substring(6, 2);
            string h = group.Key.Substring(8, 2);

            string date = $"{d}.{m}.20{y} {h}:00";

            table.AddRow(date, totalTxCost.ToString("C2", CultureInfo.CreateSpecificCulture("tr-TR")), totalTxQuantity, weightedAveragePrice.ToString("C2", CultureInfo.CreateSpecificCulture("tr-TR")));
        }

        table.Write();
    }
}
