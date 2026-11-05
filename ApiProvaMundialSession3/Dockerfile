FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ApiProvaMundialSession3.csproj ./
RUN dotnet restore ApiProvaMundialSession3.csproj
COPY . ./
RUN dotnet publish ApiProvaMundialSession3.csproj -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/publish ./
RUN mkdir -p /app/temp
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080
ENTRYPOINT ["dotnet", "ApiProvaMundialSession3.dll"]
