FROM mcr.microsoft.com/playwright/dotnet:v1.57.0-noble

WORKDIR /app

RUN apt-get update \
    && apt-get install -y wget ca-certificates jq \
    && wget https://dot.net/v1/dotnet-install.sh \
    && chmod +x dotnet-install.sh

COPY global.json .

RUN SDK_VERSION=$(jq -r '.sdk.version' global.json) \
    && ./dotnet-install.sh --version $SDK_VERSION --install-dir /usr/share/dotnet \
    && rm dotnet-install.sh

COPY src ./src
COPY tests ./tests
COPY app.slnx .
COPY Directory.Build.props .
COPY nuget.config .

RUN dotnet restore --locked-mode
