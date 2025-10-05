import sqlite3

conn = sqlite3.connect("ChatAppDB.db")
cur = conn.cursor()

cur.execute("""
CREATE TABLE users (
    id INTEGER PRIMARY KEY,
    name TEXT,
    age INTEGER
);
""")

cur.executemany("INSERT INTO users (name, age) VALUES (?, ?)", [
    ("Alice", 25),
    ("Bob", 30),
    ("Charlie", 22)
])

conn.commit()
conn.close()