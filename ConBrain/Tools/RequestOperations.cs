using ConBrain.Model;

namespace ConBrain.Tools
{
    public static class RequestOperations
    {
        public static string getFormValue(IFormCollection form, string nameProperty, ILogger? logger = null)
        {
            var values = form[nameProperty];
            if (values.Count == 0)
            {
                logger?.LogWarning(string.Format(Properties.Resources.FormNotFindValueWarning, nameProperty));
                return "";
            }
            if (values.Count > 0)
                logger?.LogWarning(string.Format(Properties.Resources.FormFoundServialValuesWarning, nameProperty));
            string res = values.First() ?? string.Empty;
            logger?.LogDebug(string.Format(Properties.Resources.ReadPorpertyByForm, nameProperty, res));
            return res;
        }
        public static Person AddPersonByForm(IFormCollection form, ILogger? logger = null)
        {
            return new Person()
            {
                Name = getFormValue(form, "name", logger),
                Family = getFormValue(form, "family", logger),
                LastName = getFormValue(form, "secondName", logger),
                Nick = getFormValue(form, "nick", logger),
                Phone = getFormValue(form, "tel", logger),
                Password = getFormValue(form, "pass", logger)
            };
        }
    }
}
