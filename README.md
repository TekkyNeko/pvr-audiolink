# PVR AudioLink

## A repository of audio reactive prefabs for Unity, written in CSharp and HLSL, compatible with PoligonVR

### This project only works on PVR Worlds CCK 0.3.3 and above

AudioLink is a system that analyzes and processes in-world audio into many different highly reactive data streams and exposes the data to Scripts and Shaders. 

The per-frequency audio amplitude data is first read briefly into Udon using Unity's GetOutputData. It is then sent to the GPU for signal processing and buffered into a CustomRenderTexture. Then, the CustomRenderTexture is broadcast globally (called `_AudioTexture`) which can be picked up by shaders both in-world and across all avatars. 

### [Frequently Asked Questions](FAQ.md)
### [Documentation for shader creators](https://github.com/llealloo/vrc-udon-audio-link/tree/master/Docs)

## 2.0.1-2.0.2 - September 11th, 2024
### Bugfixes
- Fix the AudioLinkController pickups and object sync as i accidentally reverted it before release
- Fixed the Audio Link Controller UI where it wasnt Single Pass Stereo Instanced (rendered in one eye).

## 2.0.0 - September 8th, 2024
### New features
- Added the ability to adjust how the AudioLink controller is synced. You can sync every part of it, none of it, or everything except the gain and power controls. (fundale)
- Added support for dual mono audio sources, for cases where you want to supply the left and right channel from separate sources. (fundale)
- Added utility scripts for driving PostProcessing and blend shapes with AudioLink. These are called AudioReactivePostProcessing and AudioReactiveBlendshapes respectively. Just add them to a GameObject that has a PostProcessing component or SkinnedMeshRenderer to use. (fundale)

### Changes
- Lowered the default volume for the AudioLink avatar prefab a bit. (pema)
- Reduces the network traffic incurred by syncing AudioLink state a bit. (Happyrobot33)
- Deprecated `ThemeColorController.customThemeColors` as the behavior has changed. Please use `ThemeColorController.SetCustomThemeColors` and `ThemeColorController.GetCustomThemeColors` instead. This is a (minor) breaking change.

### Bugfixes
- Fixed a bug where the color chord theme color toggle on the controller wasn't properly synced. (pema)

## Updating projects from version 0.2.8 or lower? (...first time setup? please see next section)
1. Before upgrading your project, **MAKE A BACKUP**! The latest version of AudioLink changes many things - better safe than sorry.
2. Take note of which AudioSource you are using to feed AudioLink, this reference may be lost during upgrade.
3. Open the Projects tab and select your project.
4. On the right side, find the AudioLink package and add it. If it doesn't show up, make sure you have the "Curated" toggle enabled in the top-right drop-down.
5. In a file browser, **without Unity open**, navigate to your projects Assets folder and delete the "AudioLink" folder and "AudioLink.meta" file.
6. Open the Project in Unity.
7. You may be prompted by the AudioLink shader upgrader to upgrade old shaders. You should do so if your project uses any custom AudioLink-enabled shaders.
8. If you were using assets from the AudioLink example scene, you'll have to import it, as it isn't imported by default. To do so, use the "AudioLink -> Open AudioLink Example Scene" in top menu of the editor.
9. If you were using AudioReactiveObject or AudioReactiveLight
   components, you may need to manually re-enable the "Audio Data" under AudioLink "experimental" settings. This feature is now considered experimental.
10. In scene(s) containing old versions of AudioLink:
   - Delete both AudioLink and AudioLinkController prefabs from the scene.
   - Re-add AudioLink and AudioLinkController to the scene by dragging the prefabs from the Packages/com.llealloo.audiolink/Runtime folder.
   - Click the "Link all sound reactive objects to this AudioLink" button on AudioLink inspector panel.
   - Drag the AudioSource you were using previously into the AudioLink audio source parameter.
      - NOTE: If you previously used AudioLinkInput, you are welcome to continue doing so, however now in 0.2.5+ AudioLink is much smarter about inputs. Try dragging it straight into the AudioLink / audio source parameter!

## Upgrading avatar projects
1. In a file browser, delete the "Assets/AudioLink" folder and the "AudioLink.meta" file.
2. Follow the "First time setup" steps for avatar projects described below.

## Installation
1. Download and Import the latest **UnityPackage** PVR AudioLink Release at https://github.com/TekkyNeko/pvr-audiolink/releases.
2. Use the "Tools/AudioLink/Add AudioLink Prefab to Scene" menu item.
3. Save the scene. If that doesn't work, restart Unity, theres a bug with namespace references until you save or restart. Afterwards, it should work out of the box.

## Getting started
After installation, to use AudioLink:
1. If you want to view the example scene, use the "AudioLink/Open AudioLink Example Scene" button in the top menu of the editor or use the "Tools/AudioLink/Add AudioLink Prefab to Scene" menu item.

### For PVR Avatar Testing
2. Under AudioLinkAvatar/AudioLinkInput, add a music track to the AudioClip in the AudioSource.
3. Enter playmode to test your avatar.

### For PVR Worlds and other usecases 
2. Click the "Link all sound reactive objects..." button on the AudioLink MonoBehaviour to link everything up.

## Compatible tools / assets
- [Silent Cel Shading Shader](https://gitlab.com/s-ilent/SCSS) by Silent
- [Mochies Unity Shaders](https://github.com/MochiesCode/Mochies-Unity-Shaders/releases) by Mochie
- [Fire Lite](https://discord.gg/24W435s) by Rollthered
- [Poiyomi Shader](https://poiyomi.com/) by Poiyomi
- [orels1 AudioLink Shader](https://github.com/orels1/orels1-AudioLink-Shader) by orels1
- [ShaderForge-AudioLink](https://github.com/lethanan/ShaderForge-AudioLink) by lethanan

## Thank you
- phosphenolic for the math wizardry, conceptual programming, debugging, design help and emotional support!!!
- [cnlohr](https://github.con/cnlohr) for the help with the new DFT spectrogram and helping to port AudioLink to 100% shader code
- [lox9973](https://gitlab.com/lox9973) for autocorrelator functionality and the inspirational & tangential math help with signal processing
- [Texelsaur](https://github.com/jaquadro) for the AudioLinkMiniPlayer and support!
- [Pema](https://github.com/pema99) for the help with strengthening the codebase and inspiration!
- [3](https://github.com/float3) for joining the AudioLink team, helping maintain the codebase, and being instrumental in getting version 0.3.0 out.
- [Merlin](https://github.com/merlinvr) for making UdonSharp and offering many many pointers along the way. Thank you Merlin!
- [Orels1](https://github.com/orels1) for all of the great help with MaterialPropertyBlocks & shaders and the auto configurator script for easy AV3 local testing
- [Xiexe](https://github.com/Xiexe/) for the help developing and testing
- [Thryrallo](https://github.com/thryrallo) for the help setting up local AV3 testing functionality
- [CyanLaser](https://github.com/CyanLaser/) for making CyanEmu
- [Lyuma](https://github.com/lyuma/) for helping in many ways and being super nice!
- [ACIIL](https://github.com/aciil) for the named texture check in AudioLink.cginc
- [fuopy](https://github.com/fuopy) for being awesome and reflecting great vibes back into this project
- Colonel Cthulu for incepting the idea to make the audio data visible to avatars
- jackiepi for math wizardry, emotional support and inspiration
- Barry and OM3 for stoking my fire!
- [Lamp](https://soundcloud.com/lampdx) for the awesome example music and inspiration. Follow them!! https://soundcloud.com/lampdx
- [Shelter](https://sheltervr.club/), [Loner](https://loneronline.com/), [Rizumu](https://x.com/rizumuvr), and all of the other dance communities in VRChat for making this
- [rrazgriz](https://github.com/rrazgriz) for coming up with and implementing yt-dlp support for editor testing
- [LucHeart](https://github.com/lucheart) and [DomNomNom](https://github.com/DomNomNomVR) for maintaing CVR forks of AudioLink, and letting us adopt their work
- [Rollthered](https://linktr.ee/Rollthered) for providing us with music for demo purposes.
- [fundale](https://github.com/fundale/) for figuring out WebGL support
- [ImTiara](https://github.com/ImTiara) for making this PVR port possible!
- all other [contributors](https://github.com/llealloo/vrc-udon-audio-link/graphs/contributors) and our community for their help and support
