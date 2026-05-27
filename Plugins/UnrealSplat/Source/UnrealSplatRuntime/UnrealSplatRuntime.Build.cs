// Copyright Epic Games, Inc. All Rights Reserved.
//
// UnrealSplatRuntime.Build.cs
// ----------------------------------------------------------------------------
// Runtime module for UnrealSplat.
// This module compiles for ALL platforms, including Android (Meta Quest 3/Pro).
// It contains NO editor-only dependencies so it can be packaged into an APK.
//
// Responsibilities:
//   - Register the plugin module at runtime so Niagara content is available.
//   - Provide platform-agnostic utilities (e.g. texture access helpers).
//   - Act as the runtime host for future on-device 3DGS playback features.
// ----------------------------------------------------------------------------

using UnrealBuildTool;

public class UnrealSplatRuntime : ModuleRules
{
	public UnrealSplatRuntime(ReadOnlyTargetRules Target) : base(Target)
	{
		PCHUsage = ModuleRules.PCHUsageMode.UseExplicitOrSharedPCHs;

		// ---------------------------------------------------------------
		// Public (header-visible) dependencies — all runtime-safe
		// ---------------------------------------------------------------
		PublicDependencyModuleNames.AddRange(new string[]
		{
			"Core",
			"CoreUObject",
			"Engine",
			"RHI",
			"RenderCore",
		});

		// ---------------------------------------------------------------
		// Private (implementation-only) dependencies — all runtime-safe
		// ---------------------------------------------------------------
		PrivateDependencyModuleNames.AddRange(new string[]
		{
			"Niagara",
			"InputCore",
		});

		// ---------------------------------------------------------------
		// Android / Meta Quest specific
		// When building for Android (Quest 3 / Pro), we pull in the
		// OculusXR runtime dependency only if the plugin is present.
		// This is done via a conditional so Windows builds are unaffected.
		// ---------------------------------------------------------------
		if (Target.Platform == UnrealTargetPlatform.Android)
		{
			// OculusXR provides XrSession / FFR / Eye-tracked foveation APIs.
			// Enabled only if the OculusXR plugin is active in the project.
			PrivateDependencyModuleNames.AddRange(new string[]
			{
				// Uncomment when OculusXR plugin is enabled in the .uproject:
				// "OculusXRHMD",
				// "OculusXRInput",
			});
		}
	}
}
