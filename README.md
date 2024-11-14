# Hi Guys
## If you want to use this project to learn or test, follow these steps:
- Open Sql Server on your computer and restore database using `DBNoiThat.bak` file.
- Inside `Web.config` , please change connectionString at

   > \<connectionStrings> <br/>
     \<add name="DBNoiThat" connectionString="data source=`VNICE\SQLEXPRESS`;initial catalog=DBNoiThat;integrated security=True;MultipleActiveResultSets=True;App=EntityFramework" providerName="System.Data.SqlClient" /><br/>
 \</connectionStrings>

    to your connection string.
- Build and enjoy it.

