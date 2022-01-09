using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.UserSecrets;
using Newtonsoft.Json.Linq;

namespace CraftersCloud.AspNetCore.CopyAzureKeyVaultSecrets.HostingStartup
{
    public class SecretsStore
    {
        private readonly IDictionary<string, string> _secrets;
        private readonly string _secretsFilePath;

        public SecretsStore(string userSecretsId)
        {
            _secretsFilePath = PathHelper.GetSecretsPathFromSecretsId(userSecretsId);

            CreateSecretsDirectory();

            _secrets = Load();
        }

        public string this[string key] => _secrets[key];

        public int Count => _secrets.Count;

        public bool ContainsKey(string key) => _secrets.ContainsKey(key);

        public IEnumerable<KeyValuePair<string, string>> AsEnumerable() => _secrets;

        public void Clear() => _secrets.Clear();

        public void Set(string key, string value) => _secrets[key] = value;

        public void Remove(string key)
        {
            if (_secrets.ContainsKey(key))
            {
                _secrets.Remove(key);
            }
        }

        public void Save()
        {
            CreateSecretsDirectory();

            var contents = new JObject();
            if (_secrets != null)
            {
                foreach (KeyValuePair<string, string> secret in _secrets.AsEnumerable())
                {
                    contents[secret.Key] = secret.Value;
                }
            }

            File.WriteAllText(_secretsFilePath, contents.ToString(), Encoding.UTF8);
        }

        private IDictionary<string, string> Load()
        {
            return new ConfigurationBuilder()
                .AddJsonFile(_secretsFilePath, true)
                .Build()
                .AsEnumerable()
                .Where(i => i.Value != null)
                .ToDictionary(i => i.Key, i => i.Value, StringComparer.OrdinalIgnoreCase);
        }

        public void CopyValuesFrom(IConfiguration sourceConfiguration)
        {
            foreach (KeyValuePair<string, string> item in sourceConfiguration.AsEnumerable()
                         .Where(i => i.Value != null))
            {
                Set(item.Key, item.Value);
            }
        }

        public void UpdateLastRefreshTime(DateTime value)
        {
            Set($"{GetPrefix()}_LastRefreshDateTime", value.ToString("s", CultureInfo.InvariantCulture));
        }

        public DateTime GetLastRefreshDateTime()
        {
            _secrets.TryGetValue($"{GetPrefix()}_LastRefreshDateTime", out string lastRefreshTimeStr);

            if (DateTime.TryParse(lastRefreshTimeStr, out DateTime result))
            {
                return result;
            }

            return DateTime.MinValue;
        }

        private static string GetPrefix() => nameof(SecretsStore);

        private void CreateSecretsDirectory()
        {
            string secretDir = Path.GetDirectoryName(_secretsFilePath);
            Directory.CreateDirectory(secretDir);
        }
    }
}
