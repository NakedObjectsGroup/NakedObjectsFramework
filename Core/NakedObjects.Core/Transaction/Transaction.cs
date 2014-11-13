// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System;
using System.Collections.Generic;
using Common.Logging;
using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Component;
using NakedObjects.Architecture.Persist;
using NakedObjects.Architecture.Transaction;
using NakedObjects.Core.Adapter;
using NakedObjects.Core.Persist;
using NakedObjects.Core.Util;

namespace NakedObjects.Core.Transaction {
    public class Transaction : ITransaction {
        private static readonly ILog Log;
        private readonly List<IPersistenceCommand> commands = new List<IPersistenceCommand>(10);
        private readonly IObjectStore objectStore;
        private readonly List<INakedObject> toNotify = new List<INakedObject>(10);
        private bool complete;

        static Transaction() {
            Log = LogManager.GetLogger(typeof (Transaction));
        }

        public Transaction(IObjectStore objectStore) {
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
            LockedAdd(commands, command);
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
            List<IPersistenceCommand> commandsArray = LockedClone(commands);
            if (commandsArray.Count > 0) {
                RemoveAllCommands();
                return objectStore.Flush(commandsArray.ToArray());
            }
            return false;
        }

        private void RecursivelyCommitCommands() {
            List<IPersistenceCommand> commandsArray = LockedClone(commands);
            commands.Clear();
            if (commandsArray.Count > 0) {
                objectStore.Execute(commandsArray.ToArray());

                foreach (IPersistenceCommand command in commandsArray) {
                    if (command is IDestroyObjectCommand) {
                        INakedObject onObject = command.OnObject();
                        onObject.OptimisticLock = new NullVersion();
                    }
                }

                RecursivelyCommitCommands();
            }
        }

        #endregion

        private static void LockedAdd<T>(List<T> collection, T toAdd) {
            lock (collection) {
                collection.Add(toAdd);
            }
        }

        private static List<T> LockedClone<T>(List<T> fromCollection) {
            var toCollection = new List<T>();
            lock (fromCollection) {
                toCollection.AddRange(fromCollection);
            }
            return toCollection;
        }


        internal virtual void AddNotify(INakedObject nakedObject) {
            Log.DebugFormat("Add notification for {0}", nakedObject);
            LockedAdd(toNotify, nakedObject);
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
            foreach (IPersistenceCommand command in LockedClone(commands)) {
                if (command.OnObject().Equals(onObject)) {
                    if (commandClass.IsAssignableFrom(command.GetType())) {
                        return command;
                    }
                }
            }
            return null;
        }

        private void RemoveAllCommands() {
            lock (commands) {
                commands.Clear();
            }
        }

        private void RemoveCommand(Type commandClass, INakedObject onObject) {
            lock (commands) {
                IPersistenceCommand toDelete = GetCommand(commandClass, onObject);
                commands.Remove(toDelete);
            }
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