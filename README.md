# SWE1-PPB
Party Playlist Battle

# Git:
https://github.com/jpgerh/SWE1-PPB.git (https://github.com/jpgerh/SWE1-PPB)

# Protocol:

## Technical steps:
* Multithreaded server (can handle multiple client at once)
* Implementation very oriented on the CURL Script
* Special feature: 
    * specific action (RSVLP) always wins (even if draw), but if both players choose it they are disqualified
    * winning with the special action rewards 2 points instead of the typical 1
    * losing against the special action loses 2 points instead of the typical 1   

## Unit tests:
* For the battle logic:
    * to test the special feature (action)
    * to test the battle logic itself
* Interactions done with a user on the database:
    * to test the database functionality
    * to check important functions like verifyToken() and verifyAdmin(), which are used a lot by other methods
    * to test callback values of the methods

## Problems:
* Test-driven development difficult to apply
* Specific:
    * Request payload was cut in half -> had to invest a lot of time to realize that the byte buffer was not large enough
    * A lot of the implementation is done using the database as backbone (otherwise it would be redundant to have classes and sync them with the database everytime the server restart)
* Time management

## Time spent: 
* ~30 hours
* most spent on the actual webserver implementation
* battle logic was done quickly

# SQL-Scripts:

CREATE TABLE users (
	username VARCHAR(255) PRIMARY KEY,
	password VARCHAR(255) NOT NULL,
	token VARCHAR(255),
	name VARCHAR(255),
	bio VARCHAR(255),
	image VARCHAR(255),
	online BOOLEAN,
	admin BOOLEAN
);

CREATE TABLE library (
	id SERIAL PRIMARY KEY,
	username VARCHAR(255) NOT NULL,
	name VARCHAR(255) NOT NULL,
	url VARCHAR(255),
	filetype VARCHAR(255),
	title VARCHAR(255),
	artist VARCHAR(255),
	album VARCHAR(255),
	genre VARCHAR(255),
	path VARCHAR(255),
	filesize INTEGER,
	rating INTEGER,
	length VARCHAR(255),

	FOREIGN KEY (username) REFERENCES users (username)
);

CREATE TABLE tournamentscores (
	tournamentid INTEGER,
	username VARCHAR(255),
	battlescore INTEGER,

	PRIMARY KEY (tournamentid, username),
	FOREIGN KEY (username) REFERENCES users (username)
);

CREATE TABLE playlist (
	id SERIAL PRIMARY KEY,
	playlist_order INTEGER NOT NULL,
	username VARCHAR(255) NOT NULL,
	name VARCHAR(255) NOT NULL,
	url VARCHAR(255),
	filetype VARCHAR(255),
	title VARCHAR(255),
	artist VARCHAR(255),
	album VARCHAR(255),
	genre VARCHAR(255),
	path VARCHAR(255),
	filesize INTEGER,
	rating INTEGER,
	length VARCHAR(255)
);

CREATE TABLE actions (
	username VARCHAR(255) PRIMARY KEY,
	action VARCHAR(255) NOT NULL
);

CREATE TABLE stats (
	username VARCHAR(255) PRIMARY KEY,
	games_played INTEGER DEFAULT 0,
	score INTEGER DEFAULT 100,

	FOREIGN KEY (username) REFERENCES users (username)
);