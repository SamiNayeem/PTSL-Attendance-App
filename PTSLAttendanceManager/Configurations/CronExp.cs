namespace PTSLAttendanceManager.Configurations
{
    public static class CronExp
    {

       static string GetCronExpressionFromDatabase()
        {
            
            return "0 0/5 * * * ?"; 
        }
    }
}
