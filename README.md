# PARKOUR GAME — TECHNICAL REVIEW REPORT

**Project:** Parkour-Game  
**Engine:** Unity  
**Review Type:** Codebase and Gameplay Logic Technical Review  
**Repository:** Ristwak/Parkour-Game  
**Review Scope:** C# Scripts, Gameplay Systems, Project Structure and Dependencies

---

## 1. Project Overview

Parkour Game is a Unity-based 3D parkour project focused on character movement, obstacle traversal, climbing, ledge interaction and animation-driven parkour actions.

The project contains a gameplay logic layer implemented through C# scripts, supported by Unity project configuration, packages, animation systems, scene objects, prefabs and other required assets.

This review focuses on the code and gameplay logic rather than the complete visual or playable build.

**Important scope clarification:** The repository's gameplay code is only one part of the complete game. Prefabs, animations, models, textures, materials, scenes and Inspector configurations may contain essential dependencies required for the scripts to function correctly.

## 2. Repository Structure

The GitHub repository screenshots show the following structure:

### Root Directory

- `.vscode` — Editor-related configuration.
- `Packages` — Unity package configuration and dependencies.
- `ProjectSettings` — Unity project configuration.
- `Scripts` — Gameplay and controller scripts.
- `.gitignore` — Git exclusion rules.
- `README.md` — Project documentation and sample video reference.

### Scripts Directory

The visible script folders are:

| Folder | Responsibility |
|---|---|
| CameraController | Camera behaviour and camera-related control |
| ClimbingSystem | Climbing points, ledge movement and climbing logic |
| Parkour | Player movement, environment detection and parkour control |
| ParkourActions | Parkour action definitions and animation-related behaviour |
| PlayerController | Player-related controller functionality |

These responsibilities are inferred from folder names and the supplied scripts. The complete contents of every folder were not independently inspected.

## Repository Scope and Asset Exclusion

The GitHub repository is intended to contain the core gameplay code, scripts, project configuration and supporting technical files rather than the complete game asset collection.

To keep repository size manageable, large Unity assets and resource files, such as 3D models, textures, animations, audio, environments and other heavy project resources, are not included in the shared repository.

The repository primarily focuses on the game's core programming logic and technical implementation.

**Important:** The complete Unity project requires the corresponding assets and resources to be available separately. The repository alone should not be considered a fully self-contained, ready-to-run Unity project.

## 3. Technical Review Scope

The following eight C# scripts were supplied and reviewed:

| Script | System |
|---|---|
| MainCameraController.cs | Camera Controller |
| ClimbingPoint.cs | Climbing System |
| ClimbingController.cs | Climbing System |
| NewParkourAction.cs | Parkour Actions |
| ParkourControllerScript.cs | Parkour System |
| ControlFallingMovement.cs | Player Animation / Falling Control |
| EnvironmentChecker.cs | Environment Detection |
| PlayerScript.cs | Player Movement and Character Control |

The review evaluates the supplied source code, its logical dependencies and potential runtime risks.

## 4. Gameplay Systems Analysis

### 4.1 Camera Controller

**Main script:** `MainCameraController.cs`

Responsibilities:
- Reads the Cinemachine virtual camera's final orientation.
- Extracts the camera's Y-axis rotation.
- Exposes a horizontal rotation quaternion for player movement.

Technical observations:
- The virtual camera reference requires a valid Inspector assignment.
- The original implementation calculated a rounded rotation value that was not subsequently used.
- Camera behaviour also depends on the Cinemachine configuration and scene setup.

### 4.2 Climbing System

**Main scripts:**
- `ClimbingPoint.cs`
- `ClimbingController.cs`

Responsibilities:
- Defines climbing points and their neighbouring connections.
- Supports one-way and two-way connections.
- Handles ledge-to-ledge movement.
- Supports climbing up, climbing down, jumping and shimmy movements.
- Uses animation-based hand positioning.

Technical observations:
- Climbing connections depend on correctly configured point references and direction values.
- Null or missing climbing-point references can cause runtime errors.
- Two-way connections depend on initialization order.
- Hand-position calculations must preserve the configured offsets.
- Climbing animations and point transforms must remain correctly aligned.

### 4.3 Parkour Action System

**Main scripts:**
- `NewParkourAction.cs`
- `ParkourControllerScript.cs`

Responsibilities:
- Defines parkour actions through ScriptableObject assets.
- Checks obstacle tags and height constraints.
- Selects suitable actions based on obstacle information.
- Handles animation transitions and target matching.
- Supports ledge-related parkour actions.

Technical observations:
- Parkour actions depend on correctly configured ScriptableObject references.
- Animation state names must match the Animator Controller.
- Missing obstacle-height information can produce invalid action selection.
- Target matching depends on valid animation timing and body-part configuration.

### 4.4 Environment Detection

**Main script:** `EnvironmentChecker.cs`

Responsibilities:
- Detects obstacles through raycasts.
- Performs obstacle-height checks.
- Detects ledges and their surface information.
- Checks climbing surfaces through multiple raycasts.
- Detects suitable climbing points when dropping from a ledge.

Technical observations:
- Detection accuracy depends on configured LayerMasks.
- Ray lengths and origin offsets influence gameplay behaviour.
- Excessive raycasts can affect performance when used frequently.
- Detection results must be validated before being used by other gameplay systems.

### 4.5 Player Controller and Movement

**Main script:** `PlayerScript.cs`

Responsibilities:
- Handles horizontal movement and rotation.
- Applies gravity and falling movement.
- Checks whether the player is standing on a valid surface.
- Handles ledge movement.
- Controls parkour animation execution.
- Manages CharacterController activation and player-control states.
- Supports Animator target matching.

Technical observations:
- The script depends on a valid CharacterController, Animator, EnvironmentChecker and MainCameraController.
- Movement, rotation and gravity logic are connected to environment detection.
- Incorrect control-state transitions can interrupt gameplay actions.
- Animator parameters and animation state names must remain consistent.
- CharacterController activation must be managed carefully during climbing and parkour actions.

### 4.6 Falling Animation Control

**Main script:** `ControlFallingMovement.cs`

Responsibilities:
- Disables player control when entering the configured falling animation state.
- Restores player control when exiting that state.

Technical observations:
- The StateMachineBehaviour depends on the Animator being attached to the correct player.
- Incorrect state transitions may leave player control disabled or restore it prematurely.

## 5. Technical Findings and Risk Assessment

| Priority | Area | Identified concern |
|---|---|---|
| Critical | Player Controller | Control-state and CharacterController transitions require verification |
| High | Climbing System | Climbing-point initialization and null references |
| High | Parkour Actions | Obstacle validation and animation matching |
| High | Environment Detection | LayerMask and raycast configuration |
| Medium | Camera Controller | Virtual camera reference validation |
| Medium | Performance | Repeated raycasts and unnecessary runtime logging |
| Medium | Animation System | Animator state and parameter consistency |

These are code-review findings and potential risks, not confirmation that every issue occurs in the running game.

## 6. Asset, Prefab and Scene Dependencies

The project should be understood as a combination of gameplay code and Unity assets.

The supplied scripts indicate dependencies on:

- Cinemachine virtual camera.
- Unity CharacterController.
- Animator Controller and animation clips.
- Parkour action ScriptableObjects.
- Climbing-point GameObjects and their transforms.
- Environment colliders and LayerMasks.
- Player GameObject component references.
- Scene and prefab configuration.

The repository screenshots show the project folders but do not expose the internal contents or complete configuration of the relevant assets and prefabs.

Consequently, the review does not claim that prefab references, animation assignments, scene wiring or asset configurations have been verified.

## 7. Optimization and Maintenance Recommendations

1. Remove unnecessary debug logging from frequently executed gameplay methods.
2. Validate serialized references before using them.
3. Avoid repeated component lookups where cached references are appropriate.
4. Validate obstacle and climbing detection results before triggering actions.
5. Preserve animation names, serialized fields and existing Inspector references during code maintenance.
6. Avoid changing movement values, climbing offsets and animation timings without gameplay verification.
7. Review raycast frequency and detection distances before making performance-related changes.
8. Keep gameplay logic modular and maintain consistent responsibilities across the existing script folders.

## 8. Testing and Verification Status

| Verification Area | Status |
|---|---|
| Supplied C# source review | Completed |
| Visible GitHub structure review | Completed |
| Gameplay logic analysis | Completed |
| Prefab configuration inspection | Not performed |
| Scene configuration inspection | Not performed |
| Animator and animation verification | Not performed |
| Asset dependency verification | Not performed |
| Unity compilation verification | Pending |
| Runtime gameplay testing | Pending |
| Performance profiling | Pending |

The complete Unity project and its required assets were not available for this review. Therefore, compilation, prefab integration, animation behaviour and runtime gameplay cannot be certified.

## 9. Overall Technical Assessment

The project has a modular gameplay-code structure covering camera control, player movement, environmental detection, climbing and animation-driven parkour actions.

The supplied scripts establish the core gameplay logic, while the complete experience depends on supporting Unity assets, prefabs, scenes, animation configurations and component references.

The primary maintenance priorities are reliable component references, correct control-state transitions, climbing-point configuration, obstacle validation and animation consistency.

**Final Assessment:** The supplied gameplay logic has undergone a static technical review. The codebase can be maintained and optimized independently, but the complete playable game cannot be considered runtime-verified without the original Unity assets, prefab configurations, scenes and testing environment.

**QA Status:** Runtime QA not performed. This limitation is explicitly documented and must not be interpreted as a successful gameplay test.
