dotnet build -c Release
dotnet test -c Release --no-build --settings tests/coverage.runsettings --results-directory test-results

$coverageFiles = Get-ChildItem -Path "test-results" -Recurse -Filter "*cobertura.xml"

if ($coverageFiles) {
    $reportFiles = ($coverageFiles | ForEach-Object { $_.FullName }) -join ";"
    dotnet tool run reportgenerator -reports:$reportFiles -targetdir:coverage-report

    Start-Process "coverage-report/index.html"
} else {
    Write-Host "Coverage reports not found."
}

Get-ChildItem -Path "test-results" -Recurse | ForEach-Object { Remove-Item -Recurse -Force -Path $_.FullName }
