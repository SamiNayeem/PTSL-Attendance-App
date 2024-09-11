// Helpers/FirebaseAdminHelper.cs
using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using System.IO;

public static class FirebaseAdminHelper
{
    public static void InitializeFirebase()
    {
        if (FirebaseApp.DefaultInstance == null)
        {
            var credentialPath = Path.Combine(Directory.GetCurrentDirectory(), "path/to/your/firebase-adminsdk.json");
            FirebaseApp.Create(new AppOptions()
            {
                Credential = GoogleCredential.FromFile(credentialPath)
            });
        }
    }
}
