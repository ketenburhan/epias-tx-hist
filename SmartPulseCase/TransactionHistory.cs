namespace SmartPulseCase.TransactionHistory {
    public class TransactionHistoryItem {
        public required string date { get; set; }
        public required string hour { get; set; }
        public required string contractName { get; set; }
        public required double price { get; set; }
        public required int quantity { get; set; }
        public required long id { get; set; }
    }

    public class TransactionHistory
    {
        public required List<TransactionHistoryItem> items { get; set; }
    }

}