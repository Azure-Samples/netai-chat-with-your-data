# Chat with your Data - .NET Aspire Application

Always reference these instructions first and fallback to search or bash commands only when you encounter unexpected information that does not match the info here.

## Working Effectively

### Bootstrap, Build, and Test the Repository

**Install Prerequisites**:
- Install .NET 9 SDK:
  ```bash
  curl -sSL https://dot.net/v1/dotnet-install.sh | bash /dev/stdin --version latest --channel 9.0 --install-dir ~/.dotnet
  export PATH="$HOME/.dotnet:$PATH"
  ```
- Install .NET Aspire workload:
  ```bash
  dotnet workload install aspire
  ```
  **NEVER CANCEL** - takes 2-5 minutes. Set timeout to 10+ minutes.
- Verify Docker is available: `docker --version`

**Build Process**:
- Navigate to src directory: `cd ./src`
- Restore packages: `dotnet restore Chat-With-Y-Data.sln`
  **NEVER CANCEL** - takes 60-90 seconds. Set timeout to 3+ minutes.
- Build solution: `dotnet build Chat-With-Y-Data.sln --no-restore`
  **NEVER CANCEL** - takes 15-25 seconds. Set timeout to 2+ minutes.

**Run the Application**:
- Navigate to AppHost: `cd ./src/ChatWYData.AppHost`
- Run application: `dotnet run`
  **NEVER CANCEL** - startup takes 5-10 minutes. Set timeout to 15+ minutes.
  **NOTE**: First run will download Docker images for SQL Server and Azure Storage emulator.

### Azure Developer CLI Deployment

**Install azd** (if needed for Azure deployment):
```bash
curl -fSL https://aka.ms/install-azd.sh | bash
```
**NEVER CANCEL** - installation may take 2-5 minutes.

**Deploy to Azure**:
- Login: `azd auth login`
- Deploy: `azd up`
  **NEVER CANCEL** - deployment takes 15-30 minutes. Set timeout to 45+ minutes.

## Validation

### Manual Testing Scenarios

**ALWAYS run these validation steps after making changes**:

1. **Build Validation**:
   - Run `dotnet build Chat-With-Y-Data.sln` from `./src` directory
   - Verify build succeeds with warnings (warnings are normal)

2. **Individual Service Testing**:
   - Test MarkdownBashApi: `cd ./src/ChatWYData.MarkDown.MarkitdownBashApi && dotnet run --urls http://localhost:5001`
   - Service should start and show "Python and dependencies installed successfully"
   - Press Ctrl+C to stop after verification

3. **Full Application Testing** (when Azure services configured):
   - Run `dotnet run` from `./src/ChatWYData.AppHost`
   - Wait for all services to start (may take 10+ minutes)
   - Access Aspire Dashboard (URL shown in console output)
   - Verify Chat App and Document Manager URLs are accessible

### Critical Dependencies and Timing

**Python Dependencies**:
- Python 3.12+ is auto-installed on Linux during service startup
- `markitdown` package is installed via `pipx`
- Some apt commands may fail due to permissions (this is expected and normal)
- Verify with: `which markitdown && markitdown --help`

**Docker Requirements**:
- SQL Server container for local database
- Azure Storage emulator for blob storage
- Services automatically pull required images on first run

**Build Times** (add 50% buffer for timeouts):
- Package restore: ~70 seconds
- Full solution build: ~20 seconds  
- Service startup: ~5-10 minutes
- Azure deployment: ~20-30 minutes

## Architecture Overview

### Project Structure
- **Main Solution**: `./src/Chat-With-Y-Data.sln`
- **AppHost**: `./src/ChatWYData.AppHost` - .NET Aspire orchestration
- **UI Applications**:
  - ChatApp: Chat interface with document Q&A
  - DocsMngr: Document management interface
- **Microservices**:
  - DocumentsApi: Main document processing API
  - VectorStoreAzureAISearch: Vector search using Azure AI Search
  - DescriptionApi: Document description generation
  - MarkdownApi: Document conversion to Markdown
  - MarkitdownBashApi: Python-based markdown conversion
  - MarkitdownCSnakes: Alternative Python integration
- **Background Services**:
  - DocProcessor: Document processing worker

### Development vs Production Configuration

**Local Development Mode** (default):
```csharp
// Uses local SQL Server and Azure Storage emulator
var sqldb = builder.AddSqlServer("sql").WithDataVolume().AddDatabase("sqldb");
var storage = builder.AddAzureStorage("storage").RunAsEmulator();
```

**Production Mode** (when `IsPublishMode` is true):
```csharp
// Uses Azure OpenAI, Azure AI Search, Application Insights
var aoai = builder.AddAzureOpenAI("openai");
var azureaisearch = builder.AddAzureSearch("azureaisearch");
```

### Key Configuration Files
- `./src/ChatWYData.AppHost/Program.cs` - Service orchestration
- `./src/ChatWYData.AppHost/appsettings.json` - Azure credentials config
- `./azure.yaml` - Azure deployment configuration

## Common Tasks

### Working with Existing Azure Services

To use existing Azure OpenAI and Azure AI Search services:

1. **Configure User Secrets** for API projects:
   ```bash
   cd src/ChatWYData.DescriptionApi
   dotnet user-secrets set "ConnectionStrings:openai" "Endpoint=https://<endpoint>.openai.azure.com/;Key=<key>;"
   dotnet user-secrets set "ConnectionStrings:azureaisearch" "Endpoint=https://<endpoint>.search.windows.net/;Key=<key>;"
   
   cd ../ChatWYData.VectorStoreAzureAISearch
   dotnet user-secrets set "ConnectionStrings:openai" "Endpoint=https://<endpoint>.openai.azure.com/;Key=<key>;"
   dotnet user-secrets set "ConnectionStrings:azureaisearch" "Endpoint=https://<endpoint>.search.windows.net/;Key=<key>;"
   ```

2. **Modify AppHost Configuration**:
   - Edit `./src/ChatWYData.AppHost/Program.cs`
   - Comment production Azure service creation lines
   - Uncomment local development service lines

### Troubleshooting Common Issues

**Build Failures**:
- Ensure .NET 9 SDK is installed: `dotnet --version` should show 9.x.x
- Install Aspire workload: `dotnet workload install aspire`
- Check all project references are valid

**Startup Failures**:
- Verify Docker is running: `docker ps`
- Check Azure CLI authentication: `az account show`
- Ensure required ports are available (15295, 17104, 19009, etc.)

**Python Service Issues**:
- Services auto-install Python dependencies on Linux
- Windows users: Python dependencies are skipped (normal behavior)
- Verify markitdown is available: `which markitdown`

### File Locations

**Frequently Modified Files**:
- Service orchestration: `./src/ChatWYData.AppHost/Program.cs`
- Azure configuration: `./src/ChatWYData.AppHost/appsettings.json`
- Deployment: `./azure.yaml`
- Documentation: `./README.md`, `./docs/SolutionOverview.md`

**Build Artifacts**:
- Solution builds to: `./src/*/bin/Debug/net9.0/`
- Docker containers managed by Aspire automatically

**Testing Files**:
- Sample documents: `./sample_docs/`
- Configuration templates: `./src/ChatWYData.AppHost/next-steps.md`

### Useful Commands Reference

```bash
# Quick build verification
cd ./src && dotnet build Chat-With-Y-Data.sln

# Individual service test
cd ./src/ChatWYData.MarkDown.MarkitdownBashApi && timeout 30 dotnet run --urls http://localhost:5001

# Full application (with Azure services)
cd ./src/ChatWYData.AppHost && dotnet run

# Azure deployment
azd up

# Check service status
docker ps
az account show
```

## Critical Reminders

- **NEVER CANCEL** long-running operations (builds, deployments, service startup)
- **ALWAYS TEST** build and basic service startup after making changes
- **VALIDATE** that both Chat and Document Manager interfaces are functional
- **VERIFY** Python dependencies are working with individual service tests
- **SET APPROPRIATE TIMEOUTS** - builds 3+ min, deployments 45+ min, startup 15+ min