#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
#安装 libgdiplus 以解决 System.Drawing 在 linux 下无法运行的问题
RUN apt-get update && apt-get -y install libgdiplus 
#设置时区为中国上海
ENV TZ=Asia/Shanghai
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["Quartz/TaskQuartz.csproj", ""]
RUN dotnet restore "TaskQuartz.csproj"
COPY . .
WORKDIR "/src/Quartz"
RUN dotnet build "TaskQuartz.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "TaskQuartz.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TaskQuartz.dll"]