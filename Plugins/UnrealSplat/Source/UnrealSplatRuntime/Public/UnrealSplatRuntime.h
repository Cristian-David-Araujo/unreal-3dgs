// Copyright Epic Games, Inc. All Rights Reserved.
//
// UnrealSplatRuntime.h
// ----------------------------------------------------------------------------
// Public header for the UnrealSplatRuntime module.
// Included by any other module that depends on UnrealSplatRuntime.
//
// This module is the ONLY UnrealSplat module that ships inside the Android APK.
// The Editor module (UnrealSplat) is stripped at package time — it only runs
// inside the Unreal Editor on desktop.
// ----------------------------------------------------------------------------

#pragma once

#include "Modules/ModuleManager.h"

// ============================================================================
// Module declaration
// ============================================================================

/**
 * FUnrealSplatRuntimeModule
 *
 * Runtime module for the UnrealSplat plugin.
 * Lives in memory on ALL platforms (Windows Editor, Android/Quest, etc.).
 *
 * Responsibilities:
 *   - Ensure the plugin's Niagara content directory is registered and
 *     reachable by the engine at runtime (important for Android packaging).
 *   - Future: expose on-device 3DGS streaming / LOD helpers.
 */
class FUnrealSplatRuntimeModule : public IModuleInterface
{
public:
	/** IModuleInterface implementation */
	virtual void StartupModule() override;
	virtual void ShutdownModule() override;
};
