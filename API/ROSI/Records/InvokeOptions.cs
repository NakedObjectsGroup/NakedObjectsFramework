using System.Net.Http.Headers;

namespace ROSI.Records;

public record InvokeOptions {
    public string? Token { get; init; }
    public EntityTagHeaderValue? Tag { get; init; }
}