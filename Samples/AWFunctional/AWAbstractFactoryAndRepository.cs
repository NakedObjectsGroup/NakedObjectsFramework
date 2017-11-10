// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Linq;
using System.Reflection;
using System.Security.Principal;
using NakedObjects.Resources;
using NakedObjects;
using NakedFunctions;

namespace AdventureWorksModel
{
    /// <summary>
    ///     Convenience super class for factories and repositories that wish to interact with the container
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         Subclassing is NOT mandatory; the methods in this superclass can be pushed down into domain objects and
    ///         another superclass used if required
    ///     </para>
    /// </remarks>
    /// <see cref="IDomainObjectContainer" />
    public abstract class AWAbstractFactoryAndRepository
    {
        /// <summary>
        ///     Returns the first item from an IQueryable, but warns user
        ///     'No matching object found' if the IQueryable has a Count of 0.
        /// </summary>
        protected static QueryResultSingle SingleObjectWarnIfNoMatch<T>(IQueryable<T> query)
        {
            return (!query.Any()) ?
                new QueryResultSingle(default(T), null, ProgrammingModel.NoMatchSingular) :
                new QueryResultSingle(query.First());
        }

        /// <summary>
        ///     Returns a random instance from the set of all instance of type T
        /// </summary>
        protected static QueryResultSingle Random<T>(IFunctionalContainer container) where T : class
        {
            IQueryable<T> query = container.Instances<T>();
            int random = new Random().Next(query.Count());
            //The OrderBy(...) doesn't do anything, but is a necessary precursor to using .Skip
            //which in turn is needed because LINQ to Entities doesn't support .ElementAt(x)
            return new QueryResultSingle(query.OrderBy(n => "").Skip(random).FirstOrDefault());
        }


        #region Container Helper Methods

         /// <summary>
        ///     Get details about the current user
        /// </summary>
        protected static IPrincipal Principal
        {
            get { return Container.Principal; }
        }

        /// <summary>
        ///     Determines if the specified object is persistent; that is is stored permanently outside of the virtual machine.
        /// </summary>
        protected static bool IsPersistent(object obj)
        {
            return Container.IsPersistent(obj);
        }

        /// <summary>
        ///     Make the specified transient object persistent. Throws an exception if object is already persistent.
        /// </summary>
        protected static void Persist<T>(ref T transientObject)
        {
            Container.Persist(ref transientObject);
        }

        /// <summary>
        ///     Delete the object from the persistent object store
        /// </summary>
        protected static void DisposeInstance(object persistentObject)
        {
            Container.DisposeInstance(persistentObject);
        }


        /// <summary>
        ///     Warn the user about a situation with the specified message. The container should guarantee to display
        ///     this warning to the user.
        /// </summary>
        /// <seealso cref="InformUser" />
        /// <seealso cref="RaiseError" />
        protected static void WarnUser(string message)
        {
            Container.WarnUser(message);
        }

        #endregion

        protected static T FindByKey<T>(int key)
        {
            throw new NotImplementedException();
        }
    }
}