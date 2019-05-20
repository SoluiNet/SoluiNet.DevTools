﻿using Microsoft.CSharp;
using Newtonsoft.Json;
using SoluiNet.DevTools.Core.Tools.String;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SoluiNet.DevTools.Utils.CodeOnTheFly
{
    public static class CodeOnTheFlyTools
    {
        public static string RunDynamicCode(string code, bool sourceCodeComplete = false, string executingMethod = "main", string languageProvider = "CSharp", params object[] methodParameters)
        {
            var compiler = CodeDomProvider.CreateProvider(languageProvider);
            var parameters = new CompilerParameters();

            parameters.ReferencedAssemblies.Add("System.dll");

            var generatedCode = string.Empty;

            if (!sourceCodeComplete)
            {
                generatedCode = string.Format(@"using System; " +
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
                string errorMessage = "";

                errorMessage = string.Format("{0} errors during compilation:", compiled.Errors.Count);

                foreach (CompilerError error in compiled.Errors)
                {
                    errorMessage += string.Format("\r\nError on line {0}: {1}", error.Line, error.ErrorText);
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
            codeParameters[1] = (float)1.23; // sample float
            codeParameters[2] = 1234; // sample int
            codeParameters[3] = (double)1.23; // sample double
            codeParameters[4] = DateTime.UtcNow; // sample DateTime

            try
            {
                object result = null;

                if (!sourceCodeComplete)
                {
                    result = compiledClass.GetType().InvokeMember(executingMethod, BindingFlags.InvokeMethod, null, compiledClass, codeParameters);
                }
                else
                {
                    result = compiledClass.GetType().InvokeMember(executingMethod, BindingFlags.InvokeMethod, null, compiledClass, methodParameters);
                }

                if (result != null)
                {
                    if (result.GetType().IsPrimitive)
                    {
                        return string.Format("Result ({0}):\r\n{1}", result.GetType().Name, result.ToString());
                    }
                    else
                    {
                        return string.Format("Result ({0}):\r\n{1}", result.GetType().Name, JsonConvert.SerializeObject(result));
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
    }
}
