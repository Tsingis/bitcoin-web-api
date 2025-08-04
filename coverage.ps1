dotnet test -c Debug --no-restore --no-build -- --coverage --coverage-output-format xml --coverage-output coverage.xml

$coverageFiles = Get-ChildItem -Path "tests" -Recurse -Filter "coverage.xml"

if ($coverageFiles) {
    $reportFiles = ($coverageFiles | ForEach-Object { $_.FullName }) -join ";"
    reportgenerator -reports:$reportFiles -targetdir:coveragereport

    Start-Process "coveragereport/index.html"
} else {
    Write-Host "Coverage reports not found."
}

Get-ChildItem -Path "tests" -Recurse -Filter "TestResults" | ForEach-Object { Remove-Item -Recurse -Force -Path $_.FullName }
