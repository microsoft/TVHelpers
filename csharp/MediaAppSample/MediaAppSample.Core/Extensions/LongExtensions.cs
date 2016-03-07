public static class LongExtensions
{
    /// <summary>
    /// Converts a long to memory formatted string in bytes/KB/MB/GB.
    /// </summary>
    /// <param name="sizeInBytes"></param>
    /// <returns></returns>
    public static string ToStringAsMemory(this long sizeInBytes)
    {
        if (sizeInBytes > 1024 * 1024 * 1024)
            return $"{((sizeInBytes / 1024.0) / 1024.0 / 1024):N2} GB";
        else if (sizeInBytes > 1024 * 1024)
            return $"{((sizeInBytes / 1024.0) / 1024.0):N2} MB";
        else if (sizeInBytes > 1024)
            return $"{(sizeInBytes / 1024.0):N2} KB";
        return $"{sizeInBytes} bytes";
    }
}