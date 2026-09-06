# Exemplo/ponto de partida — ajustar quando o projeto .NET existir de verdade.
# Ajustar "Backend.csproj" / "Backend.dll" para o nome real do projeto.

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY . .
RUN dotnet restore
RUN dotnet publish -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .
EXPOSE 8080
# TODO: montar certificado HTTPS (ver "Instructions" no README)
ENTRYPOINT ["dotnet", "Backend.dll"]
