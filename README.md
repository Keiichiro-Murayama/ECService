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
| 11  | 購入履歴検索API         | GET      | `/api/admin/orders`                 |                                        |
| 12  | 注文ステータス更新情報取得API | GET | `/api/admin/orders/{orderUuid}/status` | 注文ステータス更新画面で使用         |
| 13  | 注文ステータス更新API   | PUT      | `/api/admin/orders/{orderUuid}/status` |                                        |

---

## 共通エラーレスポンス

システム全体（No.1〜No.13すべて）で予期せぬエラーやサーバー内部エラーが発生した場合は、共通で以下のレスポンスを返却する。

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
- No.11: 購入履歴検索API
- No.12: 注文ステータス更新情報取得API
- No.13: 注文ステータス更新API

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
  "stock": 50,
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
  "message": "productName、price、stock、categoryUuidを入力してください。"
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

#### 400 Bad Request (不正なUUID)

```json
{
  "message": ""
}
```

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
  "stock": 50,
  "categoryUuid": "550e8400-e29b-41d4-a716-44665544000a",
  "imageUrl": "https://example.com/images/ballpen.png"
}
```

### エラーレスポンス

#### 400 Bad Request（UUID不正）

```json
{
  "message": "商品UUIDの形式が不正です。"//石原:追加しました。
}
```

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
  "stock": 40,
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
  "message": "productName、price、stock、categoryUuidを入力してください。"
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


#### 400 Bad Request (不正なUUID)

```json
{
  "message": ""
}
```

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

## 11.購入履歴検索API

| 項目           | 内容                             |
| -------------- | -------------------------------- |
| エンドポイント | `/api/admin/orders`              |
| HTTPメソッド   | `GET`                            |
| コントローラー | `SearchOrderHistoriesController` |
| メソッド       | `Search`                         |

### クエリパラメータ

| 項目                | 型     | 必須 | 内容                                      |
| ------------------- | ------ | ---- | ----------------------------------------- |
| PurchaseDate        | date   | 任意 | 購入日。`yyyy-MM-dd`形式で指定            |
| CustomerAccountName | string | 任意 | 顧客アカウント名。完全一致で検索          |

**注:**

- 両方未指定の場合は、購入履歴を全件取得する。
- 両方指定した場合は、AND条件で検索する。
- 購入履歴は購入日時の降順で取得する。

### レスポンス

```json
[
  {
    "orderUuid": "550e8400-e29b-41d4-a716-446655440000",
    "purchaseDate": "2026-07-21T10:30:00Z",
    "customerAccountName": "Yamada",
    "orderContent": "商品A × 1、商品B × 2",
    "orderStatus": "注文受付"
  }
]
```

### エラーレスポンス

#### 400 Bad Request（入力値エラー）

```json
{
  "message": "入力値に不備があります。"
}
```

## 12.注文ステータス更新情報取得API

| 項目           | 内容                                   |
| -------------- | -------------------------------------- |
| エンドポイント | `/api/admin/orders/{orderUuid}/status` |
| HTTPメソッド   | `GET`                                  |
| コントローラー | `GetOrderStatusUpdateController`       |
| メソッド       | `Get`                                  |

### パスパラメータ

| 項目      | 型     | 必須 | 内容            |
| --------- | ------ | ---- | --------------- |
| orderUuid | string | 必須 | 対象注文のUUID  |

### レスポンス

```json
{
  "orderUuid": "550e8400-e29b-41d4-a716-446655440000",
  "orderDate": "2026-07-21T10:30:00Z",
  "customerAccountName": "Yamada",
  "orderContent": "商品A × 1、商品B × 2",
  "currentOrderStatusId": 1,
  "currentOrderStatusName": "注文受付",
  "orderStatuses": [
    {
      "orderStatusId": 1,
      "orderStatusName": "注文受付"
    },
    {
      "orderStatusId": 2,
      "orderStatusName": "入金済"
    },
    {
      "orderStatusId": 3,
      "orderStatusName": "配送中"
    },
    {
      "orderStatusId": 4,
      "orderStatusName": "完了"
    }
  ]
}
```

### エラーレスポンス

#### 400 Bad Request（UUID不正）

```json
{
  "message": "注文UUIDの形式が不正です。"
}
```

#### 404 Not Found（注文が存在しない）

```json
{
  "message": "指定された注文情報が見つかりません。"
}
```

## 13.注文ステータス更新API

| 項目           | 内容                                   |
| -------------- | -------------------------------------- |
| エンドポイント | `/api/admin/orders/{orderUuid}/status` |
| HTTPメソッド   | `PUT`                                  |
| コントローラー | `UpdateOrderStatusController`          |
| メソッド       | `Update`                               |

### パスパラメータ

| 項目      | 型     | 必須 | 内容                 |
| --------- | ------ | ---- | -------------------- |
| orderUuid | string | 必須 | 更新対象注文のUUID   |

### リクエスト

```json
{
  "orderStatusId": 2
}
```

### レスポンス

```json
{
  "orderUuid": "550e8400-e29b-41d4-a716-446655440000",
  "orderStatusId": 2,
  "orderStatusName": "入金済",
  "updatedAt": "2026-07-24T01:30:00Z"
}
```

### エラーレスポンス

#### 400 Bad Request（UUID不正）

```json
{
  "message": "注文UUIDの形式が不正です。"
}
```

#### 400 Bad Request（注文ステータスID不正）

```json
{
  "message": "注文ステータスIDは1以上で指定してください。"
}
```

#### 400 Bad Request（注文が存在しない）

```json
{
  "message": "指定された注文情報が見つかりません。"
}
```

#### 400 Bad Request（注文ステータスが存在しない）

```json
{
  "message": "指定された注文ステータスが見つかりません。"
}
```

## 14.ログアウト

JWT認証のため、ログアウト時はフロントエンド側で保持しているJWTトークンを削除する。
そのため、ログアウトAPIは作成しない。
