# Publishing Guide for System.Text.Json.OptionalValue

## Package Information

- **Package Name**: `System.Text.Json.OptionalValue`
- **Current Version**: `1.0.0`
- **Target Framework**: .NET Standard 2.1
- **License**: MIT

## Package Files

The following files have been generated in `nupkg/`:
- `System.Text.Json.OptionalValue.1.0.0.nupkg` - Main NuGet package
- `System.Text.Json.OptionalValue.1.0.0.snupkg` - Symbol package for debugging

## Publishing to NuGet.org

### Prerequisites

1. **Create NuGet.org account**: Register at https://www.nuget.org/users/account/LogOn
2. **Generate API Key**: Go to https://www.nuget.org/account/apikeys
   - Click "Create"
   - Name: `System.Text.Json.OptionalValue`
   - Expiration: Choose appropriate duration
   - Scopes: Select "Push new packages and package versions"
   - Click "Create"
   - **IMPORTANT**: Copy the API key immediately (shown only once)

### Publishing Steps

#### Option 1: Using dotnet CLI (Recommended)

```bash
# Navigate to the repository root
cd C:\Domi\Repos\shared-libraries\System.Text.Json.OptionalValue

# Publish the package
dotnet nuget push nupkg/System.Text.Json.OptionalValue.1.0.0.nupkg \
  --api-key YOUR_API_KEY_HERE \
  --source https://api.nuget.org/v3/index.json
```

#### Option 2: Using NuGet CLI

```bash
# Install NuGet CLI if not already installed
# Download from https://www.nuget.org/downloads

nuget push nupkg/System.Text.Json.OptionalValue.1.0.0.nupkg \
  -ApiKey YOUR_API_KEY_HERE \
  -Source https://api.nuget.org/v3/index.json
```

#### Option 3: Using NuGet.org Web Upload

1. Go to https://www.nuget.org/packages/manage/upload
2. Click "Browse..." and select `nupkg/System.Text.Json.OptionalValue.1.0.0.nupkg`
3. Upload and verify package metadata
4. Click "Submit"

### After Publishing

1. **Package Listing**: Your package will appear at https://www.nuget.org/packages/System.Text.Json.OptionalValue/
2. **Indexing Time**: NuGet.org may take 5-15 minutes to index your package
3. **Verification**: Test installation: `dotnet add package System.Text.Json.OptionalValue`

## Updating the Package

### Version Numbering

Follow [Semantic Versioning](https://semver.org/):
- **Major** (X.0.0): Breaking changes
- **Minor** (1.X.0): New features, backward compatible
- **Patch** (1.0.X): Bug fixes, backward compatible

### Steps to Update

1. **Update version** in `src/System.Text.Json.OptionalValue/System.Text.Json.OptionalValue/System.Text.Json.OptionalValue.csproj`:
   ```xml
   <Version>1.1.0</Version>
   <PackageReleaseNotes>Added feature X, fixed bug Y</PackageReleaseNotes>
   ```

2. **Update CHANGELOG.md** (create if doesn't exist):
   ```markdown
   ## [1.1.0] - 2026-02-XX
   ### Added
   - Feature X
   ### Fixed
   - Bug Y
   ```

3. **Rebuild and repack**:
   ```bash
   dotnet clean
   dotnet build --configuration Release
   dotnet test
   dotnet pack --configuration Release --output nupkg
   ```

4. **Publish new version**:
   ```bash
   dotnet nuget push nupkg/System.Text.Json.OptionalValue.1.1.0.nupkg \
     --api-key YOUR_API_KEY_HERE \
     --source https://api.nuget.org/v3/index.json
   ```

## GitHub Repository Setup (Recommended)

### 1. Create GitHub Repository

```bash
cd C:\Domi\Repos\shared-libraries\System.Text.Json.OptionalValue

# Initialize git
git init

# Add all files
git add .

# Create initial commit
git commit -m "Initial commit: System.Text.Json.OptionalValue v1.0.0"

# Create GitHub repo (using GitHub CLI)
gh repo create relativityone/System.Text.Json.OptionalValue --public --source=. --remote=origin --push
```

### 2. Update Package Metadata

After creating the GitHub repo, update `.csproj` with actual URLs:
```xml
<PackageProjectUrl>https://github.com/relativityone/System.Text.Json.OptionalValue</PackageProjectUrl>
<RepositoryUrl>https://github.com/relativityone/System.Text.Json.OptionalValue</RepositoryUrl>
```

Then republish with updated version (e.g., 1.0.1).

### 3. Setup GitHub Actions (Optional)

Create `.github/workflows/publish.yml` for automated publishing:
```yaml
name: Publish to NuGet

on:
  release:
    types: [published]

jobs:
  publish:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v3
      - name: Setup .NET
        uses: actions/setup-dotnet@v3
        with:
          dotnet-version: '8.0.x'
      - name: Restore
        run: dotnet restore
      - name: Build
        run: dotnet build --configuration Release --no-restore
      - name: Test
        run: dotnet test --no-build --configuration Release
      - name: Pack
        run: dotnet pack --configuration Release --no-build --output nupkg
      - name: Push
        run: dotnet nuget push nupkg/*.nupkg --api-key ${{ secrets.NUGET_API_KEY }} --source https://api.nuget.org/v3/index.json
```

Add `NUGET_API_KEY` to GitHub Secrets: Settings → Secrets and variables → Actions → New repository secret

## Security Considerations

- ✅ **Never commit API keys** to source control
- ✅ Store API key securely (password manager, GitHub Secrets, Azure Key Vault)
- ✅ Use scoped API keys (limit to specific package)
- ✅ Set API key expiration
- ✅ Rotate API keys periodically

## Package Verification Checklist

Before publishing, verify:
- [ ] All tests pass (`dotnet test`)
- [ ] No security vulnerabilities (`dotnet list package --vulnerable`)
- [ ] README.md is complete and accurate
- [ ] LICENSE file is included
- [ ] Version number follows SemVer
- [ ] Package release notes are updated
- [ ] GitHub repository URL is correct (if published)
- [ ] Package icon is set (if desired)

## Support & Maintenance

- **Issues**: https://github.com/relativityone/System.Text.Json.OptionalValue/issues
- **Pull Requests**: https://github.com/relativityone/System.Text.Json.OptionalValue/pulls
- **Discussions**: https://github.com/relativityone/System.Text.Json.OptionalValue/discussions

## Additional Resources

- [NuGet Documentation](https://docs.microsoft.com/en-us/nuget/)
- [Creating NuGet Packages](https://docs.microsoft.com/en-us/nuget/create-packages/creating-a-package-dotnet-cli)
- [Publishing Packages](https://docs.microsoft.com/en-us/nuget/nuget-org/publish-a-package)
- [Semantic Versioning](https://semver.org/)
