-- OESS complete MySQL/MariaDB schema
-- Import this file in phpMyAdmin on the local server.
-- The application uses the `users`, `products`, `cart_items`, `orders`, and `order_items` tables directly.

CREATE DATABASE IF NOT EXISTS oess CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;
USE oess;

SET FOREIGN_KEY_CHECKS = 0;

CREATE TABLE IF NOT EXISTS roles (
	id INT AUTO_INCREMENT PRIMARY KEY,
	name VARCHAR(40) NOT NULL UNIQUE,
	description VARCHAR(255) NULL
) ENGINE=InnoDB;

CREATE TABLE IF NOT EXISTS users (
	id INT AUTO_INCREMENT PRIMARY KEY,
	full_name VARCHAR(120) NOT NULL,
	email VARCHAR(160) NOT NULL UNIQUE,
	phone VARCHAR(40) NULL UNIQUE,
	password_hash VARCHAR(255) NOT NULL,
	role VARCHAR(30) NOT NULL DEFAULT 'Customer',
	status VARCHAR(20) NOT NULL DEFAULT 'Active',
	email_verified_at DATETIME NULL,
	last_login DATETIME NULL,
	created_at DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
	updated_at DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
	INDEX ix_users_role_status (role, status)
) ENGINE=InnoDB;

CREATE TABLE IF NOT EXISTS addresses (
	id INT AUTO_INCREMENT PRIMARY KEY,
	user_id INT NOT NULL,
	recipient VARCHAR(120) NOT NULL,
	street VARCHAR(180) NOT NULL,
	city VARCHAR(80) NOT NULL,
	region VARCHAR(80) NULL,
	country VARCHAR(80) NOT NULL,
	phone VARCHAR(40) NULL,
	is_default BOOLEAN NOT NULL DEFAULT FALSE,
	created_at DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
	FOREIGN KEY (user_id) REFERENCES users(id) ON DELETE CASCADE,
	INDEX ix_addresses_user (user_id)
) ENGINE=InnoDB;

CREATE TABLE IF NOT EXISTS brands (
	id INT AUTO_INCREMENT PRIMARY KEY,
	name VARCHAR(100) NOT NULL UNIQUE,
	logo_url VARCHAR(500) NULL,
	description TEXT NULL,
	is_active BOOLEAN NOT NULL DEFAULT TRUE
) ENGINE=InnoDB;

CREATE TABLE IF NOT EXISTS categories (
	id INT AUTO_INCREMENT PRIMARY KEY,
	parent_id INT NULL,
	name VARCHAR(100) NOT NULL,
	description TEXT NULL,
	display_order INT NOT NULL DEFAULT 0,
	is_active BOOLEAN NOT NULL DEFAULT TRUE,
	FOREIGN KEY (parent_id) REFERENCES categories(id) ON DELETE SET NULL,
	UNIQUE KEY uq_category_parent_name (parent_id, name)
) ENGINE=InnoDB;

CREATE TABLE IF NOT EXISTS products (
	id INT AUTO_INCREMENT PRIMARY KEY,
	sku VARCHAR(50) NOT NULL UNIQUE,
	name VARCHAR(180) NOT NULL,
	brand_id INT NULL,
	category_id INT NULL,
	category VARCHAR(100) NOT NULL DEFAULT '',
	description TEXT NULL,
	warranty_months INT NOT NULL DEFAULT 0,
	status VARCHAR(20) NOT NULL DEFAULT 'Published',
	price DECIMAL(12,2) NOT NULL DEFAULT 0,
	stock_quantity INT NOT NULL DEFAULT 0,
	reorder_level INT NOT NULL DEFAULT 5,
	is_featured BOOLEAN NOT NULL DEFAULT FALSE,
	is_new_arrival BOOLEAN NOT NULL DEFAULT FALSE,
	is_active BOOLEAN NOT NULL DEFAULT TRUE,
	created_at DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
	updated_at DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
	FOREIGN KEY (brand_id) REFERENCES brands(id) ON DELETE SET NULL,
	FOREIGN KEY (category_id) REFERENCES categories(id) ON DELETE SET NULL,
	INDEX ix_products_search (name, sku, category),
	INDEX ix_products_stock (stock_quantity, reorder_level)
) ENGINE=InnoDB;

CREATE TABLE IF NOT EXISTS product_variants (
	id INT AUTO_INCREMENT PRIMARY KEY,
	product_id INT NOT NULL,
	sku VARCHAR(50) NOT NULL UNIQUE,
	attributes JSON NULL,
	price DECIMAL(12,2) NOT NULL,
	stock_quantity INT NOT NULL DEFAULT 0,
	reorder_level INT NOT NULL DEFAULT 5,
	is_active BOOLEAN NOT NULL DEFAULT TRUE,
	FOREIGN KEY (product_id) REFERENCES products(id) ON DELETE RESTRICT,
	INDEX ix_variants_product (product_id)
) ENGINE=InnoDB;

CREATE TABLE IF NOT EXISTS product_specifications (
	id INT AUTO_INCREMENT PRIMARY KEY,
	product_id INT NOT NULL,
	specification_name VARCHAR(100) NOT NULL,
	specification_value VARCHAR(255) NOT NULL,
	FOREIGN KEY (product_id) REFERENCES products(id) ON DELETE CASCADE,
	UNIQUE KEY uq_product_specification (product_id, specification_name)
) ENGINE=InnoDB;

CREATE TABLE IF NOT EXISTS product_images (
	id INT AUTO_INCREMENT PRIMARY KEY,
	product_id INT NOT NULL,
	image_url VARCHAR(500) NOT NULL,
	is_main BOOLEAN NOT NULL DEFAULT FALSE,
	display_order INT NOT NULL DEFAULT 0,
	FOREIGN KEY (product_id) REFERENCES products(id) ON DELETE CASCADE
) ENGINE=InnoDB;

CREATE TABLE IF NOT EXISTS carts (
	id INT AUTO_INCREMENT PRIMARY KEY,
	user_id INT NOT NULL UNIQUE,
	status VARCHAR(20) NOT NULL DEFAULT 'Active',
	updated_at DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
	FOREIGN KEY (user_id) REFERENCES users(id) ON DELETE CASCADE
) ENGINE=InnoDB;

CREATE TABLE IF NOT EXISTS cart_items (
	user_id INT NOT NULL,
	product_id INT NOT NULL,
	variant_id INT NULL,
	quantity INT NOT NULL,
	unit_price DECIMAL(12,2) NULL,
	PRIMARY KEY (user_id, product_id),
	FOREIGN KEY (user_id) REFERENCES users(id) ON DELETE CASCADE,
	FOREIGN KEY (product_id) REFERENCES products(id) ON DELETE RESTRICT,
	FOREIGN KEY (variant_id) REFERENCES product_variants(id) ON DELETE RESTRICT
) ENGINE=InnoDB;

CREATE TABLE IF NOT EXISTS wishlists (
	id INT AUTO_INCREMENT PRIMARY KEY,
	user_id INT NOT NULL UNIQUE,
	created_at DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
	FOREIGN KEY (user_id) REFERENCES users(id) ON DELETE CASCADE
) ENGINE=InnoDB;

CREATE TABLE IF NOT EXISTS wishlist_items (
	wishlist_id INT NOT NULL,
	product_id INT NOT NULL,
	added_at DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
	PRIMARY KEY (wishlist_id, product_id),
	FOREIGN KEY (wishlist_id) REFERENCES wishlists(id) ON DELETE CASCADE,
	FOREIGN KEY (product_id) REFERENCES products(id) ON DELETE RESTRICT
) ENGINE=InnoDB;

CREATE TABLE IF NOT EXISTS coupons (
	id INT AUTO_INCREMENT PRIMARY KEY,
	code VARCHAR(40) NOT NULL UNIQUE,
	discount_type VARCHAR(20) NOT NULL,
	value DECIMAL(12,2) NOT NULL,
	minimum_order_amount DECIMAL(12,2) NOT NULL DEFAULT 0,
	usage_limit INT NULL,
	usage_count INT NOT NULL DEFAULT 0,
	valid_from DATETIME NOT NULL,
	valid_until DATETIME NOT NULL,
	is_active BOOLEAN NOT NULL DEFAULT TRUE
) ENGINE=InnoDB;

CREATE TABLE IF NOT EXISTS promotions (
	id INT AUTO_INCREMENT PRIMARY KEY,
	name VARCHAR(150) NOT NULL,
	description TEXT NULL,
	discount_type VARCHAR(20) NOT NULL,
	discount_value DECIMAL(12,2) NOT NULL,
	valid_from DATETIME NOT NULL,
	valid_until DATETIME NOT NULL,
	is_active BOOLEAN NOT NULL DEFAULT TRUE
) ENGINE=InnoDB;

CREATE TABLE IF NOT EXISTS promotion_products (
	promotion_id INT NOT NULL,
	product_id INT NOT NULL,
	PRIMARY KEY (promotion_id, product_id),
	FOREIGN KEY (promotion_id) REFERENCES promotions(id) ON DELETE CASCADE,
	FOREIGN KEY (product_id) REFERENCES products(id) ON DELETE CASCADE
) ENGINE=InnoDB;

CREATE TABLE IF NOT EXISTS orders (
	id INT AUTO_INCREMENT PRIMARY KEY,
	order_number VARCHAR(30) NOT NULL UNIQUE,
	user_id INT NOT NULL,
	address_id INT NULL,
	total DECIMAL(12,2) NOT NULL,
	discount_amount DECIMAL(12,2) NOT NULL DEFAULT 0,
	tax_amount DECIMAL(12,2) NOT NULL DEFAULT 0,
	shipping_fee DECIMAL(12,2) NOT NULL DEFAULT 0,
	status VARCHAR(30) NOT NULL DEFAULT 'Pending',
	payment_method VARCHAR(30) NOT NULL,
	payment_status VARCHAR(30) NOT NULL DEFAULT 'Pending',
	internal_notes TEXT NULL,
	order_date DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
	updated_at DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
	FOREIGN KEY (user_id) REFERENCES users(id) ON DELETE RESTRICT,
	FOREIGN KEY (address_id) REFERENCES addresses(id) ON DELETE SET NULL,
	INDEX ix_orders_status_date (status, order_date),
	INDEX ix_orders_user_date (user_id, order_date)
) ENGINE=InnoDB;

CREATE TABLE IF NOT EXISTS order_items (
	id INT AUTO_INCREMENT PRIMARY KEY,
	order_id INT NOT NULL,
	product_id INT NOT NULL,
	variant_id INT NULL,
	quantity INT NOT NULL,
	unit_price DECIMAL(12,2) NOT NULL,
	serial_number VARCHAR(100) NULL,
	FOREIGN KEY (order_id) REFERENCES orders(id) ON DELETE RESTRICT,
	FOREIGN KEY (product_id) REFERENCES products(id) ON DELETE RESTRICT,
	FOREIGN KEY (variant_id) REFERENCES product_variants(id) ON DELETE RESTRICT
) ENGINE=InnoDB;

CREATE TABLE IF NOT EXISTS payments (
	id INT AUTO_INCREMENT PRIMARY KEY,
	order_id INT NOT NULL,
	method VARCHAR(30) NOT NULL,
	amount DECIMAL(12,2) NOT NULL,
	status VARCHAR(30) NOT NULL DEFAULT 'Pending',
	reference VARCHAR(120) NULL,
	proof_url VARCHAR(500) NULL,
	paid_at DATETIME NULL,
	created_at DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
	FOREIGN KEY (order_id) REFERENCES orders(id) ON DELETE RESTRICT,
	INDEX ix_payments_status (status)
) ENGINE=InnoDB;

CREATE TABLE IF NOT EXISTS invoices (
	id INT AUTO_INCREMENT PRIMARY KEY,
	order_id INT NOT NULL UNIQUE,
	invoice_number VARCHAR(40) NOT NULL UNIQUE,
	pdf_url VARCHAR(500) NULL,
	issued_at DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
	FOREIGN KEY (order_id) REFERENCES orders(id) ON DELETE RESTRICT
) ENGINE=InnoDB;

CREATE TABLE IF NOT EXISTS shipments (
	id INT AUTO_INCREMENT PRIMARY KEY,
	order_id INT NOT NULL,
	courier VARCHAR(120) NULL,
	delivery_agent_id INT NULL,
	tracking_number VARCHAR(100) NULL,
	delivery_method VARCHAR(30) NOT NULL DEFAULT 'Standard',
	status VARCHAR(30) NOT NULL DEFAULT 'Pending',
	estimated_delivery DATE NULL,
	delivered_at DATETIME NULL,
	delivery_note TEXT NULL,
	FOREIGN KEY (order_id) REFERENCES orders(id) ON DELETE RESTRICT,
	FOREIGN KEY (delivery_agent_id) REFERENCES users(id) ON DELETE SET NULL,
	INDEX ix_shipments_status (status)
) ENGINE=InnoDB;

CREATE TABLE IF NOT EXISTS returns (
	id INT AUTO_INCREMENT PRIMARY KEY,
	order_item_id INT NOT NULL,
	user_id INT NOT NULL,
	rma_number VARCHAR(40) NULL UNIQUE,
	reason TEXT NOT NULL,
	status VARCHAR(30) NOT NULL DEFAULT 'Requested',
	inspection_condition VARCHAR(40) NULL,
	decision_reason TEXT NULL,
	requested_at DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
	resolved_at DATETIME NULL,
	FOREIGN KEY (order_item_id) REFERENCES order_items(id) ON DELETE RESTRICT,
	FOREIGN KEY (user_id) REFERENCES users(id) ON DELETE RESTRICT
) ENGINE=InnoDB;

CREATE TABLE IF NOT EXISTS warranties (
	id INT AUTO_INCREMENT PRIMARY KEY,
	order_item_id INT NOT NULL,
	serial_number VARCHAR(100) NULL,
	start_date DATE NOT NULL,
	end_date DATE NOT NULL,
	status VARCHAR(20) NOT NULL DEFAULT 'Active',
	FOREIGN KEY (order_item_id) REFERENCES order_items(id) ON DELETE RESTRICT
) ENGINE=InnoDB;

CREATE TABLE IF NOT EXISTS warranty_claims (
	id INT AUTO_INCREMENT PRIMARY KEY,
	warranty_id INT NOT NULL,
	user_id INT NOT NULL,
	description TEXT NOT NULL,
	status VARCHAR(30) NOT NULL DEFAULT 'Received',
	attachment_url VARCHAR(500) NULL,
	resolution TEXT NULL,
	created_at DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
	resolved_at DATETIME NULL,
	FOREIGN KEY (warranty_id) REFERENCES warranties(id) ON DELETE RESTRICT,
	FOREIGN KEY (user_id) REFERENCES users(id) ON DELETE RESTRICT
) ENGINE=InnoDB;

CREATE TABLE IF NOT EXISTS reviews (
	id INT AUTO_INCREMENT PRIMARY KEY,
	product_id INT NOT NULL,
	user_id INT NOT NULL,
	order_item_id INT NULL,
	rating TINYINT NOT NULL,
	comment TEXT NULL,
	status VARCHAR(20) NOT NULL DEFAULT 'Pending',
	is_verified_buyer BOOLEAN NOT NULL DEFAULT FALSE,
	created_at DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
	FOREIGN KEY (product_id) REFERENCES products(id) ON DELETE RESTRICT,
	FOREIGN KEY (user_id) REFERENCES users(id) ON DELETE RESTRICT,
	FOREIGN KEY (order_item_id) REFERENCES order_items(id) ON DELETE SET NULL,
	INDEX ix_reviews_product_status (product_id, status)
) ENGINE=InnoDB;

CREATE TABLE IF NOT EXISTS product_questions (
	id INT AUTO_INCREMENT PRIMARY KEY,
	product_id INT NOT NULL,
	user_id INT NOT NULL,
	question TEXT NOT NULL,
	answer TEXT NULL,
	answered_by INT NULL,
	status VARCHAR(20) NOT NULL DEFAULT 'Open',
	created_at DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
	answered_at DATETIME NULL,
	FOREIGN KEY (product_id) REFERENCES products(id) ON DELETE RESTRICT,
	FOREIGN KEY (user_id) REFERENCES users(id) ON DELETE RESTRICT,
	FOREIGN KEY (answered_by) REFERENCES users(id) ON DELETE SET NULL
) ENGINE=InnoDB;

CREATE TABLE IF NOT EXISTS suppliers (
	id INT AUTO_INCREMENT PRIMARY KEY,
	name VARCHAR(150) NOT NULL,
	contact_name VARCHAR(120) NULL,
	email VARCHAR(160) NULL,
	phone VARCHAR(40) NULL,
	address TEXT NULL,
	is_active BOOLEAN NOT NULL DEFAULT TRUE
) ENGINE=InnoDB;

CREATE TABLE IF NOT EXISTS purchase_orders (
	id INT AUTO_INCREMENT PRIMARY KEY,
	supplier_id INT NOT NULL,
	purchase_order_number VARCHAR(40) NOT NULL UNIQUE,
	status VARCHAR(30) NOT NULL DEFAULT 'Draft',
	ordered_at DATETIME NULL,
	received_at DATETIME NULL,
	notes TEXT NULL,
	FOREIGN KEY (supplier_id) REFERENCES suppliers(id) ON DELETE RESTRICT
) ENGINE=InnoDB;

CREATE TABLE IF NOT EXISTS purchase_order_items (
	id INT AUTO_INCREMENT PRIMARY KEY,
	purchase_order_id INT NOT NULL,
	product_id INT NOT NULL,
	quantity INT NOT NULL,
	unit_cost DECIMAL(12,2) NOT NULL,
	received_quantity INT NOT NULL DEFAULT 0,
	FOREIGN KEY (purchase_order_id) REFERENCES purchase_orders(id) ON DELETE CASCADE,
	FOREIGN KEY (product_id) REFERENCES products(id) ON DELETE RESTRICT
) ENGINE=InnoDB;

CREATE TABLE IF NOT EXISTS stock_movements (
	id INT AUTO_INCREMENT PRIMARY KEY,
	product_id INT NOT NULL,
	user_id INT NULL,
	movement_type VARCHAR(30) NOT NULL,
	quantity INT NOT NULL,
	reason VARCHAR(255) NOT NULL,
	reference_type VARCHAR(50) NULL,
	reference_id INT NULL,
	created_at DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
	FOREIGN KEY (product_id) REFERENCES products(id) ON DELETE RESTRICT,
	FOREIGN KEY (user_id) REFERENCES users(id) ON DELETE SET NULL,
	INDEX ix_stock_product_date (product_id, created_at)
) ENGINE=InnoDB;

CREATE TABLE IF NOT EXISTS support_tickets (
	id INT AUTO_INCREMENT PRIMARY KEY,
	user_id INT NOT NULL,
	assigned_to INT NULL,
	related_order_id INT NULL,
	subject VARCHAR(180) NOT NULL,
	category VARCHAR(60) NOT NULL,
	status VARCHAR(30) NOT NULL DEFAULT 'Open',
	created_at DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
	closed_at DATETIME NULL,
	FOREIGN KEY (user_id) REFERENCES users(id) ON DELETE RESTRICT,
	FOREIGN KEY (assigned_to) REFERENCES users(id) ON DELETE SET NULL,
	FOREIGN KEY (related_order_id) REFERENCES orders(id) ON DELETE SET NULL
) ENGINE=InnoDB;

CREATE TABLE IF NOT EXISTS ticket_messages (
	id INT AUTO_INCREMENT PRIMARY KEY,
	ticket_id INT NOT NULL,
	user_id INT NOT NULL,
	message TEXT NOT NULL,
	attachment_url VARCHAR(500) NULL,
	created_at DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
	FOREIGN KEY (ticket_id) REFERENCES support_tickets(id) ON DELETE CASCADE,
	FOREIGN KEY (user_id) REFERENCES users(id) ON DELETE RESTRICT
) ENGINE=InnoDB;

CREATE TABLE IF NOT EXISTS notifications (
	id INT AUTO_INCREMENT PRIMARY KEY,
	user_id INT NULL,
	channel VARCHAR(20) NOT NULL DEFAULT 'InApp',
	notification_type VARCHAR(50) NOT NULL,
	subject VARCHAR(180) NULL,
	message TEXT NOT NULL,
	delivery_status VARCHAR(30) NOT NULL DEFAULT 'Pending',
	sent_at DATETIME NULL,
	created_at DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
	FOREIGN KEY (user_id) REFERENCES users(id) ON DELETE CASCADE
) ENGINE=InnoDB;

CREATE TABLE IF NOT EXISTS audit_logs (
	id BIGINT AUTO_INCREMENT PRIMARY KEY,
	user_id INT NULL,
	action VARCHAR(100) NOT NULL,
	entity_type VARCHAR(80) NULL,
	entity_id INT NULL,
	details JSON NULL,
	ip_address VARCHAR(45) NULL,
	created_at DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
	FOREIGN KEY (user_id) REFERENCES users(id) ON DELETE SET NULL,
	INDEX ix_audit_created (created_at),
	INDEX ix_audit_entity (entity_type, entity_id)
) ENGINE=InnoDB;

CREATE TABLE IF NOT EXISTS store_settings (
	setting_key VARCHAR(100) PRIMARY KEY,
	setting_value TEXT NULL,
	updated_by INT NULL,
	updated_at DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
	FOREIGN KEY (updated_by) REFERENCES users(id) ON DELETE SET NULL
) ENGINE=InnoDB;

INSERT IGNORE INTO roles(name, description) VALUES
	('Admin', 'Full system administration'),
	('Customer', 'Registered buyer'),
	('Seller', 'Product and inventory management'),
	('SalesOfficer', 'Order and payment processing'),
	('SupportAgent', 'Customer support and warranty handling'),
	('DeliveryAgent', 'Delivery status management');

INSERT IGNORE INTO products(sku, name, category, price, stock_quantity, reorder_level, status, is_active) VALUES
	('PHONE-001', 'OESS Smart Phone', 'Phones', 499.99, 20, 5, 'Published', TRUE),
	('LAPTOP-001', 'OESS Business Laptop', 'Laptops', 899.99, 10, 3, 'Published', TRUE),
	('TV-001', 'OESS 43 Inch Smart TV', 'Televisions', 399.99, 8, 2, 'Published', TRUE);

INSERT IGNORE INTO store_settings(setting_key, setting_value) VALUES
	('store_name', 'Online Electronic Selling System'),
	('currency', 'USD'),
	('tax_rate', '0'),
	('return_period_days', '14');

SET FOREIGN_KEY_CHECKS = 1;
