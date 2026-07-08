# ECService

## フルネス研修総合演習課題「ECサイト」


# API一覧


| No | API名 | メソッド | エンドポイント |
|---|---|---|---|
| 1 | 担当者アカウント登録API | POST | `/api/admin/accounts` |
| 2 | 未登録社員取得API | GET | `/api/admin/employees/unregistered` |
| 3 | 商品カテゴリ一覧取得API | GET | `/api/admin/categories` |
| 4 | 新商品登録API | POST | `/api/admin/products` |
| 5 | 商品検索API | GET | `/api/admin/products` |
| 6 | 商品詳細取得API | GET | `/api/admin/products/{productUuid}` |　(商品修正画面・商品削除確認画面で使用)
| 7 | 商品修正API | PUT | `/api/admin/products/{productUuid}` |
| 8 | 商品削除API | DELETE | `/api/admin/products/{productUuid}` |
| 9 | 商品カテゴリ登録API | POST | `/api/admin/categories` |
| 10 | ログインAPI | POST | `/api/admin/login` |



## 1.担当者アカウント登録API
|項目|内容|
|---|---|
|エンドポイント|`/api/admin/accounts`|
|HTTPメソッド|`POST`|
|コントローラー|`EmployeeAccountController`|
|メソッド|`Register`|

### リクエスト
```json
{
  "employeeId": 1,
  "accountName": "yamada01",
  "password": "abc123"
}
```

## 2.未登録社員取得API
|項目|内容|
|---|---|
|エンドポイント|`/api/admin/employees/unregistered`|
|HTTPメソッド|`GET`|
|コントローラー|`EmployeeController`|
|メソッド|`GetUnregisteredEmployees`|

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

## 3.商品カテゴリー一覧取得API
|項目|内容|
|---|---|
|エンドポイント|`/api/admin/categories`|
|HTTPメソッド|`GET`|
|コントローラー|`CategoryController`|
|メソッド|`GetCategories`|

### レスポンス
```json
[
  {
    "categoryId": 1,
    "categoryName": "文房具"
  },
  {
    "categoryId": 2,
    "categoryName": "ノート"
  }
]
```

## 4.新商品登録API
|項目|内容|
|---|---|
|エンドポイント|`/api/admin/products`|
|HTTPメソッド|`POST`|
|コントローラー|`ProductController`|
|メソッド|`Register`|

### リクエスト
```json
{
  "productName": "ボールペン",
  "price": 120,
   "quantity": 50,
  "categoryId": 1
}
```

### レスポンス
```json
{
   "productUuid": "550e8400-e29b-41d4-a716-446655440000",
   "message": "商品を登録しました。"
}
```

## 5.商品検索API
|項目|内容|
|---|---|
|エンドポイント|`/api/admin/products`|
|HTTPメソッド|`GET`|
|コントローラー|`ProductController`|
|メソッド|`Search`|

   ### レスポンス
```json
[
  {
    "productUuid": "550e8400-e29b-41d4-a716-446655440000",
    "productName": "水性ボールペン(黒)",
    "price": 120,
    "imageUrl": "https://example.com/images/ballpen.png"
  }
   {
    "productUuid": "660e8400-e29b-41d4-a716-446655440001",
    "productName": "水性ボールペン(赤)",
    "price": 120,
    "imageUrl": "https://example.com/images/sharp.png"
  }
]

```

## 6.商品詳細取得API
|項目|内容|
|---|---|
|エンドポイント|`/api/admin/products/{productUuid}`|
|HTTPメソッド|`GET`|
|コントローラー|`ProductController`|
|メソッド|`GetById`|

   ### レスポンス
```json
{
  "productUuid": "550e8400-e29b-41d4-a716-446655440000",
  "productName": "ボールペン(黒)",
  "price": 120,
  "quantity": 50,
  "categoryId": 1,
  "imageUrl": "https://example.com/images/ballpen.png"
}
```

## 7.商品修正API
|項目|内容|
|---|---|
|エンドポイント|`/api/admin/products/{productUuid}`|
|HTTPメソッド|`PUT`|
|コントローラー|`ProductController`|
|メソッド|`Update`|

### リクエスト
```json
{
  "productName": "ボールペン",
  "price": 150,
  "quantity": 40,
  "categoryId": 1,
  "imageUrl": "https://xxxxx.blob.core.windows.net/products/ballpen.png"
}
```

### レスポンス
```json
{
  "message": "商品情報を更新しました。"
}
```

## 8.商品削除API
|項目|内容|
|---|---|
|エンドポイント|`/api/admin/products/{productUuid}`|
|HTTPメソッド|`DELETE`|
|コントローラー|`ProductController`|
|メソッド|`Delete`|

### レスポンス
```json
{
  "message": "商品を削除しました。"
}
```

## 9.商品カテゴリ登録API
|項目|内容|
|---|---|
|エンドポイント|`/api/admin/categories`|
|HTTPメソッド|`POST`|
|コントローラー|`CategoryController`|
|メソッド|`Register`|

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

## 10.ログインAPI
|項目|内容|
|---|---|
|エンドポイント|`/api/admin/login`|
|HTTPメソッド|`POST`|
|コントローラー|`AuthenticateController`|
|メソッド|`Login`|

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

#### 400 Bad Request
```json
{
  "message": "アカウント名またはパスワードを入力してください。"
}
```

#### 401 Unauthorized
```json
{
  "message": "アカウント名またはパスワードが正しくありません。"
}
```

## 11.ログアウト
JWT認証のため、ログアウト時はフロントエンド側で保持しているJWTトークンを削除する。
そのため、ログアウトAPIは作成しない。
401 Unauthorized
```json
{
  "message": "ログインしていません。"
}
```