# 请参阅 https://aka.ms/customizecontainer 以了解如何自定义调试容器，以及 Visual Studio 如何使用此 Dockerfile 生成映像以更快地进行调试。

# 此阶段用于在快速模式(默认为调试配置)下从 VS 运行时
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
# 强制将监听端口设置为 8080
ENV ASPNETCORE_URLS=http://+:8080


# 此阶段用于生成服务项目
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
# 添加下面两行 ↓
ENV DOTNET_CLI_TELEMETRY_OPTOUT=1
ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=1
WORKDIR /src
COPY ["App/WebApi/App.WebApi.csproj", "App/WebApi/"]
COPY ["App/Application/App.Application.csproj", "App/Application/"]
COPY ["App/Domain/App.Domain.csproj", "App/Domain/"]
COPY ["App/AppShare/App.Share.csproj", "App/AppShare/"]
COPY ["Kevin/kevin.Share/kevin.Domain.Share.csproj", "Kevin/kevin.Share/"]
COPY ["Kevin/Kevin.Web.Basics/Kevin.Web.Basics.csproj", "Kevin/Kevin.Web.Basics/"]
COPY ["Kevin/Application/kevin.Application.csproj", "Kevin/Application/"]
COPY ["Kevin/kevin.Module/Kevin.ALL.Module/Kevin.ALL.Module.csproj", "Kevin/kevin.Module/Kevin.ALL.Module/"]
COPY ["Kevin/kevin.Module/kevin.AI.AgentFramework/kevin.AI.AgentFramework.csproj", "Kevin/kevin.Module/kevin.AI.AgentFramework/"]
COPY ["Kevin/kevin.Module/Kevin.Authentication.Jwt/Kevin.Authentication.Jwt.csproj", "Kevin/kevin.Module/Kevin.Authentication.Jwt/"]
COPY ["Kevin/kevin.Module/kevin.Cache/kevin.Cache.csproj", "Kevin/kevin.Module/kevin.Cache/"]
COPY ["Kevin/kevin.Module/Kevin.Common/Kevin.Common.csproj", "Kevin/kevin.Module/Kevin.Common/"]
COPY ["Kevin/kevin.Module/Kevin.HttpApiClients/Kevin.HttpApiClients.csproj", "Kevin/kevin.Module/Kevin.HttpApiClients/"]
COPY ["Kevin/kevin.Module/Kevin.log4Net/Kevin.log4Net.csproj", "Kevin/kevin.Module/Kevin.log4Net/"]
COPY ["Kevin/kevin.Module/kevin.Cap/kevin.Cap.csproj", "Kevin/kevin.Module/kevin.Cap/"]
COPY ["Kevin/kevin.Module/kevin.CodeGenerator/kevin.CodeGenerator.csproj", "Kevin/kevin.Module/kevin.CodeGenerator/"]
COPY ["Kevin/kevin.Module/kevin.Consul/kevin.Consul.csproj", "Kevin/kevin.Module/kevin.Consul/"]
COPY ["Kevin/kevin.Module/Kevin.Cors/Kevin.Cors.csproj", "Kevin/kevin.Module/Kevin.Cors/"]
COPY ["Kevin/kevin.Module/Kevin.CrawlingService/kevin.CrawlingService.csproj", "Kevin/kevin.Module/Kevin.CrawlingService/"]
COPY ["Kevin/kevin.Module/kevin.DistributedLock/kevin.DistributedLock.csproj", "Kevin/kevin.Module/kevin.DistributedLock/"]
COPY ["Kevin/kevin.Module/Kevin.Email/Kevin.Email.csproj", "Kevin/kevin.Module/Kevin.Email/"]
COPY ["Kevin/kevin.Module/kevin.FileStorage/kevin.FileStorage.csproj", "Kevin/kevin.Module/kevin.FileStorage/"]
COPY ["Kevin/kevin.Module/Kevin.Hangfire/Kevin.Hangfire.csproj", "Kevin/kevin.Module/Kevin.Hangfire/"]
COPY ["Kevin/kevin.Module/kevin.Ioc/kevin.Ioc.csproj", "Kevin/kevin.Module/kevin.Ioc/"]
COPY ["Kevin/kevin.Module/kevin.Permission/kevin.Permission.csproj", "Kevin/kevin.Module/kevin.Permission/"]
COPY ["Kevin/Kevin.EntityFrameworkCore/Kevin.EntityFrameworkCore.csproj", "Kevin/Kevin.EntityFrameworkCore/"]
COPY ["Kevin/Domain/kevin.Domain.csproj", "Kevin/Domain/"]
COPY ["Kevin/kevin.Module/kevin.Domain.EventBus/kevin.Domain.EventBus.csproj", "Kevin/kevin.Module/kevin.Domain.EventBus/"]
COPY ["Kevin/kevin.Module/Kevin.SnowflakeId/Kevin.SnowflakeId.csproj", "Kevin/kevin.Module/Kevin.SnowflakeId/"]
COPY ["Kevin/kevin.Module/kevin.RabbitMQ/kevin.RabbitMQ.csproj", "Kevin/kevin.Module/kevin.RabbitMQ/"]
COPY ["Kevin/kevin.Module/Kevin.RAG/Kevin.RAG.csproj", "Kevin/kevin.Module/Kevin.RAG/"]
COPY ["Kevin/kevin.Module/Kevin.SignalR/Kevin.SignalR.csproj", "Kevin/kevin.Module/Kevin.SignalR/"]
COPY ["Kevin/kevin.Module/Kevin.SMS/Kevin.SMS.csproj", "Kevin/kevin.Module/Kevin.SMS/"]
COPY ["Kevin/kevin.Module/Kevin.Versioning.Swagger/Kevin.Api.Versioning.csproj", "Kevin/kevin.Module/Kevin.Versioning.Swagger/"]
COPY ["Kevin/RepositorieRps/kevin.RepositorieRps.csproj", "Kevin/RepositorieRps/"]
COPY ["App/RepositorieRps/App.RepositorieRps.csproj", "App/RepositorieRps/"]
RUN dotnet restore --disable-parallel "App/WebApi/App.WebApi.csproj"
COPY . .
WORKDIR "/src/App/WebApi"
# 强制将监听端口设置为 8080
ENV ASPNETCORE_URLS=http://+:8080

RUN dotnet build "./App.WebApi.csproj" -c $BUILD_CONFIGURATION -o /app/build

# 此阶段用于发布要复制到最终阶段的服务项目
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./App.WebApi.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# 此阶段在生产中使用，或在常规模式下从 VS 运行时使用(在不使用调试配置时为默认值)
FROM base AS final
WORKDIR /app
# 添加这一行：让应用监听所有网络接口的 8080 端口
ENV ASPNETCORE_URLS=http://+:8080
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "App.WebApi.dll"]