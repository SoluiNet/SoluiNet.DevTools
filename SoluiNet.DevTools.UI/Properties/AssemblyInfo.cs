// <copyright file="AssemblyInfo.cs" company="SoluiNet">
// Copyright (c) SoluiNet. All rights reserved.
// </copyright>

using System;
using System.Reflection;
using System.Resources;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Windows;

// Durch Festlegen von ComVisible auf FALSE werden die Typen in dieser Assembly
// für COM-Komponenten unsichtbar.  Wenn Sie auf einen Typ in dieser Assembly von
// COM aus zugreifen müssen, sollten Sie das ComVisible-Attribut für diesen Typ auf "True" festlegen.
[assembly: ComVisible(false)]

// Um mit dem Erstellen lokalisierbarer Anwendungen zu beginnen, legen Sie
// <UICulture>ImCodeVerwendeteKultur</UICulture> in der .csproj-Datei
// in einer <PropertyGroup> fest.  Wenn Sie in den Quelldateien beispielsweise Deutsch
// (Deutschland) verwenden, legen Sie <UICulture> auf \"de-DE\" fest.  Heben Sie dann die Auskommentierung
// des nachstehenden NeutralResourceLanguage-Attributs auf.  Aktualisieren Sie "en-US" in der nachstehenden Zeile,
// sodass es mit der UICulture-Einstellung in der Projektdatei übereinstimmt.

// do not set NeutralResourcesLanguage since we have no resource satellite libraries until now
// [assembly: NeutralResourcesLanguage("en-GB", UltimateResourceFallbackLocation.Satellite)]
[assembly: ThemeInfo(
    ResourceDictionaryLocation.None,
    ResourceDictionaryLocation.SourceAssembly)
]

[assembly: CLSCompliant(true)]

[assembly: AssemblyVersion("0.9.0.0")]