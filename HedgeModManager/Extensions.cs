using System;
using System.Buffers;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace HedgeModManager
{
    public static class Extensions
    {
        public static int GetDeterministicHashCode(this string str)
        {
            unchecked
            {
                int hash1 = (5381 << 16) + 5381;
                int hash2 = hash1;

                for (int i = 0; i < str.Length; i += 2)
                {
                    hash1 = ((hash1 << 5) + hash1) ^ str[i];
                    if (i == str.Length - 1)
                        break;
                    hash2 = ((hash2 << 5) + hash2) ^ str[i + 1];
                }

                return hash1 + (hash2 * 1566083941);
            }
        }

        public static IEnumerable<TSource> DistinctBy<TSource, TKey>
            (this IEnumerable<TSource> source, Func<TSource, TKey> keySelector)
        {
            HashSet<TKey> seenKeys = new HashSet<TKey>();
            foreach (TSource element in source)
            {
                if (seenKeys.Add(keySelector(element)))
                {
                    yield return element;
                }
            }
        }

        public static async Task CopyToAsync(this HttpContent content, Stream destination, IProgress<double?> progress, CancellationToken cancellationToken = default)
        {
            progress?.Report(null);
            using (var source = await content.ReadAsStreamAsync().ConfigureAwait(false))
            {
                // adapted from CoreFX 
                // https://source.dot.net/#System.Private.CoreLib/Stream.cs,8048a9680abdd13b

                if (source is null)
                    throw new ArgumentNullException(nameof(source));
                if (destination is null)
                    throw new ArgumentNullException(nameof(destination));

                var contentLength = content.Headers.ContentLength;
                var bufferLength = 65536; // 64K seams reasonable?
                var buffer = ArrayPool<byte>.Shared.Rent(bufferLength); // prevents allocation of garbage arrays
                try
                {
                    int bytesRead;
                    int totalBytes = 0;
                    while ((bytesRead = await source.ReadAsync(buffer, 0, bufferLength, cancellationToken).ConfigureAwait(false)) != 0)
                    {
                        totalBytes += bytesRead;
                        await destination.WriteAsync(buffer, 0, bytesRead, cancellationToken).ConfigureAwait(false);

                        if (contentLength.HasValue)
                        {
                            progress?.Report((double)totalBytes / contentLength.Value);
                        }
                        else
                        {
                            progress?.Report(null);
                        }
                    }
                }
                finally
                {
                    ArrayPool<byte>.Shared.Return(buffer);
                }
            }
        }
    }
}
