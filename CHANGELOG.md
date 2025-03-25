# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/), and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [1.1.0] - 2025-03-25
### Added
- **Manual Player Addition**: Admins can now use the `.event add <playerName>` command to manually add a specific player to an event, improving event management flexibility.
- **Death Hooks**: Players who die during an event are now automatically removed from the event, streamlining gameplay and reducing manual oversight.
- **Unstuck Prevention**: Disabled the "Unstuck" feature for players during events to maintain fairness and prevent exploits.

## [1.0.1] - 2025-03-25
### Fixed
- **Plugin Loading Issue**: Corrected `MyPluginInfo` data to ensure the plugin loads properly on startup.

## [1.0.0] - 2025-03-24
### Added
- **Initial Release**: First version of the project, including core functionality for event management and plugin operation.