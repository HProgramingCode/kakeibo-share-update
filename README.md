# kakeibo-share-update

家族向け割り勘・精算PWA「かぞく精算帳」。

## 構成

- `frontend/` — Next.js（App Router / TypeScript / PWA）
- `backend/` — ASP.NET Core Web API（DDD 4層・EF Core・PostgreSQL）※Phase 1 以降で実装
- `docker-compose.yml` — 開発環境（db / backend / frontend）

## 開発環境の起動

Docker Desktop が必要です。

```bash
# PostgreSQL のみ起動（既定）
docker compose up -d db

# 全体起動（backend/frontend 実装後）
docker compose --profile app up
```

- コンテナ間は `localhost` ではなく **サービス名**（`db` / `backend`）で通信します。
- DB のホスト公開ポートは **55432**（ローカル 5432 と衝突回避）。コンテナ内は 5432 です。
