WITH Q AS
(
SELECT
	c.fullcode AS QFullCode,
	c.name AS QuestionName,

	c_section.code AS SectionCode,
	c_topic.code AS TopicCode,
	c_theme.code AS ThemeCode,
	
	c_section.name AS SectionName,
	c_topic.name AS TopicName,
	c_theme.name AS ThemeName,
	
	COUNT (q.id) FILTER (WHERE r.regdate_docflow_sungero >= @LYStartDate AND r.regdate_docflow_sungero <= @LYEndDate) AS LYCount,
	COUNT (q.id) FILTER (WHERE r.regdate_docflow_sungero >= @StartDate AND r.regdate_docflow_sungero <= @EndDate) AS Count,
	
	COUNT (q.id) FILTER (WHERE r.regdate_docflow_sungero >= @LYStartDate AND r.regdate_docflow_sungero <= @LYEndDate AND
						(r.ControlExState_docflow_sungero = 'OnControl' OR r.ControlExState_docflow_sungero = 'SpecialControl' OR r.controlexstate_docflow_sungero = 'SpecialFZCtrl')) AS LYCtrlCount,
	COUNT (q.id) FILTER (WHERE r.regdate_docflow_sungero >= @StartDate AND r.regdate_docflow_sungero <= @EndDate AND
						(r.ControlExState_docflow_sungero = 'OnControl' OR r.ControlExState_docflow_sungero = 'SpecialControl' OR r.controlexstate_docflow_sungero = 'SpecialFZCtrl')) AS CtrlCount,
						
	COUNT (q.id) FILTER (WHERE r.regdate_docflow_sungero >= @LYStartDate AND r.regdate_docflow_sungero <= @LYEndDate AND
						r.isrepeated_citizen_gd) AS LYRepeatedCount,
	COUNT (q.id) FILTER (WHERE r.regdate_docflow_sungero >= @StartDate AND r.regdate_docflow_sungero <= @EndDate AND
						r.isrepeated_citizen_gd) AS RepeatedCount,
						
	COUNT (q.id) FILTER (WHERE r.regdate_docflow_sungero >= @LYStartDate AND r.regdate_docflow_sungero <= @LYEndDate AND
						(r.ControlExState_docflow_sungero = 'OnControl' OR r.ControlExState_docflow_sungero = 'SpecialControl' OR r.controlexstate_docflow_sungero = 'SpecialFZCtrl') AND
						r.isrepeated_citizen_gd) AS LYCtrlRepeatedCount,
	COUNT (q.id) FILTER (WHERE r.regdate_docflow_sungero >= @StartDate AND r.regdate_docflow_sungero <= @EndDate AND
						(r.ControlExState_docflow_sungero = 'OnControl' OR r.ControlExState_docflow_sungero = 'SpecialControl' OR r.controlexstate_docflow_sungero = 'SpecialFZCtrl') AND
						r.isrepeated_citizen_gd) AS CtrlRepeatedCount,

	COUNT (q.id) FILTER (WHERE r.regdate_docflow_sungero >= @LYStartDate AND r.regdate_docflow_sungero <= @LYEndDate AND
						r.requesttype_citizen_gd = 'Collective') AS LYСollectiveCount,
	COUNT (q.id) FILTER (WHERE r.regdate_docflow_sungero >= @StartDate AND r.regdate_docflow_sungero <= @EndDate AND
						r.requesttype_citizen_gd = 'Collective') AS СollectiveCount,
						
	COUNT (q.id) FILTER (WHERE r.regdate_docflow_sungero >= @LYStartDate AND r.regdate_docflow_sungero <= @LYEndDate AND
						(r.ControlExState_docflow_sungero = 'OnControl' OR r.ControlExState_docflow_sungero = 'SpecialControl' OR r.controlexstate_docflow_sungero = 'SpecialFZCtrl') AND
						r.requesttype_citizen_gd = 'Collective') AS LYCtrlСollectiveCount,
	COUNT (q.id) FILTER (WHERE r.regdate_docflow_sungero >= @StartDate AND r.regdate_docflow_sungero <= @EndDate AND
						(r.ControlExState_docflow_sungero = 'OnControl' OR r.ControlExState_docflow_sungero = 'SpecialControl' OR r.controlexstate_docflow_sungero = 'SpecialFZCtrl') AND
						r.requesttype_citizen_gd = 'Collective') AS CtrlСollectiveCount

FROM gd_citizen_reqquestions q

LEFT JOIN sungero_content_edoc r ON q.edoc = r.id
LEFT JOIN gd_citizen_classifierbase c ON q.question = c.id
LEFT JOIN gd_citizen_classifierbase c_section ON c.section = c_section.id
LEFT JOIN gd_citizen_classifierbase c_topic ON c.topic = c_topic.id
LEFT JOIN gd_citizen_classifierbase c_theme ON c.theme = c_theme.id

WHERE	r.discriminator::citext = @RequestGuid AND
		r.businessunit_docflow_sungero = @BusinessUnitId AND
		((r.regdate_docflow_sungero >= @LYStartDate AND r.regdate_docflow_sungero <= @LYEndDate) OR
    (r.regdate_docflow_sungero >= @StartDate AND r.regdate_docflow_sungero <= @EndDate))
	
GROUP BY QFullCode, QuestionName, SectionCode, TopicCode, ThemeCode, SectionName, TopicName, ThemeName
ORDER BY QFullCode
)

SELECT *,
	SUM(LYCount) OVER(PARTITION BY SectionCode, TopicCode, ThemeCode) AS LYCountByTheme,
	SUM(Count) OVER(PARTITION BY SectionCode, TopicCode, ThemeCode) AS CountByTheme,
	
	SUM(LYCtrlCount) OVER(PARTITION BY SectionCode, TopicCode, ThemeCode) AS LYCtrlCountByTheme,
	SUM(CtrlCount) OVER(PARTITION BY SectionCode, TopicCode, ThemeCode) AS CtrlCountByTheme,
	
	SUM(LYRepeatedCount) OVER(PARTITION BY SectionCode, TopicCode, ThemeCode) AS LYRepeatedCountByTheme,
	SUM(RepeatedCount) OVER(PARTITION BY SectionCode, TopicCode, ThemeCode) AS RepeatedCountByTheme,
	
	SUM(LYCtrlRepeatedCount) OVER(PARTITION BY SectionCode, TopicCode, ThemeCode) AS LYCtrlRepeatedCountByTheme,
	SUM(CtrlRepeatedCount) OVER(PARTITION BY SectionCode, TopicCode, ThemeCode) AS CtrlRepeatedCountByTheme,
	
	SUM(LYСollectiveCount) OVER(PARTITION BY SectionCode, TopicCode, ThemeCode) AS LYСollectiveCountByTheme,
	SUM(СollectiveCount) OVER(PARTITION BY SectionCode, TopicCode, ThemeCode) AS СollectiveCountByTheme,
	
	SUM(LYCtrlСollectiveCount) OVER(PARTITION BY SectionCode, TopicCode, ThemeCode) AS LYCtrlСollectiveCountByTheme,
	SUM(CtrlСollectiveCount) OVER(PARTITION BY SectionCode, TopicCode, ThemeCode) AS CtrlСollectiveCountByTheme,
	
	SUM(LYCount) OVER(PARTITION BY SectionCode, TopicCode) AS LYCountByTopic,
	SUM(Count) OVER(PARTITION BY SectionCode, TopicCode) AS CountByTopic,
	
	SUM(LYCtrlCount) OVER(PARTITION BY SectionCode, TopicCode) AS LYCtrlCountByTopic,
	SUM(CtrlCount) OVER(PARTITION BY SectionCode, TopicCode) AS CtrlCountByTopic,
	
	SUM(LYRepeatedCount) OVER(PARTITION BY SectionCode, TopicCode) AS LYRepeatedCountByTopic,
	SUM(RepeatedCount) OVER(PARTITION BY SectionCode, TopicCode) AS RepeatedCountByTopic,
	
	SUM(LYCtrlRepeatedCount) OVER(PARTITION BY SectionCode, TopicCode) AS LYCtrlRepeatedCountByTopic,
	SUM(CtrlRepeatedCount) OVER(PARTITION BY SectionCode, TopicCode) AS CtrlRepeatedCountByTopic,
	
	SUM(LYСollectiveCount) OVER(PARTITION BY SectionCode, TopicCode) AS LYСollectiveCountByTopic,
	SUM(СollectiveCount) OVER(PARTITION BY SectionCode, TopicCode) AS СollectiveCountByTopic,
	
	SUM(LYCtrlСollectiveCount) OVER(PARTITION BY SectionCode, TopicCode) AS LYCtrlСollectiveCountByTopic,
	SUM(CtrlСollectiveCount) OVER(PARTITION BY SectionCode, TopicCode) AS CtrlСollectiveCountByTopic,

	SUM(LYCount) OVER(PARTITION BY SectionCode) AS LYCountBySection,
	SUM(Count) OVER(PARTITION BY SectionCode) AS CountBySection,
	
	SUM(LYCtrlCount) OVER(PARTITION BY SectionCode) AS LYCtrlCountBySection,
	SUM(CtrlCount) OVER(PARTITION BY SectionCode) AS CtrlCountBySection,
	
	SUM(LYRepeatedCount) OVER(PARTITION BY SectionCode) AS LYRepeatedCountBySection,
	SUM(RepeatedCount) OVER(PARTITION BY SectionCode) AS RepeatedCountBySection,
	
	SUM(LYCtrlRepeatedCount) OVER(PARTITION BY SectionCode) AS LYCtrlRepeatedCountBySection,
	SUM(CtrlRepeatedCount) OVER(PARTITION BY SectionCode) AS CtrlRepeatedCountBySection,
	
	SUM(LYСollectiveCount) OVER(PARTITION BY SectionCode) AS LYСollectiveCountBySection,
	SUM(СollectiveCount) OVER(PARTITION BY SectionCode) AS СollectiveCountBySection,
	
	SUM(LYCtrlСollectiveCount) OVER(PARTITION BY SectionCode) AS LYCtrlСollectiveCountBySection,
	SUM(CtrlСollectiveCount) OVER(PARTITION BY SectionCode) AS CtrlСollectiveCountBySection
FROM Q