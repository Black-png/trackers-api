#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["TT.Core.Api/TT.Core.Api.csproj", "TT.Core.Api/"]
COPY ["TT.Core.Repository.Sql/TT.Core.Repository.Sql.csproj", "TT.Core.Repository.Sql/"]
COPY ["TT.Core.Models/TT.Core.Models.csproj", "TT.Core.Models/"]
COPY ["TT.Core.Services/TT.Core.Services.csproj", "TT.Core.Services/"]
COPY ["TT.Core.Repository/TT.Core.Repository.csproj", "TT.Core.Repository/"]
RUN dotnet restore "TT.Core.Api/TT.Core.Api.csproj"
COPY . .
WORKDIR "/src/TT.Core.Api"
RUN dotnet build "TT.Core.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "TT.Core.Api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
RUN mkdir -p /home/https
COPY --from=publish /app/publish .
COPY --from=publish /app/publish/_.thingtrax.com_private_key.pfx /home/https
ENTRYPOINT ["dotnet", "TT.Core.Api.dll"]