﻿// <copyright file="NHibernateContext.cs" company="SoluiNet">
// Copyright (c) SoluiNet. All rights reserved.
// </copyright>

namespace SoluiNet.DevTools.Management.Finances.Data
{
    using System;
    using System.IO;
    using FluentNHibernate.Cfg;
    using FluentNHibernate.Cfg.Db;
    using NHibernate;
    using NHibernate.Cfg;
    using NHibernate.Tool.hbm2ddl;
    using NLog;
    using SoluiNet.DevTools.Core.Application;

    /// <summary>
    /// Provides a helper class for the NHibernate context.
    /// </summary>
    public sealed class NHibernateContext
    {
        private const string CurrentSessionKey = "nhibernate.current_session";
        private static readonly ISessionFactory SessionFactory;

        /// <summary>
        /// Initializes static members of the <see cref="NHibernateContext"/> class.
        /// </summary>
        static NHibernateContext()
        {
            SessionFactory = Fluently.Configure()
                .Database(SQLiteConfiguration.Standard.UsingFile(DatabaseLocation))
                .Mappings(m => m.HbmMappings.AddFromAssemblyOf<NHibernateContext>())
                .ExposeConfiguration(GenerateDatabase)
                .BuildSessionFactory();
        }

        /// <summary>
        /// Gets the default database location.
        /// </summary>
        private static string DatabaseLocation
        {
            get
            {
                return string.Format("{0}\\SoluiNet.DevTools\\Finance.sqlite", System.Environment.GetFolderPath(System.Environment.SpecialFolder.LocalApplicationData));
            }
        }

        /// <summary>
        /// Generate the database if it doesn't exist.
        /// </summary>
        /// <param name="config">The configuration.</param>
        public static void GenerateDatabase(Configuration config)
        {
            config.SetProperty(NHibernate.Cfg.Environment.ShowSql, "true");

            if (!File.Exists(DatabaseLocation))
            {
                new SchemaExport(config).Create(false, true);
            }

            try
            {
                var validator = new SchemaValidator(config);

                validator.Validate();
            }
            catch (Exception exception)
            {
                var logger = LogManager.GetCurrentClassLogger();

                logger.Warn(exception, string.Format("Schema validation failed. Trying to update. Additional Info: {0}", exception.Message));

                if (exception is SchemaValidationException validationException)
                {
                    foreach (var validationError in validationException.ValidationErrors)
                    {
                        logger.Warn(validationError);
                    }
                }

                try
                {
                    var updater = new SchemaUpdate(config);

                    updater.Execute(false, true);

                    logger.Info("Schema update successful");
                }
                catch (Exception updateException)
                {
                    logger.Error(updateException, string.Format("Schema update failed. Additional Info: {0}", exception.Message));
                }
            }
        }

        /// <summary>
        /// Get the current session.
        /// </summary>
        /// <returns>Returns the currently used session or creates a new one if it doesn't exist until now.</returns>
        public static ISession GetCurrentSession()
        {
            var currentSession = ApplicationContext.SessionValues.ContainsKey(CurrentSessionKey)
                ? ApplicationContext.SessionValues[CurrentSessionKey] as ISession
                : null;

            if (currentSession == null)
            {
                currentSession = SessionFactory.OpenSession();
                ApplicationContext.SessionValues[CurrentSessionKey] = currentSession;
            }
            else if (!currentSession.IsOpen)
            {
                currentSession = SessionFactory.OpenSession();
                ApplicationContext.SessionValues[CurrentSessionKey] = currentSession;
            }

            return currentSession;
        }

        /// <summary>
        /// Close the session.
        /// </summary>
        public static void CloseSession()
        {
            var currentSession = ApplicationContext.SessionValues[CurrentSessionKey] as ISession;

            if (currentSession == null)
            {
                // No current session
                return;
            }

            currentSession.Close();
            ApplicationContext.SessionValues.Remove(CurrentSessionKey);
        }

        /// <summary>
        /// Close the session factory.
        /// </summary>
        public static void CloseSessionFactory()
        {
            if (SessionFactory != null)
            {
                SessionFactory.Close();
            }
        }
    }
}
