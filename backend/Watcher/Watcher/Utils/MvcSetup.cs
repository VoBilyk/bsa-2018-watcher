﻿using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;

namespace Watcher.Utils
{
    public static class MvcSetup
    {
        public static Action<MvcNewtonsoftJsonOptions> JsonSetupAction = mvcJsonOptions =>
            {
                mvcJsonOptions.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver(); // For Typescript client
                // mvcJsonOptions.SerializerSettings.ContractResolver = new DefaultContractResolver();  // Standart resolver
                mvcJsonOptions.SerializerSettings.DateTimeZoneHandling = DateTimeZoneHandling.Utc;
                mvcJsonOptions.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
                mvcJsonOptions.SerializerSettings.FloatFormatHandling = FloatFormatHandling.DefaultValue;
                mvcJsonOptions.SerializerSettings.FloatParseHandling = FloatParseHandling.Decimal;
                mvcJsonOptions.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
            };
    }
}
