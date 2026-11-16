using MySqlConnector;

namespace OnlineElectronicSellingSystem.Data;

public sealed class DatabaseInitializer
{
    private readonly Database database;
    public DatabaseInitializer(Database database) => this.database = database;

    public void Initialize()
    {
        using var connection = database.OpenConnection();
        using var command = connection.CreateCommand();
        command.CommandText = $@"
CREATE TABLE IF NOT EXISTS users (
 id INT AUTO_INCREMENT PRIMARY KEY, full_name VARCHAR(120) NOT NULL, email VARCHAR(160) NOT NULL UNIQUE,
 password_hash VARCHAR(255) NOT NULL, role VARCHAR(30) NOT NULL, is_active BOOLEAN NOT NULL DEFAULT TRUE,
 created_at DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP
);
CREATE TABLE IF NOT EXISTS products (
 id INT AUTO_INCREMENT PRIMARY KEY, sku VARCHAR(50) NOT NULL UNIQUE, name VARCHAR(180) NOT NULL,
 category VARCHAR(100) NOT NULL, description TEXT NULL, price DECIMAL(12,2) NOT NULL,
 stock_quantity INT NOT NULL DEFAULT 0, reorder_level INT NOT NULL DEFAULT 5, is_active BOOLEAN NOT NULL DEFAULT TRUE,
 created_at DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP, updated_at DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP
);
ALTER TABLE users ADD COLUMN IF NOT EXISTS is_active BOOLEAN NOT NULL DEFAULT TRUE;
ALTER TABLE products ADD COLUMN IF NOT EXISTS is_active BOOLEAN NOT NULL DEFAULT TRUE;
CREATE TABLE IF NOT EXISTS cart_items (
 user_id INT NOT NULL, product_id INT NOT NULL, quantity INT NOT NULL,
 PRIMARY KEY(user_id, product_id), FOREIGN KEY(user_id) REFERENCES users(id), FOREIGN KEY(product_id) REFERENCES products(id)
);
CREATE TABLE IF NOT EXISTS orders (
 id INT AUTO_INCREMENT PRIMARY KEY, order_number VARCHAR(30) NOT NULL UNIQUE, user_id INT NOT NULL,
 total DECIMAL(12,2) NOT NULL, status VARCHAR(30) NOT NULL, payment_method VARCHAR(30) NOT NULL,
 payment_status VARCHAR(30) NOT NULL, order_date DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
 FOREIGN KEY(user_id) REFERENCES users(id)
);
CREATE TABLE IF NOT EXISTS order_items (
 id INT AUTO_INCREMENT PRIMARY KEY, order_id INT NOT NULL, product_id INT NOT NULL, quantity INT NOT NULL,
 unit_price DECIMAL(12,2) NOT NULL, FOREIGN KEY(order_id) REFERENCES orders(id), FOREIGN KEY(product_id) REFERENCES products(id)
);
INSERT IGNORE INTO users(full_name,email,password_hash,role) VALUES ('System Administrator','admin@oess.local','{PasswordHasher.Hash("admin")}','Admin');
INSERT IGNORE INTO products(sku,name,category,price,stock_quantity,reorder_level) VALUES
 ('PHONE-001','OESS Smart Phone','Phones',499.99,20,5), ('LAPTOP-001','OESS Business Laptop','Laptops',899.99,10,3), ('TV-001','OESS 43 Inch Smart TV','Televisions',399.99,8,2);";
        command.ExecuteNonQuery();
    }
}
