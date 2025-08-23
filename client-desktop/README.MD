# QuizDesktop - Build & Publish Instructions

## Prerequisites

Install required .NET MAUI workloads:

```bash
sudo dotnet workload install maui
sudo dotnet workload update
```

Verify installed workloads:

```bash
dotnet workload list
```

Example output:

```
Installed Workload Id      Manifest Version      Installation Source
--------------------------------------------------------------------
maui                       8.0.100/8.0.100       SDK 8.0.400
```

---

## Troubleshooting

If you encounter build problems:

```bash
dotnet restore
dotnet clean
```

* `dotnet restore` fetches all required NuGet packages
* `dotnet clean` removes old build outputs

---

# Build & Publish

## Build for Windows

Compile the project:

```bash
dotnet build -f net8.0-windows10.0.19041.0
```

Publish a standalone `.exe` (includes runtime):

```bash
dotnet publish -f net8.0-windows10.0.19041.0 -c Release -p:PublishSingleFile=true -p:SelfContained=true -o ./publish-win
```

The executable will be located inside the `./publish-win` folder.

---

## Build for macOS (Catalyst)

Compile for macOS:

```bash
dotnet build -f net8.0-maccatalyst
```

Publish a `.app` bundle for macOS:

```bash
dotnet publish -f net8.0-maccatalyst -c Release -o ./publish-mac
```

> **Note:** Building for macOS requires a Mac with Xcode and MAUI workloads installed.

---

