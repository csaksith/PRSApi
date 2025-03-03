namespace PRSApi.Models {
    public class RequestDTO {
        public int UserId { get; set; }
        public string RequestNumber { get; set; }
        public string Description { get; set; }
        public string Justification { get; set; }
        public DateTime DateNeeded { get; set; }
        public string DeliveryMode { get; set; }
        public string? ReasonForRejection { get; set; }
    }
}
