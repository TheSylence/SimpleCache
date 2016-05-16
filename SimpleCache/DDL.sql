﻿CREATE TABLE IF NOT EXISTS DataCache
(
	Id INT PRIMARY KEY,
	Key TEXT UNIQUE NOT NULL,
	Value TEXT NOT NULL,
	Expires BIGINT	
);