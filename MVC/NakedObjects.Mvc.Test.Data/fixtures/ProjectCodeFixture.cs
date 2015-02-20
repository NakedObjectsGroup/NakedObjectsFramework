// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using Expenses.ExpenseClaims;
using Expenses.ExpenseEmployees;
using NakedObjects;

namespace Expenses.Fixtures {
    public class ProjectCodeFixture {
        public static ProjectCode CODE1;
        public static ProjectCode CODE2;
        public static ProjectCode CODE3;
        public IDomainObjectContainer Container { protected get; set; }

        public void Install() {
            CODE1 = CreateProjectCode("001", "Marketing");
            CODE2 = CreateProjectCode("002", "Sales");
            CODE3 = CreateProjectCode("003", "Training");
            CreateProjectCode("004", "Consulting");
            CreateProjectCode("005", "Product Development");
            CreateProjectCode("006", "Recruitment");
            CreateProjectCode("007", "Overhead");
        }

        private ProjectCode CreateProjectCode(string code, string description) {
            var pCode = Container.NewTransientInstance<ProjectCode>();
            pCode.Code = code;
            pCode.Description = description;
            Container.Persist(ref pCode);
            return pCode;
        }

        #region Injected Services

        #region Injected: EmployeeRepository

        private EmployeeRepository m_employeeRepository;

        public EmployeeRepository EmployeeRepository {
            set { m_employeeRepository = value; }
        }

        #endregion

        //
        //		* This region contains references to the services (Repositories, 
        //		* Factories or other Services) used by this domain object.  The 
        //		* references are injected by the application container.
        //		

        #endregion
    }
}