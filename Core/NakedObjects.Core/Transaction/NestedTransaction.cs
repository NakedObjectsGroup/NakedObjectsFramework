// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections.Generic;
using System.Linq;
using Common.Logging;
using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Component;
using NakedObjects.Architecture.Persist;
using NakedObjects.Architecture.Transaction;
using NakedObjects.Core.Adapter;
using NakedObjects.Core.Util;

namespace NakedObjects.Core.Transaction {
    public class NestedTransaction : ITransaction {
        private static readonly ILog Log;
        private readonly List<IPersistenceCommand> commands = new List<IPersistenceCommand>(10);
        private readonly IObjectStore objectStore;
        private readonly List<INakedObject> toNotify = new List<INakedObject>(10);
        private bool complete;

        static NestedTransaction() {
            Log = LogManager.GetLogger(typeof (NestedTransaction));
        }

        public NestedTransaction(IObjectStore objectStore) {
            this.objectStore = objectStore;
            Log.DebugFormat("New transaction {0}", this);
        }

        #region ITransaction Members

        public virtual void Abort() {
            Log.InfoFormat("abort transaction {0}", this);
            if (complete) {
                throw new TransactionException("Transaction already complete; cannot abort");
            }
            complete = true;
        }

        /// <summary>
        ///     Add the non-null command to the list of commands to execute at the end of the transaction
        /// </summary>
        public virtual void AddCommand(IPersistenceCommand command) {
            if (command == null) {
                return;
            }

            INakedObject onObject = command.OnObject();

            // Saves are ignored when preceded by another save, or a delete
            if (command is ISaveObjectCommand) {
                if (AlreadyHasCreate(onObject) || AlreadyHasSave(onObject)) {
                    Log.DebugFormat("Ignored command as object already created/saved {0}", command);
                    return;
                }

                if (AlreadyHasDestroy(onObject)) {
                    Log.Info("ignored command " + command + " as object no longer exists");
                    return;
                }
            }

            // Destroys are ignored when preceded by a create
            if (command is IDestroyObjectCommand) {
                if (AlreadyHasCreate(onObject)) {
                    RemoveCreate(onObject);
                    Log.Info("ignored both create and destroy command " + command);
                    return;
                }

                if (AlreadyHasSave(onObject)) {
                    RemoveSave(onObject);
                    Log.Info("removed prior save command " + command);
                }
            }
            Log.DebugFormat("Add command {0}", command);
            commands.Add(command);
        }

        public virtual void Commit() {
            Log.Info("commit transaction " + this);
            if (complete) {
                throw new TransactionException("Transaction already complete; cannot commit");
            }
            objectStore.EndTransaction();

            RecursivelyCommitCommands();

            complete = true;
        }

        public virtual bool Flush() {
            Log.Info("flush transaction " + this);
           
            if (commands.Count > 0) {
                RemoveAllCommands();
                return objectStore.Flush(commands.ToArray());
            }
            return false;
        }

        #endregion

        private void RecursivelyCommitCommands() {
            var commandsArray = commands.ToArray();
            commands.Clear();
            if (commandsArray.Any()) {
                objectStore.Execute(commandsArray);

                foreach (IPersistenceCommand command in commandsArray) {
                    if (command is IDestroyObjectCommand) {
                        INakedObject onObject = command.OnObject();
                        onObject.OptimisticLock = new NullVersion();
                    }
                }

                RecursivelyCommitCommands();
            }
        }


        internal virtual void AddNotify(INakedObject nakedObject) {
            Log.DebugFormat("Add notification for {0}", nakedObject);
            toNotify.Add(nakedObject);
        }

        private bool AlreadyHasCommand(Type commandClass, INakedObject onObject) {
            return GetCommand(commandClass, onObject) != null;
        }

        private bool AlreadyHasCreate(INakedObject onObject) {
            return AlreadyHasCommand(typeof (ICreateObjectCommand), onObject);
        }

        private bool AlreadyHasDestroy(INakedObject onObject) {
            return AlreadyHasCommand(typeof (IDestroyObjectCommand), onObject);
        }

        private bool AlreadyHasSave(INakedObject onObject) {
            return AlreadyHasCommand(typeof (ISaveObjectCommand), onObject);
        }

        private IPersistenceCommand GetCommand(Type commandClass, INakedObject onObject) {
            return commands.Where(command => command.OnObject().Equals(onObject)).
                FirstOrDefault(commandClass.IsInstanceOfType);
        }

        private void RemoveAllCommands() {
            commands.Clear();
        }

        private void RemoveCommand(Type commandClass, INakedObject onObject) {
            IPersistenceCommand toDelete = GetCommand(commandClass, onObject);
            commands.Remove(toDelete);
        }

        private void RemoveCreate(INakedObject onObject) {
            RemoveCommand(typeof (ICreateObjectCommand), onObject);
        }

        private void RemoveSave(INakedObject onObject) {
            RemoveCommand(typeof (ISaveObjectCommand), onObject);
        }

        public override string ToString() {
            var str = new AsString(this);
            str.Append("complete", complete);
            str.Append("commands", commands.Count);
            return str.ToString();
        }
    }

    // Copyright (c) Naked Objects Group Ltd.
}