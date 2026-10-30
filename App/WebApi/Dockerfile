# 请参阅 https://aka.ms/customizecontainer 以了解如何自定义调试容器，以及 Visual Studio 如何使用此 Dockerfile 生成映像以更快地进行调试。

# 运行阶段：仅包含 ASP.NET 运行时
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 8080
# 强制将监听端口设置为 8080（对外由反向代理/端口映射暴露）
ENV ASPNETCORE_URLS=http://+:8080

# 编译阶段：SDK 镜像内还原 + 发布
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
ENV DOTNET_CLI_TELEMETRY_OPTOUT=1
ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=1
WORKDIR /src

# 先只拷贝各项目文件（不拷源码），让 NuGet 还原结果可以被缓存。
# 新增/删除项目时必须同步维护这份清单，否则 restore 会找不到项目。
# 中央包版本管理文件，restore 时必需
COPY ["Directory.Packages.props", "./"]
COPY ["App/WebApi/App.WebApi.csproj", "App/WebApi/"]
COPY ["App/Application/App.Application.csproj", "App/Application/"]
COPY ["App/Domain/App.Domain.csproj", "App/Domain/"]
COPY ["App/AppShare/App.Share.csproj", "App/AppShare/"]
COPY ["App/RepositorieRps/App.RepositorieRps.csproj", "App/RepositorieRps/"]
COPY ["Kevin/Application/kevin.Application.csproj", "Kevin/Application/"]
COPY ["Kevin/Domain/kevin.Domain.csproj", "Kevin/Domain/"]
COPY ["Kevin/kevin.Share/kevin.Domain.Share.csproj", "Kevin/kevin.Share/"]
COPY ["Kevin/Kevin.Web.Basics/Kevin.Web.Basics.csproj", "Kevin/Kevin.Web.Basics/"]
COPY ["Kevin/Kevin.EntityFrameworkCore/Kevin.EntityFrameworkCore.csproj", "Kevin/Kevin.EntityFrameworkCore/"]
COPY ["Kevin/RepositorieRps/kevin.RepositorieRps.csproj", "Kevin/RepositorieRps/"]
COPY ["Kevin/kevin.Module/Kevin.ALL.Module/Kevin.ALL.Module.csproj", "Kevin/kevin.Module/Kevin.ALL.Module/"]
COPY ["Kevin/kevin.Module/kevin.AI.AgentFramework/kevin.AI.AgentFramework.csproj", "Kevin/kevin.Module/kevin.AI.AgentFramework/"]
COPY ["Kevin/kevin.Module/Kevin.Asr/Kevin.Asr.csproj", "Kevin/kevin.Module/Kevin.Asr/"]
COPY ["Kevin/kevin.Module/Kevin.Authentication.Jwt/Kevin.Authentication.Jwt.csproj", "Kevin/kevin.Module/Kevin.Authentication.Jwt/"]
COPY ["Kevin/kevin.Module/kevin.Cache/kevin.Cache.csproj", "Kevin/kevin.Module/kevin.Cache/"]
COPY ["Kevin/kevin.Module/kevin.Cap/kevin.Cap.csproj", "Kevin/kevin.Module/kevin.Cap/"]
COPY ["Kevin/kevin.Module/kevin.CodeGenerator/kevin.CodeGenerator.csproj", "Kevin/kevin.Module/kevin.CodeGenerator/"]
COPY ["Kevin/kevin.Module/Kevin.Common/Kevin.Common.csproj", "Kevin/kevin.Module/Kevin.Common/"]
COPY ["Kevin/kevin.Module/kevin.Consul/kevin.Consul.csproj", "Kevin/kevin.Module/kevin.Consul/"]
COPY ["Kevin/kevin.Module/Kevin.Cors/Kevin.Cors.csproj", "Kevin/kevin.Module/Kevin.Cors/"]
COPY ["Kevin/kevin.Module/kevin.DistributedLock/kevin.DistributedLock.csproj", "Kevin/kevin.Module/kevin.DistributedLock/"]
COPY ["Kevin/kevin.Module/kevin.Domain.EventBus/kevin.Domain.EventBus.csproj", "Kevin/kevin.Module/kevin.Domain.EventBus/"]
COPY ["Kevin/kevin.Module/Kevin.Email/Kevin.Email.csproj", "Kevin/kevin.Module/Kevin.Email/"]
COPY ["Kevin/kevin.Module/kevin.FileStorage/kevin.FileStorage.csproj", "Kevin/kevin.Module/kevin.FileStorage/"]
COPY ["Kevin/kevin.Module/Kevin.Hangfire/Kevin.Hangfire.csproj", "Kevin/kevin.Module/Kevin.Hangfire/"]
COPY ["Kevin/kevin.Module/Kevin.HttpApiClients/Kevin.HttpApiClients.csproj", "Kevin/kevin.Module/Kevin.HttpApiClients/"]
COPY ["Kevin/kevin.Module/kevin.Ioc/kevin.Ioc.csproj", "Kevin/kevin.Module/kevin.Ioc/"]
COPY ["Kevin/kevin.Module/Kevin.log4Net/Kevin.log4Net.csproj", "Kevin/kevin.Module/Kevin.log4Net/"]
COPY ["Kevin/kevin.Module/kevin.Permission/kevin.Permission.csproj", "Kevin/kevin.Module/kevin.Permission/"]
COPY ["Kevin/kevin.Module/kevin.RabbitMQ/kevin.RabbitMQ.csproj", "Kevin/kevin.Module/kevin.RabbitMQ/"]
COPY ["Kevin/kevin.Module/Kevin.RAG/Kevin.RAG.csproj", "Kevin/kevin.Module/Kevin.RAG/"]
COPY ["Kevin/kevin.Module/Kevin.SignalR/Kevin.SignalR.csproj", "Kevin/kevin.Module/Kevin.SignalR/"]
COPY ["Kevin/kevin.Module/Kevin.SMS/Kevin.SMS.csproj", "Kevin/kevin.Module/Kevin.SMS/"]
COPY ["Kevin/kevin.Module/Kevin.SnowflakeId/Kevin.SnowflakeId.csproj", "Kevin/kevin.Module/Kevin.SnowflakeId/"]
COPY ["Kevin/kevin.Module/Kevin.Versioning.Swagger/Kevin.Api.Versioning.csproj", "Kevin/kevin.Module/Kevin.Versioning.Swagger/"]

RUN dotnet restore "App/WebApi/App.WebApi.csproj"

# 再拷贝全部源码并发布
COPY . .
WORKDIR "/src/App/WebApi"
RUN dotnet publish "./App.WebApi.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# 最终阶段
FROM base AS final
WORKDIR /app
# 运行用户需要对日志/上传目录有写权限（这两个目录运行时挂载数据卷）
RUN mkdir -p /app/Log /app/KevinFlies
COPY --from=build /app/publish .
RUN chown -R $APP_UID:$APP_UID /app
USER $APP_UID
ENTRYPOINT ["dotnet", "App.WebApi.dll"]
