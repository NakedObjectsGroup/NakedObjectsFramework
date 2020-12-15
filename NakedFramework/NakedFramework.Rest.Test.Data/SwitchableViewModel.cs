﻿// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using NakedObjects;

// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedMember.Local

namespace RestfulObjects.Test.Data {
    public class SwitchableViewModel : IViewModelSwitchable {
        public virtual IDomainObjectContainer Container { set; protected get; }

        [Key]
        [ConcurrencyCheck]
        [Hidden(WhenTo.Always)]
        public virtual int Id { get; set; }

        [Title]
        public virtual string Name { get; set; }

        [Optionally]
        [Title]
        public virtual MostSimple MostSimple { get; set; }

        [Hidden(WhenTo.Always)]
        public virtual bool IsEdit { get; set; }

        #region IViewModelSwitchable Members

        [NakedObjectsIgnore]
        public string[] DeriveKeys() {
            var keys = new List<string> {
                Id.ToString(),
                MostSimple.Id.ToString(),
                IsEdit.ToString(),
                Name
            };

            return keys.ToArray();
        }

        [NakedObjectsIgnore]
        public void PopulateUsingKeys(string[] keys) {
            Id = int.Parse(keys[0]);
            var msId = int.Parse(keys[1]);
            MostSimple = Container.Instances<MostSimple>().FirstOrDefault(ms => ms.Id == msId);
            IsEdit = bool.Parse(keys[2]);
            Name = keys[3];
        }

        public bool IsEditView() => IsEdit;

        #endregion

        public SwitchableViewModel Step() {
            var vm = Container.NewViewModel<SwitchableViewModel>();
            vm.Id = 2;
            vm.MostSimple = MostSimple;
            return vm;
        }

        public SwitchableViewModel ToggleView() {
            IsEdit = !IsEdit;
            return this;
        }
    }
}