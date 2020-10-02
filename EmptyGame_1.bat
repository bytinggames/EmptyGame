
md EmptyGame
md JuliHelper
md MonoGame.BytingGames

cd EmptyGame
start "" git clone https://github.com/bytinggames/EmptyGame.git
cd ..

cd JuliHelper
start "" git clone https://github.com/bytinggames/JuliHelperShared.git
cd ..

cd MonoGame.BytingGames
git clone https://github.com/bytinggames/MonoGame.git
cd MonoGame
git submodule update --init
dotnet build ".\Tools\MonoGame.Content.Builder\MonoGame.Content.Builder.csproj" -c Release


xcopy "C:\Program Files (x86)\MSBuild\MonoGame\v3.0\Tools" "C:\Program Files (x86)\MSBuild\MonoGame\v3.0\Tools_backup" /E /H /C /I /Y
xcopy /s /H /Y /d ".\Artifacts\MonoGame.Content.Builder\Release" "C:\Program Files (x86)\MSBuild\MonoGame\v3.0\Tools"

echo Success!