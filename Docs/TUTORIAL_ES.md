# Tutorial Completo: UnrealSplat en Meta Quest 3 / Pro
### Desde cero hasta APK instalado en el dispositivo

> **Idioma:** Español  
> **Plataforma destino:** Meta Quest 3 · Meta Quest Pro (standalone, sin PC)  
> **Sistema de desarrollo:** Windows 10/11 (64-bit)  
> **Motor:** Unreal Engine 5.5

---

## Índice

1. [Software necesario](#1-software-necesario)
2. [Habilitar modo desarrollador en el Quest](#2-habilitar-modo-desarrollador-en-el-quest)
3. [Instalar Unreal Engine 5.5](#3-instalar-unreal-engine-55)
4. [Instalar Android Studio y el SDK/NDK](#4-instalar-android-studio-y-el-sdkndk)
5. [Clonar el repositorio](#5-clonar-el-repositorio)
6. [Generar archivos del proyecto (Visual Studio)](#6-generar-archivos-del-proyecto-visual-studio)
7. [Compilar el plugin (Build)](#7-compilar-el-plugin-build)
8. [Abrir el proyecto en el Editor](#8-abrir-el-proyecto-en-el-editor)
9. [Configurar rutas Android SDK en el Editor](#9-configurar-rutas-android-sdk-en-el-editor)
10. [Importar un modelo 3DGS (archivo .ply)](#10-importar-un-modelo-3dgs-archivo-ply)
11. [Preparar el nivel para Quest](#11-preparar-el-nivel-para-quest)
12. [Empaquetar el APK](#12-empaquetar-el-apk)
13. [Instalar el APK en el Quest](#13-instalar-el-apk-en-el-quest)
14. [Actualizar la app (flujo diario)](#14-actualizar-la-app-flujo-diario)
15. [Solución de problemas frecuentes](#15-solución-de-problemas-frecuentes)

---

## 1. Software necesario

Instala todo el siguiente software **antes** de continuar.

| Herramienta | Versión mínima | Descarga |
|-------------|----------------|---------|
| Windows | 10 o 11 (64-bit) | — |
| **Git** | 2.40+ | https://git-scm.com/download/win |
| **Visual Studio 2022** | 17.x Community | https://visualstudio.microsoft.com/es/vs/community/ |
| **Epic Games Launcher** | última | https://www.epicgames.com/site/es-ES/epic-games-store |
| **Unreal Engine** | 5.5.x | (desde el Launcher, ver paso 3) |
| **Android Studio** | Hedgehog 2023.1.1+ | https://developer.android.com/studio |
| **Meta Quest Developer Hub** | última | https://developer.oculus.com/documentation/unity/ts-odh/ |
| **ADB (Android Debug Bridge)** | incluido con Android Studio | — |

### Visual Studio: cargas de trabajo requeridas
Al instalar Visual Studio 2022, selecciona estas cargas de trabajo:
- ✅ **Desarrollo de escritorio con C++**
- ✅ **Desarrollo de juegos con C++** (sección "Game Development with C++")

También marca en componentes individuales:
- ✅ **Windows 10/11 SDK** (10.0.18362.0 o superior)
- ✅ **MSVC v143 – VS 2022 C++ x64/x86 build tools**

---

## 2. Habilitar modo desarrollador en el Quest

Antes de conectar el Quest a tu PC necesitas activar el modo desarrollador.

### En el teléfono (app Meta Quest)
1. Descarga la app **Meta Quest** en tu teléfono.
2. Abre la app → **Menú** → **Dispositivos** → selecciona tu Quest.
3. Toca **Configuración del dispositivo** → **Modo desarrollador** → actívalo.

### En el headset
1. Enciende el Quest y ponlo.
2. Conecta el Quest a tu PC con un cable **USB-C que soporte datos** (no sólo carga).
3. Dentro del headset aparecerá el diálogo **"¿Permitir depuración USB?"** → toca **Permitir**.

### Verificar conexión (PowerShell)
```powershell
# Abre PowerShell y ejecuta:
adb devices
```
Deberías ver algo como:
```
List of devices attached
1WMHH812345678   device
```
Si aparece `unauthorized`, acepta el diálogo dentro del headset.

> **Nota:** Si `adb` no es reconocido, añade `C:\Users\<tu-usuario>\AppData\Local\Android\Sdk\platform-tools` al PATH del sistema.

---

## 3. Instalar Unreal Engine 5.5

1. Abre **Epic Games Launcher** e inicia sesión (crea cuenta gratis si no tienes).
2. Ve a la pestaña **Unreal Engine** → **Biblioteca**.
3. Haz clic en **+** y selecciona la versión **5.5.x**.
4. En opciones de instalación, asegúrate de marcar:
   - ✅ **Android** (bajo *Target Platforms*)
5. Instala (descarga ~30–60 GB dependiendo de las opciones).

---

## 4. Instalar Android Studio y el SDK/NDK

### 4.1 Instalar Android Studio
1. Descarga e instala Android Studio Hedgehog desde el enlace de la tabla.
2. Durante la instalación, acepta todos los valores predeterminados.
3. Al abrir Android Studio por primera vez, completa el asistente de configuración (descarga componentes adicionales si se pide).

### 4.2 Instalar SDK Platform API 34 y NDK r25b
1. En Android Studio: **Tools → SDK Manager**
2. Pestaña **SDK Platforms**:
   - ✅ Marca **Android 14 (API 34)**
3. Pestaña **SDK Tools**:
   - ✅ **Android SDK Build-Tools 34.0.0**
   - ✅ **NDK (Side by side)** → expande → ✅ **25.1.8937393**
   - ✅ **Android SDK Platform-Tools** (incluye adb)
4. Haz clic en **Apply** y espera la descarga.

### 4.3 Anotar las rutas
Abre el SDK Manager y copia la ruta del SDK (columna "Android SDK Location").  
Ejemplo típico:
```
SDK:  C:\Users\TuUsuario\AppData\Local\Android\Sdk
NDK:  C:\Users\TuUsuario\AppData\Local\Android\Sdk\ndk\25.1.8937393
JDK:  C:\Program Files\Android\Android Studio\jbr
```

---

## 5. Clonar el repositorio

Abre **PowerShell** o **Git Bash** y ejecuta:

```powershell
# Navega a la carpeta donde quieres el proyecto
cd C:\

# Clona el repositorio
git clone https://github.com/Cristian-David-Araujo/unreal-3dgs.git

# Entra al directorio
cd unreal-3dgs

# Cambia a la rama de integración (develop)
git checkout develop

# O usa la rama de la feature de Quest si quieres lo más reciente:
git checkout feature/quest3-android-support
```

Verifica la estructura:
```
unreal-3dgs/
├── Config/
│   ├── Android/
│   │   └── AndroidEngine.ini   ← overrides para Quest
│   ├── DefaultEngine.ini
│   └── ...
├── Docs/
│   ├── QUEST_SETUP.md
│   └── TUTORIAL_ES.md          ← este archivo
├── Plugins/
│   └── UnrealSplat/
│       ├── UnrealSplat.uplugin
│       └── Source/
│           ├── UnrealSplat/         ← módulo Editor (solo Windows)
│           └── UnrealSplatRuntime/  ← módulo Runtime (Windows + Quest APK)
├── Source/
├── UnrealSplatPlugin.uproject   ← archivo de proyecto UE5
└── README.md
```

---

## 6. Generar archivos del proyecto (Visual Studio)

El archivo `.sln` de Visual Studio **no** se guarda en git. Debes generarlo tú.

### Método 1 — Clic derecho en el .uproject (recomendado)
1. En el Explorador de Windows, navega hasta `C:\unreal-3dgs`.
2. Clic derecho sobre `UnrealSplatPlugin.uproject`.
3. Selecciona **"Generate Visual Studio project files"**.
4. Espera a que termine (puede tomar 1–3 minutos).
5. Aparecerá `UnrealSplatPlugin.sln` en la carpeta.

### Método 2 — Línea de comandos
```powershell
# Reemplaza la ruta con tu instalación de UE5.5
$UBT = "C:\Program Files\Epic Games\UE_5.5\Engine\Binaries\DotNET\UnrealBuildTool\UnrealBuildTool.exe"
& $UBT -ProjectFiles -Project="C:\unreal-3dgs\UnrealSplatPlugin.uproject" -Game -Rocket -Progress
```

---

## 7. Compilar el plugin (Build)

1. Abre `UnrealSplatPlugin.sln` con **Visual Studio 2022**.
2. En la barra superior, configura:
   - **Solución**: `Development Editor`
   - **Plataforma**: `Win64`
3. Clic derecho sobre el proyecto `UnrealSplatPlugin` en el Explorador de soluciones.
4. Selecciona **Build** (o presiona `Ctrl+Shift+B`).
5. Espera a que compile (primera vez puede ser 5–15 minutos).

Si la compilación falla:
- Verifica que instalaste Visual Studio con las cargas de trabajo correctas (Paso 1).
- Revisa la pestaña **Output → Build** para ver el error específico.

---

## 8. Abrir el proyecto en el Editor

1. Haz doble clic en `UnrealSplatPlugin.uproject`.
2. Si pregunta si quieres compilar, haz clic en **Sí**.
3. El Editor de Unreal 5.5 abrirá el proyecto.

> **Primera vez**: el Editor puede tardar varios minutos mientras compila los shaders.

Verifica que el plugin esté activo:
- Ve a **Edit → Plugins**.
- Busca **"UnrealSplat"** — debe aparecer activado (check verde).
- Debería aparecer también el botón **"UnrealSplat"** en la barra de herramientas superior.

---

## 9. Configurar rutas Android SDK en el Editor

Esta configuración le dice a UE5 dónde encontrar el SDK de Android para compilar el APK.

1. En el Editor: **Edit → Project Settings**.
2. En el panel izquierdo, busca **Platforms → Android SDK**.
3. Rellena los campos con las rutas que anotaste en el Paso 4.3:

| Campo | Valor |
|-------|-------|
| **SDK API Level** | `android-34` |
| **NDK API Level** | `android-34` |
| **SDK Path** | `C:\Users\TuUsuario\AppData\Local\Android\Sdk` |
| **NDK Path** | `C:\Users\TuUsuario\AppData\Local\Android\Sdk\ndk\25.1.8937393` |
| **JDK Path** | `C:\Program Files\Android\Android Studio\jbr` |

4. Haz clic en **Set SDK Config** (si aparece el botón) o simplemente cierra la ventana — los cambios se guardan automáticamente.

> **Verificación rápida**: Si los campos están en rojo, las rutas no son correctas. Navega a esas carpetas con el Explorador de Windows para confirmar que existen.

También abre **Platforms → Android** y verifica:
- ✅ **Support arm64** marcado
- ✅ **Support Vulkan** marcado
- ✅ **Support OpenGL ES 3.1** desmarcado (opcional, reduce el tamaño del APK)

---

## 10. Importar un modelo 3DGS (archivo .ply)

El plugin importa modelos en formato **Gaussian Splat PLY** (el que genera el entrenamiento estándar 3DGS).

### 10.1 Copiar el modelo al proyecto
1. Navega a la carpeta `C:\unreal-3dgs\Content\Models\` (créala si no existe).
2. Copia tu archivo `.ply` dentro de esa carpeta.

   Ejemplo: `C:\unreal-3dgs\Content\Models\mi_escena.ply`

### 10.2 Procesar el modelo con el plugin
1. En el Editor, haz clic en el botón **"UnrealSplat"** de la barra de herramientas.
2. Se abrirá el panel del plugin.
3. En el campo de texto, escribe **solo el nombre del archivo** (relativo a `Content/Models/`):
   ```
   mi_escena.ply
   ```
4. Presiona el botón **Render** (o el botón de procesado).
5. El plugin:
   - Leerá el archivo PLY.
   - Creará las texturas de posición, escala, rotación y color en `Content/`.
   - Colocará un actor Niagara en el nivel.
6. Espera a que termine (puede tomar 1–5 minutos según el tamaño del modelo).

> **Modelos compatibles**: Solo `.ply` del pipeline estándar de 3DGS (campos `x, y, z, nx, ny, nz, f_dc_0..2, f_rest_0..44, opacity, scale_0..2, rot_0..3`).

### 10.3 Guardar el nivel
- **File → Save Current Level** (`Ctrl+S`).
- Guarda el nivel con el nombre que prefieras en `Content/Levels/`.

---

## 11. Preparar el nivel para Quest

El Quest 3 / Pro no soporta todas las características del renderer de PC. Hay que desactivar las que son incompatibles.

### 11.1 Desactivar Ray Tracing y Nanite en Project Settings
1. **Edit → Project Settings → Engine → Rendering**
2. Asegúrate de que estén desactivados:
   - ❌ **Ray Tracing** → Off
   - ❌ **Nanite** → Off  
   - ❌ **Virtual Shadow Maps** → Off
   - ❌ **Lumen** → Off (en Global Illumination y Reflections)

> **Nota**: Estas configuraciones ya están sobreescritas en `Config/Android/AndroidEngine.ini` para la build de Android. Pero es buena práctica tenerlas apagadas también en el proyecto.

### 11.2 Cambiar el modo de Anti-Aliasing para VR
1. En **Project Settings → Engine → Rendering → Default Settings**:
   - **Anti-Aliasing Method**: `MSAA`
2. MSAA es más eficiente que TAA en VR y evita el ghosting.

### 11.3 Verificar el Game Mode
Para Quest standalone necesitas un Game Mode que no dependa de sistemas de escritorio:
1. Ve al nivel que contiene tu modelo.
2. **Window → World Settings**.
3. En **Game Mode**, deja el valor predeterminado o asigna uno simple (puedes dejar `None`).

### 11.4 (Opcional) Ajustar la cámara para VR
Si tienes un Pawn o Character en el nivel, el Quest usará el tracking de cabeza automáticamente gracias a OpenXR. Para una experiencia básica funciona sin configuración adicional.

---

## 12. Empaquetar el APK

### 12.1 Configurar el Package Name
Antes de empaquetar, define el identificador único de tu app:
1. **Edit → Project Settings → Platforms → Android**
2. En **Android Package Name**: cambia `com.yourcompany.unrealsplat` por algo como:
   ```
   com.tuempresa.unrealsplat
   ```

### 12.2 Aceptar la licencia del SDK (primera vez)
La primera vez que empaquetas para Android es necesario aceptar las licencias:
```powershell
# En PowerShell, ejecuta:
C:\Users\TuUsuario\AppData\Local\Android\Sdk\cmdline-tools\latest\bin\sdkmanager.bat --licenses
# Escribe 'y' y Enter para cada licencia
```

### 12.3 Empaquetar desde el Editor
1. En el Editor: **Platforms → Android → Package Project**
2. Selecciona o crea una carpeta de salida, por ejemplo:
   ```
   C:\unreal-3dgs\Builds\Quest
   ```
3. Haz clic en **Select Folder**.
4. El proceso de packaging comenzará (barra de progreso en la esquina inferior derecha).

**Tiempos estimados** (primera vez):
| Fase | Tiempo |
|------|--------|
| Cook de assets | 10–30 min |
| Compilación del código Android | 5–15 min |
| Creación del APK | 2–5 min |
| **Total primera vez** | **~30–50 min** |

Las veces siguientes son mucho más rápidas porque los assets ya están cocinados.

5. Cuando termine, en la carpeta de salida encontrarás:
   ```
   UnrealSplatPlugin-arm64.apk
   ```

### 12.4 Empaquetar desde la línea de comandos (alternativa / CI)
```powershell
$UATPath = "C:\Program Files\Epic Games\UE_5.5\Engine\Build\BatchFiles\RunUAT.bat"

& $UATPath BuildCookRun `
    -project="C:\unreal-3dgs\UnrealSplatPlugin.uproject" `
    -noP4 `
    -platform=Android `
    -clientconfig=Development `
    -cook `
    -allmaps `
    -build `
    -stage `
    -pak `
    -archive `
    -archivedirectory="C:\unreal-3dgs\Builds\Quest"
```

---

## 13. Instalar el APK en el Quest

### 13.1 Con ADB (método más rápido)
Conecta el Quest por USB y ejecuta:
```powershell
# Instalar (la primera vez)
adb install "C:\unreal-3dgs\Builds\Quest\UnrealSplatPlugin-arm64.apk"

# Actualizar una versión ya instalada
adb install -r "C:\unreal-3dgs\Builds\Quest\UnrealSplatPlugin-arm64.apk"
```

Lanza la app directamente desde la PC:
```powershell
adb shell monkey -p com.tuempresa.unrealsplat 1
```

### 13.2 Con Meta Quest Developer Hub (MQDH) (método gráfico)
1. Abre **Meta Quest Developer Hub**.
2. Conecta el Quest por USB — debe aparecer en la lista de dispositivos.
3. Ve a la pestaña **My Apps → Device Apps**.
4. Arrastra el `.apk` sobre la lista, o usa el botón **Install APK**.
5. La app aparecerá en el Quest bajo **Apps → Unknown Sources**.

### 13.3 Encontrar la app en el Quest
1. Pon el headset.
2. Ve al **menú de aplicaciones** (botón App en el controlador).
3. En la esquina superior derecha, filtra por **Unknown Sources**.
4. Busca **"UnrealSplat"** y ejecútala.

---

## 14. Actualizar la app (flujo diario)

Una vez tienes el entorno configurado, el flujo de trabajo diario es:

```
1. Hacer cambios en el Editor (nivel, actores, etc.)
2. Platforms → Android → Package Project
3. adb install -r <ruta al apk>
4. adb shell monkey -p <package_name> 1
```

Para iterar más rápido usa el **Android File Server (AFS)** que ya viene configurado:
- El AFS permite enviar assets actualizados al Quest sin recompilar el APK completo.
- Actívalo en **Project Settings → Plugins → Android File Server**.

---

## 15. Solución de problemas frecuentes

### ❌ `adb: command not found`
**Causa**: `adb` no está en el PATH.  
**Solución**: Añade esta carpeta al PATH de Windows:
```
C:\Users\TuUsuario\AppData\Local\Android\Sdk\platform-tools
```
(*Sistema → Variables de entorno → Path → Nueva*)

---

### ❌ `adb devices` muestra `unauthorized`
**Causa**: No aceptaste el diálogo USB dentro del headset.  
**Solución**: 
1. Pon el headset.
2. Acepta el diálogo "¿Permitir depuración USB desde este equipo?".
3. Marca "Recordar siempre".

---

### ❌ El packaging falla con "SDK license not accepted"
**Solución**:
```powershell
C:\Users\TuUsuario\AppData\Local\Android\Sdk\cmdline-tools\latest\bin\sdkmanager.bat --licenses
```
Escribe `y` y Enter para cada licencia.

---

### ❌ Pantalla negra al abrir la app en Quest
**Causas posibles**:
1. Ray Tracing sigue activo → Verifica `Config/Android/AndroidEngine.ini` tiene `r.RayTracing=False`.
2. Vulkan no seleccionado → **Project Settings → Android → Graphics API → Solo Vulkan**.
3. El nivel de mapa predeterminado no está configurado → **Project Settings → Maps & Modes → Default Maps**.

Revisa los logs en tiempo real:
```powershell
adb logcat -s UE5 UnrealSplat
```

---

### ❌ Splats no visibles (app arranca pero no se ve el modelo)
**Causas posibles**:
1. Los assets de texturas no fueron incluidos en el paquete.  
   → Verifica que las texturas del modelo estén en `Content/` (no en una ruta externa).
2. El actor Niagara no existe en el nivel empaquetado.  
   → Abre el nivel en el Editor y confirma que el actor de splats está colocado.
3. GPU particles desactivadas.  
   → En `Config/Android/AndroidEngine.ini` verifica `bAllowGPUParticles=True`.

---

### ❌ Build de Visual Studio falla: "error MSB3073"
**Solución**: Abre Visual Studio como Administrador y reintenta el build.

---

### ❌ `Generate Visual Studio project files` no aparece en el menú contextual
**Solución**: Asocia la extensión `.uproject` con el launcher de UE:
1. Clic derecho en `UnrealSplatPlugin.uproject` → **Propiedades**.
2. **Abrir con → Cambiar** → busca `UnrealVersionSelector.exe` en:
   ```
   C:\Program Files\Epic Games\Launcher\Engine\Binaries\Win64\UnrealVersionSelector.exe
   ```

---

## Flujo GitFlow para contribuir

Este proyecto sigue **GitFlow**. Nunca hagas commits directamente a `main`.

```
main          ← solo releases estables
  └─ develop  ← integración continua
       ├─ feature/xxx   ← nuevas funcionalidades
       ├─ bugfix/xxx    ← correcciones
       └─ hotfix/xxx    ← parches urgentes a main
```

Para añadir una nueva funcionalidad:
```powershell
git checkout develop
git pull origin develop
git checkout -b feature/nombre-de-tu-feature

# ... haz tus cambios ...

git add .
git commit -m "feat(scope): descripción del cambio"
git push origin feature/nombre-de-tu-feature
# Luego crea un Pull Request: feature/xxx → develop en GitHub
```

---

*Tutorial creado para el proyecto UnrealSplat — 3D Gaussian Splatting en Unreal Engine 5.5*  
*Repositorio: https://github.com/Cristian-David-Araujo/unreal-3dgs*
