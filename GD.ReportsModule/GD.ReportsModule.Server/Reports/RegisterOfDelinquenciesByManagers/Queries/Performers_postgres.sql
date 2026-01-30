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
	CASE
		WHEN COUNT(r.id) FILTER(WHERE qanswer.regdate_docflow_sungero IS NULL) OVER(PARTITION BY r.id, rc.name, bu.name) > 0 THEN NULL
		ELSE MAX(qanswer.regdate_docflow_sungero) OVER(PARTITION BY r.id, rc.name, bu.name)
	END AS ACompleteDate

FROM sungero_content_edoc r

-- Задания на исполнение поручения
LEFT JOIN public.sungero_wf_attachment attachment ON attachment.attachmentid = r.id AND
	attachment.group::citext IN (@ActionItemExecutionTaskDocumentsGroupGuid, @DocumentReviewTaskDocumentForReviewGroupGuid)
	
LEFT JOIN public.sungero_wf_task task ON attachment.task = task.id AND
	task.discriminator::citext IN (@ActionItemExecutionTaskGuid, @DocumentReviewTaskGuid) AND
	task.status IN ('Completed', 'InProcess', 'UnderReview') AND
	task.id = task.maintask

LEFT JOIN public.sungero_wf_assignment asg ON task.id = asg.maintask AND
	asg.discriminator::citext = @ActionItemExecutionAssignmentGuid AND
	asg.status IN ('Completed', 'InProcess')

LEFT JOIN sungero_core_recipient rc ON (task.assignedby_recman_sungero = rc.id AND
											task.discriminator::citext = @ActionItemExecutionTaskGuid) OR
										(task.addressee_recman_sungero = rc.id AND
											task.discriminator::citext = @DocumentReviewTaskGuid)

LEFT JOIN sungero_core_recipient d ON rc.department_company_sungero = d.id
LEFT JOIN sungero_core_recipient b ON d.businessunit_company_sungero = b.id	-- НОР руководителя
		   
-- Организация исполнителя
LEFT JOIN sungero_core_recipient ap ON asg.performer = ap.id
LEFT JOIN sungero_core_recipient de ON ap.department_company_sungero = de.id
LEFT JOIN sungero_core_recipient bu ON de.businessunit_company_sungero = bu.id

-- Сопровод. письмо/ответ заявителю
LEFT JOIN public.gd_citizen_reqquestions q ON q.edoc = r.id
LEFT JOIN public.sungero_content_edoc qanswer ON q.transfer = qanswer.id

WHERE	r.discriminator::citext = @RequestGuid AND
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
			WHEN ACompleteDate IS NULL THEN DATE_PART('day', CURRENT_DATE - RequestDeadline)::integer
			ELSE DATE_PART('day', ACompleteDate - RequestDeadline)::integer
		END AS Delay
		
  FROM R) AS ResultTable
  
WHERE Delay > 0

ORDER BY ManagerName