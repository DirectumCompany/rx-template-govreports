WITH R AS
(
SELECT DISTINCT
	r.id AS RequestId,
	rc.name AS ManagerName,
	bu.name AS BUName,			-- [6] Исполнители
	r.deadline_citizen_gd AS RequestDeadline,
	
	-- [7] Срок исполнения поручения
	MAX(asg.deadline) OVER(PARTITION BY r.id, rc.name, bu.name) AS AssignmentDeadline,
	
	-- [8] Фактическая дата исполнения обращения
	CASE WHEN r.id IS NULL OR qanswer.regdate_docflow_sungero IS NULL THEN NULL ELSE MAX(qanswer.regdate_docflow_sungero) OVER(PARTITION BY r.id, rc.name, bu.name)
	END AS ACompleteDate

FROM sungero_content_edoc r

-- Задания на исполнение поручения
LEFT JOIN sungero_wf_attachment attachment ON attachment.attachmentid = r.id AND
	attachment.[group] IN (@ActionItemExecutionTaskDocumentsGroupGuid, @DocumentReviewTaskDocumentForReviewGroupGuid)
	
LEFT JOIN sungero_wf_task task ON attachment.task = task.id AND
	task.discriminator IN (@ActionItemExecutionTaskGuid, @DocumentReviewTaskGuid) AND
	task.status IN ('Completed', 'InProcess', 'UnderReview') AND
	task.id = task.maintask

LEFT JOIN sungero_wf_assignment asg ON task.id = asg.maintask AND
	asg.discriminator = @ActionItemExecutionAssignmentGuid AND
	asg.status IN ('Completed', 'InProcess')

LEFT JOIN sungero_core_recipient rc ON (task.assignedby_recman_sungero = rc.id AND
											task.discriminator = @ActionItemExecutionTaskGuid) OR
										(task.addressee_recman_sungero = rc.id AND
											task.discriminator = @DocumentReviewTaskGuid)

LEFT JOIN sungero_core_recipient d ON rc.department_company_sungero = d.id
LEFT JOIN sungero_core_recipient b ON d.businessunit_company_sungero = b.id	-- НОР руководителя
		   
-- Организация исполнителя
LEFT JOIN sungero_core_recipient ap ON asg.performer = ap.id
LEFT JOIN sungero_core_recipient de ON ap.department_company_sungero = de.id
LEFT JOIN sungero_core_recipient bu ON de.businessunit_company_sungero = bu.id

-- Сопровод. письмо/ответ заявителю
LEFT JOIN gd_citizen_reqquestions q ON q.edoc = r.id
LEFT JOIN sungero_content_edoc qanswer ON q.transfer = qanswer.id

WHERE	r.discriminator = @RequestGuid AND
		  r.regdate_docflow_sungero >= @StartDate AND r.regdate_docflow_sungero <= @EndDate AND  		
		  b.id = @BusinessUnitId AND
		  asg.id IS NOT NULL
)

SELECT *
FROM 

  (SELECT	*,
		
		CASE
			WHEN ACompleteDate IS NULL THEN 0
			ELSE 1
		END AS HasACompleteDate,
		
		-- [9] Количество дней просрочки
		CASE
			WHEN ACompleteDate IS NULL THEN DATEDIFF(day, RequestDeadline, GETDATE())
			ELSE DATEDIFF(day, RequestDeadline, ACompleteDate)
		END AS Delay
		
  FROM R) AS ResultTable

WHERE Delay > 0

ORDER BY ManagerName