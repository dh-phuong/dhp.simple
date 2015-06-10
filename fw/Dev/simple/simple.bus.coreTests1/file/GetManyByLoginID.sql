SELECT
	mus.*
FROM dbo.m_user_sp mus
WHERE mus.login_id in(@in_login_id)