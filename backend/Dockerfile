
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

WORKDIR /src

RUN dotnet new web -n DemoApi --no-https

COPY Program.cs DemoApi/Program.cs

WORKDIR /src/DemoApi

RUN dotnet publish -c Release -o /out

FROM mcr.microsoft.com/dotnet/aspnet:8.0

WORKDIR /app

COPY --from=build /out .

ENV ASPNETCORE_URLS=http://+:8080

EXPOSE 8080

ENTRYPOINT ["dotnet", "DemoApi.dll"]
