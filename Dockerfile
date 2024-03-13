FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build-env
WORKDIR /apps

RUN apt-get update && \
      apt-get -y install sudo

RUN useradd -m docker && echo "docker:docker" | chpasswd && adduser docker sudo

COPY . ./
RUN dotnet restore
RUN dotnet publish -c Release -o out

# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:7.0
WORKDIR /apps
COPY --from=build-env /apps/out .
ENTRYPOINT ["dotnet", "HeartbeatServiceTest.dll"]
