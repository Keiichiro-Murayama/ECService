# ECService

## フルネス研修総合演習課題「ECサイト」


# API一覧

# API一覧

| No | API名 | メソッド | エンドポイント |
|---|---|---|---|
| 1 | 担当者アカウント登録 | POST | `/api/admin/accounts` |
| 2 | 未登録社員取得 | GET | `/api/admin/employees/unregistered` |
| 3 | 商品カテゴリー一覧取得 | GET | `/api/admin/categories` |
| 4 | 新商品登録 | POST | `/api/admin/products` |


## 1.担当者アカウント登録

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

## 2.未登録社員取得

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

## 3.商品カテゴリー一覧取得

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
## 4.新商品登録

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
