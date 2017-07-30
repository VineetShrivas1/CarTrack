using CarTrack.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;
using System.Web;
using System.Web.Http;

namespace CarTrackAPI.Utilities
{
    public class TokenUtility
    {
        static MemoryCache requestLog = MemoryCache.Default;
        static CacheItemPolicy keepSuspiciousPolicy = new CacheItemPolicy();
        static TokenUtility()
        {
            keepSuspiciousPolicy.SlidingExpiration = TimeSpan.Parse("365:00:00");
        }
        public static string GenerateToken(string key)
        {
            string outMsg = String.Empty;
            var authToken = CryptoUtility.Encrypt(Constants.Key, Guid.NewGuid().ToString(), out outMsg);

            if (requestLog.Contains(key))
            {
                CacheItem record = requestLog.GetCacheItem(key);
                record.Value = authToken;
                requestLog.Set(record, keepSuspiciousPolicy);
            }
            else
            {
                requestLog.Add(new CacheItem(key, authToken), keepSuspiciousPolicy);
                AccountUtility.UpdateToken(key, authToken);
            }
            return authToken;
        }
        public static bool ValidateToken(string key, string authToken)
        {
            if (requestLog.Contains(key))
            {
                CacheItem record = requestLog.GetCacheItem(key);
                if (record.Value.ToString() == authToken)
                    return true;
            }
            return true;//for now only
        }
        public static string IsValidToken(string key, string errorCode)
        {
            var headers = HttpContext.Current.Request.Headers;
            if (headers != null && headers["Authorization"] != null)
            {
                string token = headers.GetValues("Authorization").First();
                if (!TokenUtility.ValidateToken(key, token))
                    errorCode = "Err0019";
            }
            else
                errorCode = "Err0019";
            return errorCode;
        }
    }
}