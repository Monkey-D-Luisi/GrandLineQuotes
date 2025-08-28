#!/usr/bin/env bash
set -euo pipefail

# default to Sandbox flavor unless overridden
FLAVOR=${FLAVOR:-Sandbox}

# load shared environment variables and optional flavor-specific overrides
ENV_DIR="$(dirname "$0")/../../DevOps/Docker-Linux/Stack"
set -a
source "$ENV_DIR/.env"
if [ -f "$ENV_DIR/.env.$FLAVOR" ]; then
  source "$ENV_DIR/.env.$FLAVOR"
fi
set +a

export Minio__Endpoint="$MINIO_ENDPOINT"
export Minio__AccessKey="$MINIO_ROOT_USER"
export Minio__SecretKey="$MINIO_ROOT_PASSWORD"
export Minio__Secure="$MINIO_SECURE"
export MinioPublic__Endpoint="$MINIO_PUBLIC_ENDPOINT"
export MinioPublic__AccessKey="$MINIO_ROOT_USER"
export MinioPublic__SecretKey="$MINIO_ROOT_PASSWORD"
export MinioPublic__Secure="$MINIO_PUBLIC_SECURE"
export Api__HttpHost="$API_HTTP_HOST"
export PlayIntegrity__SessionSecret="$PLAYINTEGRITY_SESSION_SECRET"
export LuckyPenny__LicenseKey="$LUCKYPENNY_LICENSE_KEY"
export ConnectionStrings__grandlinequotes="server=localhost;port=3306;database=grandlinequotes;user=${MYSQL_USER};password=${MYSQL_PASSWORD};Allow User Variables=True;"

# install .NET SDK 8.0 if missing
if ! command -v dotnet >/dev/null 2>&1; then
  wget -q -O /tmp/packages-microsoft-prod.deb https://packages.microsoft.com/config/ubuntu/24.04/packages-microsoft-prod.deb
  dpkg -i /tmp/packages-microsoft-prod.deb
  rm -f /tmp/packages-microsoft-prod.deb
  apt-get update
  apt-get install -y dotnet-sdk-8.0
fi

# install MariaDB if missing and start service
if ! command -v mysql >/dev/null 2>&1; then
  apt-get update
  apt-get install -y mariadb-server
fi
service mariadb start

# create test database and user
MYSQL_HOST= MYSQL_PWD= mysql -u root -e "CREATE USER IF NOT EXISTS '${MYSQL_USER}'@'%' IDENTIFIED BY '${MYSQL_PASSWORD}'; GRANT ALL PRIVILEGES ON grandlinequotes.* TO '${MYSQL_USER}'@'%'; FLUSH PRIVILEGES;"
MYSQL_HOST= MYSQL_PWD= mysql -u root < "$(dirname "$0")/setup-test-db.sql"

# install MinIO server and client
pkill -f "/tmp/minio" >/dev/null 2>&1 || true
rm -rf /tmp/minio /tmp/minio-data /tmp/mc
wget -q -O /tmp/minio https://dl.min.io/server/minio/release/linux-amd64/minio
chmod +x /tmp/minio
wget -q -O /tmp/mc https://dl.min.io/client/mc/release/linux-amd64/mc
chmod +x /tmp/mc
# start MinIO and create bucket
MINIO_ROOT_USER=$MINIO_ROOT_USER MINIO_ROOT_PASSWORD=$MINIO_ROOT_PASSWORD /tmp/minio server /tmp/minio-data --console-address :9090 >/tmp/minio.log 2>&1 &
sleep 5
/tmp/mc alias set local http://127.0.0.1:9000 "$MINIO_ROOT_USER" "$MINIO_ROOT_PASSWORD" >/dev/null
/tmp/mc mb local/quotes >/dev/null 2>&1 || true

# install JDK and Android SDK if missing
if ! command -v java >/dev/null 2>&1 || ! command -v unzip >/dev/null 2>&1; then
  apt-get update
fi
if ! command -v java >/dev/null 2>&1; then
  apt-get install -y openjdk-17-jdk
fi
if ! command -v unzip >/dev/null 2>&1; then
  apt-get install -y unzip
fi
if ! command -v sdkmanager >/dev/null 2>&1; then
  mkdir -p /opt/android
  cd /opt/android
  ANDROID_CLI_ZIP=commandlinetools-linux-11076708_latest.zip
  ANDROID_CLI_URL=https://dl.google.com/android/repository/${ANDROID_CLI_ZIP}
  for attempt in 1 2 3; do
    if curl -fsSL --retry 3 --retry-delay 2 -o "$ANDROID_CLI_ZIP" "$ANDROID_CLI_URL" \
      && unzip -q "$ANDROID_CLI_ZIP"; then
      break
    fi
    echo "Retrying Android command line tools download ($attempt)..."
    rm -f "$ANDROID_CLI_ZIP"
    if [ "$attempt" -eq 3 ]; then
      echo "Failed to download Android command line tools" >&2
      exit 1
    fi
  done
  mv cmdline-tools cmdline-tools-temp
  mkdir -p cmdline-tools/latest
  mv cmdline-tools-temp/* cmdline-tools/latest/
  rm -rf cmdline-tools-temp
  export ANDROID_HOME=/opt/android
  export ANDROID_SDK_ROOT=/opt/android
  export PATH=$PATH:$ANDROID_HOME/cmdline-tools/latest/bin:$ANDROID_HOME/platform-tools
  yes | sdkmanager --licenses >/dev/null || true
  sdkmanager --uninstall 'build-tools;35.0.0' >/dev/null 2>&1 || true
  sdkmanager 'platform-tools' 'platforms;android-36' 'build-tools;36.0.0' 'platforms;android-35' 'build-tools;35.0.0' >/dev/null
  cd - >/dev/null
else
  export ANDROID_HOME=$(dirname $(dirname $(command -v sdkmanager)))
  export ANDROID_SDK_ROOT=$ANDROID_HOME
  export PATH=$PATH:$ANDROID_HOME/cmdline-tools/latest/bin:$ANDROID_HOME/platform-tools
fi

# build once to avoid file locking when running multiple hosts
dotnet build >/tmp/build.log

# start APIs using prebuilt assemblies
ASPNETCORE_ENVIRONMENT=$FLAVOR ASPNETCORE_URLS=http://localhost:5023 dotnet run --project src/Front/Api --no-launch-profile --no-build >/tmp/api.log 2>&1 &
ASPNETCORE_ENVIRONMENT=$FLAVOR ASPNETCORE_URLS=http://localhost:5024 dotnet run --project src/Front/Api.Public --no-launch-profile --no-build >/tmp/api-public.log 2>&1 &
sleep 5

# run .NET tests without rebuilding
ASPNETCORE_ENVIRONMENT=$FLAVOR DOTNET_ENVIRONMENT=$FLAVOR dotnet test --no-build

# run Kotlin Multiplatform tests
cd src/Front/Ui
# ensure Android SDK path is configured for Gradle
if [ -n "${ANDROID_HOME:-}" ]; then
  if [ ! -f local.properties ]; then
    echo "sdk.dir=${ANDROID_HOME}" > local.properties
  elif ! grep -q '^sdk.dir=' local.properties; then
    echo "sdk.dir=${ANDROID_HOME}" >> local.properties
  fi
fi

# stop any running Gradle daemons and run tests without daemon or configuration cache
unset ASPNETCORE_ENVIRONMENT DOTNET_ENVIRONMENT
sh gradlew --stop >/tmp/gradle-stop.log 2>&1 || true
sh gradlew \
  -Dorg.gradle.configuration-cache=false \
  -Dkotlin.compiler.execution.strategy=in-process \
  clean :composeApp:assembleSandboxDebug :composeApp:testSandboxDebugUnitTest --no-daemon
cd - >/dev/null
