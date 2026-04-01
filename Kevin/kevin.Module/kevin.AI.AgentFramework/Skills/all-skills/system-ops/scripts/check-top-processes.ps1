# check-top-processes.ps1 — 查看资源占用 Top 进程
param(
    [int]$Top = 10
)

Write-Host "=== CPU 占用 Top $Top 进程 ==="
Get-Process | Sort-Object CPU -Descending | Select-Object -First $Top | 
    Format-Table -Property @{N='进程名';E={$_.ProcessName}}, 
                           @{N='PID';E={$_.Id}}, 
                           @{N='CPU(s)';E={[math]::Round($_.CPU, 2)}}, 
                           @{N='内存(MB)';E={[math]::Round($_.WorkingSet64/1MB, 1)}} -AutoSize |
    Out-String | Write-Host

Write-Host ""
Write-Host "=== 内存占用 Top $Top 进程 ==="
Get-Process | Sort-Object WorkingSet64 -Descending | Select-Object -First $Top |
    Format-Table -Property @{N='进程名';E={$_.ProcessName}}, 
                           @{N='PID';E={$_.Id}}, 
                           @{N='内存(MB)';E={[math]::Round($_.WorkingSet64/1MB, 1)}},
                           @{N='CPU(s)';E={[math]::Round($_.CPU, 2)}} -AutoSize |
    Out-String | Write-Host