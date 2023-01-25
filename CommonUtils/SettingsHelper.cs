using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace CommonUtils
{
    public class SettingsHelper
    {
        public static void WriteSettings<T>(string name, T value)
        {
            var localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
            localSettings.Values[name] = value;
        }

        public static T ReadSettings<T>(string name)
        {
            var localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
            return (T)(localSettings.Values[name] ?? default(T));
        }

        /// <summary>
        /// Read settings with setting name and default value as KeyValue pair.
        /// </summary>
        /// <typeparam name="T">The type of the value.</typeparam>
        /// <param name="keyValuePair">Setting name as Key and default value as Value.</param>
        /// <returns></returns>
        public static T ReadSettings<T>(KeyValuePair<string, T> keyValuePair)
        {
            var localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
            return (T)(localSettings.Values[keyValuePair.Key] ?? keyValuePair.Value);
        }
    }
}
