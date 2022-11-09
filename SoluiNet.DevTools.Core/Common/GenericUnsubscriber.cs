// <copyright file="GenericUnsubscriber.cs" company="SoluiNet">
// Copyright (c) SoluiNet. All rights reserved.
// </copyright>

namespace SoluiNet.DevTools.Core.Common
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Text;
    using NLog;

    /// <summary>
    /// Provides a class to support unsubscriptions.
    /// </summary>
    /// <typeparam name="TObservers">The collection type of observers which holds the observable.</typeparam>
    /// <typeparam name="TObject">The object type which will be observed.</typeparam>
    public class GenericUnsubscriber<TObservers, TObject>
        : IDisposable
        where TObservers : ICollection<IObserver<TObject>>
        where TObject : class
    {
        private ICollection<IObserver<TObject>> observers;
        private IObserver<TObject> observer;

        /// <summary>
        /// Initializes a new instance of the <see cref="GenericUnsubscriber{TObservers, TObject}"/> class.
        /// </summary>
        /// <param name="observers">The observer collection.</param>
        /// <param name="observer">The observer which should be unsubscribed.</param>
        public GenericUnsubscriber(ICollection<IObserver<TObject>> observers, IObserver<TObject> observer)
        {
            this.observers = observers;
            this.observer = observer;
        }

        /// <summary>
        /// Gets the logger.
        /// </summary>
        private static Logger Logger
        {
            get { return LogManager.GetCurrentClassLogger(); }
        }

        /// <summary>
        /// Dispose the unsubscriber.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);  // Violates rule
        }

        /// <summary>
        /// Dispose the observer and remove it from the collection.
        /// </summary>
        /// <param name="disposing">A value indicating whether the object is being disposed.</param>
        protected virtual void Dispose(bool disposing = true)
        {
            if (disposing)
            {
                Logger.Trace(string.Format(
                    CultureInfo.InvariantCulture,
                    "Dispose observer of type {0} from {1}",
                    this.observer.GetType().FullName,
                    this.observers.GetType().FullName));

                if (this.observers.Contains(this.observer))
                {
                    this.observers.Remove(this.observer);
                }
            }
        }
    }
}
