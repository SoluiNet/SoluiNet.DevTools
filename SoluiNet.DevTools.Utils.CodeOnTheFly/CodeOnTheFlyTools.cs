﻿// <copyright file="CodeOnTheFlyTools.cs" company="SoluiNet">
// Copyright (c) SoluiNet. All rights reserved.
// </copyright>

namespace SoluiNet.DevTools.Utils.CodeOnTheFly
{
    using System;
    using System.CodeDom.Compiler;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Reflection;
    using System.Text;
    using System.Threading.Tasks;
    using Microsoft.CSharp;
    using Newtonsoft.Json;
    using SoluiNet.DevTools.Core.Tools.String;

    /// <summary>
    /// Provides a collection of methods for code handling.
    /// </summary>
    public static class CodeOnTheFlyTools
    {
        /// <summary>
        /// Run code dynamically.
        /// </summary>
        /// <param name="code">The source code in a string.</param>
        /// <param name="sourceCodeComplete">A value which indicates if the source code is complete or if only a part has been delivered.</param>
        /// <param name="executingMethod">The method which should be executed.</param>
        /// <param name="languageProvider">The programming language in which the source code has been delivered.</param>
        /// <param name="methodParameters">A list of parameters which should be overgiven to the executing method.</param>
        /// <returns>Returns the returned value from the executing method (casted to string, will be converted to JSON if returned value isn't a primitive one).</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1118:Parameter should not span multiple lines", Justification = "To improve readability allow multiple lines for this method")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "We want to catch all exception that occur during a dynamic code execution.")]
        public static string RunDynamicCode(string code, bool sourceCodeComplete = false, string executingMethod = "main", string languageProvider = "CSharp", params object[] methodParameters)
        {
            var compiler = CodeDomProvider.CreateProvider(languageProvider);
            try
            {
                var parameters = new CompilerParameters();

                parameters.ReferencedAssemblies.Add("System.dll");

                var generatedCode = string.Empty;

                if (!sourceCodeComplete)
                {
                    generatedCode = string.Format(
                        CultureInfo.InvariantCulture,
                        @"using System; " +
                        "namespace DynamicCode {{ " +
                        "public class DynamicCodeClass {{ " +
                        "public object main(params object[] parameters) {{ {0} }} }} }}", code);
                }
                else
                {
                    generatedCode = code;
                }

                parameters.GenerateInMemory = false;

                var compiled = compiler.CompileAssemblyFromSource(parameters, generatedCode);

                if (compiled.Errors.HasErrors)
                {
                    string errorMessage = string.Empty;

                    errorMessage = string.Format(CultureInfo.InvariantCulture, "{0} errors during compilation:", compiled.Errors.Count);

                    foreach (CompilerError error in compiled.Errors)
                    {
                        errorMessage += string.Format(CultureInfo.InvariantCulture, "\r\nError on line {0}: {1}", error.Line, error.ErrorText);
                    }

                    return errorMessage + "\r\n---\r\n" + code.AddLineNumbers();
                }

                var assembly = compiled.CompiledAssembly;

                object compiledClass = null;

                if (!sourceCodeComplete)
                {
                    compiledClass = assembly.CreateInstance("DynamicCode.DynamicCodeClass");
                }
                else
                {
                    var firstAssemblyType = assembly.GetTypes().First();

                    compiledClass = assembly.CreateInstance(firstAssemblyType.Namespace + "." + firstAssemblyType.Name);
                }

                if (compiledClass == null)
                {
                    return "Couldn't load class.";
                }

                var codeParameters = new object[5];
                codeParameters[0] = "SampleString 123";
                codeParameters[1] = 1.23F; // sample float
                codeParameters[2] = 1234; // sample int
                codeParameters[3] = (double)1.23; // sample double
                codeParameters[4] = DateTime.UtcNow; // sample DateTime

                try
                {
                    object result = null;

                    if (!sourceCodeComplete)
                    {
                        result = compiledClass.GetType().InvokeMember(executingMethod, BindingFlags.InvokeMethod, null, compiledClass, codeParameters, CultureInfo.CurrentCulture);
                    }
                    else
                    {
                        result = compiledClass.GetType().InvokeMember(executingMethod, BindingFlags.InvokeMethod, null, compiledClass, methodParameters, CultureInfo.CurrentCulture);
                    }

                    if (result != null)
                    {
                        if (result.GetType().IsPrimitive)
                        {
                            return string.Format(CultureInfo.CurrentCulture, "Result ({0}):\r\n{1}", result.GetType().Name, result.ToString());
                        }
                        else
                        {
                            return string.Format(CultureInfo.CurrentCulture, "Result ({0}):\r\n{1}", result.GetType().Name, JsonConvert.SerializeObject(result));
                        }
                    }
                    else
                    {
                        return "No result received";
                    }
                }
                catch (Exception exception)
                {
                    return exception.Message;
                }
            }
            finally
            {
                compiler.Dispose();
            }
        }
    }
}
