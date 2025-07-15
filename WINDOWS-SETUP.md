# Windows/Visual Studio Setup Guide

## Prerequisites for Windows

- **Visual Studio 2022** (Community, Professional, or Enterprise)
- **.NET 8.0 SDK** (included with Visual Studio 2022)
- **Git for Windows** (optional, for cloning)

## Setup Instructions

### 1. Clone or Download the Repository
```bash
git clone <repository-url>
# OR download and extract the ZIP file
```

### 2. Open in Visual Studio
1. Open **Visual Studio 2022**
2. Select **File > Open > Project or Solution**
3. Navigate to the `NumberToWords` folder
4. Open `NumberToWords.sln`

### 3. Restore Packages
Visual Studio should automatically restore NuGet packages. If not:
1. Right-click on the solution in **Solution Explorer**
2. Select **Restore NuGet Packages**

### 4. Set Startup Project
1. Right-click on **NumberToWords.Web** in Solution Explorer
2. Select **Set as Startup Project**

### 5. Build and Run
- Press **F5** or click **Debug > Start Debugging**
- Or press **Ctrl+F5** for **Start Without Debugging**

## Troubleshooting

### "DirectoryNotFoundException: wwwroot" Error
This has been fixed by adding proper wwwroot content. If you still encounter this:

1. **Ensure wwwroot folder exists**: 
   - Check that `src/NumberToWords.Web/wwwroot/` folder exists
   - It should contain: `css/`, `js/`, `favicon.ico`, and `.gitkeep`

2. **Clean and Rebuild**:
   - Go to **Build > Clean Solution**
   - Then **Build > Rebuild Solution**

3. **Check Project Properties**:
   - Right-click **NumberToWords.Web** > **Properties**
   - Ensure **Target Framework** is **.NET 8.0**

### Other Common Issues

**Package Restore Issues**:
```bash
# In Package Manager Console
Update-Package -Reinstall
```

**Port Already in Use**:
- Check `Properties/launchSettings.json`
- Change the port numbers if needed
- Default ports are 5203 (HTTP) and 7284 (HTTPS)

**Missing .NET 8.0**:
- Install .NET 8.0 SDK from: https://dotnet.microsoft.com/download
- Restart Visual Studio after installation

## Default URLs (Windows)

When running from Visual Studio:
- **HTTP**: http://localhost:5203
- **HTTPS**: https://localhost:7284  
- **Swagger**: https://localhost:7284/swagger

## Project Structure

```
NumberToWords/
├── NumberToWords.sln           # Solution file - open this
├── src/
│   ├── NumberToWords.Web/      # ← Set as startup project
│   ├── NumberToWords.Application/
│   ├── NumberToWords.Domain/
│   └── NumberToWords.Infrastructure/
└── tests/
    └── NumberToWords.Tests.Unit/
```

## Running Tests in Visual Studio

1. **Build** the solution first
2. Open **Test Explorer** (Test > Test Explorer)
3. Click **Run All Tests** or use **Ctrl+R, A**
4. All 111 tests should pass

## Features Available

- **Web Interface**: Professional Bootstrap UI
- **REST API**: Full OpenAPI/Swagger documentation  
- **Real-time Validation**: Input validation as you type
- **Error Handling**: User-friendly error messages
- **Performance Monitoring**: Response timing included

## API Testing in Visual Studio

Use the built-in **HTTP Client** or **Postman**:

```http
POST https://localhost:7284/api/convert
Content-Type: application/json

{
  "input": "123.45"
}
```

Expected response:
```json
{
  "isSuccess": true,
  "result": "ONE HUNDRED TWENTY THREE DOLLARS AND FORTY FIVE CENTS",
  "originalValue": 123.45,
  "processingTimeMs": 15
}
```

## Development Tips

- **Hot Reload**: Enabled by default in .NET 8.0
- **Debug**: Set breakpoints in any layer
- **IntelliSense**: Full code completion available
- **Error List**: Shows compilation errors and warnings
- **Package Manager Console**: For NuGet operations

This setup provides a complete development environment for the Number to Words Converter on Windows with Visual Studio.