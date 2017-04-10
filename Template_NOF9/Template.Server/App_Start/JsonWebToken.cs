namespace NakedObjects.Template.App_Start
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Security.Claims;
    using Newtonsoft.Json.Linq;
    using System.Text;

    public static class JsonWebToken
    {
        private const string NameClaimType = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name";
        private const string RoleClaimType = "http://schemas.microsoft.com/ws/2008/06/identity/claims/role";
        private const string ActorClaimType = "http://schemas.xmlsoap.org/ws/2009/09/identity/claims/actor";
        private const string DefaultIssuer = "LOCAL AUTHORITY";
        private const string StringClaimValueType = "http://www.w3.org/2001/XMLSchema#string";

        // sort claim types by relevance
        private static IEnumerable<string> claimTypesForUserName = new[] { "name", "email", "user_id", "sub" };
        private static ISet<string> claimsToExclude = new HashSet<string>(new[] { "iss", "sub", "aud", "exp", "iat", "identities" });

        private static DateTime unixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        public static ClaimsPrincipal ValidateToken(string token, string secretKey, string audience = null, bool checkExpiration = false, string issuer = null, bool isSecretBase64Encoded = true)
        {
            byte[] secret;
            if (isSecretBase64Encoded)
                secret = Convert.FromBase64String(secretKey);
            else
                secret = Encoding.UTF8.GetBytes(secretKey);

            var payloadJson = JWT.JsonWebToken.Decode(token, secret, verify: true);
            var payloadData = JObject.Parse(payloadJson).ToObject<Dictionary<string, object>>();

            // audience check
            object aud;
            if (!string.IsNullOrEmpty(audience) && payloadData.TryGetValue("aud", out aud))
            {
                if (!aud.ToString().Equals(audience, StringComparison.Ordinal))
                {
                    throw new TokenValidationException(string.Format("Audience mismatch. Expected: '{0}' and got: '{1}'", audience, aud));
                }
            }

            // expiration check
            object exp;
            if (checkExpiration && payloadData.TryGetValue("exp", out exp))
            {
                DateTime validTo = FromUnixTime(long.Parse(exp.ToString()));
                if (DateTime.Compare(validTo, DateTime.UtcNow) <= 0)
                {
                    throw new TokenValidationException(
                        string.Format("Token is expired. Expiration: '{0}'. Current: '{1}'", validTo, DateTime.UtcNow));
                }
            }

            // issuer check
            object iss;
            if (payloadData.TryGetValue("iss", out iss))
            {
                if (!string.IsNullOrEmpty(issuer))
                {
                    if (!iss.ToString().Equals(issuer, StringComparison.Ordinal))
                    {
                        throw new TokenValidationException(string.Format("Token issuer mismatch. Expected: '{0}' and got: '{1}'", issuer, iss));
                    }
                }
                else
                {
                    // if issuer is not specified, set issuer with jwt[iss]
                    issuer = iss.ToString();
                }
            }

            return new ClaimsPrincipal(ClaimsIdentityFromJwt(payloadData, issuer));
        }

        private static ICollection<Claim> ClaimsFromJwt(IDictionary<string, object> jwtData, string issuer)
        {
            issuer = issuer ?? DefaultIssuer;

            var list = jwtData.Where(p => !claimsToExclude.Contains(p.Key)) // don't include specific claims
                              .Select(p => new { Key = p.Key, Values = p.Value as JArray ?? new JArray { p.Value } }) // p.Value is either claim value of ArrayList of values
                              .SelectMany(p => p.Values.Cast<object>()
                                                .Select(v => new Claim(p.Key, v.ToString(), StringClaimValueType, issuer, issuer)))
                              .ToList();

            // set claim for user name
            // use original jwtData because claimsToExclude filter has sub and otherwise it wouldn't be used
            var userNameClaimType = claimTypesForUserName.FirstOrDefault(ct => jwtData.ContainsKey(ct));
            if (userNameClaimType != null)
            {
                list.Add(new Claim(NameClaimType, jwtData[userNameClaimType].ToString(), StringClaimValueType, issuer, issuer));
            }

            // set claims for roles array
            list.Where(c => c.Type == "roles").ToList().ForEach(r =>
            {
                list.Add(new Claim(RoleClaimType, r.Value, StringClaimValueType, issuer, issuer));
            });

            list.RemoveAll(c => c.Type == "roles");

            return list;
        }

        private static ClaimsIdentity ClaimsIdentityFromJwt(IDictionary<string, object> jwtData, string issuer)
        {
            var subject = new ClaimsIdentity("Federation", NameClaimType, RoleClaimType);
            var claims = ClaimsFromJwt(jwtData, issuer);

            foreach (Claim claim in claims)
            {
                var type = claim.Type;
                if (type == ActorClaimType)
                {
                    if (subject.Actor != null)
                    {
                        throw new InvalidOperationException(string.Format(
                            "Jwt10401: Only a single 'Actor' is supported. Found second claim of type: '{0}', value: '{1}'", new object[] { "actor", claim.Value }));
                    }

                    subject.AddClaim(new Claim(type, claim.Value, claim.ValueType, issuer, issuer, subject));

                    continue;
                }

                subject.AddClaim(new Claim(type, claim.Value, claim.ValueType, issuer, issuer, subject));
            }

            return subject;
        }

        private static DateTime FromUnixTime(long unixTime)
        {
            return unixEpoch.AddSeconds(unixTime);
        }

        public class TokenValidationException : Exception
        {
            public TokenValidationException(string message)
                : base(message)
            {
            }
        }
    }
}
