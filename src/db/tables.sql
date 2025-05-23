CREATE TABLE IF NOT EXISTS Actors   
        (
        id int AUTO_INCREMENT PRIMARY KEY,
        firstname VARCHAR(64) NOT NULL,
        lastname VARCHAR(4096),
        bio VARCHAR(4096),   
        rating float
        );
      
        CREATE TABLE IF NOT EXISTS Movies  
        (
        id int AUTO_INCREMENT PRIMARY KEY,
        title VARCHAR(256) NOT NULL,
        year int NOT NULL,
        description VARCHAR(4096),
        rating float
        );
        
        CREATE TABLE IF NOT EXISTS ActorsMovies  
        (
        id INT AUTO_INCREMENT PRIMARY KEY,
        actorId INT NOT NULL,
        movieId INT NOT NULL,
        rolename VARCHAR(64),
        FOREIGN KEY(actorId) REFERENCES Actors(id) ON DELETE CASCADE,
        FOREIGN KEY(movieId) REFERENCES Movies(id) ON DELETE CASCADE
        );

        CREATE TABLE IF NOT EXISTS Users
        (
        id int AUTO_INCREMENT PRIMARY KEY,
        username NVARCHAR(64) NOT NULL UNIQUE,
        password NVARCHAR(64)NOT NULL,
        salt NVARCHAR(64) NOT NULL,
        role ENUM('Admin', 'User') NOT NULL
        );
        