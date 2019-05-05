.\scripts\start-mysql-server.ps1
.\scripts\create-wetr-schema.ps1
$cmd1 = "docker exec -i mysql mysql -u root --default-character-set=utf8 WetrDB < "
$cmd2 = "docker exec -i mysql mysql -u root --default-character-set=utf8 WetrDB < "
$cmd1 += Get-Location
$cmd2 += Get-Location
$cmd1 += "\scripts\insert_test_data.sql"
$cmd2 += "\DataSources\measurements\measurement_small.sql"
cmd.exe /c $cmd1
cmd.exe /c $cmd2