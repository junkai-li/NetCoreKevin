# 请参阅 https://aka.ms/customizecontainer 以了解如何自定义调试容器，以及 Visual Studio 如何使用此 Dockerfile 生成映像以更快地进行调试。

# 此阶段用于在快速模式(默认为调试配置)下从 VS 运行时
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base

USER root

#修改镜像内部软件源采用阿里镜像
RUN sed -i 's@http://deb.debian.org@https://mirrors.aliyun.com@g' /etc/apt/sources.list.d/debian.sources

# 安装 SkiaSharp 依赖（libfontconfig1）并清理缓存
RUN apt-get update && apt-get -y install libfontconfig1 \
    && apt-get clean \
    && rm -rf /var/lib/apt/lists/*

#设置时区为中国上海
ENV TZ=Asia/Shanghai

USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

# 此阶段用于生成服务项目
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY . .
WORKDIR "/src/Presentation/App.WebApi"
RUN dotnet restore "App.WebApi.csproj"
RUN dotnet build "App.WebApi.csproj" -c Release -o /app/build


# 此阶段用于发布要复制到最终阶段的服务项目
FROM build AS publish
RUN dotnet publish "App.WebApi.csproj" -c Release -r linux-x64 -o /app/publish /p:UseAppHost=false


# 此阶段在生产中使用，或在常规模式下从 VS 运行时使用(在不使用调试配置时为默认值)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "App.WebApi.dll"]