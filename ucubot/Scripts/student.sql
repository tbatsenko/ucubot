CREATE TABLE student (
     id INT NOT NULL AUTO_INCREMENT,
     first_name VARCHAR(128) NOT NULL,
     second_name VARCHAR(128) NOT NULL,
     user_id VARCHAR(128) NOT NULL,
     KEY (user_id),
     UNIQUE (user_id),
     CONSTRAINT PRIMARY KEY(id)
);

