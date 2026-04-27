using BCCStudents.Presentation.Properties;
using Newtonsoft.Json;

namespace BCCStudents.Presentation
{
    /// <summary>
    /// Helper კლასი ფორმების და DataGridView-ების კონფიგურაციის შესანახად.
    /// მონაცემები ინახება JSON ფორმატით Properties.Settings.Default-ში.
    /// გამოიყენება კონსოლიდირებული Dictionary-ები: ColumnVisibilitySettings და ColumnWidthsSettings,
    /// სადაც Key არის "FormName_DataGridViewName" და Value არის სვეტების კონფიგურაცია.
    /// </summary>
    public static class SettingsHelper
    {
        // კონსტანტები Settings Property-ებისთვის
        private const string COLUMN_VISIBILITY_SETTINGS_KEY = "ColumnVisibilitySettings";
        private const string COLUMN_WIDTHS_SETTINGS_KEY = "ColumnWidthsSettings";

        /// <summary>
        /// სვეტების ხილულობის კონფიგურაციის შენახვა
        /// </summary>
        /// <param name="formName">ფორმის სახელი</param>
        /// <param name="dgvName">DataGridView-ის სახელი</param>
        /// <param name="visibility">სვეტების ხილულობის Dictionary</param>
        public static void SaveColumnVisibility(string formName, string dgvName, Dictionary<string, bool> visibility)
        {
            try
            {
                string settingsKey = $"{formName}_{dgvName}";

                // ვტვირთავთ არსებულ Dictionary-ს
                Dictionary<string, Dictionary<string, bool>> allSettings = LoadAllColumnVisibility();
                if (allSettings == null)
                {
                    allSettings = new Dictionary<string, Dictionary<string, bool>>();
                }

                // ვამატებთ/ვანაცვლებთ კონკრეტული ფორმის კონფიგურაციას
                allSettings[settingsKey] = visibility;

                // ვინახავთ
                string jsonData = JsonConvert.SerializeObject(allSettings);
                Settings.Default[COLUMN_VISIBILITY_SETTINGS_KEY] = jsonData;
                Settings.Default.Save();
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"შეცდომა სვეტების ხილულობის შენახვისას ({formName}_{dgvName}): {ex.Message}", ex);
            }
        }

        /// <summary>
        /// სვეტების ხილულობის კონფიგურაციის ჩატვირთვა
        /// </summary>
        /// <param name="formName">ფორმის სახელი</param>
        /// <param name="dgvName">DataGridView-ის სახელი</param>
        /// <param name="defaultValue">ნაგულისხმევი მნიშვნელობა, თუ კონფიგურაცია არ არსებობს</param>
        /// <returns>სვეტების ხილულობის Dictionary</returns>
        public static Dictionary<string, bool> LoadColumnVisibility(string formName, string dgvName, Dictionary<string, bool> defaultValue = null)
        {
            try
            {
                string settingsKey = $"{formName}_{dgvName}";

                Dictionary<string, Dictionary<string, bool>> allSettings = LoadAllColumnVisibility();

                if (allSettings != null && allSettings.ContainsKey(settingsKey))
                {
                    return allSettings[settingsKey];
                }

                return defaultValue ?? new Dictionary<string, bool>();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"შეცდომა სვეტების ხილულობის ჩატვირთვისას ({formName}_{dgvName}): {ex.Message}");
                return defaultValue ?? new Dictionary<string, bool>();
            }
        }

        /// <summary>
        /// სვეტების ზომების კონფიგურაციის შენახვა
        /// </summary>
        /// <param name="formName">ფორმის სახელი</param>
        /// <param name="dgvName">DataGridView-ის სახელი</param>
        /// <param name="widths">სვეტების ზომების Dictionary</param>
        public static void SaveColumnWidths(string formName, string dgvName, Dictionary<string, int> widths)
        {
            try
            {
                string settingsKey = $"{formName}_{dgvName}";

                // ვტვირთავთ არსებულ Dictionary-ს
                Dictionary<string, Dictionary<string, int>> allSettings = LoadAllColumnWidths();
                if (allSettings == null)
                {
                    allSettings = new Dictionary<string, Dictionary<string, int>>();
                }

                // ვამატებთ/ვანაცვლებთ კონკრეტული ფორმის კონფიგურაციას
                allSettings[settingsKey] = widths;

                // ვინახავთ
                string jsonData = JsonConvert.SerializeObject(allSettings);
                Settings.Default[COLUMN_WIDTHS_SETTINGS_KEY] = jsonData;
                Settings.Default.Save();
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"შეცდომა სვეტების ზომების შენახვისას ({formName}_{dgvName}): {ex.Message}", ex);
            }
        }

        /// <summary>
        /// სვეტების ზომების კონფიგურაციის ჩატვირთვა
        /// </summary>
        /// <param name="formName">ფორმის სახელი</param>
        /// <param name="dgvName">DataGridView-ის სახელი</param>
        /// <param name="defaultValue">ნაგულისხმევი მნიშვნელობა, თუ კონფიგურაცია არ არსებობს</param>
        /// <returns>სვეტების ზომების Dictionary</returns>
        public static Dictionary<string, int> LoadColumnWidths(string formName, string dgvName, Dictionary<string, int> defaultValue = null)
        {
            try
            {
                string settingsKey = $"{formName}_{dgvName}";

                Dictionary<string, Dictionary<string, int>> allSettings = LoadAllColumnWidths();

                if (allSettings != null && allSettings.ContainsKey(settingsKey))
                {
                    return allSettings[settingsKey];
                }

                return defaultValue ?? new Dictionary<string, int>();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"შეცდომა სვეტების ზომების ჩატვირთვისას ({formName}_{dgvName}): {ex.Message}");
                return defaultValue ?? new Dictionary<string, int>();
            }
        }

        /// <summary>
        /// ყველა ფორმის სვეტების ხილულობის კონფიგურაციის ჩატვირთვა
        /// </summary>
        private static Dictionary<string, Dictionary<string, bool>> LoadAllColumnVisibility()
        {
            try
            {
                string jsonData = Settings.Default[COLUMN_VISIBILITY_SETTINGS_KEY] as string;

                if (string.IsNullOrWhiteSpace(jsonData))
                {
                    return new Dictionary<string, Dictionary<string, bool>>();
                }

                return JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, bool>>>(jsonData)
                    ?? new Dictionary<string, Dictionary<string, bool>>();
            }
            catch
            {
                return new Dictionary<string, Dictionary<string, bool>>();
            }
        }

        /// <summary>
        /// ყველა ფორმის სვეტების ზომების კონფიგურაციის ჩატვირთვა
        /// </summary>
        private static Dictionary<string, Dictionary<string, int>> LoadAllColumnWidths()
        {
            try
            {
                string jsonData = Settings.Default[COLUMN_WIDTHS_SETTINGS_KEY] as string;

                if (string.IsNullOrWhiteSpace(jsonData))
                {
                    return new Dictionary<string, Dictionary<string, int>>();
                }

                return JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, int>>>(jsonData)
                    ?? new Dictionary<string, Dictionary<string, int>>();
            }
            catch
            {
                return new Dictionary<string, Dictionary<string, int>>();
            }
        }

        #region დეპრეკატირებული მეთოდები (Backward Compatibility)

        /// <summary>
        /// [დეპრეკატირებული] ზოგადი Save მეთოდი - გამოიყენება სხვა ტიპის კონფიგურაციებისთვის
        /// </summary>
        [Obsolete("გამოიყენეთ SaveColumnVisibility ან SaveColumnWidths სვეტების კონფიგურაციისთვის")]
        public static void Save(string key, object data)
        {
            try
            {
                string jsonData = data == null ? null : JsonConvert.SerializeObject(data);
                Settings.Default[key] = jsonData;
                Settings.Default.Save();
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"შეცდომა მონაცემების შენახვისას (Key: {key}): {ex.Message}", ex);
            }
        }

        /// <summary>
        /// [დეპრეკატირებული] ზოგადი Load მეთოდი - გამოიყენება სხვა ტიპის კონფიგურაციებისთვის
        /// </summary>
        [Obsolete("გამოიყენეთ LoadColumnVisibility ან LoadColumnWidths სვეტების კონფიგურაციისთვის")]
        public static T Load<T>(string key, T defaultValue = default(T))
        {
            try
            {
                string jsonData = Settings.Default[key] as string;

                if (string.IsNullOrWhiteSpace(jsonData))
                {
                    return defaultValue;
                }

                T result = JsonConvert.DeserializeObject<T>(jsonData);
                return result ?? defaultValue;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"შეცდომა მონაცემების წაკითხვისას (Key: {key}): {ex.Message}");
                return defaultValue;
            }
        }

        /// <summary>
        /// [დეპრეკატირებული] მონაცემების წაშლა
        /// </summary>
        [Obsolete("გამოიყენეთ RemoveColumnSettings სვეტების კონფიგურაციისთვის")]
        public static void Remove(string key)
        {
            try
            {
                if (Settings.Default.Properties[key] != null)
                {
                    Settings.Default[key] = null;
                    Settings.Default.Save();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"შეცდომა მონაცემების წაშლისას (Key: {key}): {ex.Message}");
            }
        }

        /// <summary>
        /// [დეპრეკატირებული] Key-ის არსებობის შემოწმება
        /// </summary>
        [Obsolete("გამოიყენეთ ExistsColumnSettings სვეტების კონფიგურაციისთვის")]
        public static bool Exists(string key)
        {
            try
            {
                string value = Settings.Default[key] as string;
                return !string.IsNullOrWhiteSpace(value);
            }
            catch
            {
                return false;
            }
        }

        #endregion

        /// <summary>
        /// კონკრეტული ფორმის სვეტების კონფიგურაციის წაშლა
        /// </summary>
        public static void RemoveColumnSettings(string formName, string dgvName)
        {
            try
            {
                string settingsKey = $"{formName}_{dgvName}";

                // ვტვირთავთ და ვშლით visibility-ს
                Dictionary<string, Dictionary<string, bool>> visibilitySettings = LoadAllColumnVisibility();
                if (visibilitySettings != null && visibilitySettings.ContainsKey(settingsKey))
                {
                    visibilitySettings.Remove(settingsKey);
                    string jsonData = JsonConvert.SerializeObject(visibilitySettings);
                    Settings.Default[COLUMN_VISIBILITY_SETTINGS_KEY] = jsonData;
                }

                // ვტვირთავთ და ვშლით widths-ს
                Dictionary<string, Dictionary<string, int>> widthsSettings = LoadAllColumnWidths();
                if (widthsSettings != null && widthsSettings.ContainsKey(settingsKey))
                {
                    widthsSettings.Remove(settingsKey);
                    string jsonData = JsonConvert.SerializeObject(widthsSettings);
                    Settings.Default[COLUMN_WIDTHS_SETTINGS_KEY] = jsonData;
                }

                Settings.Default.Save();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"შეცდომა სვეტების კონფიგურაციის წაშლისას ({formName}_{dgvName}): {ex.Message}");
            }
        }

        /// <summary>
        /// შეამოწმებს, არსებობს თუ არა კონფიგურაცია კონკრეტული ფორმისთვის
        /// </summary>
        public static bool ExistsColumnSettings(string formName, string dgvName)
        {
            try
            {
                string settingsKey = $"{formName}_{dgvName}";

                Dictionary<string, Dictionary<string, bool>> visibilitySettings = LoadAllColumnVisibility();
                return visibilitySettings != null && visibilitySettings.ContainsKey(settingsKey);
            }
            catch
            {
                return false;
            }
        }
    }
}
