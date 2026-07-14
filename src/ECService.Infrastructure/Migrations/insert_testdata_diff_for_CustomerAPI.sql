START TRANSACTION;

-- すべてのテーブル内部のデータを削除するコマンド
-- TRUNCATE TABLE customer, orders_detail, orders, product, department, employee,  employee_account, order_status, payment_method, product_category, product_stock RESTART IDENTITY CASCADE;

--------------------------------------------------
-- 1. 支払い方法 (payment_method) -> id: 1
--------------------------------------------------
INSERT INTO payment_method (name) VALUES ('現金');

--------------------------------------------------
-- 2. 注文ステータス (order_status) -> id: 1, 2, 3, 4
--------------------------------------------------
INSERT INTO order_status (name) VALUES ('注文済');
INSERT INTO order_status (name) VALUES ('入金済');
INSERT INTO order_status (name) VALUES ('配送中');
INSERT INTO order_status (name) VALUES ('完了');

--------------------------------------------------
-- 3. 顧客 (customer) -> id: 1, 2
--------------------------------------------------
INSERT INTO customer (customer_uuid, name, address1, address2, phone_number, mail_address, username, password, created_at) 
VALUES (gen_random_uuid(), '田中 健二', '東京都新宿区西新宿1-1-1', '新宿マンション101', '090-1234-5678', 'tanaka@example.com', 'tanaka_ken', 'hashed_password_abc', CURRENT_TIMESTAMP);

INSERT INTO customer (customer_uuid, name, address1, address2, phone_number, mail_address, username, password, created_at) 
VALUES (gen_random_uuid(), '山田 花子', '埼玉県さいたま市大宮区桜木町2-2-2', '大宮レジデンス205', '080-9876-5432', 'hanako_yamada@example.com', 'hanako_y', 'hashed_password_xyz', CURRENT_TIMESTAMP);

--------------------------------------------------
-- 4. 注文 (orders) -> id: 1, 2
--------------------------------------------------
-- 注文1: 田中さん (customer_id: 1) / 現金 (payment_method_id: 1) / 注文済 (order_status_id: 1)
INSERT INTO orders (order_uuid, order_date, amount_total, customer_id, order_status_id, payment_method_id) 
VALUES (gen_random_uuid(), CURRENT_TIMESTAMP, 2530, 1, 1, 1); 

-- 注文2: 山田さん (customer_id: 2) / 現金 (payment_method_id: 2) / 入金済 (order_status_id: 2)
INSERT INTO orders (order_uuid, order_date, amount_total, customer_id, order_status_id, payment_method_id) 
VALUES (gen_random_uuid(), CURRENT_TIMESTAMP, 3980, 2, 2, 1); 

--------------------------------------------------
-- 5. 注文詳細 (orders_detail)
-- 既存の商品テーブル (product) の ID を指定して紐付けます。
--------------------------------------------------
-- 【注文1の内訳】 計 2,530円
-- 高級ボールペン (product_id: 1, 単価1,200円) × 1
INSERT INTO orders_detail (order_id, product_id, count) VALUES (1, 1, 1);
-- 耐水ノート(A5)  (product_id: 2, 単価450円)  × 1
INSERT INTO orders_detail (order_id, product_id, count) VALUES (1, 2, 1);
-- エコバッグ       (product_id: 3, 単価880円)  × 1
INSERT INTO orders_detail (order_id, product_id, count) VALUES (1, 3, 1);

-- 【注文2の内訳】 計 3,980円
-- Type-C ハブ 6in1 (product_id: 5, 単価3,980円) × 1
INSERT INTO orders_detail (order_id, product_id, count) VALUES (2, 5, 1);

COMMIT;