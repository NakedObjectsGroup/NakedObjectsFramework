using System.Collections.Immutable;
using System.Net.Http.Headers;
using System.Reflection;

namespace ROSI.Interfaces;

public record InvokeOptions {
    public string? Token { get; init;  }
    public EntityTagHeaderValue? Tag { get; init; }
    public virtual HttpClient HttpClient { get; init; } = new();
    public IImmutableDictionary<string, object> ReservedArguments { get; init; } = new Dictionary<string, object>().ToImmutableDictionary();
}