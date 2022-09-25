#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["SoluiNet.DevTools.UI.Blazor/SoluiNet.DevTools.UI.Blazor.csproj", "SoluiNet.DevTools.UI.Blazor/"]
RUN dotnet restore "SoluiNet.DevTools.UI.Blazor/SoluiNet.DevTools.UI.Blazor.csproj"
COPY . .
WORKDIR "/src/SoluiNet.DevTools.UI.Blazor"
RUN dotnet build "SoluiNet.DevTools.UI.Blazor.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "SoluiNet.DevTools.UI.Blazor.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "SoluiNet.DevTools.UI.Blazor.dll"]