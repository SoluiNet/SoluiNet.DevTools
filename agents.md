# SoluiNet.DevTools - AI Agent Context

## Project Overview

SoluiNet.DevTools is a comprehensive, extensible developer toolkit built on .NET that provides a plugin-based architecture to support developers in their daily tasks. The application serves as a centralized platform for various development utilities, tools, and integrations.

## Architecture

### Core Components

- **SoluiNet.DevTools.Core**: The foundational library containing base interfaces, plugin system, and core functionality
- **SoluiNet.DevTools.UI**: Main WPF-based user interface application
- **SoluiNet.DevTools.UI.Blazor**: Web-based Blazor interface for cross-platform access
- **SoluiNet.DevTools.Console**: Command-line interface for automation and scripting
- **SoluiNet.DevTools.Web**: Web server component for hosting web-based functionality

### Plugin System

The application is built around a robust plugin architecture with the following key interfaces:

- `IBasePlugin`: Base interface for all plugins providing the `Name` property
- `IUtilitiesDevPlugin`: Interface for utility plugins with menu integration
- `ISmartHomePlugin`: Specialized interface for smart home integrations
- `IManagementPlugin`: Interface for management and administrative tools
- `ICommunicationSender/Receiver`: Interfaces for communication plugins
- `IDataObserver/Observable`: Interfaces for data monitoring and observation
- `ISupportsStorage`: Interface for plugins requiring data persistence

### Plugin Categories

1. **Utilities** (`SoluiNet.DevTools.Utils.*`):
   - Crypto tools for encryption/hashing
   - File manipulation utilities
   - JSON processing tools
   - Certificate management
   - Git/TFS/SVN integration
   - Time tracking
   - Machine learning tools
   - Web client utilities

2. **Communication** (`SoluiNet.DevTools.Communication.*`):
   - Email integration
   - Telegram bot functionality

3. **Data Exchange** (`SoluiNet.DevTools.DataExchange.*`):
   - Jira integration
   - Various API connectors

4. **Smart Home** (`SoluiNet.DevTools.SmartHome.*`):
   - Busch-Jaeger Free@Home integration
   - Senec energy system integration

5. **Management** (`SoluiNet.DevTools.Management.*`):
   - Financial management tools
   - Project management utilities

6. **Transform** (`SoluiNet.DevTools.Transform.*`):
   - UML diagram transformations
   - ER diagram generation

## Development Guidelines

### Plugin Development

When creating new plugins:

1. **Implement Base Interface**: All plugins must implement `IBasePlugin` at minimum
2. **Choose Appropriate Category**: Implement additional interfaces based on plugin functionality
3. **Follow Naming Convention**: Use `SoluiNet.DevTools.[Category].[PluginName]` namespace
4. **Provide User Control**: WPF plugins should include a corresponding UserControl for UI
5. **Support Multiple Platforms**: Consider Blazor components for web compatibility

### Code Standards

- Follow Microsoft C# coding conventions
- Use StyleCop for code analysis (stylecop.json files present in projects)
- Implement proper error handling and logging using NLog
- Include comprehensive XML documentation
- Follow dependency injection patterns where applicable

### Configuration

- Use `App.config` or `appsettings.json` for configuration
- Support both development and production configurations
- Implement `IContainsSettings` interface for plugins requiring configuration

## Key Technologies

- **.NET Framework/Core**: Primary development platform
- **WPF**: Desktop user interface framework
- **Blazor**: Web-based user interface
- **Entity Framework**: Data access layer
- **NLog**: Logging framework
- **StyleCop**: Code analysis and standards enforcement

## Build and Deployment

- **Solution File**: `SoluiNet.DevTools.sln` contains all projects
- **Build Configurations**: Debug and Release configurations for multiple platforms
- **Installation**: WiX-based installer project (`SoluiNet.DevTools.Install`)
- **Mobile Support**: Xamarin projects for Android and iOS

## Extension Points

The application provides multiple extension points for AI agents:

1. **Plugin Creation**: Develop new plugins for specific developer tasks
2. **Data Integration**: Connect to external APIs and services
3. **Automation**: Create background tasks and scheduled operations
4. **UI Enhancement**: Add new user interface components
5. **Communication**: Integrate with messaging platforms and notification systems

## Common Use Cases

- **Code Generation**: Automated code scaffolding and template generation
- **API Integration**: Connecting to external services and APIs
- **Data Processing**: File manipulation, transformation, and analysis
- **Development Workflow**: Git operations, build automation, deployment
- **Monitoring**: System monitoring, log analysis, performance tracking
- **Communication**: Team notifications, status updates, alerts

## Getting Started for AI Agents

When working with SoluiNet.DevTools:

1. **Understand the Plugin Architecture**: Review existing plugins to understand patterns
2. **Identify Extension Points**: Determine which interfaces and base classes to implement
3. **Follow Established Patterns**: Use existing plugins as templates for new functionality
4. **Consider Cross-Platform Support**: Plan for both WPF and Blazor implementations
5. **Implement Proper Error Handling**: Use the established logging and error handling patterns
6. **Test Integration**: Ensure new components integrate properly with the main application

This context should help AI agents understand the structure, patterns, and extension points available in the SoluiNet.DevTools ecosystem for effective development and enhancement of the platform.