WITH R AS
(
SELECT
	r.id AS RequestId, 
	rc.name AS ManagerName,								-- [1] Руководители
	r.regnumber_docflow_sungero AS RegNumber,			-- [2] № обращения
	r.regdate_docflow_sungero AS RegDate,				-- [2] Дата регистрации обращения
	cp.name AS CorName,									-- [3] ФИО заявителя
	r.deadline_citizen_gd AS RequestDeadline,			-- [4] Срок исполнения обращения
	
	-- [5] Фактическая дата исполнения обращения
	qanswer.regdate_docflow_sungero AS RCompleteDate

FROM sungero_content_edoc r

-- Задания на исполнение поручения
LEFT JOIN sungero_wf_attachment attachment ON attachment.attachmentid = r.id AND
	attachment.[group] IN (@ActionItemExecutionTaskDocumentsGroupGuid, @DocumentReviewTaskDocumentForReviewGroupGuid)
	
LEFT JOIN sungero_wf_task task ON attachment.task = task.id AND
	task.discriminator IN (@ActionItemExecutionTaskGuid, @DocumentReviewTaskGuid) AND
	task.status IN ('Completed', 'InProcess', 'UnderReview') AND
	task.id = task.maintask

LEFT JOIN sungero_core_recipient rc ON (task.assignedby_recman_sungero = rc.id AND
                    					task.discriminator = @ActionItemExecutionTaskGuid) OR
                    				   (task.addressee_recman_sungero = rc.id AND
                    					task.discriminator = @DocumentReviewTaskGuid)
LEFT JOIN sungero_core_recipient d ON rc.department_company_sungero = d.id
LEFT JOIN sungero_core_recipient b ON d.businessunit_company_sungero = b.id     -- НОР Ответственного		   
LEFT JOIN sungero_parties_counterparty cp ON cp.id = r.incorr_docflow_sungero	  -- ФИО заявителя
-- Сопровод. письмо/ответ заявителю
LEFT JOIN gd_citizen_reqquestions q ON q.edoc = r.id
LEFT JOIN sungero_content_edoc qanswer ON q.transfer = qanswer.id

WHERE	r.discriminator = @RequestGuid AND
  		r.regdate_docflow_sungero >= @StartDate AND r.regdate_docflow_sungero <= @EndDate AND
  		b.id = @BusinessUnitId AND
		task.id IS NOT NULL
)

SELECT	*,
    		CASE
    			WHEN RCompleteDate IS NULL THEN 0
    			ELSE 1
    		END AS HasRCompleteDate
FROM R
GROUP BY RequestId, ManagerName, RegNumber, RegDate, CorName, RequestDeadline, RCompleteDate
ORDER BY ManagerName, RegNumber