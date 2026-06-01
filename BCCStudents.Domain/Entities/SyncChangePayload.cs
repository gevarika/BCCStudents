using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
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

            _payloadJson = new Lazy<string>(() => Serialize(Data, KeyColumns));
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

        private static string Serialize(IReadOnlyDictionary<string, object> data, IReadOnlyDictionary<string, object> keyColumns)
        {
            var normalizedData = data.ToDictionary(
                pair => pair.Key,
                pair => NormalizeValue(pair.Value),
                StringComparer.OrdinalIgnoreCase);

            var wrapper = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase)
            {
                ["Data"] = normalizedData
            };

            if (keyColumns != null && keyColumns.Count > 0)
            {
                wrapper["KeyColumns"] = keyColumns.ToDictionary(
                    pair => pair.Key,
                    pair => NormalizeValue(pair.Value),
                    StringComparer.OrdinalIgnoreCase);
            }

            return JsonConvert.SerializeObject(wrapper);
        }

        internal static Dictionary<string, object> DeserializeData(string payloadJson)
        {
            if (string.IsNullOrWhiteSpace(payloadJson))
            {
                return new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
            }

            var raw = JsonConvert.DeserializeObject<Dictionary<string, object>>(payloadJson)
                      ?? new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);

            if (raw.TryGetValue("Data", out var dataToken))
            {
                return ConvertToDictionary(dataToken);
            }

            return new Dictionary<string, object>(raw, StringComparer.OrdinalIgnoreCase);
        }

        internal static Dictionary<string, object> DeserializeKeyColumns(string payloadJson)
        {
            if (string.IsNullOrWhiteSpace(payloadJson))
            {
                return new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
            }

            var raw = JsonConvert.DeserializeObject<Dictionary<string, object>>(payloadJson);
            if (raw == null || !raw.TryGetValue("KeyColumns", out var keyToken))
            {
                return new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
            }

            return ConvertToDictionary(keyToken);
        }

        private static Dictionary<string, object> ConvertToDictionary(object token)
        {
            if (token is Dictionary<string, object> dict)
            {
                return new Dictionary<string, object>(dict, StringComparer.OrdinalIgnoreCase);
            }

            if (token is JObject jObject)
            {
                return jObject.ToObject<Dictionary<string, object>>()
                       ?? new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
            }

            return new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
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
                    return ToUtcIsoString(dateTime);
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

        private static string ToUtcIsoString(DateTime dateTime)
        {
            if (dateTime.Kind == DateTimeKind.Unspecified)
            {
                return DateTime.SpecifyKind(dateTime, DateTimeKind.Local).ToUniversalTime().ToString("o");
            }

            return dateTime.ToUniversalTime().ToString("o");
        }
    }
}


