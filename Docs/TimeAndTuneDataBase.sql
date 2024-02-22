-- Створення таблиці "user"
CREATE TABLE "user" (
    userId SERIAL PRIMARY KEY UNIQUE NOT NULL,
    username VARCHAR(255) NOT NULL,
    email VARCHAR(255) UNIQUE NOT NULL,
    coinsAmount INTEGER DEFAULT 0,
    "password" VARCHAR(255) NOT NULL,
	passwordSalt VARCHAR(255) NOT NULL
);

-- Створення таблиці "task"
CREATE TABLE task (
    taskId SERIAL PRIMARY KEY UNIQUE NOT NULL,
    "name" VARCHAR(255) NOT NULL,
    description TEXT DEFAULT '',
    dateOfCreation DATE NOT NULL,
    expectedFinishTime DATE NOT NULL,
    finishTime DATE DEFAULT '0001-01-01',
    priority INTEGER NOT NULL,
    completed BOOLEAN DEFAULT 'FALSE',
    executionTime INTERVAL DEFAULT '1 second',
    userIdRef INTEGER REFERENCES "user"(userId) 
);