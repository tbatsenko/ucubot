USE ucubot;

CREATE VIEW student_signals AS 
    SELECT student.first_name, student.second_name,
    (CASE lesson_signal.SignalType WHEN -1 THEN "Simple" WHEN 0 THEN "Normal" WHEN 1 THEN "Hard" END) AS signal_type,
       COUNT(lesson_signal.student_id) AS count
   FROM student 
    JOIN lesson_signal ON lesson_signal.student_id = student.id 
    GROUP BY lesson_signal.SignalType, student.user_id;