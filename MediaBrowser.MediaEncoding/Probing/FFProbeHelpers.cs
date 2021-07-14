using System;
using System.Collections.Generic;
using System.Globalization;

namespace MediaBrowser.MediaEncoding.Probing
{
    /// <summary>
    /// Class containing helper methods for working with FFprobe output.
    /// </summary>
    public static class FFProbeHelpers
    {
        /// <summary>
        /// Normalizes the FF probe result.
        /// </summary>
        /// <param name="result">The result.</param>
        public static void NormalizeFFProbeResult(InternalMediaInfoResult result)
        {
            if (result == null)
            {
                throw new ArgumentNullException(nameof(result));
            }

            if (result.Format?.Tags != null)
            {
                result.Format.Tags = ConvertDictionaryToCaseInsensitive(result.Format.Tags);
            }

            if (result.Streams != null)
            {
                // Convert all dictionaries to case insensitive
                foreach (var stream in result.Streams)
                {
                    if (stream.Tags != null)
                    {
                        stream.Tags = ConvertDictionaryToCaseInsensitive(stream.Tags);
                    }
                }
            }
        }

        /// <summary>
        /// Gets an int from an FFProbeResult tags dictionary.
        /// </summary>
        /// <param name="tags">The tags.</param>
        /// <param name="key">The key.</param>
        /// <returns>System.Nullable{System.Int32}.</returns>
        public static int? GetDictionaryNumericValue(IReadOnlyDictionary<string, string> tags, string key)
        {
            if (tags.TryGetValue(key, out var val) && int.TryParse(val, out var i))
            {
                return i;
            }

            return null;
        }

        /// <summary>
        /// Gets a DateTime from an FFProbeResult tags dictionary.
        /// </summary>
        /// <param name="tags">The tags.</param>
        /// <param name="key">The key.</param>
        /// <returns>System.Nullable{DateTime}.</returns>
        public static DateTime? GetDictionaryDateTime(IReadOnlyDictionary<string, string> tags, string key)
        {
            if (tags.TryGetValue(key, out var val)
                && DateTime.TryParse(val, DateTimeFormatInfo.CurrentInfo, DateTimeStyles.AssumeUniversal, out var dateTime))
            {
                return dateTime.ToUniversalTime();
            }

            return null;
        }

        /// <summary>
        /// Converts a dictionary to case insensitive.
        /// </summary>
        /// <param name="dict">The dict.</param>
        /// <returns>Dictionary{System.StringSystem.String}.</returns>
        private static Dictionary<string, string> ConvertDictionaryToCaseInsensitive(IReadOnlyDictionary<string, string> dict)
        {
            return new Dictionary<string, string>(dict, StringComparer.OrdinalIgnoreCase);
        }
    }
}
