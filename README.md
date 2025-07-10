# Azure Function MCP Server Template

A standardized template for building Azure Functions with MCP (Model Context Protocol) server capabilities in .NET 8.

## Features

- **Dual-mode operation**: Run as Azure Function or standalone MCP server
- **Azure OpenAI integration**: Built-in adapters for GPT-4 and GPT-3.5 models
- **Clean architecture**: Separated Core, Services, Functions, and Tests projects
- **MCP Protocol implementation**: Complete JSON-RPC based MCP server
- **HTTP resilience**: Built-in retry and circuit breaker policies
- **Comprehensive testing**: Unit and integration test infrastructure
- **Configuration management**: Environment-based configuration with Options pattern
- **Logging**: Structured logging with console and Application Insights support

## Quick Start

See the full documentation in the repository for detailed instructions on converting this template to your own MCP application.

## Repository Structure

```
AzureFunctionMCPTemplate/
├── src/
│   ├── Template.Core/           # Domain models and interfaces
│   ├── Template.Functions/      # Azure Functions and MCP server
│   ├── Template.Services/       # Business logic and external clients
│   └── Template.Tests/          # Unit and integration tests
├── scripts/                     # Utility scripts
├── docs/                        # Additional documentation
└── Template.sln                 # Solution file
```

## AI Model Support

- **Claude** via MCP Protocol
- **Azure OpenAI** (GPT-4, GPT-3.5) via Function Calling
- **Generic HTTP endpoints** for any AI service

For detailed instructions, see the complete README and documentation in the repository.