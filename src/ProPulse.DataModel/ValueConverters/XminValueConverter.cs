using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace ProPulse.DataModel.ValueConverters;

/// <summary>
/// Converts between a PostgreSQL <c>uint</c> (xid/xmin) and a <c>byte[]</c> for concurrency tokens.
/// </summary>
public sealed class XminValueConverter : ValueConverter<byte[], uint>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="XminValueConverter"/> class.
    /// </summary>
    public XminValueConverter()
        : base(
            v => v != null && v.Length == 4 ? BitConverter.ToUInt32(v) : 0u,
            v => BitConverter.GetBytes(v))
    {
    }
}
