USE ucubot;

CREATE VIEW student_signals AS 
    SELECT student.first_name, student.last_name,
    (CASE lesson_signal.SignalType WHEN -1 THEN "simple" WHEN 0 THEN "normal" WHEN 1 THEN "hard" END) AS signalType,
       COUNT(lesson_signal.student_id) AS count
   FROM student 
    JOIN lesson_signal ON lesson_signal.student_id = student.id 
    GROUP BY lesson_signal.SignalType, student.user_id;