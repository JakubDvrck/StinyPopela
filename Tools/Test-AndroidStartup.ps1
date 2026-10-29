param(
    [Parameter(Mandatory=$true)][string]$AdbPath,
    [string]$Package = 'com.stinypopela.prototype',
    [int]$MaxPssMb = 1024
)
$ErrorActionPreference = 'Stop'
& $AdbPath shell am force-stop $Package | Out-Null
& $AdbPath shell monkey -p $Package -c android.intent.category.LAUNCHER 1 | Out-Null
if ($LASTEXITCODE -ne 0) { throw 'Unable to start game' }
Start-Sleep -Seconds 3
$appProcessId = (& $AdbPath shell pidof $Package).Trim()
if (-not $appProcessId) { throw 'Game process exited during startup' }
$samples = @()
for ($sample = 0; $sample -lt 10; $sample++) {
    $currentProcessId = (& $AdbPath shell pidof $Package)
    if (-not $currentProcessId -or $currentProcessId.Trim() -ne $appProcessId) {
        throw 'Game process exited or restarted during startup test'
    }
    $memory = & $AdbPath shell dumpsys meminfo $Package
    $match = [regex]::Match(($memory -join "`n"), 'TOTAL PSS:\s*([\d,]+)')
    if (-not $match.Success) { throw 'Unable to read game memory usage' }
    $pssMb = [math]::Round([double]$match.Groups[1].Value.Replace(',', '') / 1024, 1)
    $samples += $pssMb
    if ($pssMb -gt $MaxPssMb) {
        & $AdbPath shell am force-stop $Package | Out-Null
        throw "Game exceeded memory limit: $pssMb MB"
    }
    Start-Sleep -Seconds 3
}
$gameLogs = & $AdbPath logcat -d "--pid=$appProcessId" -v brief Unity:V AndroidRuntime:E '*:S'
if ($gameLogs -match '^E/(Unity|AndroidRuntime)|different serialization layout|FATAL EXCEPTION|OutOfMemoryException') {
    throw 'Startup log contains a serialization, fatal exception or memory error'
}
[pscustomobject]@{
    Package = $Package
    ProcessId = $appProcessId
    SamplesMb = $samples
    PeakPssMb = ($samples | Measure-Object -Maximum).Maximum
    Result = 'Passed process, memory and error-log checks; inspect rendered game separately'
} | ConvertTo-Json -Depth 3
