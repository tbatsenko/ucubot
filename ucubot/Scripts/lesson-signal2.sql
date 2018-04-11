ALTER TABLE lesson_signal DROP COLUMN UserId;

ALTER TABLE lesson_signal ADD COLUMN student_id VARCHAR(128) NOT NULL;

ALTER TABLE lesson_signal
ADD CONSTRAINT fk_student_id
    FOREIGN KEY (student_id)
    REFERENCES student (user_id);
    
TRUNCATE TABLE lesson_signal;
