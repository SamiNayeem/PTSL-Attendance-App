namespace PTSLAttendanceManager.Configurations
{
    public static class CronExp
    {

       static string GetCronExpressionFromDatabase()
        {

            //return "0 0/5 * * * ?"; 
            return "0 59 23 ? * 7,1-4";
        }
    }
}
