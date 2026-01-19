FROM mcr.microsoft.com/playwright/dotnet:v1.57.0-noble

WORKDIR /app

RUN apt-get update \
    && apt-get install -y wget ca-certificates \
    && wget https://dot.net/v1/dotnet-install.sh \
    && chmod +x dotnet-install.sh \
    && ./dotnet-install.sh --version 10.0.102 --install-dir /usr/share/dotnet \
    && rm dotnet-install.sh

COPY src ./src
COPY tests ./tests
COPY app.slnx .
COPY Directory.Build.props .
COPY global.json .
COPY nuget.config .

RUN dotnet restore --locked-mode
