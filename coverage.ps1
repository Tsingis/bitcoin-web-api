dotnet build -c Release
dotnet test --project tests/UnitTests -c Release --no-build --results-directory test-results `
--coverage --coverage-output-format cobertura --coverage-output coverage.cobertura.xml

$coverageFiles = Get-ChildItem -Path "test-results" -Recurse -Filter "*cobertura.xml"

if ($coverageFiles) {
    $reportFiles = ($coverageFiles | ForEach-Object { $_.FullName }) -join ";"
    dotnet tool run reportgenerator -reports:$reportFiles -targetdir:coverage-report

    Start-Process "coverage-report/index.html"
} else {
    Write-Host "Coverage reports not found."
}

Get-ChildItem -Path "test-results" -Recurse | ForEach-Object { Remove-Item -Recurse -Force -Path $_.FullName }
