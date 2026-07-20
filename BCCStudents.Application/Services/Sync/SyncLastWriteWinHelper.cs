using System.Globalization;

namespace BCCStudents.Application.Services.Sync
{
    /// <summary>
    /// Last-Write-Wins (UpdatedAt/CreatedAt) SQL ფრაგმენტები UpStream/DownStream upsert-ისთვის.
    /// </summary>
    internal static class SyncLastWriteWinHelper
    {
        public static string ResolveVersionColumn(string tableName, IEnumerable<string> columnNames)
        {
            var columns = new HashSet<string>(columnNames ?? Array.Empty<string>(), StringComparer.OrdinalIgnoreCase);

            switch (tableName?.Trim().ToUpperInvariant())
            {
                case "FAILEDPAYMENTS":
                case "IMPORTEDPAYMENTSLOG":
                    return columns.Contains("CreatedAt") ? "CreatedAt" : null;

                case "PAYMENTS":
                    if (columns.Contains("UpdatedAt"))
                    {
                        return "UpdatedAt";
                    }

                    return columns.Contains("PaymentDate") ? "PaymentDate" : null;

                default:
                    return columns.Contains("UpdatedAt") ? "UpdatedAt" : null;
            }
        }

        public static string BuildDuplicateKeyAssignment(string columnName, string versionColumn)
        {
            var column = EscapeIdentifier(columnName);
            var version = EscapeIdentifier(versionColumn);

            if (string.Equals(columnName, versionColumn, StringComparison.OrdinalIgnoreCase))
            {
                return $"{column} = IF(VALUES({version}) >= {version} OR {version} IS NULL, VALUES({column}), {column})";
            }

            return $"{column} = IF(VALUES({version}) >= {version} OR {version} IS NULL, VALUES({column}), {column})";
        }

        public static bool TryResolveWatermark(
            string tableName,
            IReadOnlyDictionary<string, object> data,
            out DateTime syncedAt,
            out int syncedId)
        {
            syncedAt = default;
            syncedId = TryGetRecordId(data);
            if (syncedId <= 0)
            {
                return false;
            }

            var columns = data?.Keys ?? Array.Empty<string>();

            switch (tableName?.Trim().ToUpperInvariant())
            {
                case "STUDENTS":
                case "GROUPS":
                case "SUBGROUPS":
                case "STUDENTGROUPS":
                case "STUDENTSUBGROUPS":
                case "USERS":
                    return TryGetDateTime(data, "UpdatedAt", out syncedAt);

                case "PAYMENTS":
                    if (TryGetDateTime(data, "UpdatedAt", out syncedAt))
                    {
                        return true;
                    }

                    return TryGetDateTime(data, "PaymentDate", out syncedAt);

                case "FAILEDPAYMENTS":
                case "IMPORTEDPAYMENTSLOG":
                    return TryGetDateTime(data, "CreatedAt", out syncedAt);

                default:
                    if (TryGetDateTime(data, "UpdatedAt", out syncedAt))
                    {
                        return true;
                    }

                    return TryGetDateTime(data, "CreatedAt", out syncedAt);
            }
        }

        public static bool IsAhead(DateTime syncedAt, int syncedId, DateTime? currentSyncedAt, int currentSyncedId)
        {
            if (!currentSyncedAt.HasValue)
            {
                return true;
            }

            if (syncedAt > currentSyncedAt.Value)
            {
                return true;
            }

            return syncedAt == currentSyncedAt.Value && syncedId > currentSyncedId;
        }

        private static int TryGetRecordId(IReadOnlyDictionary<string, object> data)
        {
            if (data == null)
            {
                return 0;
            }

            if (data.TryGetValue("Id", out var idValue) && idValue != null)
            {
                if (int.TryParse(Convert.ToString(idValue, CultureInfo.InvariantCulture), out var id))
                {
                    return id;
                }
            }

            if (data.TryGetValue("ID", out var altIdValue) && altIdValue != null)
            {
                if (int.TryParse(Convert.ToString(altIdValue, CultureInfo.InvariantCulture), out var id))
                {
                    return id;
                }
            }

            return 0;
        }

        private static bool TryGetDateTime(IReadOnlyDictionary<string, object> data, string key, out DateTime value)
        {
            value = default;
            if (data == null || !data.TryGetValue(key, out var raw) || raw == null || raw == DBNull.Value)
            {
                return false;
            }

            switch (raw)
            {
                case DateTime dateTime:
                    value = dateTime;
                    return true;
                case DateTimeOffset dateTimeOffset:
                    value = dateTimeOffset.DateTime;
                    return true;
                default:
                    return DateTime.TryParse(
                        Convert.ToString(raw, CultureInfo.InvariantCulture),
                        CultureInfo.InvariantCulture,
                        DateTimeStyles.RoundtripKind,
                        out value);
            }
        }

        private static string EscapeIdentifier(string name)
        {
            return $"`{name}`";
        }
    }
}
