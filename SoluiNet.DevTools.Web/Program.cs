// <copyright file="Program.cs" company="SoluiNet">
// Copyright (c) SoluiNet. All rights reserved.
// </copyright>

namespace SoluiNet.DevTools.Web
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.ServiceProcess;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// The service program.
    /// </summary>
    public static class Program
    {
        /// <summary>
        /// The main entrance of the application.
        /// </summary>
        public static void Main()
        {
            ServiceBase[] servicesToRun;

            servicesToRun = new ServiceBase[]
            {
                new SoluiNetService(),
            };

            ServiceBase.Run(servicesToRun);
        }
    }
}
