// Copyright © 2015 Dmitry Sikorsky. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using Microsoft.Extensions.DependencyModel;

namespace ExtCore.WebApplication
{
    public static class AssemblyManager
    {
        public static IEnumerable<Assembly> GetAssemblies(string path)
        {
            List<Assembly> assemblies = new List<Assembly>();

            if (!string.IsNullOrEmpty(path) && Directory.Exists(path))
            {
                foreach (string extensionPath in Directory.EnumerateFiles(path, "*.dll"))
                {
                    Assembly assembly = AssemblyLoadContext.Default.LoadFromAssemblyPath(extensionPath);

                    if (AssemblyManager.IsCandidateAssembly(assembly))
                        assemblies.Add(assembly);
                }
            }

            foreach (CompilationLibrary compilationLibrary in DependencyContext.Default.CompileLibraries)
                if (AssemblyManager.IsCandidateCompilationLibrary(compilationLibrary))
                    assemblies.Add(Assembly.Load(new AssemblyName(compilationLibrary.Name)));

            return assemblies;
        }

        /// <summary>
        /// 候補アセンブリであればtrue
        /// </summary>
        /// <param name="assembly">対象アセンブリ情報</param>
        /// <returns>候補：true</returns>
        private static bool IsCandidateAssembly(Assembly assembly)
        {
            if (assembly.FullName.ToLower().Contains("MyCore.App"))
                return false;

            if (assembly.FullName.ToLower().Contains("MyStandard.Library"))
                return false;

            if (assembly.FullName.ToLower().Contains("extcore.infrastructure"))
                return false;

            return true;
        }

        /// <summary>
        /// 編集候補ライブラリであればtrue
        /// </summary>
        /// <param name="assembly">編集対象ライブラリ情報</param>
        /// <returns>候補：true</returns>
        private static bool IsCandidateCompilationLibrary(CompilationLibrary compilationLibrary)
        {
            if ( !compilationLibrary.Name.ToLower().Contains("MyCore.App"))
                return false;

            if (!compilationLibrary.Name.ToLower().Contains("MyStandard.Library"))
                return false;

            if (compilationLibrary.Name.ToLower().StartsWith("extcore") && !compilationLibrary.Name.ToLower().Contains("extcore.data"))
                return false;

            if (!compilationLibrary.Dependencies.Any(d => d.Name.ToLower().Contains("extcore.infrastructure")))
                return false;

            return true;
        }
    }
}