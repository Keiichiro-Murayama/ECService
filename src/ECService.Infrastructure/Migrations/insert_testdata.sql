--------------------------------------------------
-- 1. 部署 (department)
--------------------------------------------------
INSERT INTO department (department_uuid, name) VALUES (gen_random_uuid(), '開発部');
INSERT INTO department (department_uuid, name) VALUES (gen_random_uuid(), '営業部');
INSERT INTO department (department_uuid, name) VALUES (gen_random_uuid(), '総務部');

--------------------------------------------------
-- 2. 社員 (employee) ※小文字に統一
--------------------------------------------------
INSERT INTO employee (employee_uuid, department_id, name, name_kana) VALUES (gen_random_uuid(), 1, '山田 太郎', 'やまだ たろう');
INSERT INTO employee (employee_uuid, department_id, name, name_kana) VALUES (gen_random_uuid(), 1, '佐藤 美咲', 'さとう みさき');
INSERT INTO employee (employee_uuid, department_id, name, name_kana) VALUES (gen_random_uuid(), 2, '鈴木 一郎', 'すずき ichiro');

--------------------------------------------------
-- 3. 社員アカウント (employee_account) ※小文字に統一
--------------------------------------------------
INSERT INTO employee_account (account_uuid, employee_id, name, password) VALUES (gen_random_uuid(), 1, 'yamada01', 'password123');
INSERT INTO employee_account (account_uuid, employee_id, name, password) VALUES (gen_random_uuid(), 2, 'satou_test_max_len20', 'secure_pass_456');

--------------------------------------------------
-- 4. 商品カテゴリ (product_category)
--------------------------------------------------
INSERT INTO product_category (category_uuid, name) VALUES (gen_random_uuid(), '文具');
INSERT INTO product_category (category_uuid, name) VALUES (gen_random_uuid(), '雑貨');
INSERT INTO product_category (category_uuid, name) VALUES (gen_random_uuid(), 'パソコン周辺機器');

--------------------------------------------------
-- 5. 商品 (product)
--------------------------------------------------
INSERT INTO product (product_uuid, product_category_id, name, price, image_url, delete_flag) VALUES (gen_random_uuid(), 1, '高級ボールペン', 1200, 'https://example.com/images/pen.jpg', 0);
INSERT INTO product (product_uuid, product_category_id, name, price, image_url, delete_flag) VALUES (gen_random_uuid(), 1, '耐水ノート(A5)', 450, 'https://example.com/images/notebook.jpg', 0);
INSERT INTO product (product_uuid, product_category_id, name, price, image_url, delete_flag) VALUES (gen_random_uuid(), 2, 'エコバッグ', 880, 'https://example.com/images/bag.jpg', 0);
INSERT INTO product (product_uuid, product_category_id, name, price, image_url, delete_flag) VALUES (gen_random_uuid(), 2, 'アロマキャンドル', 1500, 'https://example.com/images/candle.jpg', 0);
INSERT INTO product (product_uuid, product_category_id, name, price, image_url, delete_flag) VALUES (gen_random_uuid(), 3, 'Type-C ハブ 6in1', 3980, 'https://example.com/images/hub.jpg', 0);
INSERT INTO product (product_uuid, product_category_id, name, price, image_url, delete_flag) VALUES (gen_random_uuid(), 3, '充電式ワイヤレスマウス', 2480, 'https://example.com/images/mouse.jpg', 0);
INSERT INTO product (product_uuid, product_category_id, name, price, image_url, delete_flag) VALUES (gen_random_uuid(), 3, '旧型USBメモリ 8GB', 500, 'https://example.com/images/usb.jpg', 1);

--------------------------------------------------
-- 6. 商品在庫 (product_stock)
--------------------------------------------------
INSERT INTO product_stock (stock_uuid, product_id, quantity) VALUES (gen_random_uuid(), 1, 50);
INSERT INTO product_stock (stock_uuid, product_id, quantity) VALUES (gen_random_uuid(), 2, 120);
INSERT INTO product_stock (stock_uuid, product_id, quantity) VALUES (gen_random_uuid(), 3, 200);
INSERT INTO product_stock (stock_uuid, product_id, quantity) VALUES (gen_random_uuid(), 4, 15);
INSERT INTO product_stock (stock_uuid, product_id, quantity) VALUES (gen_random_uuid(), 5, 45);
INSERT INTO product_stock (stock_uuid, product_id, quantity) VALUES (gen_random_uuid(), 6, 0);
INSERT INTO product_stock (stock_uuid, product_id, quantity) VALUES (gen_random_uuid(), 7, 0);