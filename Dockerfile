# Utilizar la imagen base de .NET SDK 9.0
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build

# Establecer el directorio de trabajo
WORKDIR /app

# Copiar el archivo de solución y los proyectos
COPY LoopifyFinal/*.csproj ./LoopifyFinal/
WORKDIR /app/LoopifyFinal
RUN dotnet restore

# Copiar el resto de la aplicación y compilar
COPY LoopifyFinal/. .
RUN dotnet publish -c Release -o out

# Utilizar la imagen base de .NET Runtime 9.0
FROM mcr.microsoft.com/dotnet/aspnet:9.0-alpine AS runtime
LABEL org.opencontainers.image.source="https://github.com/tomasyoel/lab01u2tomasyoelweb"

# Establecer el directorio de trabajo
WORKDIR /app
ENV ASPNETCORE_URLS=http://+:80
RUN apk add icu-libs
ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false

# Copiar los archivos compilados desde la etapa de construcción
COPY --from=build /app/LoopifyFinal/out .

# Definir el comando de entrada para ejecutar la aplicación
ENTRYPOINT ["dotnet", "LoopifyFinal.dll"]
```