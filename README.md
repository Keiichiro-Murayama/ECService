# ECService

## フルネス研修総合演習課題「ECサイト」

# API一覧

| No  | API名                   | メソッド | エンドポイント                      | 備考                                   |
| --- | ----------------------- | -------- | ----------------------------------- | -------------------------------------- |
| 1   | 担当者アカウント登録API | POST     | `/api/admin/accounts`               |                                        |
| 2   | 未登録社員取得API       | GET      | `/api/admin/employees/unregistered` |                                        |
| 3   | 商品カテゴリ一覧取得API | GET      | `/api/admin/categories`             |                                        |
| 4   | 新商品登録API           | POST     | `/api/admin/products`               |                                        |
| 5   | 商品検索API             | GET      | `/api/admin/products`               |                                        |
| 6   | 商品詳細取得API         | GET      | `/api/admin/products/{productUuid}` | (商品修正画面・商品削除確認画面で使用) |
| 7   | 商品修正API             | PUT      | `/api/admin/products/{productUuid}` |                                        |
| 8   | 商品削除API             | DELETE   | `/api/admin/products/{productUuid}` |                                        |
| 9   | 商品カテゴリ登録API     | POST     | `/api/admin/categories`             |                                        |
| 10  | ログインAPI             | POST     | `/api/admin/login`                  |                                        |

---

## 共通エラーレスポンス

システム全体（No.1〜No.10すべて）で予期せぬエラーやサーバー内部エラーが発生した場合は、共通で以下のレスポンスを返却する。

#### 500 Internal Server Error（予期せぬエラー）

```json
{
  "message": "InternalException: サーバー内部で予期せぬエラーが発生しました。"
}
```

---

## 認証が必要なAPI

以下のAPIはJWT認証が必須です。リクエストのHTTPヘッダーに認証トークンを含める必要があります。

### 認証が必須のAPI

- No.1: 担当者アカウント登録API
- No.2: 未登録社員取得API
- No.3: 商品カテゴリ一覧取得API
- No.4: 新商品登録API
- No.5: 商品検索API
- No.6: 商品詳細取得API
- No.7: 商品修正API
- No.8: 商品削除API
- No.9: 商品カテゴリ登録API

### リクエストヘッダー

すべての認証が必須のAPIに対して、以下のヘッダーを含める：

```
Authorization: Bearer {JWTトークン}
```

### 認証エラーレスポンス

#### 401 Unauthorized（認証なし・無効なトークン）

```json
{
  "message": "認証が必要です。ログインしてください。"
}
```

---

## 1.担当者アカウント登録API

| 項目           | 内容                                |
| -------------- | ----------------------------------- |
| エンドポイント | `/api/admin/accounts`               |
| HTTPメソッド   | `POST`                              |
| コントローラー | `RegisterEmployeeAccountController` |
| メソッド       | `RegisterEmployee`                  |

### リクエスト

```json
{
  "employeeId": 1,
  "accountName": "yamada01",
  "password": "abc123"
}
```

### エラーレスポンス

#### 400 Bad Request（未入力エラー）

```json
{
  "message": "employeeId、accountName、passwordを入力してください。"
}
```

#### 400 Bad Request（入力値エラー）

```json
{
  "message": "入力値に不備があります。"
}
```

#### 404 Not Found（社員IDが存在しない）

```json
{
  "message": "指定された社員IDが存在しません。"
}
```

#### 409 Conflict（アカウント重複）

```json
{
  "message": "このアカウント名は既に登録されています。"
}
```

## 2.未登録社員取得API

| 項目           | 内容                                |
| -------------- | ----------------------------------- |
| エンドポイント | `/api/admin/employees/unregistered` |
| HTTPメソッド   | `GET`                               |
| コントローラー | `GetUnregisterdEmployeeController`  |
| メソッド       | `GetUnregisteredEmployees`          |

### レスポンス

```json
[
  {
    "employeeId": 1,
    "employeeName": "山田太郎"
  },
  {
    "employeeId": 2,
    "employeeName": "佐藤花子"
  }
]
```

## 3.商品カテゴリ一覧取得API

| 項目           | 内容                      |
| -------------- | ------------------------- |
| エンドポイント | `/api/admin/categories`   |
| HTTPメソッド   | `GET`                     |
| コントローラー | `GetCategoriesController` |
| メソッド       | `GetCategories`           |

### レスポンス

```json
[
  {
    "categoryUuid": "550e8400-e29b-41d4-a716-44665544000a",
    "categoryName": "文房具"
  },
  {
    "categoryUuid": "550e8400-e29b-41d4-a716-44665544000b",
    "categoryName": "ノート"
  }
]
```

## 4.新商品登録API

| 項目           | 内容                        |
| -------------- | --------------------------- |
| エンドポイント | `/api/admin/products`       |
| HTTPメソッド   | `POST`                      |
| コントローラー | `RegisterProductController` |
| メソッド       | `RegisterProduct`           |

### リクエスト

```json
{
  "productName": "ボールペン",
  "price": 120,
  "quantity": 50,
  "categoryUuid": "550e8400-e29b-41d4-a716-44665544000a"
}
```

### レスポンス

```json
{
  "productUuid": "550e8400-e29b-41d4-a716-446655440000",
  "message": "商品を登録しました。"
}
```

### エラーレスポンス

#### 400 Bad Request（未入力エラー）

```json
{
  "message": "productName、price、quantity、categoryUuidを入力してください。"
}
```

#### 400 Bad Request（入力値エラー）

```json
{
  "message": "入力値に不備があります。"
}
```

#### 409 Conflict（重複エラー）

```json
{
  "message": "同じ商品名が既に登録されています。"
}
```

## 5.商品検索API

| 項目           | 内容                       |
| -------------- | -------------------------- |
| エンドポイント | `/api/admin/products`      |
| HTTPメソッド   | `GET`                      |
| コントローラー | `SearchProductsController` |
| メソッド       | `Search`                   |

### クエリパラメータ

| 項目         | 型     | 必須 | 内容             |
| ------------ | ------ | ---- | ---------------- |
| categoryUuid | string | 任意 | 商品カテゴリUUID |

**注:** `categoryUuid` が指定されない場合は、全商品を返します。

### レスポンス

```json
[
  {
    "productUuid": "550e8400-e29b-41d4-a716-446655440000",
    "productName": "水性ボールペン(黒)",
    "price": 120,
    "imageUrl": "https://example.com/images/ballpen.png"
  },
  {
    "productUuid": "660e8400-e29b-41d4-a716-446655440001",
    "productName": "水性ボールペン(赤)",
    "price": 120,
    "imageUrl": "https://example.com/images/sharp.png"
  }
]
```

### エラーレスポンス

#### 404 Not Found（カテゴリIDが登録されていない）

```json
{
  "message": "指定されたカテゴリID（UUID）が存在しません。"
}
```

## 6.商品詳細取得API

| 項目           | 内容                                |
| -------------- | ----------------------------------- |
| エンドポイント | `/api/admin/products/{productUuid}` |
| HTTPメソッド   | `GET`                               |
| コントローラー | `GetProductInfoByIdController`      |
| メソッド       | `GetInfoById`                       |

### レスポンス

```json
{
  "productUuid": "550e8400-e29b-41d4-a716-446655440000",
  "productName": "ボールペン(黒)",
  "price": 120,
  "quantity": 50,
  "categoryUuid": "550e8400-e29b-41d4-a716-44665544000a",
  "imageUrl": "https://example.com/images/ballpen.png"
}
```

### エラーレスポンス

#### 404 Not Found（UUIDが存在しない）

```json
{
  "message": "指定された商品が見つかりません。"
}
```

## 7.商品修正API

| 項目           | 内容                                |
| -------------- | ----------------------------------- |
| エンドポイント | `/api/admin/products/{productUuid}` |
| HTTPメソッド   | `PUT`                               |
| コントローラー | `UpdateProductController`           |
| メソッド       | `UpdateProduct`                     |

### リクエスト

```json
{
  "productName": "ボールペン",
  "price": 150,
  "quantity": 40,
  "categoryUuid": "550e8400-e29b-41d4-a716-44665544000a",
  "imageUrl": "https://xxxxx.blob.core.windows.net/products/ballpen.png"
}
```

### レスポンス

```json
{
  "message": "商品情報を更新しました。"
}
```

### エラーレスポンス

#### 400 Bad Request（未入力エラー）

```json
{
  "message": "productName、price、quantity、categoryUuidを入力してください。"
}
```

#### 400 Bad Request（入力値エラー）

```json
{
  "message": "入力値に不備があります。"
}
```

#### 404 Not Found（UUIDが存在しない）

```json
{
  "message": "指定された商品が見つかりません。"
}
```

#### 409 Conflict（重複エラー）

```json
{
  "message": "変更後の商品名は既に登録されています。"
}
```

## 8.商品削除API

| 項目           | 内容                                |
| -------------- | ----------------------------------- |
| エンドポイント | `/api/admin/products/{productUuid}` |
| HTTPメソッド   | `DELETE`                            |
| コントローラー | `DeleteProductController`           |
| メソッド       | `DeleteProduct`                     |

### レスポンス

```json
{
  "message": "商品を削除しました。"
}
```

### エラーレスポンス

#### 404 Not Found（UUIDが存在しない）

```json
{
  "message": "指定された商品が見つかりません。"
}
```

## 9.商品カテゴリ登録API

| 項目           | 内容                         |
| -------------- | ---------------------------- |
| エンドポイント | `/api/admin/categories`      |
| HTTPメソッド   | `POST`                       |
| コントローラー | `RegisterCategoryController` |
| メソッド       | `RegisterCategory`           |

### リクエスト

```json
{
  "categoryName": "文房具"
}
```

### レスポンス

```json
{
  "categoryUuid": "550e8400-e29b-41d4-a716-446655440000",
  "message": "商品カテゴリを登録しました。"
}
```

### エラーレスポンス

#### 400 Bad Request（入力値エラー / 未入力エラー）

```json
{
  "message": "カテゴリ名を入力してください。"
}
```

#### 409 Conflict（重複エラー）

```json
{
  "message": "このカテゴリ名は既に登録されています。"
}
```

## 10.ログインAPI

| 項目           | 内容                     |
| -------------- | ------------------------ |
| エンドポイント | `/api/admin/login`       |
| HTTPメソッド   | `POST`                   |
| コントローラー | `AuthenticateController` |
| メソッド       | `Login`                  |

### リクエスト

```json
{
  "accountName": "yamada01",
  "password": "abc123"
}
```

### レスポンス（200 OK）

```json
{
  "token": "JWTトークン",
  "accountUuid": "550e8400-e29b-41d4-a716-446655440000",
  "accountName": "yamada01",
  "message": "ログインしました。"
}
```

### エラーレスポンス

#### 400 Bad Request（入力値エラー / 未入力エラー）

```json
{
  "message": "アカウント名またはパスワードを入力してください。"
}
```

#### 401 Unauthorized（認証エラー）

```json
{
  "message": "アカウント名またはパスワードが正しくありません。"
}
```

## 11.ログアウト

JWT認証のため、ログアウト時はフロントエンド側で保持しているJWTトークンを削除する。
そのため、ログアウトAPIは作成しない。
