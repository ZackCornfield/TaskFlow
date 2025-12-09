FROM mcr.microsoft.com/dotnet/aspnet:10.0-nanoserver-ltsc2022 AS base
WORKDIR /app
EXPOSE 5287

ENV ASPNETCORE_URLS=http://+:5287

FROM mcr.microsoft.com/dotnet/sdk:10.0-nanoserver-ltsc2022 AS build
ARG configuration=Release
WORKDIR /src
COPY ["TaskFlowApi.csproj", "./"]
RUN dotnet restore "TaskFlowApi.csproj"
COPY . .
WORKDIR "/src/."
RUN dotnet build "TaskFlowApi.csproj" -c $configuration -o /app/build

FROM build AS publish
ARG configuration=Release
RUN dotnet publish "TaskFlowApi.csproj" -c $configuration -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TaskFlowApi.dll"]
