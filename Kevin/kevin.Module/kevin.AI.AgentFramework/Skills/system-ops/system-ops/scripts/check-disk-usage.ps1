# check-disk-usage.ps1 — 检查磁盘使用情况
Write-Host "=== 磁盘使用情况 ==="
Get-CimInstance -ClassName Win32_LogicalDisk -Filter "DriveType=3" | ForEach-Object {
    $totalGB = [math]::Round($_.Size / 1GB, 2)
    $freeGB = [math]::Round($_.FreeSpace / 1GB, 2)
    $usedGB = [math]::Round($totalGB - $freeGB, 2)
    $usagePercent = if ($totalGB -gt 0) { [math]::Round(($usedGB / $totalGB) * 100, 1) } else { 0 }
    
    $status = if ($usagePercent -gt 90) { "🔴 严重" }
              elseif ($usagePercent -gt 70) { "🟡 警告" }
              else { "🟢 正常" }
    
    Write-Host ""
    Write-Host "驱动器 $($_.DeviceID)"
    Write-Host "  总容量: ${totalGB} GB"
    Write-Host "  已使用: ${usedGB} GB ($usagePercent%)"
    Write-Host "  可用: ${freeGB} GB"
    Write-Host "  状态: $status"
}