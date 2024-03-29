﻿using Newtonsoft.Json.Linq;
using ROSI.Interfaces;

namespace ROSI.Records;

public record CollectionDetails(JObject Wrapped, InvokeOptions Options) : IHasValue, ICollection;