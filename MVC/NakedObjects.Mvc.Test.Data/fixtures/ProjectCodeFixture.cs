// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 
using Expenses.ExpenseClaims;
using Expenses.ExpenseEmployees;
using NakedObjects;

namespace Expenses.Fixtures {
    public class ProjectCodeFixture  {

        public IDomainObjectContainer Container { protected get; set; }

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

        public static ProjectCode CODE1;
        public static ProjectCode CODE2;
        public static ProjectCode CODE3;

        public  void Install() {
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
    }
}