FROM node:22-alpine
WORKDIR /app

# pnpm を有効化（このプロジェクトは pnpm）
RUN corepack enable

# 依存だけ先に入れる（lockfile が変わらない限りキャッシュが効く）
COPY package.json pnpm-lock.yaml ./
RUN pnpm install

# ソースは volume で同期するので CMD のみ。開発サーバーを起動
EXPOSE 3000
CMD ["pnpm", "dev"]
