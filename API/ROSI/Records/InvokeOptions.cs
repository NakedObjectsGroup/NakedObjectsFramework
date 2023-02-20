using System.Net.Http.Headers;

namespace ROSI.Records;

public record InvokeOptions {
    public string? Token { get; init; }
    public EntityTagHeaderValue? Tag { get; init; }
    public virtual HttpClient HttpClient { get; set; } = new();

    public IDictionary<string, object>? ReservedArguments { get; set; }
}