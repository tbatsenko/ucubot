-- Connect to MySQL. Write a select query which aggregates a number
-- of lesson signals by type by student. 
-- 
-- Transform numerical signal types
-- into meaningful type names (Simple, Normal, Hard). Save this select
-- as a view student signals. Save your code into Scripts/student-signals.sql.

CREATE VIEW student_signals AS 
SELECT id, column2, ...
FROM student
