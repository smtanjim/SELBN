CREATE DATABASE IF NOT EXISTS selbn CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;
USE selbn;

CREATE TABLE IF NOT EXISTS users (
    id INT AUTO_INCREMENT PRIMARY KEY,
    name VARCHAR(120) NOT NULL,
    email VARCHAR(150) NOT NULL UNIQUE,
    password_hash VARCHAR(255) NOT NULL,
    role ENUM('customer', 'supplier', 'sales_executive', 'accounts_officer', 'admin') NOT NULL DEFAULT 'customer',
    phone VARCHAR(30) NULL,
    address VARCHAR(255) NULL,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);