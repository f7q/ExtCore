// Fork for Dmitry Sikorsky. ExtCore/ExtCore.
// Copyright © 2016 f7Q. All rights reserved.
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
        /// <summary>
        /// 追加対象アセンブリフォルダパスにあるアセンブリを取得する。
        /// </summary>
        /// <param name="assemblies">追加前assemblies</param>
        /// <param name="path">追加対象アセンブリフォルダパス</param>
        /// <returns>追加済assemblies</returns>
        public static IEnumerable<Assembly> GetAssemblies(IEnumerable<Assembly> assemblies, string path)
        {
            foreach (var asm in assemblies)
            {
                yield return asm;
            }

            if (!string.IsNullOrEmpty(path) && Directory.Exists(path))
            {
                foreach (string extensionPath in Directory.EnumerateFiles(path, "*.dll"))
                {
                    Assembly assembly = AssemblyLoadContext.Default.LoadFromAssemblyPath(extensionPath);

                    if (AssemblyManager.IsCandidateAssembly(assembly))
                    {
                        yield return assembly;
                    }
                }
            }
        }

        /// <summary>
        /// CompilationLibraryをアセンブリ一覧に追加して返却
        /// </summary>
        /// <param name="assemblies">追加前assemblies</param>
        /// <returns>追加済assemblies</returns>
        public static IEnumerable<Assembly> GetAssembliesCompilationLibrary(IEnumerable<Assembly> assemblies)
        {
            foreach (var assembly in assemblies)
            {
                yield return assembly;
            }

            foreach (CompilationLibrary compilationLibrary in DependencyContext.Default.CompileLibraries)
            { 
                if (AssemblyManager.IsCandidateCompilationLibrary(compilationLibrary))
                { 
                    yield return Assembly.Load(new AssemblyName(compilationLibrary.Name));
                }
            }
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