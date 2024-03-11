﻿using System.Runtime.CompilerServices;

public static class ModuleInitializer
{
    [ModuleInitializer]
    public static void Initialize()
    {
        VerifierSettings.SortJsonObjects();
        VerifyHttp.Initialize();
    }
}
