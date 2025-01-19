curl -sSL https://dot.net/v1/dotnet-install.sh > dotnet-install.sh 
chmod +x dotnet-install.sh 
./dotnet-install.sh -c 9.0 --version 9.0.100 -InstallDir ./dotnet9 
./dotnet9/dotnet --version 
./dotnet9/dotnet tool install Excubo.WebCompiler --global
./dotnet9/dotnet publish CodeBeam.MudBlazor.Extensions.Docs.Wasm/CodeBeam.MudBlazor.Extensions.Docs.Wasm.csproj -c Release -o output
