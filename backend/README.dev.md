# マイグレーションについて

データベースのマイグレーションは[EF Core](https://learn.microsoft.com/ja-jp/ef/core/)で管理しています。

# ツールのローカルインストール

EF Coreを使用するには[dotnet-ef](https://learn.microsoft.com/ja-jp/ef/core/cli/dotnet)というツールを使用します。  
このプロジェクトではツールのバージョンを統一するため、ローカルインストールを行います。  
以下の手順で`dotnet-ef`の指定バージョンをインストールします。

```bash
# カレントディレクトリを移動
cd src/Web

# dotnet-efのローカルインストール
dotnet tool restore
```

# マイグレーション方法

## 1. テーブル構造の変更

`src/Infrastructure`のソースを変更し、テーブル構造を変更します。

## 2. マイグレーションファイルの作成

マイグレーションファイル(テーブル構造の変更履歴)を作成します。

```bash
# 「XXXX」は変更内容がわかるようなマイグレーション名を付ける
dotnet ef migrations add XXXX -p ../Infrastructure/Infrastructure.csproj

# 例:
# dotnet ef migrations add CreateUsersTable -p ../Infrastructure/Infrastructure.csproj
```

## 3. マイグレーションの実行

マイグレーションファイルをもとに、データベースを更新します。

```bash
dotnet ef database update -p ../Infrastructure/Infrastructure.csproj
```
