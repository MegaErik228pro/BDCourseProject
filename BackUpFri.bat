SQLCMD -S (localdb)\MSSQLLocalDB -E -Q "BACKUP DATABASE Delivery TO DISK = 'C:\Users\Erik\Desktop\3course\Delivery\DBBackup\BackUpDelivery_Fri.bak' WITH INIT, NOFORMAT, SKIP, NOUNLOAD"