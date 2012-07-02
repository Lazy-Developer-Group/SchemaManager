param (
	$DatabaseName = $(Throw "DatabaseName is required."),
	$DatabaseScriptsFolder = $(Throw "DatabaseScriptsFolder is required."),
	$Target = "(local)"
)

$DatabaseScriptsFolder = Resolve-Path $DatabaseScriptsFolder

$ToolsPath = (Get-ItemProperty "HKLM:\SOFTWARE\Microsoft\Microsoft SQL Server\100\Tools\ClientSetup" -name "Path").Path

$SqlCmd = $ToolsPath + "sqlcmd.exe"

Write-Host "Creating database..."
. $SqlCmd -S $Target -d master -v dbName=$DatabaseName -v autoDrop=True -i "$DatabaseScriptsFolder\01.CreateDatabase.sql"
#Write-Host "Creating schema..."
#. $SqlCmd -S $Target -d $DatabaseName -i "$DatabaseScriptsFolder\02.CreateSchema.sql"
#Write-Host "Loading standard seed data..."
#. $SqlCmd -S $Target -d $DatabaseName -i "$DatabaseScriptsFolder\03.InsertStandardSeedData.sql"

ECHO Complete.
Write-Host -NoNewLine "Press any key to continue..."
$null = $Host.UI.RawUI.ReadKey("NoEcho,IncludeKeyDown")
