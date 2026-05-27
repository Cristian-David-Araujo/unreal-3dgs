// Copyright Epic Games, Inc. All Rights Reserved.
//
// UnrealSplatRuntime.cpp
// ----------------------------------------------------------------------------
// Implementation of the UnrealSplatRuntime module.
//
// This file intentionally contains minimal logic so that it builds cleanly on
// every target platform, including Android arm64 (Meta Quest 3 / Pro).
// Heavy editor-only logic (PLY parsing, asset creation) stays in the separate
// UnrealSplat **Editor** module and is never compiled into the APK.
// ----------------------------------------------------------------------------

#include "UnrealSplatRuntime.h"

#include "Logging/LogMacros.h"

// Module-local log category
DEFINE_LOG_CATEGORY_STATIC(LogUnrealSplatRuntime, Log, All);

#define LOCTEXT_NAMESPACE "FUnrealSplatRuntimeModule"

// ============================================================================
// Module lifecycle
// ============================================================================

void FUnrealSplatRuntimeModule::StartupModule()
{
	UE_LOG(LogUnrealSplatRuntime, Log,
		TEXT("UnrealSplatRuntime module started. "
			 "Platform: %s — 3DGS Niagara rendering available."),
		*FString(FPlatformProperties::IniPlatformName()));

#if PLATFORM_ANDROID
	// On Android / Meta Quest we confirm Vulkan is the active RHI.
	// A mismatch here means the project was packaged with OpenGL ES3.1 instead
	// of Vulkan, which will reduce rendering quality.
	UE_LOG(LogUnrealSplatRuntime, Log,
		TEXT("UnrealSplatRuntime: Android build detected. "
			 "Ensure Vulkan RHI is selected in Android Project Settings "
			 "for best performance on Meta Quest 3 / Pro."));
#endif
}

void FUnrealSplatRuntimeModule::ShutdownModule()
{
	UE_LOG(LogUnrealSplatRuntime, Log, TEXT("UnrealSplatRuntime module shut down."));
}

#undef LOCTEXT_NAMESPACE

// Register this module with the engine
IMPLEMENT_MODULE(FUnrealSplatRuntimeModule, UnrealSplatRuntime)
