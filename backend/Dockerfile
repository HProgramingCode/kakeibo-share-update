# バックエンド開発用イメージ（ホットリロード）。中身は Phase 1 で実装する。
FROM mcr.microsoft.com/dotnet/sdk:8.0
WORKDIR /app

# EF のCLI（マイグレーション用）をコンテナにも入れておく
RUN dotnet tool install --global dotnet-ef --version 8.*
ENV PATH="$PATH:/root/.dotnet/tools"

# ソースは compose の volume で同期する（開発用なのでここではコピーしない）。
# ファイル変更を検知して自動で再起動する
CMD ["dotnet", "watch", "run", "--project", "src/KakeiboShare.Api", "--urls", "http://+:8080"]
