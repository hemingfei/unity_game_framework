# CHANGELOG

All notable changes to this package will be documented in this file.

## [1.1.8] - 2023-08-22

### Changed

- Auto Build Tool's Output Dir Changed (has the project's name)

## [1.1.7] - 2023-08-04

### Changed

- Auto Build Tool Will Not Delete The Build Dir, instead, will delete the sub platform dir

## [1.1.6] - 2023-08-04

### Changed

- Custom Web Request's Auto Generated Script Editor Tool Use Its Own Event Return Data Instead of The Global Newest Updated Data
  
## [1.1.5] - 2023-08-03

### Changed

- Custom Web Request's Auto Generated Script Editor Tool Use SerialId to determine if the callback is the specific one

## [1.1.4] - 2023-08-02

### Changed

- Custom Web Request Support Both Encrypt Or Not Encrypt ResultData

## [1.1.3] - 2023-08-02

### Added

- Log of Custom Web Request Error

## [1.1.2] - 2023-07-14

### Added

- Log of Decrypt Result Data of Custom Web Request

## [1.1.1] - 2023-07-12

### Fixed

- Custom Web Request Remove Encrypt Body
- Custom Web Request ResultData's Type Are Correct Now
  
## [1.1.0] - 2023-07-10

### Changed

- Custom Web Request Support Encrypt Body and Decript ResultData

## [1.0.28] - 2023-06-20

### Changed

- Support UNITY 2022.3

## [1.0.27] - 2023-06-20

### Added

- Resource Component Can Get Package Version

### Changed

- YooAsset Resource Helper no longer need to new IQueryServices when Initialize, will use default inside GameQueryService

## [1.0.26] - 2023-06-20

### Changed

- Easy Build Tool Params Settings will be set in config file instead of write in code

### Fixed

- BuildTool's compress process may pause and cannot finish in windows editor because of of max length of path

## [1.0.25] - 2023-06-20

### Fixed

- BuildTool's compress process may pause and cannot finish

## [1.0.24] - 2023-06-20

### Added

- Resource Component add 'destroy pacakge' and 'destroy resource system' methods

### Fixed

- BuildTool's compress process may pause and cannot finish

## [1.0.23] - 2023-06-19

### Changed

- BuildTool's build params are now from the AssetBundleBuilderSettingData.Setting
- BuildTool add new build param 'AutoAnalyzeRedundancy' and 'SBPParameters'

## [1.0.22] - 2023-06-16

### Added

- Debugger Can Set Custom Title

## [1.0.21] - 2023-06-15

### Fixed

- Easy Build in Android Created usless Folder
- Web Request Events cast null params

## [1.0.20] - 2023-06-08

### Added

- Easy Build Tool

## [1.0.19] - 2023-06-08

### Added

- Sprite Atlas User Config Can Show Sprites Size Details
- DataTable Add Open Excel Folder Menu

### Changed

- GF Menu Name Changed To Tools
- RaycastTargetChecker and ReplaceFont Back

## [1.0.18] - 2023-06-08

### Added

- Sprite Atlas Config Tools

## [1.0.17] - 2023-06-02

### Fixed

- UniTask Timer Helper infinit repeat count not work

## [1.0.16] - 2023-06-01

### Changed

- YooAsset Resource Helper Support Control Specific Package
- YooAsset Update Package Version Not Append TimeTicks After URL

## [1.0.15] - 2023-05-29

### Changed

- YooAsset Init Status

## [1.0.14] - 2023-05-26

### Added

- UI Show/HideOthers now can ignore some custom uis

## [1.0.13] - 2023-05-22

### Added

- Http Web Request Editor Auto Generate Code
  
## [1.0.12] - 2023-05-22

### Changed

- Scene Component add handle_loadscene callback

### Fixed

- ui anim compile error

## [1.0.11] - 2023-05-21

### Changed

- Custom WebRequest changed name to Http WebRequest
- Format and Clean Code
- Resource Component Load Gameobject Async has callback function

## [1.0.10] - 2023-05-17

### Added

- Custom WebRequest Component and Custom WebRequest Agent Helper
- Litjson Helper
- DataNode Component Add TryGetData Method

### Fixed

- Unitask Timer Helper Cancel Error

## [1.0.9] - 2023-05-15

### Changed

- Sound Component Open the API of Turn On/Off Sound Listener
- WWWFormInfo Class Now is Public

## [1.0.8] - 2023-05-14

### Added

- Resource Component Load Scene Support Action Callback
- Debugger Component Support Register Self Methods

### Changed

- Debugger Component Default Position Changed

## [1.0.7] - 2023-05-13

### Changed

- Resource Download Progress Event Name Changed
- Resource Update Manifest Evenet Name Changed

## [1.0.6] - 2023-05-12

### Added

- ResourceComponent support clear downloaded cache
- ResourceComponent support unload unused assets

## [1.0.5] - 2023-05-12

### Added

- ResourceComponent support Update Patch

## [1.0.4] - 2023-05-12

### Removed

- Useless Editor Menus

## [1.0.3] - 2023-05-11

### Added

- SceneComponent

## [1.0.2] - 2023-05-11

### Added

- TimerComponent
- ResourceComponent Support Load Scene 

## [1.0.1] - 2023-05-11

### Added

- CustomGameFramework

## [1.0.0] - 2023-05-10

### Added

- GameFramework & UnityGamework
