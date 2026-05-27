# Quest 3 / Pro Standalone Build Guide

This document explains how to set up, build, and deploy the UnrealSplat plugin to a **Meta Quest 3** or **Meta Quest Pro** in standalone (no-PC) mode.

---

## Prerequisites

### 1. Hardware
| Device | Minimum firmware |
|--------|-----------------|
| Meta Quest 3 | v62 |
| Meta Quest Pro | v62 |

### 2. Software on your build machine (Windows)
| Tool | Recommended version | Notes |
|------|--------------------|-------|
| Unreal Engine | 5.5.x | Must match `EngineAssociation` in `.uproject` |
| Android Studio | Hedgehog (2023.1.1+) | Provides JDK 17 and SDK Manager |
| Android SDK Platform | API 34 | Install via SDK Manager |
| Android NDK | r25b (25.1.8937393) | Install via SDK Manager |
| Meta XR SDK (OculusXR UE plugin) | Bundled with UE 5.5 | Enable in .uproject (already done) |

### 3. UE5 Android setup wizard
Run **Edit → Project Settings → Platforms → Android SDK** and fill in the paths:

```
SDK Path  : C:\Users\<you>\AppData\Local\Android\Sdk
NDK Path  : C:\Users\<you>\AppData\Local\Android\Sdk\ndk\25.1.8937393
JDK Path  : C:\Program Files\Android\Android Studio\jbr
```

These paths are also stored in `Config/DefaultEngine.ini` under `[/Script/AndroidPlatformEditor.AndroidSDKSettings]`.

---

## Enabling Developer Mode on Your Quest

1. Open the **Meta Quest mobile app** on your phone.
2. Go to **Menu → Devices** and select your headset.
3. Tap **Developer Mode** and enable it.
4. Put on the headset and accept the developer-mode prompt.
5. Connect the headset to your PC via USB-C.
6. Inside the headset, allow the **"Allow USB debugging"** prompt.

Verify the connection:
```powershell
adb devices
# Should list: <serial>  device
```

---

## Project Build Settings (already configured)

The following changes have been made to support standalone Quest builds. You should **not** need to change these manually:

| File | What was changed |
|------|-----------------|
| `UnrealSplatPlugin.uproject` | Added `OculusXR`, `OpenXR`, `AndroidPermission` plugins |
| `Plugins/UnrealSplat/UnrealSplat.uplugin` | Added `UnrealSplatRuntime` (Runtime module) alongside the existing `UnrealSplat` (Editor-only) module |
| `Config/Android/AndroidEngine.ini` | Quest-specific renderer overrides (Vulkan, no Ray Tracing, no Nanite, FFR level 2, Multiview) |
| `Config/DefaultEngine.ini` | Added Android SDK paths and OpenXR/OculusXR base settings |

### Why a separate Runtime module?
The original `UnrealSplat` module is declared as `"Type": "Editor"` in the `.uplugin`. This means it compiles **only** inside the Unreal Editor on Windows and is **stripped** from APK builds. Without a Runtime module the plugin's Niagara content would not load on-device.

The new `UnrealSplatRuntime` module is `"Type": "Runtime"` and has **zero editor-only dependencies**, so it compiles cleanly for `android-arm64`.

---

## Packaging the APK

### Via the Editor UI
1. Open the project in UE5.5 (`UnrealSplatPlugin.uproject`).
2. Go to **Platforms → Android → Package Project**.
3. Select an output folder (e.g. `Saved/StagedBuilds/Android`).
4. Wait for the cook and package step to finish (5–20 minutes first time).
5. Find `<ProjectName>-arm64.apk` in your output folder.

### Via Command Line (recommended for CI)
```powershell
# Replace paths with your actual UE installation
$UATPath = "C:\Program Files\Epic Games\UE_5.5\Engine\Build\BatchFiles\RunUAT.bat"
$ProjectPath = "C:\OneLink\unreal-3dgs\UnrealSplatPlugin.uproject"
$OutputDir = "C:\OneLink\unreal-3dgs\Saved\StagedBuilds\Android"

& $UATPath BuildCookRun `
    -project="$ProjectPath" `
    -noP4 `
    -platform=Android `
    -clientconfig=Development `
    -cook `
    -allmaps `
    -build `
    -stage `
    -pak `
    -archive `
    -archivedirectory="$OutputDir"
```

---

## Installing & Running on Quest

### Install via adb
```powershell
# Install (replace the APK name with your actual output file)
adb install -r "C:\OneLink\unreal-3dgs\Saved\StagedBuilds\Android\UnrealSplatPlugin-arm64.apk"

# Launch the app
adb shell monkey -p com.yourcompany.unrealsplat 1
```

### Install via Meta Quest Developer Hub (MQDH)
1. Open MQDH and select your device.
2. Drag and drop the `.apk` onto the **My Apps** panel.
3. The app will appear under **Unknown Sources** in the Quest library.

---

## Texture Compression Notes

The Gaussian Splat data textures (position, scale, rotation, color) are stored as **32-bit float RGBA** (`PF_A32B32G32R32F`) with `CompressionNone=True`. This is intentional — lossy compression (ASTC/ETC2) would corrupt the float splat data.

Quest 3 and Pro both support 32-bit float textures via their Vulkan 1.1 implementation. No changes to texture settings are required.

---

## Performance Tuning on Quest

### Fixed Foveated Rendering (FFR)
Configured in `Config/Android/AndroidEngine.ini`:
```ini
FFRLevel=2   ; 0=Off, 1=Low, 2=Medium, 3=High, 4=HighTop
```
Increase to `3` or `4` if GPU is the bottleneck (check OVR Metrics Tool).

### GPU / CPU Levels
```ini
CPULevel=2   ; 0–5, -1=auto
GPULevel=3   ; 0–5, -1=auto
```
Profile with **OVR Metrics Tool** (free on the Meta Store) to find the right balance.

### Niagara GPU Particles
The 3DGS rendering uses Niagara GPU simulation. On Quest, GPU particles are supported via Vulkan compute shaders. If you see black splats, check:
1. **Vulkan RHI is selected** (not OpenGL ES 3.1).
2. **`bAllowGPUParticles=True`** is set (already in `AndroidEngine.ini`).
3. The Niagara system's **Simulation Stage** is not using features unsupported on mobile (e.g., Raytraced visibility).

---

## Known Limitations (Quest)

| Limitation | Status | Workaround |
|-----------|--------|-----------|
| PLY file import | Not supported on-device | Import on PC in the editor, then package the resulting textures |
| Models > 2M splats | May exceed Quest 3 GPU memory | Reduce splat count at export time |
| Splat transformations | Rendering may break when actor is moved | Keep actor at world origin or fix in Niagara system |
| Spherical Harmonics | WIP — disabled by default | Use base colour texture variant (`3DGSActor.uasset`) |

---

## Troubleshooting

### "adb: device not found"
- Ensure USB debugging is allowed inside the headset.
- Try a different USB cable (data cable, not charge-only).

### Black screen on Quest
- Confirm Vulkan is selected: **Project Settings → Android → Graphics API → Vulkan**.
- Check device logs: `adb logcat -s UE | grep -i "unrealsplat"`.

### Cook fails with "Ray Tracing shader compile error"
- Open **Project Settings → Platforms → Android** and confirm **Support Ray Tracing** is unchecked.
- The `AndroidEngine.ini` override `r.RayTracing=False` should handle this automatically.

### "Module UnrealSplatRuntime not found"
- Regenerate project files: right-click `UnrealSplatPlugin.uproject → Generate Visual Studio project files`.
- Build the `Development Android` target in Visual Studio.
