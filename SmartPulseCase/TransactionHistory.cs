namespace SmartPulseCase.TransactionHistory {
    public class TransactionHistoryItem {
        public string? date { get; set; }
        public string? hour { get; set; }
        public string? contractName { get; set; }
        public double? price { get; set; }
        public int? quantity { get; set; }
        public long? id { get; set; }
    }

    public class TransactionHistory
    {
        public List<TransactionHistoryItem>? items { get; set; }
    }

}