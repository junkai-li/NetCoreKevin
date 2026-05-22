# check-system-info.ps1 — 获取系统基本信息
Write-Host "=== 系统基本信息 ==="
Write-Host "计算机名: $env:COMPUTERNAME"
Write-Host "操作系统: $([System.Runtime.InteropServices.RuntimeInformation]::OSDescription)"
Write-Host "处理器架构: $([System.Runtime.InteropServices.RuntimeInformation]::ProcessArchitecture)"
Write-Host "逻辑处理器数: $([Environment]::ProcessorCount)"

$os = Get-CimInstance -ClassName Win32_OperatingSystem -ErrorAction SilentlyContinue
if ($os) {
    $totalMemGB = [math]::Round($os.TotalVisibleMemorySize / 1MB, 2)
    $freeMemGB = [math]::Round($os.FreePhysicalMemory / 1MB, 2)
    $usedMemGB = [math]::Round($totalMemGB - $freeMemGB, 2)
    $memUsagePercent = [math]::Round(($usedMemGB / $totalMemGB) * 100, 1)
    Write-Host ""
    Write-Host "=== 内存信息 ==="
    Write-Host "总内存: ${totalMemGB} GB"
    Write-Host "已使用: ${usedMemGB} GB ($memUsagePercent%)"
    Write-Host "可用: ${freeMemGB} GB"
}

Write-Host ""
Write-Host "=== 系统运行时间 ==="
$uptime = (Get-Date) - (Get-CimInstance -ClassName Win32_OperatingSystem).LastBootUpTime
Write-Host "已运行: $($uptime.Days) 天 $($uptime.Hours) 小时 $($uptime.Minutes) 分钟"