using Newtonsoft.Json;
using System.Globalization;

namespace BCCStudents.Domain.Entities
{
    /// <summary>
    /// UpStream ცვლილებების payload – შეიცავს მონაცემებს, ოპერაციის ტიპს და JSON-სერიალიზებულ ვერსიას.
    /// </summary>
    public class SyncChangePayload
    {
        private readonly Lazy<string> _payloadJson;

        public SyncChangePayload(
            string tableName,
            SyncOperationType operation,
            IDictionary<string, object> data,
            int? recordId = null,
            IDictionary<string, object> keyColumns = null)
        {
            if (string.IsNullOrWhiteSpace(tableName))
                throw new ArgumentNullException(nameof(tableName));
            if (data == null || data.Count == 0)
                throw new ArgumentNullException(nameof(data));

            TableName = tableName;
            Operation = operation;
            RecordId = recordId ?? TryGetIdFromData(data);
            Data = new Dictionary<string, object>(data, StringComparer.OrdinalIgnoreCase);
            KeyColumns = keyColumns != null
                ? new Dictionary<string, object>(keyColumns, StringComparer.OrdinalIgnoreCase)
                : new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
            CreatedAt = DateTime.Now;

            _payloadJson = new Lazy<string>(() => Serialize(Data));
        }

        public string TableName { get; }
        public SyncOperationType Operation { get; }
        public int? RecordId { get; }
        public IReadOnlyDictionary<string, object> Data { get; }
        public IReadOnlyDictionary<string, object> KeyColumns { get; }
        public DateTime CreatedAt { get; }

        /// <summary>
        /// უნიკალური იდენტიფიკატორი (Id ან კომპოზიტური გასაღები) – გამოიყენება ლოგებში და SyncOutbox-ში.
        /// </summary>
        public string RecordKey
        {
            get
            {
                if (RecordId.HasValue)
                {
                    return RecordId.Value.ToString(CultureInfo.InvariantCulture);
                }

                if (KeyColumns.Count > 0)
                {
                    return string.Join("|", KeyColumns
                        .OrderBy(k => k.Key, StringComparer.OrdinalIgnoreCase)
                        .Select(k => $"{k.Key}:{(k.Value ?? "NULL")}"));
                }

                // უკანაასკნელი fallback – უნიკალური Guid
                return Guid.NewGuid().ToString("N");
            }
        }

        /// <summary>
        /// JSON-ად სერიალიზებული payload (SyncOutbox-ში შესანახად).
        /// </summary>
        public string PayloadJson => _payloadJson.Value;

        private static int? TryGetIdFromData(IDictionary<string, object> data)
        {
            if (data == null)
            {
                return null;
            }

            if (data.TryGetValue("Id", out var idValue) && idValue != null)
            {
                if (int.TryParse(Convert.ToString(idValue, CultureInfo.InvariantCulture), out var id))
                {
                    return id;
                }
            }

            return null;
        }

        private static string Serialize(IReadOnlyDictionary<string, object> data)
        {
            var normalized = data.ToDictionary(
                pair => pair.Key,
                pair => NormalizeValue(pair.Value),
                StringComparer.OrdinalIgnoreCase);

            return JsonConvert.SerializeObject(normalized);
        }

        private static object NormalizeValue(object value)
        {
            if (value == null)
            {
                return null;
            }

            switch (value)
            {
                case DateTime dateTime:
                    return dateTime.ToUniversalTime().ToString("o");
                case DateTimeOffset dateTimeOffset:
                    return dateTimeOffset.UtcDateTime.ToString("o");
                case bool boolean:
                    return boolean;
                case decimal dec:
                    return dec;
                case double dbl:
                    return dbl;
                case float fl:
                    return fl;
                default:
                    return value;
            }
        }
    }
}



