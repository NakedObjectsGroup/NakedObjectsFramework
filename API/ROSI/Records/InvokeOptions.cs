using System.Net.Http.Headers;
using ROSI.Interfaces;

namespace ROSI.Records;

public record InvokeOptions : IInvokeOptions {
    public string? Token { get; init; }
    public EntityTagHeaderValue? Tag { get; init; }
    public virtual HttpClient HttpClient { get; set; } = new();

    public IDictionary<string, object> ReservedArguments { get; init; } = new Dictionary<string, object>();
    public IInvokeOptions Copy() => this with { ReservedArguments = new Dictionary<string, object>() };
}