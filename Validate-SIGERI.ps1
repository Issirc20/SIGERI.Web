<#
Validate-SIGERI.ps1
Script de validacion integral para SIGERI.Web
Genera Build.log, Test.log, Server.log, Home.html, Index.html y ValidationReport/

Ejecutalo desde la raiz del repositorio en PowerShell:
> .\Validate-SIGERI.ps1
#>

param(
	[int]$ServerTimeoutSeconds = 120,
	[int]$PerfBuildThresholdSeconds = 60,
	[int]$PerfStartupThresholdSeconds = 30,
	[int]$PerfResponseThresholdMs = 1000
)
[System.Net.ServicePointManager]::ServerCertificateValidationCallback = {$true}

$ErrorActionPreference = 'Stop'
$ExitCode = 0

function Write-Info($msg){ Write-Host "[INFO]    $msg" -ForegroundColor Cyan }
function Write-Success($msg){ Write-Host "[OK]      $msg" -ForegroundColor Green }
function Write-Warn($msg){ Write-Host "[WARN]    $msg" -ForegroundColor Yellow }
function Write-ErrorMsg($msg){ Write-Host "[ERROR]   $msg" -ForegroundColor Red }
function Safe-Write($path, $content) {
    $dir = Split-Path -Path $path -Parent
    if (-not (Test-Path $dir)) { New-Item -ItemType Directory -Path $dir -Force | Out-Null }
    $content | Out-File -FilePath $path -Encoding UTF8 -Force
}

# Paths
$Root = Split-Path -Path $MyInvocation.MyCommand.Path -Parent
Set-Location $Root
$Solution = Join-Path $Root "SIGERI.Web.slnx"
$Project = Join-Path $Root "SIGERI.Web\SIGERI.Web.csproj"
$BuildLog = Join-Path $Root "Build.log"
$TestLog = Join-Path $Root "Test.log"
$ServerLog = Join-Path $Root "Server.log"
$HomeHtml = Join-Path $Root "Home.html"
$IndexHtml = Join-Path $Root "Index.html"
$ReportDir = Join-Path $Root "ValidationReport"
$SummaryFile = Join-Path $ReportDir "Summary.txt"
$ArchReport = Join-Path $ReportDir "ArchitectureReport.md"
$PerfReport = Join-Path $ReportDir "PerformanceReport.md"
$DashReport = Join-Path $ReportDir "DashboardReport.md"
$QualityGate = Join-Path $ReportDir "QualityGate.md"
$CiSummary = Join-Path $ReportDir "CiSummary.txt"
$NextActions = Join-Path $ReportDir "NextActions.md"

if(-not (Test-Path $Solution)){
	Write-Warn "Solution file not found at $Solution. Trying to proceed with discover."
}

# Helper: time measurement
function Measure-Time([scriptblock]$action){
	$start = Get-Date
	& $action
	$end = Get-Date
	return ([math]::Round(($end - $start).TotalSeconds, 2))
}

Write-Info "FASE 1 - LIMPIEZA"
# Optional clean: remove bin/obj under projects (safe guard: do not delete other files)
$projDirs = Get-ChildItem -Path $Root -Directory -Force | Where-Object { Test-Path (Join-Path $_.FullName "*.csproj") }
foreach($d in $projDirs){
	$bin = Join-Path $d.FullName "bin"
	$obj = Join-Path $d.FullName "obj"
	if(Test-Path $bin){ Remove-Item $bin -Recurse -Force -ErrorAction SilentlyContinue; Write-Info "Removed $bin" }
	if(Test-Path $obj){ Remove-Item $obj -Recurse -Force -ErrorAction SilentlyContinue; Write-Info "Removed $obj" }
}

Write-Info "Restaurando paquetes NuGet..."
try{
	dotnet restore $Solution 2>&1 | Tee-Object -FilePath $BuildLog
	Write-Success "dotnet restore finalizado"
}catch{
	Write-ErrorMsg "dotnet restore fallo: $_"
	exit 1
}

Write-Info "Versiones .NET instaladas:"; dotnet --list-sdks | ForEach-Object { Write-Host "  $_" }
Write-Info "SDK usado por entorno (dotnet --info):"; dotnet --info | Select-String "^  OS|^  Version|^  .NET" | ForEach-Object { Write-Host "  $_" }

Write-Info "FASE 2 - COMPILACION"
$buildTime = 0
try{
	$buildTime = Measure-Time { dotnet build $Solution -c Debug 2>&1 | Tee-Object -FilePath $BuildLog -Append }
	Write-Success "dotnet build finalizo en $buildTime s"
}catch{
	Write-ErrorMsg "Compilacion fallo. Revisa $BuildLog"
	exit 1
}

# Check for Errors in Build.log
$buildContent = Get-Content $BuildLog -Raw
if($buildContent -match "error\s+CS\d+" -or $buildContent -match "\berror:\b"){ Write-ErrorMsg "Se detectaron errores de compilacion. Revisa $BuildLog"; exit 1 } else { Write-Success "Compilacion terminada sin errores criticos" }

# Warnings
$warnings = (Select-String -Path $BuildLog -Pattern "warning" -SimpleMatch | Measure-Object).Count
if($warnings -gt 0){ Write-Warn "$warnings advertencias detectadas en build. Ver archivo $BuildLog" } else { Write-Success "Sin advertencias de build" }

Write-Info "FASE 3 - TESTS"
try{
	dotnet test $Solution --logger 'trx;LogFileName=TestResults.trx' 2>&1 | Tee-Object -FilePath $TestLog
	Write-Success "Ejecucion de tests completada"
}catch{
	Write-Warn "Ejecucion de tests fallo o no hay tests: $_"
}

Write-Info "FASE 4 - EJECUCION"
if(Test-Path $ServerLog){ Remove-Item $ServerLog -Force }

# Start the server in background and redirect output
Write-Info "Iniciando SIGERI.Web (dotnet run --project $Project)"
$process = Start-Process -FilePath "cmd.exe" -ArgumentList "/c dotnet run --project `"$Project`" --no-launch-profile > `"$ServerLog`" 2> `"$Root\ServerError.log`"" -NoNewWindow -PassThru

# Wait for server availability by parsing Server.log for listening url
$baseUrl = $null
$startTime = Get-Date
Write-Info "Esperando que el servidor inicie (timeout ${ServerTimeoutSeconds}s)..."
while(((Get-Date) - $startTime).TotalSeconds -lt $ServerTimeoutSeconds){
	Start-Sleep -Milliseconds 500
	if(Test-Path $ServerLog){
		$lines = Get-Content $ServerLog -Tail 200 -ErrorAction SilentlyContinue
		foreach($ln in $lines){
			if($ln -match 'Now listening on:\s*(https?://[^\s,;]+)'){ $baseUrl = $matches[1]; break }
			if($ln -match 'Now listening on'){ if($ln -match '(https?://[^\s,;]+)') { $baseUrl = $matches[1]; break } }
			if($ln -match 'Application started. Press Ctrl\+C to shut down.' ){
				if(-not $baseUrl){ $candidates = @('https://localhost:5001','http://localhost:5000','https://127.0.0.1:5100'); foreach($c in $candidates){ try{ $r = Invoke-WebRequest -Uri $c -UseBasicParsing -TimeoutSec 1 -ErrorAction Stop; $baseUrl = $c; break }catch{} } if($baseUrl){ break } }
			}
		}
	}
	if($baseUrl){ break }
}

$startupTime = if($baseUrl){ ([math]::Round((Get-Date -UFormat %s) - ([math]::Round(($startTime - [datetime]'1970-01-01').TotalSeconds)),2) ) } else { 0 }
if(-not $baseUrl){ Write-Warn "No se detecto URL de escucha en Server.log. Intentando URLs por defecto..."; $candidates = @('https://localhost:5001','http://localhost:5000','https://127.0.0.1:5100'); foreach($c in $candidates){ try{ $r = Invoke-WebRequest -Uri $c -UseBasicParsing -TimeoutSec 3 -ErrorAction Stop; $baseUrl = $c; break }catch{} } }
if(-not $baseUrl){ Write-ErrorMsg "No se pudo determinar la URL del servidor. Revisa $ServerLog"; exit 1 }
Write-Success "Servidor disponible en $baseUrl"

Write-Info "FASE 5 - SMOKE TEST (con Autenticacion)"
$session = New-Object Microsoft.PowerShell.Commands.WebRequestSession
try {
    Write-Info "Obteniendo token anti-forgery de la pagina de Login..."
    $loginPage = Invoke-WebRequest -Uri "$baseUrl/Account/Login" -WebSession $session -UseBasicParsing -TimeoutSec 10 -ErrorAction Stop
    if ($loginPage.Content -match 'name="__RequestVerificationToken" type="hidden" value="([^"]+)"') {
        $token = $matches[1]
        Write-Info "Token anti-forgery obtenido con exito"
        
        Write-Info "Iniciando sesion como admin@sigeri.local..."
        $escapedToken = [Uri]::EscapeDataString($token)
        $body = "Correo=admin%40sigeri.local&Password=Admin123%23&__RequestVerificationToken=$escapedToken"
        $loginResult = Invoke-WebRequest -Uri "$baseUrl/Account/Login" -Method Post -Body $body -ContentType "application/x-www-form-urlencoded" -WebSession $session -UseBasicParsing -TimeoutSec 10 -ErrorAction Stop
        
        if ($session.Cookies.GetCookies($baseUrl)['.AspNetCore.Cookies']) {
            Write-Success "Sesion iniciada correctamente"
        } else {
            Write-Warn "No se detecto la cookie de sesion. Las paginas protegidas podrian redirigir al login."
        }
    } else {
        Write-Warn "No se pudo extraer el token anti-forgery. Se realizaran peticiones anonimas."
    }
} catch {
    Write-Warn "Error durante el flujo de login. Se continuara con peticiones anonimas: $_"
}

$paths = @('/','/Home','/Home/Index')
$httpResults = @{}
foreach($p in $paths){
	$uri = [System.Uri]::new($baseUrl.TrimEnd('/'))
	$full = $uri.OriginalString.TrimEnd('/') + $p
	try{
		$sw = [System.Diagnostics.Stopwatch]::StartNew()
		$resp = Invoke-WebRequest -Uri $full -UseBasicParsing -WebSession $session -TimeoutSec 30 -ErrorAction Stop
		$sw.Stop()
		$elapsedMs = $sw.ElapsedMilliseconds
		$status = $resp.StatusCode
		Write-Success "$full -> HTTP $status in ${elapsedMs}ms"
		if($p -eq '/' -or $p -eq '/Home'){ $resp.Content | Out-File -FilePath $HomeHtml -Encoding UTF8 }
		if($p -eq '/Home/Index'){ $resp.Content | Out-File -FilePath $IndexHtml -Encoding UTF8 }
		$httpResults[$p] = @{ Status = $status; Size = ($resp.Content.Length); ResponseMs = $elapsedMs }
	}catch{
		Write-ErrorMsg "$full -> Request failed: $_"
		$httpResults[$p] = @{ Status = 'Error'; Error = $_.Exception.Message }
	}
}

Write-Info "FASE 6 - VALIDACIONES HTML"
$warnings = @(); $foundComponents = @{}
function Check-Html($file,$name){
	if(-not (Test-Path $file)){ Write-Warn "$name not saved: $file"; return }
	$content = Get-Content $file -Raw
	$checks = @{
		'MetricCards' = 'kpi-card|kpi-card-value|sig-kpi-value'
		'MetricCardPartial' = '_KpiCard|_MetricCard'
		'DashboardCards' = 'dashboard-card|sig-card'
		'CompliancePanel' = 'chartIsoCompliance|chartNistRadar|sig-progress|_CompliancePanel|_ProgressCard'
		'ChartCanvas' = 'chart-canvas|<canvas'
		'ChartData' = 'chartTrend-data|chartCategories-data|chart-data'
		'ChartsInit' = 'dashboard.js|charts-init.js|Chart\('
		'NotificationCenter' = 'notification|notifications|NotificationCenter|btnNotifications'
		'Timeline' = 'timeline|Timeline|_Timeline|_RecentActivity'
		'Heatmap' = 'risk-matrix-grid|matrix-cell|Heatmap|chartHeat|chartHeatmap|heatmap'
		'Radar' = 'chartNistRadar|Radar|chartRadar|radar'
		'KPI' = 'kpi-card|sig-kpi-value|_StatCard'
	}
	foreach($k in $checks.Keys){
		$pattern = $checks[$k]
		if($content -match $pattern){ Write-Info "Found $k in $name"; $foundComponents[$k] = $true } else { Write-Warn "$k NOT found in $name"; $warnings += "${name}: Missing $k"; $foundComponents[$k] = $false }
	}
	# Check MetricCard non-empty values
	$matches = [regex]::Matches($content,'<div[^>]*class=.*?sig-kpi-value.*?>(.*?)<\/div>', 'Singleline')
	foreach($m in $matches){
		$val = ($m.Groups[1].Value).Trim()
		if([string]::IsNullOrWhiteSpace($val)){ Write-Warn "Empty KPI value found in $name"; $warnings += "${name}: Empty KPI value" }
	}
}
Check-Html $HomeHtml 'Home'
Check-Html $IndexHtml 'Home/Index'

Write-Info "FASE 7 - VALIDACION DE LOGS"
$exceptionPatterns = @('Exception','ERROR','Unhandled','StackTrace','InvalidOperationException','NullReferenceException','SqlException')
$logSummary = @{}
if(Test-Path $ServerLog){
	foreach($p in $exceptionPatterns){
		$matches = Select-String -Path $ServerLog -Pattern $p -SimpleMatch -ErrorAction SilentlyContinue
		$count = 0
		if($matches){ $count = $matches.Count }
		$logSummary[$p] = $count
		if($count -gt 0){ Write-Warn "$count matches for '$p' in Server.log" }
	}
} else { Write-Warn "Server.log not found" }

Write-Info "FASE 9 - VERIFICACION DE ARQUITECTURA"
# Analyze project references to detect layering violations
$csprojs = Get-ChildItem -Path $Root -Recurse -Filter *.csproj | Select-Object -ExpandProperty FullName
$projRefs = @{}
foreach($cs in $csprojs){
	try{
		$xml = [xml](Get-Content $cs)
		$name = [System.IO.Path]::GetFileNameWithoutExtension($cs)
		$refs = @()
		$nodes = $xml.Project.ItemGroup.ProjectReference
		if($nodes){ foreach($n in $nodes){ $include = $n.Include; $refs += [System.IO.Path]::GetFileNameWithoutExtension($include) } }
		$projRefs[$name] = $refs
	}catch{ }
}
# Detect direct infra -> domain reverse dependencies etc.
$archWarnings = @()
# Helper to check if project A references B directly
function Has-Ref($A,$B){ return ($projRefs[$A] -contains $B) }
if($projRefs.Keys -contains 'SIGERI.Domain' -and $projRefs.Keys -contains 'SIGERI.Infrastructure'){
	if(Has-Ref 'SIGERI.Domain' 'SIGERI.Infrastructure'){ $archWarnings += 'Domain references Infrastructure (violation)'; Write-Warn 'Domain references Infrastructure' }
}
if($projRefs.Keys -contains 'SIGERI.Application' -and $projRefs.Keys -contains 'SIGERI.Web'){
	if(Has-Ref 'SIGERI.Application' 'SIGERI.Web'){ $archWarnings += 'Application references Web (violation)'; Write-Warn 'Application references Web' }
}
# Web should not reference DbContext directly (project-level check: Web should reference Infrastructure only via Application, but we check for direct file usages)
$webFiles = Get-ChildItem -Path (Join-Path $Root 'SIGERI.Web') -Recurse -Include *.cs
$webDbContextUsage = Select-String -Path $webFiles.FullName -Pattern 'SigeriDbContext|ISigeriDbContext' -SimpleMatch -ErrorAction SilentlyContinue
if($webDbContextUsage){ Write-Warn 'SIGERI.Web references DbContext types directly (check code)'; $archWarnings += 'Web accesses DbContext types' }
# Controllers should not use repositories directly: search for IRepository or Repository in Controllers
$controllers = Get-ChildItem -Path (Join-Path $Root 'SIGERI.Web') -Recurse -Include *Controller.cs -ErrorAction SilentlyContinue
$repoUsage = Select-String -Path $controllers.FullName -Pattern 'Repository' -SimpleMatch -ErrorAction SilentlyContinue
if($repoUsage){ Write-Warn 'Some Controllers reference Repository types directly'; $archWarnings += 'Controllers use repositories directly' }
# Check IAnalyticsService usage
$analyticsUsage = Select-String -Path $webFiles.FullName -Pattern 'IAnalyticsService' -SimpleMatch -ErrorAction SilentlyContinue
if(-not $analyticsUsage){ Write-Warn 'IAnalyticsService not used in Web project' ; $archWarnings += 'IAnalyticsService missing' } else { Write-Success 'IAnalyticsService detected in Web project' }
# Detect cycles in projRefs (simple DFS)
$visited = @{}
$onStack = @{}
$hasCycle = $false
function dfs($node){
	if($onStack[$node]){ $script:hasCycle = $true; return }
	if($visited[$node]){ return }
	$visited[$node] = $true; $onStack[$node] = $true
	foreach($n in $projRefs[$node]){ if($n){ dfs $n } }
	$onStack[$node] = $false
}
foreach($k in $projRefs.Keys){ if(-not $visited[$k]){ dfs $k } }
if($hasCycle){ Write-Warn 'Cyclic project references detected' ; $archWarnings += 'Cyclic references' } else { Write-Success 'No cyclic project references found' }

Write-Info "FASE 10 - VALIDACION DE CLEAN ARCHITECTURE (heuristica)"
$cleanWarnings = @()
# Heuristic checks: Files placement
# Controllers in SIGERI.Web/Controllers
$controllersCount = (Get-ChildItem -Path (Join-Path $Root 'SIGERI.Web\Controllers') -Recurse -Include *Controller.cs -ErrorAction SilentlyContinue).Count
if($controllersCount -eq 0){ Write-Warn 'No controllers found under SIGERI.Web/Controllers'; $cleanWarnings += 'No controllers in expected folder' } else { Write-Success "Found $controllersCount controllers" }
# Services in Web should be interfaces in SIGERI.Web/Services
$serviceInterfaces = Get-ChildItem -Path (Join-Path $Root 'SIGERI.Web\Services') -Recurse -Include *.cs -ErrorAction SilentlyContinue
if($serviceInterfaces.Count -eq 0){ Write-Warn 'No services found in SIGERI.Web/Services' ; $cleanWarnings += 'Services not found' } else { Write-Success 'Services folder present' }
# DTOs in Application
$dtos = Get-ChildItem -Path (Join-Path $Root 'SIGERI.Application') -Recurse -Include *Dto.cs -ErrorAction SilentlyContinue
if($dtos.Count -eq 0){ Write-Warn 'No DTOs found under SIGERI.Application' ; $cleanWarnings += 'No DTOs in Application' } else { Write-Success "Found $($dtos.Count) DTO files in Application" }
# ViewModels in Web
$viewmodels = Get-ChildItem -Path (Join-Path $Root 'SIGERI.Web') -Recurse -Include *ViewModel*.cs -ErrorAction SilentlyContinue
if($viewmodels.Count -eq 0){ Write-Warn 'No ViewModels found in SIGERI.Web' ; $cleanWarnings += 'No ViewModels' } else { Write-Success "Found $($viewmodels.Count) ViewModel files" }

Write-Info "FASE 11 - VALIDACION DE BASE DE DATOS"
# Check for pending migrations (best-effort)
$migrationsInfo = ''
try{
	$infraProj = Get-ChildItem -Path $Root -Recurse -Filter SIGERI.Infrastructure.csproj | Select-Object -First 1 -ExpandProperty FullName
	if($infraProj){
		$migrationsInfo = dotnet ef migrations list --project "$infraProj" 2>&1
		Write-Info "dotnet ef migrations list result:"
		Write-Info $migrationsInfo
	} else { Write-Warn 'SIGERI.Infrastructure project not found for migrations check' }
}catch{
	Write-Warn "dotnet ef migrations list failed or EF tools not available: $_"
}
# Attempt simple SQL connection test if connection string present in appsettings.Development.json
$cfgFile = Join-Path $Root 'SIGERI.Web\appsettings.Development.json'
$sqlConnectionOk = $false
if(Test-Path $cfgFile){
	try{
		$cfg = Get-Content $cfgFile -Raw | ConvertFrom-Json
		$conn = $cfg.ConnectionStrings.DefaultConnection
		if($conn){
			Write-Info "Attempting SQL connection test to connection string from appsettings.Development.json"
			Add-Type -AssemblyName System.Data
			$cn = New-Object System.Data.SqlClient.SqlConnection $conn
			try{ $cn.Open(); $cn.Close(); $sqlConnectionOk = $true; Write-Success 'SQL Server connection OK' }catch{ Write-Warn "SQL connection failed: $($_.Exception.Message)" }
		}
	}catch{ Write-Warn 'Failed to parse appsettings.Development.json' }
} else { Write-Warn 'appsettings.Development.json not found; skipping SQL connection test' }

# Attempt to ensure DbContext can be resolved via Server.log (Program.cs attempts EnsureCreated in dev)
$dbInitFound = $false
if(Test-Path $ServerLog){
	$lines = Get-Content $ServerLog -Tail 300 -ErrorAction SilentlyContinue
	foreach($ln in $lines){ if($ln -match 'EnsureCreated' -or $ln -match 'Applying migrations' -or $ln -match 'Database created') { $dbInitFound = $true; break } }
	if($dbInitFound){ Write-Success 'DbContext initialization detected in Server.log' } else { Write-Warn 'No DB initialization log lines detected; check Server.log' }
}

Write-Info "FASE 12 - VALIDACION DEL DASHBOARD (component detection)"
$dashboardComponents = @{}
$dashboardComponents['MetricCards'] = $foundComponents['MetricCards']
$dashboardComponents['ProgressCards'] = $foundComponents['CompliancePanel']
$dashboardComponents['ChartCards'] = $foundComponents['ChartCanvas']
$dashboardComponents['Heatmap'] = $foundComponents['Heatmap']
$dashboardComponents['Radar'] = $foundComponents['Radar']
$dashboardComponents['NotificationCenter'] = $foundComponents['NotificationCenter']
$dashboardComponents['Timeline'] = $foundComponents['Timeline']

Write-Info "FASE 13 - VALIDACION DE RENDIMIENTO"
# Build time already measured
$perfSummary = @{}
$perfSummary['BuildSeconds'] = $buildTime
if($buildTime -gt $PerfBuildThresholdSeconds){ Write-Warn "Build time $buildTime s exceeds threshold $PerfBuildThresholdSeconds s" }
# Startup time: estimate by scanning server log time between start and listening
$serverContent = if(Test-Path $ServerLog){ Get-Content $ServerLog -Raw } else { '' }
# We measured HTTP response times in $httpResults
$totalResponses = $httpResults.Keys.Count
$avgResponseMs = 0
if($totalResponses -gt 0){ $avgResponseMs = [math]::Round(($httpResults.Values | ForEach-Object { $_.ResponseMs } | Measure-Object -Average).Average,2) }
$perfSummary['AvgResponseMs'] = $avgResponseMs
if($avgResponseMs -gt $PerfResponseThresholdMs){ Write-Warn "Average response $avgResponseMs ms exceeds threshold $PerfResponseThresholdMs ms" }

Write-Info "FASE 14 - REPORTE TECNICO"
if(Test-Path $ReportDir){ Remove-Item $ReportDir -Recurse -Force }
New-Item -Path $ReportDir -ItemType Directory | Out-Null
Copy-Item -Path $BuildLog -Destination $ReportDir -ErrorAction SilentlyContinue
Copy-Item -Path $TestLog -Destination $ReportDir -ErrorAction SilentlyContinue
Copy-Item -Path $ServerLog -Destination $ReportDir -ErrorAction SilentlyContinue
Copy-Item -Path $HomeHtml -Destination (Join-Path $ReportDir 'Home.html') -ErrorAction SilentlyContinue
Copy-Item -Path $IndexHtml -Destination (Join-Path $ReportDir 'Index.html') -ErrorAction SilentlyContinue

# Summary
$summary = @()
$summary += "SIGERI Validation Report - $(Get-Date -Format o)"
$summary += ""
$summary += "Build time (s): $buildTime"
$summary += "Build warnings: $warnings"
$summary += "Tests log: $(if(Test-Path $TestLog){ 'present' } else { 'none' })"
foreach($p in $paths){ $r = $httpResults[$p]; $summary += "$p -> $($r.Status) (size: $($r.Size)) ResponseMs: $($r.ResponseMs)" }
$summary += ""
$summary += "Exceptions summary from Server.log:"
foreach($k in $logSummary.Keys){ $summary += "$k : $($logSummary[$k])" }
$summary += ""
if($archWarnings.Count -gt 0){ $summary += "Architecture warnings:"; $summary += $archWarnings }
if($cleanWarnings.Count -gt 0){ $summary += "Clean architecture heuristics:"; $summary += $cleanWarnings }
$summary += ""
$summary += "Dashboard components detected:"
foreach($k in $dashboardComponents.Keys){ $summary += "$k : $($dashboardComponents[$k])" }
$summary += ""
$summary += "Recommendations:"
$summary += " - Ejecuta este script en tu entorno local donde la app puede iniciarse."
$summary += " - Revisa ArchitectureReport.md, PerformanceReport.md y DashboardReport.md en ValidationReport/"

$summary | Out-File -FilePath $SummaryFile -Encoding UTF8

# ArchitectureReport.md
$arch = @()
$arch += "# Architecture Report"
$arch += "Generated: $(Get-Date -Format o)"
$arch += ""
$arch += "## Project references (partial)"
foreach($k in $projRefs.Keys){ $arch += "- $k -> $(($projRefs[$k] -join ', '))" }
$arch += ""
$arch += "## Detected architecture issues"
if($archWarnings.Count -eq 0){ $arch += "No major architecture violations detected (heuristic)." } else { $arch += $archWarnings }
$arch += ""
$arch += "## Recommendations"
$arch += "- Ensure Domain has no references to Infrastructure. Use Application layer interfaces."
$arch += "- Keep controllers thin; use services from Application/Web."
$arch | Out-File -FilePath $ArchReport -Encoding UTF8

# PerformanceReport.md
$perf = @()
$perf += "# Performance Report"
$perf += "Generated: $(Get-Date -Format o)"
$perf += ""
$perf += "Build time (s): $buildTime"
$perf += "Average response (ms): $avgResponseMs"
$perf += "Server startup detected URL: $baseUrl"
$perf += ""
$perf += "Recommendations"
$perf += "- Investigate long build times (use dotnet build -v:minimal for details)."
$perf += "- Add caching and AsNoTracking for read-only queries in AnalyticsService."
$perf | Out-File -FilePath $PerfReport -Encoding UTF8

# DashboardReport.md
$dash = @()
$dash += "# Dashboard Report"
$dash += "Generated: $(Get-Date -Format o)"
$dash += ""
$dash += "Components detected:"
foreach($k in $dashboardComponents.Keys){ $dash += "- $k : $($dashboardComponents[$k])" }
$dash += ""
$dash += "Warnings:"
$dash += $warnings
$dash | Out-File -FilePath $DashReport -Encoding UTF8

Write-Success "Validation report generated: $ReportDir"
Write-Info "Summary:"; Get-Content $SummaryFile | ForEach-Object { Write-Host "  $_" }
Write-Info "Architecture report: $ArchReport"
Write-Info "Performance report: $PerfReport"
Write-Info "Dashboard report: $DashReport"

# ===== FASE 15 - CI READY =====
Write-Info "Generando resumen CI: $CiSummary"
$ciLines = @()
$ciLines += "# CI Summary"
$ciLines += "BuildTimeSeconds: $buildTime"
$ciLines += "BuildWarnings: $warnings"
$ciLines += "TestsFailed: $testsFailed"
foreach($p in $paths){ $r = $httpResults[$p]; $ciLines += "${p}: Status=$($r.Status); ResponseMs=$($r.ResponseMs); Size=$($r.Size)" }
foreach($k in $logSummary.Keys){ $ciLines += "Log.${k}: $($logSummary[$k])" }
$ciLines | Out-File -FilePath $CiSummary -Encoding UTF8
Write-Success "CI summary written to $CiSummary"

# ===== FASE 16 - QUALITY GATE =====
Write-Info "Calculating Quality Gate"
# Scoring rules (0-100 per area)
$scoreBuild = if($buildContent -match "error\s+CS\d+" -or $buildContent -match "\berror:\b"){ 0 } else { 100 }
$scoreTests = if($testsFailed){ 0 } else { 100 }
# Dashboard score: presence of core components
$dashboardTotal = 6
$dashboardFound = 0
$dashboardFound += [int]($dashboardComponents['MetricCards'] -eq $true)
$dashboardFound += [int]($dashboardComponents['ProgressCards'] -eq $true)
$dashboardFound += [int]($dashboardComponents['ChartCards'] -eq $true)
$dashboardFound += [int]($dashboardComponents['Heatmap'] -eq $true)
$dashboardFound += [int]($dashboardComponents['Radar'] -eq $true)
$dashboardFound += [int]($dashboardComponents['NotificationCenter'] -eq $true)
$scoreDashboard = [math]::Round(($dashboardFound / $dashboardTotal) * 100)
# Architecture score: penalize if archWarnings found
$scoreArchitecture = if($archWarnings.Count -gt 0){ 75 } else { 100 }
# Database score
$scoreDatabase = if($sqlConnectionOk -and $dbInitFound){ 100 } elseif($sqlConnectionOk -or $dbInitFound){ 80 } else { 50 }
# Performance score (based on avg response and build time)
$scorePerformance = 100
if($buildTime -gt $PerfBuildThresholdSeconds){ $scorePerformance -= 10 }
if($avgResponseMs -gt $PerfResponseThresholdMs){ $scorePerformance -= 20 }
$scorePerformance = [math]::Max(0, $scorePerformance)

# Compose QualityGate.md
$qg = @()
$qg += "# Quality Gate"
$qg += "Generated: $(Get-Date -Format o)"
$qg += ""
$qg += "Build: $scoreBuild%"
$qg += "Tests: $scoreTests%"
$qg += "Dashboard: $scoreDashboard% ($dashboardFound of $dashboardTotal components)"
$qg += "Architecture: $scoreArchitecture%"
$qg += "Database: $scoreDatabase%"
$qg += "Performance: $scorePerformance%"
$globalScore = [math]::Round((($scoreBuild + $scoreTests + $scoreDashboard + $scoreArchitecture + $scoreDatabase + $scorePerformance) / 6),2)
$qg += ""
$qg += "Global Quality Score: $globalScore%"
if($globalScore -ge 95){ $qg += "Status: EXCELENTE" }
elseif($globalScore -ge 85){ $qg += "Status: APROBADO" }
elseif($globalScore -ge 70){ $qg += "Status: REQUIERE MEJORAS" } else { $qg += "Status: NO APTO PARA RELEASE" }
Safe-Write $QualityGate $qg
Write-Success "Quality Gate saved to $QualityGate"

# ===== FASE 17 - ROADMAP (NextActions) =====
Write-Info "Generating NextActions report"
$next = @()
$next += "# Next Actions"
$next += "Generated: $(Get-Date -Format o)"
$next += ""
# Critical errors from ExitReasons
if($ExitReasons.Count -gt 0){
    $next += "## Critical Errors"
    foreach($r in $ExitReasons){
        $next += "- $r"
    }
}
# Warnings and architecture issues
$next += ""
$next += "## Warnings & Improvements"
if($warnings.Count -gt 0){ $next += "### HTML Warnings"; $next += $warnings }
if($archWarnings.Count -gt 0){ $next += "### Architecture Warnings"; $next += $archWarnings }
if($cleanWarnings.Count -gt 0){ $next += "### Clean Architecture Heuristics"; $next += $cleanWarnings }
if($logSummary.GetEnumerator() | Where-Object { $_.Value -gt 0 }){ $next += "### Log Exceptions"; foreach($k in $logSummary.Keys){ if($logSummary[$k] -gt 0){ $next += "- $k : $($logSummary[$k]) occurrences" } } }

# Prioritize items (simple heuristic)
$next += ""
$next += "## Priorities"
$next += "### Alta"
if($ExitCode -ne 0){ $next += "- Fix build/tests/smoke failures (see Build.log/Test.log/Server.log)" }
if($logSummary['NullReferenceException'] -gt 0 -or $logSummary['SqlException'] -gt 0){ $next += "- Resolve runtime exceptions found in Server.log" }
$next += "### Media"
if($archWarnings.Count -gt 0){ $next += "- Address architecture violations (see ArchitectureReport.md)" }
$next += "### Baja"
if($warnings.Count -gt 0){ $next += "- Address HTML warnings and missing dashboard components" }

Safe-Write $NextActions $next
Write-Success "NextActions saved to $NextActions"

# CI-friendly output (GitHub Actions / Azure DevOps compatible summary)
$ciOut = @()
$ciOut += "## SIGERI Validation CI Summary"
$ciOut += "BuildTimeSeconds: $buildTime"
$ciOut += "GlobalQualityScore: $globalScore"
$ciOut += "ExitCode: $ExitCode"
$ciOut += "Reports: $ReportDir"
Safe-Write $CiSummary $ciOut
Write-Success "CI summary updated: $CiSummary"

# Stop server if requested (only the process we started)
Write-Info "Finalizing: exit code $ExitCode"
if($StopServerOnExit){
    try{
        if($process -and -not $process.HasExited){
            Write-Info "Stopping server process (Id: $($process.Id)) as requested..."
            Stop-Process -Id $process.Id -Force -ErrorAction SilentlyContinue
            Write-Success "Server process $($process.Id) stopped"
        } else {
            Write-Info "Server process already exited or not found"
        }
    }catch{ Write-Warn "Failed to stop process: $_" }
} else {
    Write-Info "Server left running for manual inspection (use -StopServerOnExit to stop automatically)"
}

# Exit with CI-appropriate code
if($ExitCode -eq 0){ Write-Success "Validation completed successfully. Exiting 0"; exit 0 } else { Write-ErrorMsg "Validation completed with issues. Exiting $ExitCode"; exit $ExitCode }
