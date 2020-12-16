// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections.Generic;
using System.Linq;

namespace NakedObjects {
    /// <summary>
    /// This class serves only to document the signature of the methods (on domain objects) that are recognised by the framework.
    /// Note that this is not code to be used:  the methods in this class typically throw NotImplementedExceptions.
    /// </summary>
    public class RecognisedMethods {
        #region Default controls

        /// <summary>
        /// Dynamically hides all the properties on the object, except where overridden by a Hide method associated with a specific property.
        /// </summary>
        /// <returns>Return true to hide properties, false to show them.</returns>
        public bool HidePropertyDefault() {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Dynamically hides all the actions on the object, except where overridden by a Hide method associated with a specific action.
        /// </summary>
        /// <returns>Return true to hide actions, false to show them.</returns>
        public bool HideActionDefault() {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Dynamically renders all the properties on the object uneditable by the user, except where overridden by a Disable method associated with a specific property.
        /// </summary>
        /// <returns>If a String is returned the properties are disabled and the String is made visible to user to inform them why they are disabled. If the method returns a null value then properties remains editable.</returns>
        public string DisablePropertyDefault() {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Dynamically renders all the actions on the object unusable by the user, except where overridden by a Disable method associated with a specific action.
        /// </summary>
        /// <returns>If a String is returned the actions are disabled and the String is made visible to user to inform them why they are disabled. If the method returns a null value then actions remains usable.</returns>
        public string DisableActionDefault() {
            throw new NotImplementedException();
        }

        #endregion

        #region LifeCycle methods

        ///Life cycle method called whem object is first created. This is the instance's logical creation. This method will not be called when the object is retrieved from persistent storage into memory.
        public void Created() { }

        /// <summary>
        /// Life cycle method called when object has just been removed from the persistent store. At this point the object will exist in memory, but no longer exist in the persistent store.
        /// </summary>
        public void Deleted() { }

        /// <summary>
        /// Life cycle method called when object is just about to be removed from the persistent store. At this point the object still exists in the persistent store.
        /// </summary>
        public void Deleting() { }

        /// <summary>
        ///  Life cycle method called when object has just been loaded in from the persistent store. At this point the object has had its state fully restored. Loaded will be called after the object has been loaded and before the transaction has completed. When retrieving an object via the user interface this means that Loaded will have been called by the time the object appears on the screen. However, if you are processing objects programmatically - whether from within a user action or from an external call - then be aware that the Loaded might not be called on any (or all) of the objects being processed until the very end of the transaction. So if your method involves loading objects and processing them, you cannot assume that Loaded will have been called before you get hold of each object. In general, it is recommended that you use Loaded only for very simple, non-invasive purposes, such as calculating a total for display purposes before an object is returned to the user.
        /// </summary>
        public void Loaded() { }

        /// <summary>
        /// Life cycle method called when object is just about to be loaded from the persistent store. At this point the object exists in memory but has not had its state restored.
        /// </summary>
        public void Loading() { }

        /// <summary>
        /// Life cycle method called if the persistor throws an exception when object is persisted. Typically this will be a DataUpdateException or an OptimisticConcurrencyException. 
        /// </summary>
        private void OnPersistingError() { }

        /// <summary>
        ///Life cycle method called if the object persistor throws an exception when object is being updated. Works in a similar manner to OnPersistingError.
        /// </summary>
        public void OnUpdatingError() { }

        /// <summary>
        /// Life cycle method called after a transient object has been persisted. Unlike Persisting(), this method will be a separate transaction to the persisting of the object, but still within a single over-arching transacton
        /// </summary>
        public void Persisted() { }

        /// <summary>
        /// Life cycle method called when a transient object is just about to be persisted via the object store, as part of the same transaction. At this point the object exists only in memory and not in the persistent store.
        /// </summary>
        public void Persisting() { }

        /// <summary>
        /// Life cycle method called when a modified persistent object has just been saved to the persistent store. At this point the object in the persistent store will be in its new state.       
        /// </summary>
        public void Updated() { }

        /// <summary>
        /// Life cycle method called when a persistent object has just been modified and is about to be saved to the persistent store. At this point the object's data held in the persistent store will not yet have been modified.
        /// </summary>
        public void Updating() { }

        #endregion

        #region Complementary methods - properties

        /// <summary>
        /// To be recognised by Naked Objects a property should be public, virtual, read-write and its type should either be
        /// another domain object, or a recognised .NET value type such as string, int, DateTime or an enum.
        /// </summary>
        public virtual string AProperty { get; set; }

        /// <summary>
        /// A collection should follow the same rules as a property, but return a generic ICollection of a domain type.
        /// It should also be initialised with a private variable.
        /// </summary>
        public virtual ICollection<ADomainType> ACollection { get; set; }

        /// <summary>
        /// Provides a set of choices for the property, dynamically, based on typed input. 
        /// Number of choices offered may be limited by using [PageSize()] attribute on the method.
        /// </summary>
        /// <param name="matching">The input parameter may have a specified [MinLength] attribute before the method is called.</param>
        /// <returns>Must return an IQueryable of same type as the property"/></returns>
        public IQueryable<string> AutoCompleteAProperty(string matching) {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Specifies a set of explicit choices from which the user must select for a particular property.
        /// Create this method using the 'propcho' snippet.
        /// </summary>
        /// <returns>Must return an IEnumerable of the same type as the corresponding property.</returns>
        public IList<string> ChoicesAProperty() {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Called when the user (rather than programmatic code) clears a reference field, or blanks (so there is no entry) a value field. Changing a property from one value to another value, is deemed by the framework to be a 'clear field', immediately followed by a 'modify field'. 
        /// </summary>
        public void ClearAProperty() { }

        /// <summary>
        ///  Called when the user (rather than programmatic code) sets a reference or value field. This is typically used to trigger other behaviours such as updating a total.
        /// </summary>
        /// <param name="value"></param>
        public void ModifyAProperty(string value) {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Specifies the default value to be provided for a property, when the object is first created.
        /// Alternative to using the [DefaultValue] attribute.
        /// </summary>
        /// <returns>The type of the corresponding property.</returns>
        public string DefaultAProperty() {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Dynamically controls whether a field is editable, or an action can be initiated. 
        /// Alternative to using the (static) [Disabled] attribute.
        /// Use the 'dis' code snippet to create this method.
        /// </summary>
        /// <returns>If a String is returned the property is disabled and the String is made visible to user to inform them why it is disabled. If the method returns a null value then property remains editable.</returns>
        public string DisableAProperty() {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Dynamically hides the corresponding property.  Alternative to using the static [Hidden] attribute.
        /// Use the 'hide' code snippet to create this method.
        /// </summary>
        /// <returns>Return true to hide the property, false to show it.</returns>
        public bool HideAProperty() {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Validates the value being entered by the user into a property. Use the 'val' code snippet to create this method.
        /// </summary>
        /// <param name="value">Should be of the same type as the corresponding property; parameter name is not important in this case.</param>
        /// <returns>If the method returns null, the value is deemed to be valid; if a string is returned, the value is deemed invalid, and the
        /// string is returned to the user as a validation error message.
        /// </returns>
        public string ValidateAProperty(string value) {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Validates more than one property at at a time - for example to check that the 'To Date' falls after the 'From Date'.
        /// </summary>
        /// <param name="aProperty">Both the types and the names of the parameters, must match those of specific properties, but with the name starting lower-case.</param>
        /// <param name="anotherProperty"></param>
        /// <returns></returns>
        public string Validate(string aProperty, int anotherProperty) {
            throw new NotImplementedException();
        }

        #endregion

        #region Complementary methods - actions

        /// <summary>
        /// To be recognised by Naked Objects an action should be public and its type should either be
        /// a domain object, a recognised .NET value type such as string, int, DateTime or an enum, or an IEnumerable of a domain type.
        /// </summary>
        /// <param name="param0">Parameters should be other domain types, recognised .NET value types, or, for multi-selection an IEnumerable of any recognised type.</param>
        /// <param name="param1"></param>
        /// <param name="param2"></param>
        /// <returns></returns>
        public string AnAction(string param0, int param1, ADomainType param2) {
            throw new NotImplementedException();
        }

        /// <summary>
        /// An action may allow one of more of its parameters to accept multiple values of either values types or domain types.
        /// Unless the value type is an enum, or the domain type is [Bounded], the choices for each multi-select parameter
        /// should be specified via a Choices method.
        /// </summary>
        /// <param name="values">The parameter type should be an IEnumerable of a domain type or recognised value type.</param>
        /// <returns></returns>
        public string AnActionWithMultiSelect(IEnumerable<string> strings, IEnumerable<ADomainType> objects) {
            throw new NotImplementedException();
        }

        /// <summary>
        /// An action returning an IQueryable of a domain type will return a paged display.  The default page
        /// size of 50 results may be overridden with the [PageSize] attribute.
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public IQueryable<ADomainType> AQueryAction(string param) {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Provides a set of choices for the specified parameter on the corresponding action -  in this case parameter 0.
        /// Number of choices offered may be limited by using [PageSize()] attribute on the method.
        /// Create this method using the 'auto' code snippet.
        /// </summary>
        /// <param name="matching">The input parameter may have a specified [MinLength] attribute before the method is called.</param>
        /// <returns>Must return an IQueryable of same type as the specified parameter for the specified action"/></returns>
        public IQueryable<string> AutoComplete0AnAction(string matching) {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Specifies a set of explicit choices from which the user must select for a particular parameter on the corresponding action -
        /// in this case parameter 1
        /// Create this method using the 'actcho' snippet.
        /// </summary>
        /// <returns>Must return an IEnumerable of the same type as the action parameter specified..</returns>
        public IList<int> Choices1AnAction() {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Specifies the default value to be provided for a specified parameter on an action - in this case parameter 1.
        /// Alternative to using the [DefaultValue] attribute.
        /// </summary>
        /// <returns>The type of the corresponding action parameter.</returns>
        public int Default1AnAction() {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Dynamically controls whether an action is usable or not.
        /// Alternative to using the (static) [Disabled] attribute.
        /// Use the 'dis' code snippet to create this method.
        /// </summary>
        /// <returns>If a String is returned the action is disabled and the String is made visible to user to inform them why it is disabled. 
        /// If the method returns a null value then action remains usable.</returns>
        public string DisableAnAction() {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Dynamically hides the corresponding action.  Alternative to using the static [Hidden] attribute.
        /// Use the 'hide' code snippet to create this method.
        /// </summary>
        /// <returns>Return true to hide the action, false to show it.</returns>
        public bool HideAnAction() {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Validates all the values for one or more of the parameters being entered by the user for a given action. 
        /// Use the 'val' code snippet to create this method.
        /// </summary>
        /// <param name="value">Should be of the same type as the corresponding action parameter. The parameter names must also match.</param>
        /// <returns>If the method returns null, the value is deemed to be valid; if a string is returned, the value(s) is/are deemed invalid, and the
        /// string is returned to the user as a validation error message.
        /// </returns>
        public string ValidateAnAction(string param0, int param1, ADomainType value) {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Validates the value being entered by the user into a single specified parameter on an action -  in this case parameter 2.
        /// Use the 'val' code snippet to create this method.
        /// </summary>
        /// <param name="value">Should be of the same type as the corresponding action parameter.</param>
        /// <returns>If the method returns null, the value is deemed to be valid; if a string is returned, the value is deemed invalid, and the
        /// string is returned to the user as a validation error message.
        /// </returns>
        public string Validate2AnAction(ADomainType value) {
            throw new NotImplementedException();
        }

        #endregion

        #region Other recognised methods

        /// <summary>
        /// Provides an alternative to the IconName attribute for specifying the icon to be used by an object.
        /// May provide instance-specific icon.
        /// Use the 'icon' code snippet to create this method.
        /// </summary>
        /// <returns></returns>
        public string IconName() {
            throw new NotImplementedException();
        }

        /// <summary>
        /// If no Title attribute, or Title() method has been specified, the framework will call the object's ToString method to get a title for the object.
        /// </summary>
        /// <returns></returns>
        public override string ToString() {
            throw new NotImplementedException();
        }

        /// <summary>
        /// May be used to construct a title from several properties (value and or reference properties).
        /// Use the 'title' code snippet to create this method.
        /// </summary>
        /// <returns></returns>
        public string Title() {
            throw new NotImplementedException();
        }

        #endregion

        #region ContributedActions

        /// <summary>
        /// An action defined on a service (registered as such in the Run class) and that takes one or more
        /// parameters of domain types will be 'contributed' to domain objects of those type(s).
        /// </summary>
        /// <param name="domainObject"></param>
        /// <param name="otherParam"></param>
        public void AContibutedAction(ADomainType domainObject, string otherParam) {
            throw new NotImplementedException();
        }

        /// <summary>
        /// An action defined on a service (registered as such in the Run class) and that takes one or more
        /// parameters that are IQueryables of domain types will be 'contributed' to an IQueryable of
        /// domain objects of the same type (i.e. the results returned by another query action).  A collection-contributed
        /// action should NOT itself return an IQueryable.
        /// </summary>
        public void ACollectionContibutedAction(IQueryable<ADomainType> domainObject, string otherParam) {
            throw new NotImplementedException();
        }

        #endregion
    }

    /// <summary>
    /// This class serves only as illustration for the RecognisedMethods -  it contains no executable code.
    /// A domain type need not extend any type, implement any interface, nor adopt any attribute.  
    /// </summary>
    public class ADomainType { }
}