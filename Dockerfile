
FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
WORKDIR /app


FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["DragonBlog.csproj", ""]
RUN dotnet restore "./DragonBlog.csproj"
COPY . .
WORKDIR "/src/."
RUN dotnet build "DragonBlog.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "DragonBlog.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
CMD ASPNETCORE_URLS=HTTP://*:$PORT dotnet DragonBlog.dll