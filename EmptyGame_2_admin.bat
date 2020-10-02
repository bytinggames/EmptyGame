
xcopy /E /H /C /I /Y "C:\Program Files (x86)\MSBuild\MonoGame\v3.0\Tools" "C:\Program Files (x86)\MSBuild\MonoGame\v3.0\Tools_backup"
xcopy /s /H /Y /d "MonoGame.BytingGames\MonoGame\Artifacts\MonoGame.Content.Builder\Release" "C:\Program Files (x86)\MSBuild\MonoGame\v3.0\Tools"

echo Success!