// <copyright file="NHibernateContext.cs" company="SoluiNet">
// Copyright (c) SoluiNet. All rights reserved.
// </copyright>

namespace SoluiNet.DevTools.Management.Finances.Data
{
    using System;
    using System.Web;
    using NHibernate;
    using NHibernate.Cfg;
    using SoluiNet.DevTools.Core.Application;

    /// <summary>
    /// Provides a helper class for the NHibernate context.
    /// </summary>
    public sealed class NHibernateContext
    {
        private const string CurrentSessionKey = "nhibernate.current_session";
        private static readonly ISessionFactory _sessionFactory;

        static NHibernateContext()
        {
            _sessionFactory = new Configuration().Configure().BuildSessionFactory();
        }

        /// <summary>
        /// Get the current session.
        /// </summary>
        /// <returns>Returns the currently used session.</returns>
        public static ISession GetCurrentSession()
        {
            // var context = ApplicationContext.Application;
            var currentSession = ApplicationContext.Storage[CurrentSessionKey] as ISession;

            if (currentSession == null)
            {
                currentSession = _sessionFactory.OpenSession();
                ApplicationContext.Storage[CurrentSessionKey] = currentSession;
            }

            return currentSession;
        }

        /// <summary>
        /// Close the session.
        /// </summary>
        public static void CloseSession()
        {
            // var context = ApplicationContext.Application;
            var currentSession = ApplicationContext.Storage[CurrentSessionKey] as ISession;

            if (currentSession == null)
            {
                // No current session
                return;
            }

            currentSession.Close();
            ApplicationContext.Storage.Remove(CurrentSessionKey);
        }

        /// <summary>
        /// Close the session factory.
        /// </summary>
        public static void CloseSessionFactory()
        {
            if (_sessionFactory != null)
            {
                _sessionFactory.Close();
            }
        }
    }
}
