WITH ReviewedByBU AS
(
  SELECT	b.name AS BusinessUnit,
 		count (distinct r.id) /*FILTER (WHERE r.citizensamount_main_gd IS NULL) +
 			coalesce(sum (r.citizensamount_main_gd) FILTER (WHERE r.citizensamount_main_gd IS NOT NULL), 0) раскомментировать после добавления кол-ва заявителей в ОБР*/
 			AS citizensAmount,
		sum (case when (q.reviewresult = 'Supported' OR q.reviewresult = 'ActionsTaken') AND
				        t.regdate_docflow_sungero >= @StartDate AND t.regdate_docflow_sungero <= @EndDate then 1 else 0 end) AS supportedCount,
			
		sum (case when q.reviewresult = 'ActionsTaken' AND
					   t.regdate_docflow_sungero >= @StartDate AND t.regdate_docflow_sungero <= @EndDate then 1 else 0 end) AS actionsTakenCount,
		
		sum (case when (q.reviewresult = 'NotSupported' OR q.reviewresult = 'Obsolete') AND
					   t.regdate_docflow_sungero >= @StartDate AND t.regdate_docflow_sungero <= @EndDate then 1 else 0 end) AS notSupportedCount,
			 
		sum (case when q.reviewresult = 'Explained' AND
					   t.regdate_docflow_sungero >= @StartDate AND t.regdate_docflow_sungero <= @EndDate then 1 else 0 end) AS explainedCount,

		sum (case when (q.reviewresult IS NULL OR
					    q.reviewresult = 'Draft' OR
					    q.reviewresult = 'InWorkExtended' OR
					    q.reviewresult = 'Transferred') then 1 else 0 end) AS activeCount,

		sum (case when(q.reviewresult = 'Supported' OR q.reviewresult = 'ActionsTaken') AND 
		               t.regdate_docflow_sungero >= @StartDate AND t.regdate_docflow_sungero <= @EndDate then 1 else 0 end) +
		sum (case when(q.reviewresult = 'NotSupported' OR q.reviewresult = 'Obsolete') AND 
		               t.regdate_docflow_sungero >= @StartDate AND t.regdate_docflow_sungero <= @EndDate then 1 else 0 end) +
		sum (case when q.reviewresult = 'Explained' AND 
		               t.regdate_docflow_sungero >= @StartDate AND t.regdate_docflow_sungero <= @EndDate then 1 else 0 end) AS Reviewed
FROM	gd_citizen_reqquestions  q 
JOIN	sungero_content_edoc r ON q.edoc = r.id
JOIN	sungero_content_edoc t ON t.id = ISNULL(ISNULL(q.transfer, r.answerletter_citizen_gd), r.coverletter_citizen_gd)
JOIN	sungero_core_recipient b ON r.businessunit_docflow_sungero = b.id  
WHERE 
   @AllBusinessUnit = 'TRUE' or b.id = @BusinessUnitId
    
GROUP BY b.name ), 

ReviewedByBusinessUnit AS
(
SELECT	*,
		CASE WHEN Reviewed > 0
		THEN CONVERT(decimal(6,3), supportedCount * 100.0/ Reviewed) 
		ELSE NULL END AS SupportedProportion,
		
		CASE WHEN Reviewed > 0 AND supportedCount > 0
		THEN CONVERT(decimal(6,3), actionsTakenCount * 100.00/ Reviewed) 
		ELSE NULL END AS ActionsTakenProportion,
		
		CASE WHEN Reviewed > 0
		THEN CONVERT(decimal(6,3),notSupportedCount * 100.00/ Reviewed)
		ELSE NULL END AS NotSupportedProportion,
		
		CASE WHEN Reviewed > 0
		THEN CONVERT(decimal(6,3),explainedCount * 100.00/ Reviewed)
		ELSE NULL END AS ExplainedProportion,
	
		SUM (supportedCount) OVER () AS TotalSupportedCount,
		SUM (actionsTakenCount) OVER () AS TotalActionsTakenCount,
		SUM (notSupportedCount) OVER () AS TotalNoSupportedCount,
		SUM (explainedCount) OVER () AS TotalExplainedCount,
		SUM (Reviewed) OVER () AS TotalReviewed
	
FROM ReviewedByBU

GROUP BY BusinessUnit, citizensAmount, supportedCount, actionsTakenCount, notSupportedCount, explainedCount, activeCount, Reviewed
)

SELECT	*,
		CASE WHEN SupportedProportion > 0
		THEN (SELECT CONVERT(decimal(5,2), SUM(TotalSupportedCount) * 100.00 / SUM(TotalReviewed)) FROM	ReviewedByBusinessUnit) - SupportedProportion
		ELSE NULL END AS SupportedShareComparison,
		
		CASE WHEN ActionsTakenProportion > 0
		THEN (SELECT CONVERT(decimal(5,2), SUM(TotalActionsTakenCount) * 100.00 / SUM(TotalReviewed)) FROM ReviewedByBusinessUnit) - ActionsTakenProportion
		ELSE NULL END AS ActionsTakenShareComparison,
		
		CASE WHEN NotSupportedProportion > 0
		THEN (SELECT CONVERT(decimal(5,2), SUM(TotalNoSupportedCount) * 100.00 / SUM(TotalReviewed)) FROM	ReviewedByBusinessUnit) - NotSupportedProportion 
		ELSE NULL END AS NotSupportedShareComparison,
		
		CASE WHEN ExplainedProportion > 0
		THEN (SELECT CONVERT(decimal(5,2), SUM(TotalExplainedCount) * 100.00 / SUM(TotalReviewed)) FROM	ReviewedByBusinessUnit) - ExplainedProportion
		ELSE NULL END AS ExplainedShareComparison
		
FROM	ReviewedByBusinessUnit

ORDER BY BusinessUnit ASC