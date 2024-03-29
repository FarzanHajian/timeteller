#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["src/TimeTeller/TimeTeller.csproj", "TimeTeller/"]
RUN dotnet restore "TimeTeller/TimeTeller.csproj"
COPY ./src .
WORKDIR "/src/TimeTeller"
RUN dotnet build "TimeTeller.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "TimeTeller.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
ENV ASPNETCORE_URLS=http://0.0.0.0:80
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TimeTeller.dll"]