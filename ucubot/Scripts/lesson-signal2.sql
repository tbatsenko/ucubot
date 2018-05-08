ALTER TABLE lesson_signal DROP COLUMN UserId;

ALTER TABLE lesson_signal ADD COLUMN student_id INT NOT NULL;

ALTER TABLE lesson_signal
ADD CONSTRAINT fk_student_id
    FOREIGN KEY (student_id)
    REFERENCES student (id) ON UPDATE RESTRICT ON DELETE RESTRICT;
    
TRUNCATE TABLE lesson_signal;
