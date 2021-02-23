﻿// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using Microsoft.Extensions.Configuration;
using NakedObjects.Core.Configuration;
using NakedObjects.Persistor.Entity.Configuration;
using NakedObjects.Menu;
using NakedObjects.Architecture.Menu;
using NakedObjects.Meta.Audit;
using NakedObjects.Meta.Authorization;
using Template.DataBase;
using Template.SeedData;
using Template.Model;

namespace NakedObjects.Rest.App.Demo {
    public static class NakedObjectsRunSettings {

        private static string[] ModelNamespaces {
            get {
                return new string[] { "Template.Model" };
            }
        }

        private static Type[] Types {
            get {
                return new Type[] {
                };
            }
        }

        private static Type[] Services {
            get {
                return new [] {
                    typeof(ExampleService)
                };
            }
        }

        public static ReflectorConfiguration ReflectorConfig() {
            return new ReflectorConfiguration(Types, Services, ModelNamespaces, MainMenus);
        }

        public static EntityObjectStoreConfiguration EntityObjectStoreConfig(IConfiguration configuration) {
            var config = new EntityObjectStoreConfiguration();
            var cs = configuration.GetConnectionString("ExampleConnectionString");
            config.UsingContext(() => new ExampleDbContext(cs, new ExampleDbInitializer()));
            return config;
        }

        public static IAuditConfiguration AuditConfig() {
            return null;
        }

        public static IAuthorizationConfiguration AuthorizationConfig() {
            return null;
        }

        /// <summary>
        /// Return an array of IMenus (obtained via the factory, then configured) to
        /// specify the Main Menus for the application. If none are returned then
        /// the Main Menus will be derived automatically from the Services.
        /// </summary>
        public static IMenu[] MainMenus(IMenuFactory factory) {
            return new IMenu[] {
                factory.NewMenu<ExampleService>(true, "Menu")
            };
        }
    }
}