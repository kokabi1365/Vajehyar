$artifacts = ".\artifacts"

If(!(test-path $artifacts))
{
      New-Item -ItemType Directory -Force -Path $artifacts
}

Remove-Item .\artifacts\* -Recurse -Force
dotnet clean --configuration Release
dotnet build --configuration Release
Remove-Item .\artifacts\obj -Recurse -Force
ISCC.exe .\setup\setup.iss