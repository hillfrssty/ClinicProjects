USE clinic_db;

DROP TABLE IF EXISTS medicines;
CREATE TABLE medicines (
    id INT AUTO_INCREMENT PRIMARY KEY,
    name VARCHAR(200) NOT NULL,
    manufacturer VARCHAR(150),
    form VARCHAR(100),
    dosage VARCHAR(50),
    optimal_quantity INT DEFAULT 100,
    price DECIMAL(10,2)
);

DROP TABLE IF EXISTS stocks;
CREATE TABLE stocks (
    id INT AUTO_INCREMENT PRIMARY KEY,
    warehouse VARCHAR(100) NOT NULL,
    medicine_id INT NOT NULL,
    quantity INT DEFAULT 0,
    expiry_date DATE,
    FOREIGN KEY (medicine_id) REFERENCES medicines(id) ON DELETE CASCADE
);

DROP TABLE IF EXISTS requests;
CREATE TABLE requests (
    id INT AUTO_INCREMENT PRIMARY KEY,
    department VARCHAR(100) NOT NULL,
    created_at DATETIME DEFAULT CURRENT_TIMESTAMP,
    status VARCHAR(50) DEFAULT 'Новая'
);

DROP TABLE IF EXISTS request_items;
CREATE TABLE request_items (
    id INT AUTO_INCREMENT PRIMARY KEY,
    request_id INT NOT NULL,
    medicine_id INT NOT NULL,
    quantity INT NOT NULL,
    issued TINYINT(1) DEFAULT 0,
    FOREIGN KEY (request_id) REFERENCES requests(id) ON DELETE CASCADE,
    FOREIGN KEY (medicine_id) REFERENCES medicines(id)
);

DROP TABLE IF EXISTS stock_movements;
CREATE TABLE stock_movements (
    id INT AUTO_INCREMENT PRIMARY KEY,
    medicine_id INT NOT NULL,
    from_warehouse VARCHAR(100),
    to_warehouse VARCHAR(100),
    quantity INT NOT NULL,
    reason VARCHAR(255),
    movement_type VARCHAR(50),
    created_at DATETIME DEFAULT CURRENT_TIMESTAMP
);

-- 10 препаратов
INSERT INTO medicines (name, manufacturer, form, dosage, optimal_quantity, price) VALUES
('Аспирин', 'Bayer', 'Таблетки', '100 мг', 200, 45.50),
('Лизиноприл', 'KRKA', 'Таблетки', '10 мг', 150, 120.00),
('Ибупрофен', 'Синтез', 'Таблетки', '400 мг', 300, 80.00),
('Амоксициллин', 'Синтез', 'Капсулы', '500 мг', 100, 250.00),
('Парацетамол', 'Фармстандарт', 'Таблетки', '500 мг', 400, 35.00),
('Омепразол', 'KRKA', 'Капсулы', '20 мг', 120, 180.00),
('Анальгин', 'Фармстандарт', 'Таблетки', '500 мг', 250, 55.00),
('Дротаверин', 'Софарма', 'Таблетки', '40 мг', 100, 130.00),
('Активированный уголь', 'Фармстандарт', 'Таблетки', '250 мг', 500, 20.00),
('Нимесулид', 'Berlin-Chemie', 'Таблетки', '100 мг', 80, 220.00);

-- Остатки на 2 складах
INSERT INTO stocks (warehouse, medicine_id, quantity, expiry_date) VALUES
('Склад А', 1, 150, '2027-06-01'), ('Склад А', 2, 80,  '2026-12-01'),
('Склад А', 3, 200, '2027-03-01'), ('Склад А', 4, 50,  '2026-10-01'),
('Склад А', 5, 300, '2028-01-01'), ('Склад А', 6, 90,  '2027-09-01'),
('Склад А', 7, 180, '2026-11-01'), ('Склад А', 8, 60,  '2027-04-01'),
('Склад А', 9, 400, '2029-01-01'), ('Склад А', 10, 40, '2026-08-01'),
('Склад Б', 1, 50,  '2027-06-01'), ('Склад Б', 2, 30,  '2026-12-01'),
('Склад Б', 3, 100, '2027-03-01'), ('Склад Б', 4, 20,  '2026-10-01'),
('Склад Б', 5, 150, '2028-01-01'), ('Склад Б', 6, 40,  '2027-09-01'),
('Склад Б', 7, 80,  '2026-11-01'), ('Склад Б', 8, 25,  '2027-04-01'),
('Склад Б', 9, 200, '2029-01-01'), ('Склад Б', 10, 15, '2026-08-01');

-- 3 заявки
INSERT INTO requests (department, status) VALUES
('Терапия', 'Новая'),
('Хирургия', 'Новая'),
('Кардиология', 'В работе');

INSERT INTO request_items (request_id, medicine_id, quantity, issued) VALUES
(1, 1, 20, 0), (1, 3, 30, 0), (1, 5, 50, 0),
(2, 4, 15, 0), (2, 7, 25, 0),
(3, 2, 10, 0), (3, 6, 20, 0);

SELECT * FROM medicines;
SELECT * FROM stocks;
SELECT * FROM requests;