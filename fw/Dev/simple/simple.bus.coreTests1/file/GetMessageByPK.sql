SELECT
	message_strings
FROM dbo.m_message
WHERE message_id = @in_message_id
AND [language] = @in_language