#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
#安装 libgdiplus 以解决 System.Drawing 在 linux 下无法运行的问题
RUN apt-get update && apt-get -y install libgdiplus 
#设置时区为中国上海
ENV TZ=Asia/Shanghai
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["AuthorizationService/Kevin.AuthorizationService.csproj", "AuthorizationService/"]
COPY ["Common/Kevin.Common.csproj", "Common/"]
COPY ["kevin.Share/kevin.Share.csproj", "kevin.Share/"]
COPY ["Repository/Kevin.EntityFrameworkCore.csproj", "Repository/"]
COPY ["kevin.Domain/kevin.Domain.csproj", "kevin.Domain/"]
RUN dotnet restore "AuthorizationService/Kevin.AuthorizationService.csproj"
COPY . .
WORKDIR "/src/AuthorizationService"
RUN dotnet build "Kevin.AuthorizationService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Kevin.AuthorizationService.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Kevin.AuthorizationService.dll"]