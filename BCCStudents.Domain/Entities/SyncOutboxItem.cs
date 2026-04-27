using Newtonsoft.Json;

namespace BCCStudents.Domain.Entities
{
    /// <summary>
    /// წარმოადგენს SyncOutbox ცხრილში არსებულ ერთ ჩანაწერს (retry queue item).
    /// </summary>
    public sealed class SyncOutboxItem
    {

        public long Id { get; set; }
        public string TableName { get; set; }
        public SyncOperationType Operation { get; set; }
        public int? RecordId { get; set; }
        public string RecordKey { get; set; }
        public string PayloadJson { get; set; }
        public DateTime OccurredAt { get; set; }
        public int Attempts { get; set; }
        public string LastError { get; set; }

        /// <summary>
        /// აღადგენს payload-ს SyncOutbox ჩანაწერიდან, რათა ხელახლა ვცადოთ გაგზავნა.
        /// </summary>
        public SyncChangePayload ToPayload()
        {
            if (string.IsNullOrWhiteSpace(TableName))
            {
                throw new InvalidOperationException("TableName is required to build payload.");
            }

            var data = DeserializePayload();
            return new SyncChangePayload(TableName, Operation, data, RecordId);
        }

        private Dictionary<string, object> DeserializePayload()
        {
            try
            {
                var raw = string.IsNullOrWhiteSpace(PayloadJson)
                    ? new Dictionary<string, object>()
                    : JsonConvert.DeserializeObject<Dictionary<string, object>>(PayloadJson) ?? new Dictionary<string, object>();

                return new Dictionary<string, object>(raw, StringComparer.OrdinalIgnoreCase);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Payload JSON parsing failed (Id={Id}).", ex);
            }
        }
    }
}



