# TaskFlow

![Unit Test CI](https://github.com/taserbeat/task-flow/actions/workflows/unit-test.yml/badge.svg)

チームで利用できるカンバン方式でシンプルなタスク管理Webアプリです。

---

## スクリーンショット

https://github.com/user-attachments/assets/0c6feb3b-5086-4a36-92b8-8884019ce36b

https://github.com/user-attachments/assets/445f619f-5a05-4a5d-ad51-7597fbbbc20c

https://github.com/user-attachments/assets/a31ca68e-702b-4a38-babf-cfe428ca7165

![デモ画像1](./docs/screenshots/demo1.png)

---

## 機能一覧

- 認証
    - ログイン、ログアウト
- テナント管理
    - 1つの組織(グループ) を 1テナントとみなして、複数のチームを管理できます
- ユーザー管理・権限設定
- タスク管理
    - ボード、列、タスクの作成・編集・削除

---

## 技術スタック

### フロントエンド

- React
- TypeScript
- TailWindCSS

### バックエンド

- ASP.NET Core (.NET 10)
- Entity Framework Core
- XUnit (Unitテスト、Integrationテスト)

### データベース

- PostgreSQL

---

## 今後の機能追加案

- [ ] ダッシュボード機能 (期限・優先度・プロジェクトなどで集計)
- [ ] 通知機能 (期限が近づいている/過ぎたタスクの通知)
- [ ] トークン認証 / APIキー認証
- [ ] レスポンシブUI対応

---
