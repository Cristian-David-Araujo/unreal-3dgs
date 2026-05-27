// Copyright Epic Games, Inc. All Rights Reserved.
//
// UnrealSplat.Build.cs  —  EDITOR-ONLY module
// ----------------------------------------------------------------------------
// This module compiles exclusively for Editor targets (Win64 editor builds).
// It provides:
//   - PLY file parsing and texture generation (Preprocess3DGSModel / Parser)
//   - Editor toolbar UI button (UnrealSplat.cpp)
//   - Asset-creation helpers that rely on editor-only UE APIs
//
// It is intentionally NOT compiled for Android / Quest APK builds.
// Runtime rendering on Quest is handled by the UnrealSplatRuntime module.
// ----------------------------------------------------------------------------

using UnrealBuildTool;

public class UnrealSplat : ModuleRules
{
	public UnrealSplat(ReadOnlyTargetRules Target) : base(Target)
	{
		PCHUsage = ModuleRules.PCHUsageMode.UseExplicitOrSharedPCHs;

		// This module is Editor-only; enforce that assumption at build time.
		if (!Target.bBuildEditor)
		{
			System.Console.Error.WriteLine(
				"[UnrealSplat] ERROR: The UnrealSplat Editor module must not be " +
				"compiled for non-editor targets. Check .uplugin platform allow-list.");
		}

		PrivateIncludePaths.AddRange(new string[]
		{
			"UnrealSplat",
		});

		// ---------------------------------------------------------------
		// Public dependencies (exposed to modules that depend on this one)
		// ---------------------------------------------------------------
		PublicDependencyModuleNames.AddRange(new string[]
		{
			"Core",
			"RHI",
			"RenderCore",
			"Renderer",
		});

		// ---------------------------------------------------------------
		// Private dependencies — all Editor-safe
		// ---------------------------------------------------------------
		PrivateDependencyModuleNames.AddRange(new string[]
		{
			"CoreUObject",
			"Engine",
			"Slate",
			"SlateCore",
			"InputCore",
			"EnhancedInput",
			"Niagara",
			// --- Editor-only modules (never available on Android) ---
			"UnrealEd",
			"ToolMenus",
			"Blutility",
			"UMG",
			"UMGEditor",
			"EditorScriptingUtilities",
			"ContentBrowser",
			"AssetRegistry",
			"AssetTools",
		});

		// ---------------------------------------------------------------
		// Depend on our own Runtime module so shared types are accessible
		// ---------------------------------------------------------------
		PublicDependencyModuleNames.Add("UnrealSplatRuntime");
	}
}

