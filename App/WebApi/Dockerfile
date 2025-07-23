# 请参阅 https://aka.ms/customizecontainer 以了解如何自定义调试容器，以及 Visual Studio 如何使用此 Dockerfile 生成映像以更快地进行调试。

# 此阶段用于在快速模式(默认为调试配置)下从 VS 运行时
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
#安装 libgdiplus 以解决 System.Drawing 在 linux 下无法运行的问题
#RUN apt-get update && apt-get -y install libgdiplus 
#设置时区为中国上海
ENV TZ=Asia/Shanghai
EXPOSE 80


# 此阶段用于生成服务项目
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["WebApi/App.WebApi.csproj", "WebApi/"]
COPY ["Service/App.Application.csproj", "Service/"]
COPY ["AppShare/App.Share.csproj", "AppShare/"]
COPY ["DomainServices/App.Domain.csproj", "DomainServices/"]
COPY ["kevin.Domain/kevin.Domain.csproj", "kevin.Domain/"]
COPY ["Common/Kevin.Common.csproj", "Common/"]
COPY ["kevin.Share/kevin.Domain.Share.csproj", "kevin.Share/"]
COPY ["kevin.Domain.EventBus/kevin.Domain.EventBus.csproj", "kevin.Domain.EventBus/"]
COPY ["Repositories/App.RepositorieRps.csproj", "Repositories/"]
COPY ["Repository/Kevin.EntityFrameworkCore.csproj", "Repository/"]
COPY ["Web/Kevin.Web.csproj", "Web/"]
COPY ["CrawlingService/kevin.CrawlingService.csproj", "CrawlingService/"]
COPY ["kevin.Cache/kevin.Cache.csproj", "kevin.Cache/"]
COPY ["kevin.Cap/kevin.Cap.csproj", "kevin.Cap/"]
COPY ["kevin.Consul/kevin.Consul.csproj", "kevin.Consul/"]
COPY ["Kevin.Cors/Kevin.Cors.csproj", "Kevin.Cors/"]
COPY ["kevin.DistributedLock/kevin.DistributedLock.csproj", "kevin.DistributedLock/"]
COPY ["kevin.FileStorage/kevin.FileStorage.csproj", "kevin.FileStorage/"]
COPY ["Kevin.HttpApiClients/Kevin.HttpApiClients.csproj", "Kevin.HttpApiClients/"]
COPY ["kevin.Ioc/kevin.Ioc.csproj", "kevin.Ioc/"]
COPY ["Kevin.log4Net/Kevin.log4Net.csproj", "Kevin.log4Net/"]
COPY ["kevin.Permission/kevin.Permission.csproj", "kevin.Permission/"]
COPY ["Kevin.SignalR/Kevin.SignalR.csproj", "Kevin.SignalR/"]
COPY ["Kevin.SMS/Kevin.SMS.csproj", "Kevin.SMS/"]
COPY ["Kevin_MCP_Server/Kevin_MCP_Server.csproj", "Kevin_MCP_Server/"]
RUN dotnet restore "./WebApi/App.WebApi.csproj"
COPY . .
WORKDIR "/src/WebApi"
RUN dotnet build "./App.WebApi.csproj" -c $BUILD_CONFIGURATION -o /app/build

# 此阶段用于发布要复制到最终阶段的服务项目
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./App.WebApi.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# 此阶段在生产中使用，或在常规模式下从 VS 运行时使用(在不使用调试配置时为默认值)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "App.WebApi.dll"]