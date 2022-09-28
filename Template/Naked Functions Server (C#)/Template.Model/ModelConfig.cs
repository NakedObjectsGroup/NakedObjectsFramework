﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace Template.Model; 

//A helper class to provide model configuration for use by Startup.cs in the Server project.
//The implementation here relies on the conventions that:
//- All domain classes are defined in namespace "Template.Model.Types"
//- All domain functions are defined on static types in namespace "Template.Model.Functions"
//- All main menu function are defined on static types that have 'MenuFunctions' in their name
//This ModelConfig may be re-written to change the conventions, or to remove conventions altogether, and
//specify the lists of types, functions, and menus explicitly.
public static class ModelConfig {
    public static Type[] DomainTypes() =>
        PublicClassesInterfacesEnums.Where(t => t.Namespace == "Template.Model.Types" && !t.IsStaticClass()).ToArray();

    public static Type[] TypesDefiningDomainFunctions() =>
        PublicClassesInterfacesEnums.Where(t => t.Namespace == "Template.Model.Functions" && t.IsStaticClass()).ToArray();

    public static Type[] MainMenus() =>
        TypesDefiningDomainFunctions().Where(t => t.FullName.Contains("MenuFunctions")).ToArray();

    #region Helpers

    private static IEnumerable<Type> PublicClassesInterfacesEnums =>
        typeof(ModelConfig).Assembly.GetTypes().Where(t => t.IsPublic && (t.IsClass || t.IsInterface || t.IsEnum));

    private static bool IsStaticClass(this Type t) => t.IsAbstract && t.IsSealed;

    #endregion
}