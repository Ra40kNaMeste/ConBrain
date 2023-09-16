using ConBrain.Model;
using ConBrain.Tools;

namespace ConBrain
{ 
    public static class UserReplyTreeComponent
    {
        public static void OnReplyUserMap(IApplicationBuilder builder)
        {
            builder.Run(async (context) =>
            {
                var logger = builder.ApplicationServices.GetService<ILogger>();

                var form = context?.Request.Form;
                if (form == null)
                {
                    logger?.LogError(Properties.Resources.FormNotReceivedError);
                    return;
                }

                var person = await Task.Run(() => RequestOperations.AddPersonByForm(form, logger));

                var db = builder.ApplicationServices.GetService<UserDbContext>();
                if (db == null)
                {
                    logger?.LogCritical(Properties.Resources.NotFoundDatabase);
                    throw new Exception(Properties.Resources.NotFoundDatabase);
                }

                db.People.Add(person);
                db.SaveChanges();
            });
        }

    }
}
