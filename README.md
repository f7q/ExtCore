# ExtCore 1.0.0-alpha901

## Introduction

ExtCore is free, open source and cross-platform framework for creating modular and extendable web applications
based on ASP.NET Core. It is built using the best and the most modern tools and languages (Visual Studio 2017, C#
etc). Join our team!

ExtCore allows you to decouple your application into the modules (or extensions) and reuse that modules in other
applications in various combinations. Each ExtCore extension may consist of one or more projects and each project
may include everything you want (as any other ASP.NET Core project). Controllers, view components, views (added as
resources and/or precompiled), static content (added as resources) will be resolved automatically. These projects
(extension pieces) may be added to the application directly as dependencies in project.json of your main
application project (as source code or NuGet packages), or by copying compiled DLL-files to the Extensions
folder. ExtCore supports both of these approaches out of the box and at the same time.

By default, ExtCore doesn’t know anything about data and storage, but you can use ExtCore.Data extension to have
unified approach to working with data and single storage context. It supports Microsoft SQL Server, PostgreSql
and SQLite, but it is very easy to add another storage support.

You can find more information using the links at the bottom of this page.

### Samples

Please take a look at our [sample](https://github.com/f7q/ExtCore) on GitHub.
Please take a look at our [sample](https://github.com/f7q/ExtCore-Sample) on GitHub.
